using Microsoft.AspNetCore.Mvc;
using AirportBudget.Server.Interfaces.Repositorys;
using AirportBudget.Server.Models;
using AirportBudget.Server.Datas;
using Microsoft.EntityFrameworkCore;
using NPOI.XSSF.UserModel; // NPOI 的 Excel 套件
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using NPOI.SS.Formula.Functions;
using AirportBudget.Server.Mappings;
using AutoMapper.QueryableExtensions;
using AirportBudget.Server.DTOs;
using AirportRenovate.Server.ViewModels;
using System.Text.RegularExpressions;
//using Money = AirportRenovate.Server.Models.Money;
using MathNet.Numerics.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using LinqKit;
using AirportBudget.Server.ViewModels;
using AirportBudget.Server.Enums;



namespace AirportRenovate.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BudgetAmountController(IGenericRepository<BudgetAmount> BudgetAmountRepository, IGenericRepository<Budget> BudgetRepository, IMapper mapper, AirportBudgetDbContext context) : ControllerBase
{
    private readonly IGenericRepository<BudgetAmount> _BudgetAmountRepository = BudgetAmountRepository;
    private readonly IGenericRepository<Budget> _BudgetRepository = BudgetRepository;
    private readonly IMapper _mapper = mapper;
    private readonly AirportBudgetDbContext _context = context;

    /// <summary>
    /// Groups的預算資料查詢
    /// </summary>
    /// <returns>查詢結果</returns>
    [HttpGet]
    public IActionResult GetGroupData(int Year, int GroupId) // 前端傳Year值,後端回傳符合Year值的工務組資料
    {
        Expression<Func<BudgetAmount, bool>> condition = item => true;
        condition = BudgetAmount => BudgetAmount.Budget != null && BudgetAmount.Budget.GroupId == GroupId;
        condition = condition.And(BudgetAmount => BudgetAmount.Budget != null && BudgetAmount.Budget.GroupId == GroupId);
        condition = condition.And(BudgetAmount => BudgetAmount.CreatedYear == Year && BudgetAmount.Budget != null && BudgetAmount.Budget.CreatedYear == Year);
        condition = condition.And(BudgetAmount => BudgetAmount.Status != null && (BudgetAmount.Status.Trim() == "O" || BudgetAmount.Status.Trim() == "C"));
        condition = condition.And(BudgetAmount => BudgetAmount.IsValid == true);
        try
        {
            var results = _BudgetAmountRepository.GetByCondition(condition)
            .Include(BudgetAmount => BudgetAmount.Budget)
            .ThenInclude(Budget => Budget!.Group)
             .OrderBy(BudgetAmount => BudgetAmount.Budget!.BudgetName)
            .ToList();

            List<BudgetAmountViewModel> budgetAmountViewModels = _mapper.Map<List<BudgetAmountViewModel>>(results);
            return Ok(budgetAmountViewModels);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }


    /// <summary>
    /// Groups的預算詳細資料查詢
    /// </summary>
    /// <returns>查詢結果</returns>
    [HttpGet("SelectedDetail")]
    public IActionResult GetDetailData(int BudgetId, string? BudgetName, int? Year, int? GroupId, string? Description = null, int? RequestAmountStart = null, int? RequestAmountEnd = null) // 前端傳Year值,後端回傳符合Year值的工務組資料
    {
        try
        {
            Expression<Func<BudgetAmount, bool>> condition = item => true;
            //condition = condition.And(BudgetAmount => BudgetAmount.Budget!.BudgetName == BudgetName);
            //condition = condition.And(BudgetAmount => BudgetAmount.Budget!.GroupId == GroupId && BudgetAmount.Budget != null && BudgetAmount.Budget.GroupId == GroupId);
            condition = condition.And(BudgetAmount => BudgetAmount.CreatedYear == Year && BudgetAmount.Budget != null && BudgetAmount.Budget.CreatedYear == Year && BudgetAmount.Status == "O");
            condition = condition.And(BudgetAmount => BudgetAmount.BudgetId == BudgetId && BudgetAmount.Status == "O");
            condition = condition.And(BudgetAmount => BudgetAmount.IsValid == true);


            if (!string.IsNullOrEmpty(Description))
            {
                condition = condition.And(BudgetAmount => BudgetAmount.Description != null && BudgetAmount.Description.Contains(Description));
            }

            if (RequestAmountStart.HasValue)
            {
                condition = condition.And(BudgetAmount => BudgetAmount.RequestAmount >= RequestAmountStart.Value);
            }

            if (RequestAmountEnd.HasValue)
            {
                condition = condition.And(BudgetAmount => BudgetAmount.RequestAmount <= RequestAmountEnd.Value);
            }

            var results = _BudgetAmountRepository.GetByCondition(condition)
           .Include(BudgetAmount => BudgetAmount.Budget)
           .AsEnumerable() // 轉為本地處理，避免 EF Core 的限制
           .Select(BudgetAmount =>
           {
               // 移除需要的欄位中的多餘空格
               BudgetAmount.Status = BudgetAmount.Status?.Trim() ?? "";
               return BudgetAmount;
           })
           .ToList();
            return Ok(results);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }


    /// <summary>
    /// 新增資料
    /// </summary>
    /// <returns>新增結果</returns>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] BudgetAmount request)
    {
        try
        {
            //request.Money = null;
            request.Budget = null;
            // 檢查 request 的 AmountSerialNumber 是否為 0 且 Type 是否為 "一般"
            //if (request.AmountSerialNumber == 0 && request.Type == AmountType.Ordinary)
            //{
            //    // 取得資料庫中 ID1 欄位的最大值並遞增
            //    int maxAmountSerialNumber = await _BudgetAmountRepository.GetAll().MaxAsync(BudgetAmount => (int?)BudgetAmount.AmountSerialNumber) ?? 0;
            //    request.AmountSerialNumber = maxAmountSerialNumber + 1;
            //}

            // 取得當年民國年分
            //var currentYear = DateTime.Now.Year - 1911;
            //request.Year = currentYear;
            // 使用 AutoMapper 將 ViewModel 映射到 Model
            BudgetAmount BudgetAmount = _mapper.Map<BudgetAmount>(request);
            // 檢查 RequestPerson 和 PaymentPerson 欄位，若為 null 則存空值
            BudgetAmount.RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson;
            BudgetAmount.PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson;
            _BudgetAmountRepository.Add(BudgetAmount);

            return Ok("Record added successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

    [HttpPost("ByAllocateForm")]
    public IActionResult PostByAllocateForm([FromBody] List<AllocateFormViewModel> requests)
    {

        if (requests == null || requests.Count != 2)
        {
            return BadRequest("You must provide exactly two records.");
        }

        try
        {
            var entities = _mapper.Map<List<BudgetAmount>>(requests);

            // 使用服務類別的方法新增多筆資料
            _BudgetAmountRepository.AddRange(entities);


            // 保存更改後，檢索剛剛新增的兩筆資料
            var firstEntity = entities[0];
            var secondEntity = entities[1];

            // 更新 LinkedBudgetAmountId
            firstEntity.LinkedBudgetAmountId = secondEntity.BudgetAmountId;
            secondEntity.LinkedBudgetAmountId = firstEntity.BudgetAmountId;

            // update
            _BudgetAmountRepository.Update(firstEntity);
            _BudgetAmountRepository.Update(secondEntity);

            return Ok("Record added successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

    //[HttpPost]
    //public async Task<IActionResult> Post([FromBody] BudgetAmount request)
    //{
    //    try
    //    {
    //        // 取得當年民國年分
    //        //var currentYear = DateTime.Now.Year - 1911;
    //        //request.Year = currentYear;
    //        // 使用 AutoMapper 將 ViewModel 映射到 Model
    //        BudgetAmount BudgetAmount1 = _mapper.Map<BudgetAmount>(request);
    //        BudgetAmount BudgetAmount2 = _mapper.Map<BudgetAmount>(request);
    //        // 檢查 RequestPerson 和 PaymentPerson 欄位，若為 null 則存空值
    //        BudgetAmount.RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson;
    //        BudgetAmount.PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson;
    //        _BudgetAmountRepository.Add(BudgetAmount);
    //        BudgetAmount1.AmountSerialNumber = BudgetAmount2.AmountSerialNumber;

    //        //update
    //        return Ok("Record added successfully");
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex}");
    //    }
    //}

    /// <summary>
    /// 更新細項
    /// </summary>
    /// <returns>更新結果</returns>
    [HttpPut]
    public IActionResult DoUpdate([FromBody] BudgetAmount request)
    {
        try
        {
            request.Budget = null;
            Expression<Func<BudgetAmount, bool>> condition = item => true;

            if (request.Type == AmountType.Ordinary)
            {
                condition = condition.And(BudgetAmount => BudgetAmount.BudgetAmountId == request.BudgetAmountId);
            }
            //else
            //{   // Type為勻出入,跑這邊的程式碼
            //    // 查找第一筆資料
            //    condition = condition.And(BudgetAmount => BudgetAmount.BudgetAmountId == request.BudgetAmountId);
            //    var firstBudgetAmount = _BudgetAmountRepository.GetByCondition(condition).AsNoTracking().FirstOrDefault();

            //    if (firstBudgetAmount == null)
            //    {
            //        return NotFound("First record not found");
            //    }

            //    // 查找關聯的第二筆資料
            //    Expression<Func<BudgetAmount, bool>> linkedCondition = item => item.BudgetAmountId == firstBudgetAmount.LinkedBudgetAmountId;
            //    var secondBudgetAmount = _BudgetAmountRepository.GetByCondition(linkedCondition).AsNoTracking().FirstOrDefault();

            //    if (secondBudgetAmount == null)
            //    {
            //        return NotFound("Linked record not found");
            //    }

            //    // 更新第一筆資料
            //    BudgetAmount updatedFirstBudgetAmount = _mapper.Map<BudgetAmount, BudgetAmount>(request, firstBudgetAmount);
            //    updatedFirstBudgetAmount.LinkedBudgetAmountId = firstBudgetAmount.LinkedBudgetAmountId;
            //    updatedFirstBudgetAmount.BudgetId = firstBudgetAmount.BudgetId;
            //    updatedFirstBudgetAmount.Type = firstBudgetAmount.Type;
            //    updatedFirstBudgetAmount.RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson;
            //    updatedFirstBudgetAmount.PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson;
            //    _BudgetAmountRepository.Update(updatedFirstBudgetAmount);

            //    // 更新第二筆資料
            //    BudgetAmount updatedSecondBudgetAmount = _mapper.Map<BudgetAmount, BudgetAmount>(request, secondBudgetAmount);
            //    updatedSecondBudgetAmount.LinkedBudgetAmountId = secondBudgetAmount.LinkedBudgetAmountId;
            //    updatedSecondBudgetAmount.BudgetId = secondBudgetAmount.BudgetId;
            //    updatedSecondBudgetAmount.Type = secondBudgetAmount.Type;
            //    updatedSecondBudgetAmount.RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson;
            //    updatedSecondBudgetAmount.PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson;
            //    _BudgetAmountRepository.Update(updatedSecondBudgetAmount);

            //    return Ok("Records updated successfully");
            //}

            // Type為一般,跑這邊的程式碼
            var ExistBudgetAmount = _BudgetAmountRepository.GetByCondition(condition).AsNoTracking().FirstOrDefault(); // 這邊不能用find(AmountSerialNumber不是PK)
            if (ExistBudgetAmount == null)
            {
                return NotFound("Record not found");
            }
            // 將 ViewModel 映射到實體模型
            BudgetAmount updatedBudgetAmount = _mapper.Map<BudgetAmount, BudgetAmount>(request, ExistBudgetAmount);
            // 檢查 People 和 People1 欄位，若為 null 則存空值
            ExistBudgetAmount.RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson;
            ExistBudgetAmount.PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson;
            _BudgetAmountRepository.Update(updatedBudgetAmount);
            return Ok("Record updated successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }


    /// <summary>
    /// 更新勻出入細項
    /// </summary>
    /// <returns>更新結果</returns>
    [HttpPut("ByUpdateAllocate")]
    public IActionResult DoAllocateUpdate([FromBody] BudgetAmount request)
    {
        try
        {      
                if(request.Type == AmountType.Ordinary)
                {
                return BadRequest("非勻出入資料");
                }

                 request.Budget = null;
                 Expression<Func<BudgetAmount, bool>> condition = item => true;

                // Type為勻出入,跑這邊的程式碼
                // 查找第一筆資料
                condition = condition.And(BudgetAmount => BudgetAmount.BudgetAmountId == request.BudgetAmountId);
                var firstBudgetAmount = _BudgetAmountRepository.GetByCondition(condition).AsNoTracking().FirstOrDefault();

                if (firstBudgetAmount == null)
                {
                    return NotFound("First record not found");
                }

                // 查找關聯的第二筆資料
                Expression<Func<BudgetAmount, bool>> linkedCondition = item => item.BudgetAmountId == firstBudgetAmount.LinkedBudgetAmountId;
                var secondBudgetAmount = _BudgetAmountRepository.GetByCondition(linkedCondition).AsNoTracking().FirstOrDefault();

                if (secondBudgetAmount == null)
                {
                    return NotFound("Linked record not found");
                }

            //// 使用新的實體進行更新
            //BudgetAmount updatedFirstBudgetAmount = new BudgetAmount
            //{
            //    BudgetAmountId = firstBudgetAmount.BudgetAmountId,
            //    LinkedBudgetAmountId = firstBudgetAmount.LinkedBudgetAmountId,
            //    BudgetId = firstBudgetAmount.BudgetId,
            //    Type = firstBudgetAmount.Type,
            //    RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson,
            //    PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson,
            //    // 其他屬性依需求映射
            //};

            //_BudgetAmountRepository.Update(updatedFirstBudgetAmount);

            //BudgetAmount updatedSecondBudgetAmount = new BudgetAmount
            //{
            //    BudgetAmountId = secondBudgetAmount.BudgetAmountId,
            //    LinkedBudgetAmountId = secondBudgetAmount.LinkedBudgetAmountId,
            //    BudgetId = secondBudgetAmount.BudgetId,
            //    Type = secondBudgetAmount.Type,
            //    RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson,
            //    PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson,
            //    // 其他屬性依需求映射
            //};

            //_BudgetAmountRepository.Update(updatedSecondBudgetAmount);

            // 使用新的實體進行更新
            BudgetAmount updatedFirstBudgetAmount = _mapper.Map<BudgetAmount>(request);
            updatedFirstBudgetAmount.BudgetAmountId = firstBudgetAmount.BudgetAmountId;
            updatedFirstBudgetAmount.LinkedBudgetAmountId = firstBudgetAmount.LinkedBudgetAmountId;
            updatedFirstBudgetAmount.BudgetId = firstBudgetAmount.BudgetId;
            updatedFirstBudgetAmount.Type = firstBudgetAmount.Type;
            updatedFirstBudgetAmount.RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson;
            updatedFirstBudgetAmount.PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson;
            _BudgetAmountRepository.Update(updatedFirstBudgetAmount);

            BudgetAmount updatedSecondBudgetAmount = _mapper.Map<BudgetAmount>(request);
            updatedSecondBudgetAmount.BudgetAmountId = secondBudgetAmount.BudgetAmountId;
            updatedSecondBudgetAmount.LinkedBudgetAmountId = secondBudgetAmount.LinkedBudgetAmountId;
            updatedSecondBudgetAmount.BudgetId = secondBudgetAmount.BudgetId;
            updatedSecondBudgetAmount.Type = secondBudgetAmount.Type;
            updatedSecondBudgetAmount.RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson;
            updatedSecondBudgetAmount.PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson;
            _BudgetAmountRepository.Update(updatedSecondBudgetAmount);

            //// 更新第一筆資料
            //_context.Entry(firstBudgetAmount).State = EntityState.Detached;// 將實體從上下文中分離
            //BudgetAmount updatedFirstBudgetAmount = _mapper.Map<BudgetAmount, BudgetAmount>(request, firstBudgetAmount);
            //updatedFirstBudgetAmount.LinkedBudgetAmountId = firstBudgetAmount.LinkedBudgetAmountId;
            //updatedFirstBudgetAmount.BudgetId = firstBudgetAmount.BudgetId;
            //updatedFirstBudgetAmount.Type = firstBudgetAmount.Type;
            //updatedFirstBudgetAmount.RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson;
            //updatedFirstBudgetAmount.PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson;


            //// 更新第二筆資料
            //_context.Entry(secondBudgetAmount).State = EntityState.Detached;
            //BudgetAmount updatedSecondBudgetAmount = _mapper.Map<BudgetAmount, BudgetAmount>(request, secondBudgetAmount);
            //updatedSecondBudgetAmount.LinkedBudgetAmountId = secondBudgetAmount.LinkedBudgetAmountId;
            //updatedSecondBudgetAmount.BudgetId = secondBudgetAmount.BudgetId;
            //updatedSecondBudgetAmount.Type = secondBudgetAmount.Type;
            //updatedSecondBudgetAmount.RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson;
            //updatedSecondBudgetAmount.PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson;

            //_BudgetAmountRepository.Update(updatedFirstBudgetAmount);
            //_BudgetAmountRepository.Update(updatedSecondBudgetAmount);

            return Ok("Records updated successfully");
            
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }


    /// <summary>
    /// 軟刪除
    /// </summary>
    /// <returns>刪除結果</returns>
    [HttpPut("SoftDelete")]
    public IActionResult DoSoftDelete([FromBody] BudgetAmount request)
    {
        try
        {
            //var BudgetAmount = _BudgetAmountRepository.GetByCondition(BudgetAmount => BudgetAmount.ID1 == request.ID1).FirstOrDefault();
            var BudgetAmount = _BudgetAmountRepository.GetById(request.BudgetAmountId);
            if (BudgetAmount == null)
            {
                return NotFound("not exist");
            }
            // 更新Status欄位
            BudgetAmount.IsValid = false;
            _BudgetAmountRepository.Update(BudgetAmount);
            return Ok("ok");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

    /// <summary>
    /// 軟刪除查詢
    /// </summary>
    /// <returns>查詢結果</returns>
    [HttpGet("ByDeletedRecords")]
    public IActionResult SearchDeletedRecords(int CreatedYear, string? Description)
    {
        try
        {
            var deletedRecords = _BudgetAmountRepository.GetByCondition(BudgetAmount => BudgetAmount.CreatedYear == CreatedYear && BudgetAmount.IsValid == false)
                .Include(BA => BA.Budget)
                .ThenInclude(B => B!.Group)
                .ToList();

            if (!string.IsNullOrEmpty(Description))
            {
                deletedRecords = deletedRecords.Where(BudgetAmount => BudgetAmount.Description != null && BudgetAmount.Description.Contains(Description)).ToList();
            }


            // 將 Status 欄位的多餘空格移除
            var cleanedRecords = deletedRecords
                .Select(record =>
                {
                    record.Status = record.Status.Trim();
                    return record;
                })
                .ToList();
            //var cleanedRecords = _mapper.Map<List<DeletedRecordsViewModel>>(deletedRecords);

            return Ok(cleanedRecords);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// 還原軟刪除
    /// </summary>
    /// <returns>執行結果</returns>
    [HttpPut("ByRestoreData")]
    public IActionResult RestoreData([FromBody] BudgetAmount request)
    {
        try
        {
            if (request == null || request.BudgetAmountId <= 0)
            {
                return BadRequest("Invalid request.");
            }

            //var entity =  _context.BudgetAmount.Find(request.ID1); // 因為find方法是透過主鍵去搜尋,ID1不是主鍵,找不到資料
            var entity = _BudgetAmountRepository.GetById(request.BudgetAmountId);
            //var entity = _BudgetAmountRepository.GetByCondition(e => e.BudgetAmountId == request.BudgetAmountId).FirstOrDefault();

            if (entity == null)
            {
                return NotFound("Record not found.");
            }

            //entity.Status = "O";
            entity.IsValid = true;
            _BudgetAmountRepository.Update(entity);

            return Ok("IsValid updated to 'O'.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }



    ///// <summary>
    ///// 查詢ID1最大值
    ///// </summary>
    ///// <returns>查詢結果</returns>
    //[HttpGet("ID1")]
    //public async Task<IActionResult> GetID1()
    //{
    //    try
    //    {
    //        Expression<Func<BudgetAmount, bool>> condition = item => true;
    //        //condition = condition.And(BudgetAmount => BudgetAmount.Money!.Budget == Budget);
    //        // 取得資料庫中ID1欄位的最大值並遞增
    //        int maxID1 = await _BudgetAmountRepository.GetAll().MaxAsync(m => (int?)m.ID1) ?? 0;
    //        if (maxID1 <= 0)
    //        {
    //            return NotFound("No records found with valid ID1");
    //        }
    //        int ID1 = maxID1 + 1;
    //        return Ok(ID1);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex}");
    //    }
    //}

    // [HttpPost]
    // public async Task<IActionResult> Create([FromBody] BalanceFormViewModel balanceForm)
    // {
    //     //User
    //     try
    //     {
    //         var results = await _BudgetAmountRepository.GetByCondition(BudgetAmount => BudgetAmount.Group1 == balanceForm.Group
    //&& BudgetAmount.Money != null && BudgetAmount.Money.Group == balanceForm.Group
    //&& BudgetAmount.Money.Budget == balanceForm.Budget
    //&& BudgetAmount.Year == balanceForm.Year && BudgetAmount.Money.Year == balanceForm.Year
    //&& (BudgetAmount.Status != null && (BudgetAmount.Status.Trim() == "O" || BudgetAmount.Status.Trim() == "C")))
    //.Include(BudgetAmount => BudgetAmount.Money)
    //  .ToListAsync();

    //         var query = results
    //         .GroupBy(m3 => new {
    //             Subject6 = m3.Money != null ? m3.Money.Subject6 : "",
    //             Subject7 = m3.Money != null ? m3.Money.Subject7 : "",
    //             Subject8 = m3.Money != null ? m3.Money.Subject8 : "",
    //             BudgetYear = m3.Money != null ? m3.Money.BudgetYear : 0,
    //             Final = m3.Money != null && decimal.TryParse(m3.Money.Final, out var final) ? final : 0m,
    //             Budget = m3.Money != null ? m3.Money.Budget : "",
    //             Group = m3.Money != null ? m3.Money.Group : "",
    //             m3.Year
    //         })
    //         .Select(g => new BudgetDetailsViewModel
    //         {
    //             Budget = g.Key.Budget ?? "",
    //             Subject6 = g.Key.Subject6 ?? "",
    //             Group = g.Key.Group ?? "",
    //             Subject7 = g.Key.Subject7 ?? "",
    //             Subject8 = g.Key.Subject8 ?? "",
    //             BudgetYear = g.Key.BudgetYear,
    //             Final = g.Key.Final,
    //             General = g.Sum(x => x.Text == "一般" ? x.PurchaseMoney : 0),
    //             Out = g.Sum(x => x.Text == "勻出" ? x.PurchaseMoney : 0),
    //             UseBudget = g.Key.BudgetYear - g.Sum(x => x.Text == "勻出" ? x.PurchaseMoney : 0) - g.Sum(x => x.Text == "一般" ? x.PurchaseMoney : 0) + g.Key.Final,
    //             In = g.Sum(x => x.Text == "勻入" ? x.PurchaseMoney : 0),
    //             InActual = g.Sum(x => x.Text == "勻入" ? x.PayMoney : 0),
    //             InBalance = g.Sum(x => x.Text == "勻入" ? x.PurchaseMoney : 0) - g.Sum(x => x.Text == "勻入" ? x.PayMoney : 0),
    //             SubjectActual = g.Sum(x => x.Text == "勻入" ? x.PayMoney : 0) + g.Sum(x => x.Text == "一般" ? x.PayMoney : 0)
    //         }).ToList();

    //         if (!query.Any())
    //         {
    //             return Ok("沒有找到符合條件的資料!");
    //         }

    //         return Ok(query);

    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
    //     }
    // }


}




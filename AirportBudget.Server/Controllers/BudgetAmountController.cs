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
using System.Text.RegularExpressions;
using MathNet.Numerics.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using LinqKit;
using AirportBudget.Server.ViewModels;
using AirportBudget.Server.Enums;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.POIFS.FileSystem;
using System.Text.Json;
using AirportBudget.Server.Biz;
using Microsoft.EntityFrameworkCore.Metadata.Internal;



namespace AirportBudget.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BudgetAmountController(
    IGenericRepository<BudgetAmount> budgetAmountRepository,
    IGenericRepository<EntityLog> EntityLogRepository,
    IGenericRepository<User> userRepository,
    IMapper mapper,
    BudgetAmountExcelExportService budgetAmountExcelExportService,
    ExportBudgetExcelService exportBudgetExcelService,
    ExportFundExcelService exportFundExcelService,
    IHttpContextAccessor httpContextAccessor) : ControllerBase
{
    private readonly IGenericRepository<BudgetAmount> _budgetAmountRepository = budgetAmountRepository;
    private readonly IGenericRepository<EntityLog> _EntityLogRepository = EntityLogRepository;
    private readonly IGenericRepository<User> _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;
    private readonly BudgetAmountExcelExportService _budgetAmountExcelExportService = budgetAmountExcelExportService;
    private readonly ExportBudgetExcelService _exportBudgetExcelService = exportBudgetExcelService;
    private readonly ExportFundExcelService _exportFundExcelService = exportFundExcelService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

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
        //condition = condition.And(BudgetAmount => BudgetAmount.Status != null && (BudgetAmount.Status.Trim() == "O" || BudgetAmount.Status.Trim() == "C"));
        condition = condition.And(BudgetAmount => BudgetAmount.IsValid == true);
        try
        {
            var results = _budgetAmountRepository.GetByCondition(condition)
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
    public IActionResult GetDetailData(int BudgetId, string? Description = null, int? RequestAmountStart = null, int? RequestAmountEnd = null) // 前端傳Year值,後端回傳符合Year值的工務組資料
    {
        try
        {
            Expression<Func<BudgetAmount, bool>> condition = item => true;
            //condition = condition.And(BudgetAmount => BudgetAmount.Budget!.BudgetName == BudgetName);
            //condition = condition.And(BudgetAmount => BudgetAmount.Budget!.GroupId == GroupId && BudgetAmount.Budget != null && BudgetAmount.Budget.GroupId == GroupId);
            //condition = condition.And(BudgetAmount => BudgetAmount.CreatedYear == Year && BudgetAmount.Budget != null && BudgetAmount.Budget.CreatedYear == Year && BudgetAmount.Status == "O");
            //condition = condition.And(BudgetAmount => BudgetAmount.BudgetId == BudgetId && BudgetAmount.Status == "O");
            condition = condition.And(BudgetAmount => BudgetAmount.BudgetId == BudgetId && BudgetAmount.IsValid == true);


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

            var results = _budgetAmountRepository.GetByCondition(condition)
           .Include(BudgetAmount => BudgetAmount.Budget)
           .AsEnumerable() // 轉為本地處理，避免 EF Core 的限制
           .OrderByDescending(BudgetAmount => BudgetAmount.RequestDate)
           //.Select(BudgetAmount =>
           //{
           //    // 移除需要的欄位中的多餘空格
           //    BudgetAmount.Status = BudgetAmount.Status?.Trim() ?? "";
           //    return BudgetAmount;
           //})
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
            request.Budget = null;
            // 使用 AutoMapper 將 ViewModel 映射到 Model
            BudgetAmount BudgetAmount = _mapper.Map<BudgetAmount>(request);
            // 檢查 RequestPerson 和 PaymentPerson 欄位，若為 null 則存空值
            BudgetAmount.RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson;
            BudgetAmount.PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson;
            _budgetAmountRepository.Add(BudgetAmount);

            // log
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserID")?.Value ?? "Unknown";
            var log = new EntityLog
            {
                EntityId = BudgetAmount.BudgetAmountId,
                EntityType = MyEntityType.BudgetAmount,
                ActionType = ActionType.Insert,
                ChangedBy = userId,
                ChangeTime = DateTime.Now,
                Values = JsonSerializer.Serialize(BudgetAmount)
            };

            _EntityLogRepository.Add(log);

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
            _budgetAmountRepository.AddRange(entities);


            // 保存更改後，檢索剛剛新增的兩筆資料
            var firstEntity = entities[0];
            var secondEntity = entities[1];

            // 更新 LinkedBudgetAmountId
            firstEntity.LinkedBudgetAmountId = secondEntity.BudgetAmountId;
            secondEntity.LinkedBudgetAmountId = firstEntity.BudgetAmountId;

            // update
            _budgetAmountRepository.Update(firstEntity);
            _budgetAmountRepository.Update(secondEntity);

            // log
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserID")?.Value ?? "Unknown";
            var firstLog = new EntityLog
            {
                EntityId = firstEntity.BudgetAmountId,
                EntityType = MyEntityType.BudgetAmount,
                ActionType = ActionType.Insert,
                ChangedBy = userId,
                ChangeTime = DateTime.Now,
                Values = JsonSerializer.Serialize(firstEntity)
            };

            var secondLog = new EntityLog
            {
                EntityId = secondEntity.BudgetAmountId,
                EntityType = MyEntityType.BudgetAmount,
                ActionType = ActionType.Insert,
                ChangedBy = userId,
                ChangeTime = DateTime.Now,
                Values = JsonSerializer.Serialize(secondEntity)
            };

            _EntityLogRepository.Add(firstLog);
            _EntityLogRepository.Add(secondLog);

            return Ok("Record added successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

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
            else
            {
                if (request.Type == AmountType.Ordinary)
                {
                    return BadRequest("非勻出入資料");
                }

                // Type為勻出入(2,3),跑這邊的程式碼
                // 查找第一筆資料
                condition = condition.And(BudgetAmount => BudgetAmount.BudgetAmountId == request.BudgetAmountId);
                var firstBudgetAmount = _budgetAmountRepository.GetByCondition(condition).AsNoTracking().FirstOrDefault();

                if (firstBudgetAmount == null)
                {
                    return NotFound("First record not found");
                }

                // 查找關聯的第二筆資料
                Expression<Func<BudgetAmount, bool>> linkedCondition = item => item.BudgetAmountId == firstBudgetAmount.LinkedBudgetAmountId;
                var secondBudgetAmount = _budgetAmountRepository.GetByCondition(linkedCondition).AsNoTracking().FirstOrDefault();

                if (secondBudgetAmount == null)
                {
                    return NotFound("Linked record not found");
                }

                // 使用新的實體進行更新
                BudgetAmount updatedFirstBudgetAmount = _mapper.Map<BudgetAmount>(request);
                updatedFirstBudgetAmount.BudgetAmountId = firstBudgetAmount.BudgetAmountId;
                updatedFirstBudgetAmount.LinkedBudgetAmountId = firstBudgetAmount.LinkedBudgetAmountId;
                updatedFirstBudgetAmount.BudgetId = firstBudgetAmount.BudgetId;
                updatedFirstBudgetAmount.Type = firstBudgetAmount.Type;
                updatedFirstBudgetAmount.RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson;
                updatedFirstBudgetAmount.PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson;
                _budgetAmountRepository.Update(updatedFirstBudgetAmount);

                BudgetAmount updatedSecondBudgetAmount = _mapper.Map<BudgetAmount>(request);
                updatedSecondBudgetAmount.BudgetAmountId = secondBudgetAmount.BudgetAmountId;
                updatedSecondBudgetAmount.LinkedBudgetAmountId = secondBudgetAmount.LinkedBudgetAmountId;
                updatedSecondBudgetAmount.BudgetId = secondBudgetAmount.BudgetId;
                updatedSecondBudgetAmount.Type = secondBudgetAmount.Type;
                updatedSecondBudgetAmount.RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson;
                updatedSecondBudgetAmount.PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson;
                _budgetAmountRepository.Update(updatedSecondBudgetAmount);


                // 將原始資料與更新後的資料序列化為 JSON 字串
                var originalfirstBudgetAmount = JsonSerializer.Serialize(firstBudgetAmount);
                var updatedFirstBudgetAmountJson = JsonSerializer.Serialize(updatedFirstBudgetAmount);

                // 比較原始資料與更新後的資料是否不同
                if (originalfirstBudgetAmount != updatedFirstBudgetAmountJson)
                {
                    // 如果資料有變動，執行更新操作並記錄 log
                    //_budgetAmountRepository.Update(updatedFirstBudgetAmount);

                    // log
                    var logUserId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserID")?.Value ?? "Unknown";
                    var firstLog = new EntityLog
                    {
                        EntityId = updatedFirstBudgetAmount.BudgetAmountId,
                        EntityType = MyEntityType.BudgetAmount,
                        ActionType = ActionType.Update,
                        ChangedBy = logUserId,
                        ChangeTime = DateTime.Now,
                        Values = originalfirstBudgetAmount // 存更新前的值
                    };

                    var secondLog = new EntityLog
                    {
                        EntityId = updatedSecondBudgetAmount.BudgetAmountId,
                        EntityType = MyEntityType.BudgetAmount,
                        ActionType = ActionType.Update,
                        ChangedBy = logUserId,
                        ChangeTime = DateTime.Now,
                        Values = JsonSerializer.Serialize(secondBudgetAmount) // 存更新前的值
                    };

                    _EntityLogRepository.Add(firstLog);
                    _EntityLogRepository.Add(secondLog);

                    return Ok("Record updated successfully");
                }
                else
                {
                    // 資料無變動，不進行更新和記錄 log
                    return Ok("No changes detected, update skipped");
                }
            }

            // Type為一般(1),跑這邊的程式碼
            var ExistBudgetAmount = _budgetAmountRepository.GetByCondition(condition).AsNoTracking().FirstOrDefault(); // 這邊不能用find(AmountSerialNumber不是PK)
            if (ExistBudgetAmount == null)
            {
                return NotFound("Record not found");
            }
            // 將 ViewModel 映射到實體模型
            BudgetAmount updatedBudgetAmount = _mapper.Map<BudgetAmount>(request);
            updatedBudgetAmount.RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson;
            updatedBudgetAmount.PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson;
            //BudgetAmount updatedBudgetAmount = _mapper.Map<BudgetAmount, BudgetAmount>(request, ExistBudgetAmount);
            //// 檢查 People 和 People1 欄位，若為 null 則存空值
            //ExistBudgetAmount.RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson;
            //ExistBudgetAmount.PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson;
            _budgetAmountRepository.Update(updatedBudgetAmount);

            // 將原始資料與更新後的資料序列化為 JSON 字串
            var originalExistBudgetAmountJson = JsonSerializer.Serialize(ExistBudgetAmount);
            var updatedBudgetAmountJson = JsonSerializer.Serialize(updatedBudgetAmount);

            // 比較原始資料與更新後的資料是否不同
            if (originalExistBudgetAmountJson != updatedBudgetAmountJson)
            {
                // 如果資料有變動，執行更新操作並記錄 log
                _budgetAmountRepository.Update(updatedBudgetAmount);

                // log
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserID")?.Value ?? "Unknown";
                var log = new EntityLog
                {
                    EntityId = updatedBudgetAmount.BudgetAmountId,
                    EntityType = MyEntityType.BudgetAmount,
                    ActionType = ActionType.Update,
                    ChangedBy = userId,
                    ChangeTime = DateTime.Now,
                    Values = originalExistBudgetAmountJson // 存更新前的值
                };

                _EntityLogRepository.Add(log);

                return Ok("Record updated successfully");
            }
            else
            {
                // 資料無變動，不進行更新和記錄 log
                return Ok("No changes detected, update skipped");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }


    ///// <summary>
    ///// 更新勻出入細項
    ///// </summary>
    ///// <returns>更新結果</returns>
    //[HttpPut("ByUpdateAllocate")]
    //public IActionResult DoAllocateUpdate([FromBody] BudgetAmount request)
    //{
    //    try
    //    {      
    //            if(request.Type == AmountType.Ordinary)
    //            {
    //            return BadRequest("非勻出入資料");
    //            }

    //             request.Budget = null;
    //             Expression<Func<BudgetAmount, bool>> condition = item => true;

    //            // Type為勻出入,跑這邊的程式碼
    //            // 查找第一筆資料
    //            condition = condition.And(BudgetAmount => BudgetAmount.BudgetAmountId == request.BudgetAmountId);
    //            var firstBudgetAmount = _budgetAmountRepository.GetByCondition(condition).AsNoTracking().FirstOrDefault();

    //            if (firstBudgetAmount == null)
    //            {
    //                return NotFound("First record not found");
    //            }

    //            // 查找關聯的第二筆資料
    //            Expression<Func<BudgetAmount, bool>> linkedCondition = item => item.BudgetAmountId == firstBudgetAmount.LinkedBudgetAmountId;
    //            var secondBudgetAmount = _budgetAmountRepository.GetByCondition(linkedCondition).AsNoTracking().FirstOrDefault();

    //            if (secondBudgetAmount == null)
    //            {
    //                return NotFound("Linked record not found");
    //            }

    //        //// 使用新的實體進行更新
    //        //BudgetAmount updatedFirstBudgetAmount = new BudgetAmount
    //        //{
    //        //    BudgetAmountId = firstBudgetAmount.BudgetAmountId,
    //        //    LinkedBudgetAmountId = firstBudgetAmount.LinkedBudgetAmountId,
    //        //    BudgetId = firstBudgetAmount.BudgetId,
    //        //    Type = firstBudgetAmount.Type,
    //        //    RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson,
    //        //    PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson,
    //        //    // 其他屬性依需求映射
    //        //};

    //        //_budgetAmountRepository.Update(updatedFirstBudgetAmount);

    //        //BudgetAmount updatedSecondBudgetAmount = new BudgetAmount
    //        //{
    //        //    BudgetAmountId = secondBudgetAmount.BudgetAmountId,
    //        //    LinkedBudgetAmountId = secondBudgetAmount.LinkedBudgetAmountId,
    //        //    BudgetId = secondBudgetAmount.BudgetId,
    //        //    Type = secondBudgetAmount.Type,
    //        //    RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson,
    //        //    PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson,
    //        //    // 其他屬性依需求映射
    //        //};

    //        //_budgetAmountRepository.Update(updatedSecondBudgetAmount);

    //        // 使用新的實體進行更新
    //        BudgetAmount updatedFirstBudgetAmount = _mapper.Map<BudgetAmount>(request);
    //        updatedFirstBudgetAmount.BudgetAmountId = firstBudgetAmount.BudgetAmountId;
    //        updatedFirstBudgetAmount.LinkedBudgetAmountId = firstBudgetAmount.LinkedBudgetAmountId;
    //        updatedFirstBudgetAmount.BudgetId = firstBudgetAmount.BudgetId;
    //        updatedFirstBudgetAmount.Type = firstBudgetAmount.Type;
    //        updatedFirstBudgetAmount.RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson;
    //        updatedFirstBudgetAmount.PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson;
    //        _budgetAmountRepository.Update(updatedFirstBudgetAmount);

    //        BudgetAmount updatedSecondBudgetAmount = _mapper.Map<BudgetAmount>(request);
    //        updatedSecondBudgetAmount.BudgetAmountId = secondBudgetAmount.BudgetAmountId;
    //        updatedSecondBudgetAmount.LinkedBudgetAmountId = secondBudgetAmount.LinkedBudgetAmountId;
    //        updatedSecondBudgetAmount.BudgetId = secondBudgetAmount.BudgetId;
    //        updatedSecondBudgetAmount.Type = secondBudgetAmount.Type;
    //        updatedSecondBudgetAmount.RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson;
    //        updatedSecondBudgetAmount.PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson;
    //        _budgetAmountRepository.Update(updatedSecondBudgetAmount);

    //        //// 更新第一筆資料
    //        //_context.Entry(firstBudgetAmount).State = EntityState.Detached;// 將實體從上下文中分離
    //        //BudgetAmount updatedFirstBudgetAmount = _mapper.Map<BudgetAmount, BudgetAmount>(request, firstBudgetAmount);
    //        //updatedFirstBudgetAmount.LinkedBudgetAmountId = firstBudgetAmount.LinkedBudgetAmountId;
    //        //updatedFirstBudgetAmount.BudgetId = firstBudgetAmount.BudgetId;
    //        //updatedFirstBudgetAmount.Type = firstBudgetAmount.Type;
    //        //updatedFirstBudgetAmount.RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson;
    //        //updatedFirstBudgetAmount.PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson;


    //        //// 更新第二筆資料
    //        //_context.Entry(secondBudgetAmount).State = EntityState.Detached;
    //        //BudgetAmount updatedSecondBudgetAmount = _mapper.Map<BudgetAmount, BudgetAmount>(request, secondBudgetAmount);
    //        //updatedSecondBudgetAmount.LinkedBudgetAmountId = secondBudgetAmount.LinkedBudgetAmountId;
    //        //updatedSecondBudgetAmount.BudgetId = secondBudgetAmount.BudgetId;
    //        //updatedSecondBudgetAmount.Type = secondBudgetAmount.Type;
    //        //updatedSecondBudgetAmount.RequestPerson = request.RequestPerson == "無" ? string.Empty : request.RequestPerson;
    //        //updatedSecondBudgetAmount.PaymentPerson = request.PaymentPerson == "無" ? string.Empty : request.PaymentPerson;

    //        //_budgetAmountRepository.Update(updatedFirstBudgetAmount);
    //        //_budgetAmountRepository.Update(updatedSecondBudgetAmount);

    //        return Ok("Records updated successfully");
            
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex}");
    //    }
    //}


    /// <summary>
    /// 軟刪除
    /// </summary>
    /// <returns>刪除結果</returns>
    [HttpPut("SoftDelete")]
    public IActionResult DoSoftDelete([FromBody] BudgetAmount request)
    {
        try
        {   
            if(request != null && request.Type != AmountType.Ordinary) // 勻出入資料兩筆要同時刪除
            {
                
                if(request == null)
                {
                    return BadRequest("No request");
                }

                var updatedFirstBudgetAmount = _budgetAmountRepository.GetById(request.BudgetAmountId);
                var updatedSecondBudgetAmount = _budgetAmountRepository.GetByCondition(BudgetAmount => BudgetAmount.BudgetAmountId == request.LinkedBudgetAmountId).FirstOrDefault();
                if (updatedFirstBudgetAmount == null)
                {
                    return NotFound("not exist");
                }
                if (updatedSecondBudgetAmount == null)
                {
                    var originalForOnlyOneLog = JsonSerializer.Serialize(updatedFirstBudgetAmount);
                    updatedFirstBudgetAmount!.IsValid = false;
                    _budgetAmountRepository.Update(updatedFirstBudgetAmount);
                    var forLogUserId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserID")?.Value ?? "Unknown";
                    var onlyOneLog = new EntityLog
                    {
                        EntityId = updatedFirstBudgetAmount.BudgetAmountId,
                        EntityType = MyEntityType.BudgetAmount,
                        ActionType = ActionType.Update,
                        ChangedBy = forLogUserId,
                        ChangeTime = DateTime.Now,
                        Values = originalForOnlyOneLog // 儲存更新前的值
                    };
                    _EntityLogRepository.Add(onlyOneLog);
                    return Ok("LinkedBudgetAmountId is not exist");
                }

                var originalFirstBudgetAmount = JsonSerializer.Serialize(updatedFirstBudgetAmount);
                var originalSecondBudgetAmount = JsonSerializer.Serialize(updatedSecondBudgetAmount);

                // 更新Status欄位
                updatedFirstBudgetAmount!.IsValid = false;
                updatedSecondBudgetAmount!.IsValid = false;
                _budgetAmountRepository.Update(updatedFirstBudgetAmount);
                _budgetAmountRepository.Update(updatedSecondBudgetAmount);

                // log
                var logUserId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserID")?.Value ?? "Unknown";
                var firstLog = new EntityLog
                {
                    EntityId = updatedFirstBudgetAmount.BudgetAmountId,
                    EntityType = MyEntityType.BudgetAmount,
                    ActionType = ActionType.Update,
                    ChangedBy = logUserId,
                    ChangeTime = DateTime.Now,
                    Values = originalFirstBudgetAmount
                };

                var secondLog = new EntityLog
                {
                    EntityId = updatedSecondBudgetAmount.BudgetAmountId,
                    EntityType = MyEntityType.BudgetAmount,
                    ActionType = ActionType.Update,
                    ChangedBy = logUserId,
                    ChangeTime = DateTime.Now,
                    Values = originalSecondBudgetAmount
                };

                _EntityLogRepository.Add(firstLog);
                _EntityLogRepository.Add(secondLog);

                return Ok("success");
            }

            //var BudgetAmount = _budgetAmountRepository.GetByCondition(BudgetAmount => BudgetAmount.ID1 == request.ID1).FirstOrDefault();
            var updatedBudgetAmount = _budgetAmountRepository.GetById(request!.BudgetAmountId);
            if (updatedBudgetAmount == null)
            {
                return NotFound("not exist");
            }

            var originalBudgetAmountLog = JsonSerializer.Serialize(updatedBudgetAmount);

            // 更新Status欄位
            updatedBudgetAmount.IsValid = false;
            _budgetAmountRepository.Update(updatedBudgetAmount);

            // log
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserID")?.Value ?? "Unknown";
            var log = new EntityLog
            {
                EntityId = updatedBudgetAmount.BudgetAmountId,
                EntityType = MyEntityType.BudgetAmount,
                ActionType = ActionType.Update,
                ChangedBy = userId,
                ChangeTime = DateTime.Now,
                Values = originalBudgetAmountLog
            };

            _EntityLogRepository.Add(log);

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
    public IActionResult SearchDeletedRecords(int createdYear, string? description, string? requestPerson, string? paymentPerson)
    {
        try
        {
            Expression<Func<BudgetAmount, bool>> condition = item => true;
            condition = condition.And(BudgetAmount => BudgetAmount.CreatedYear == createdYear && BudgetAmount.IsValid == false);

            if (!string.IsNullOrEmpty(description))
            {
                condition = condition.And(BudgetAmount => BudgetAmount.Description != null && BudgetAmount.Description.Contains(description));
            }

            if (!string.IsNullOrEmpty(requestPerson))
            {
                condition = condition.And(BudgetAmount => BudgetAmount.RequestPerson != null && BudgetAmount.RequestPerson == requestPerson);
            }

            if (!string.IsNullOrEmpty(paymentPerson))
            {
                condition = condition.And(BudgetAmount => BudgetAmount.PaymentPerson != null && BudgetAmount.PaymentPerson == paymentPerson);
            }

            var deletedRecords = _budgetAmountRepository.GetByCondition(condition)
                .Include(BA => BA.Budget)
                .ThenInclude(B => B!.Group)
                .ToList();

            // 將 Status 欄位的多餘空格移除
            var cleanedRecords = deletedRecords
                //.Select(record =>
                //{
                //    record.Status = record.Status.Trim();
                //    return record;
                //})
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
        {   if(request != null && request.Type != AmountType.Ordinary)
            {
                var updatedFirstBudgetAmount = _budgetAmountRepository.GetById(request.BudgetAmountId);
                var updatedSecondBudgetAmount = _budgetAmountRepository.GetByCondition(BudgetAmount => BudgetAmount.BudgetAmountId == request.LinkedBudgetAmountId).FirstOrDefault();
                if (updatedFirstBudgetAmount == null)
                {
                    return NotFound("not exist");
                }
                if (updatedFirstBudgetAmount != null &&　updatedSecondBudgetAmount == null)
                {
                    var originalForOnlyOneLog = JsonSerializer.Serialize(updatedFirstBudgetAmount);
                    updatedFirstBudgetAmount.IsValid = true;
                    _budgetAmountRepository.Update(updatedFirstBudgetAmount);
                    var forLogUserId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserID")?.Value ?? "Unknown";
                    var onlyOneLog = new EntityLog
                    {
                        EntityId = updatedFirstBudgetAmount.BudgetAmountId,
                        EntityType = MyEntityType.BudgetAmount,
                        ActionType = ActionType.Update,
                        ChangedBy = forLogUserId,
                        ChangeTime = DateTime.Now,
                        Values = originalForOnlyOneLog // 儲存更新前的值
                    };
                    _EntityLogRepository.Add(onlyOneLog);
                    return Ok("the second data does not exist");
                }
                var originalFirstBudgetAmountLog = JsonSerializer.Serialize(updatedFirstBudgetAmount);
                var originalSecondBudgetAmountLog = JsonSerializer.Serialize(updatedSecondBudgetAmount);
                updatedFirstBudgetAmount!.IsValid = true;
                updatedSecondBudgetAmount!.IsValid = true;
                _budgetAmountRepository.Update(updatedFirstBudgetAmount);
                _budgetAmountRepository.Update(updatedSecondBudgetAmount);

                // log
                var logUserId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserID")?.Value ?? "Unknown";
                var firstLog = new EntityLog
                {
                    EntityId = updatedFirstBudgetAmount.BudgetAmountId,
                    EntityType = MyEntityType.BudgetAmount,
                    ActionType = ActionType.Update,
                    ChangedBy = logUserId,
                    ChangeTime = DateTime.Now,
                    Values = originalFirstBudgetAmountLog
                };

                var secondLog = new EntityLog
                {
                    EntityId = updatedSecondBudgetAmount.BudgetAmountId,
                    EntityType = MyEntityType.BudgetAmount,
                    ActionType = ActionType.Update,
                    ChangedBy = logUserId,
                    ChangeTime = DateTime.Now,
                    Values = originalSecondBudgetAmountLog
                };

                _EntityLogRepository.Add(firstLog);
                _EntityLogRepository.Add(secondLog);

                return Ok("success");
            }

            if (request == null || request.BudgetAmountId <= 0)
            {
                return BadRequest("Invalid request.");
            }

            //var entity =  _context.BudgetAmount.Find(request.ID1); // 因為find方法是透過主鍵去搜尋,ID1不是主鍵,找不到資料
            var updatedBudgetAmount = _budgetAmountRepository.GetById(request.BudgetAmountId);
            //var entity = _budgetAmountRepository.GetByCondition(e => e.BudgetAmountId == request.BudgetAmountId).FirstOrDefault();

            if (updatedBudgetAmount == null)
            {
                return NotFound("Record not found.");
            }

            var originalBudgetAmountLog = JsonSerializer.Serialize(updatedBudgetAmount);

            updatedBudgetAmount.IsValid = true;
            _budgetAmountRepository.Update(updatedBudgetAmount);

            // log
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserID")?.Value ?? "Unknown";
            var log = new EntityLog
            {
                EntityId = updatedBudgetAmount.BudgetAmountId,
                EntityType = MyEntityType.BudgetAmount,
                ActionType = ActionType.Update,
                ChangedBy = userId,
                ChangeTime = DateTime.Now,
                Values = originalBudgetAmountLog // 儲存更新前的資料
            };

            _EntityLogRepository.Add(log);

            return Ok("IsValid updated to 'O'.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    /// <summary>
    /// Groups的預算資料查詢
    /// </summary>
    /// <returns>查詢結果</returns>
    [HttpGet("ByLinkBudgetData")]
    public IActionResult GetLinkData(int LinkedBudgetAmountId) // 前端傳Year值,後端回傳符合Year值的工務組資料
    {
        Expression<Func<BudgetAmount, bool>> condition = item => true;
        condition = BudgetAmount => BudgetAmount.Budget != null && BudgetAmount.BudgetAmountId == LinkedBudgetAmountId;
        condition = condition.And(BudgetAmount => BudgetAmount.IsValid == true);
        try
        {
            var results = _budgetAmountRepository.GetByCondition(condition)
            .Include(BudgetAmount => BudgetAmount.Budget)
            .ThenInclude(Budget => Budget!.Group)
            .FirstOrDefault();
            // .OrderBy(BudgetAmount => BudgetAmount.Budget!.BudgetName)
            //.ToList();

            //List<BudgetAmountViewModel> budgetAmountViewModels = _mapper.Map<List<BudgetAmountViewModel>>(results);
            BudgetAmountViewModel budgetAmountViewModel = _mapper.Map<BudgetAmountViewModel>(results);
            return Ok(budgetAmountViewModel);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

    [HttpPost("ExportToExcel")]
    public async Task<IActionResult> ExportToExcel([FromBody] BudgetAmountExcelViewModel request)
    {
        //using (var stream = new MemoryStream())
        //{
        //    workbook.Write(stream);
        //    var content = stream.ToArray();

        //    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export.xlsx");
        //}
        if (request == null || request.BudgetId <= 0)
        {
            return BadRequest("Invalid request data.");
        }

        try
        {
            var result = GetDetailData(request.BudgetId);

            if (result is OkObjectResult okResult && okResult.Value is List<BudgetAmount> dataDetail) // IActionResult返回型別確認是否為List<BudgetAmount>
            {
                var excelFile = _budgetAmountExcelExportService.ExportBudgetAmountToExcel(request, dataDetail);
                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export.xlsx");
            }
            else
            { return BadRequest();  }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }


    [HttpGet("ExportBudgetExcel")]
    public IActionResult ExportBudgetExcel([FromQuery] ExportBudgetRequestViewModel request)
    {
        try
        {
            var excelFile = _exportBudgetExcelService.ExportBudgetToExcel(request);
            return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export.xlsx");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

    [HttpGet("ExportFundExcel")]
    public IActionResult ExportFundExcel([FromQuery] ExportFundRequestViewModel request)
    {
        try
        {
            var excelFile = _exportFundExcelService.ExportFundToExcel(request);
            return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export.xlsx");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

    /// <summary>
    /// BudgetAmountForExcel資料查詢
    /// </summary>
    /// <returns>查詢結果</returns>
    [HttpGet("BudgetAmountForExcel")]
    public IActionResult BudgetAmountForExcel(int BudgetId)
    {
        Expression<Func<BudgetAmount, bool>> condition = item => true;
        //condition = condition.And(BudgetAmount => BudgetAmount.Status != null && (BudgetAmount.Status.Trim() == "O" || BudgetAmount.Status.Trim() == "C"));
        condition = condition.And(BudgetAmount => BudgetAmount.IsValid == true);
        try
        {
            var results = _budgetAmountRepository.GetByCondition(condition)
           .Include(b => b.Budget) // 確保包含 Budget 表資料
           .Where(b => b.BudgetId == BudgetId)
           .GroupBy(b => new
           {
               b.Budget!.Subject6,
               b.Budget!.Subject7,
               b.Budget!.Subject8,
               b.Budget!.AnnualBudgetAmount,
               b.Budget!.FinalBudgetAmount
           })
           .Select(g => new
           {
               g.Key.Subject6,
               g.Key.Subject7,
               g.Key.Subject8,
               g.Key.AnnualBudgetAmount,
               g.Key.FinalBudgetAmount,
               General = g.Sum(b => b.Type == AmountType.Ordinary ? b.RequestAmount : 0),
               Out = g.Sum(b => b.Type == AmountType.BalanceOut ? b.RequestAmount : 0),
               UseBudget = (g.Key.AnnualBudgetAmount -
                               g.Sum(b => b.Type == AmountType.BalanceOut ? b.RequestAmount : 0) -
                               g.Sum(b => b.Type == AmountType.Ordinary ? b.RequestAmount : 0)) + g.Key.FinalBudgetAmount,
               In = g.Sum(b => b.Type == AmountType.BalanceIn ? b.RequestAmount : 0),
               InActual = g.Sum(b => b.Type == AmountType.BalanceIn ? b.PaymentAmount : 0),
               InBalance = g.Sum(b => b.Type == AmountType.BalanceIn ? b.RequestAmount : 0) - g.Sum(b => b.Type == AmountType.BalanceIn ? b.PaymentAmount : 0),
               SubjectActual = g.Sum(b => b.Type == AmountType.BalanceIn ? b.PaymentAmount : 0) + g.Sum(b => b.Type == AmountType.Ordinary ? b.PaymentAmount : 0),
               InUseBudget = g.Key.AnnualBudgetAmount - g.Sum(b => b.Type == AmountType.BalanceOut ? b.RequestAmount : 0) - g.Sum(b => b.Type == AmountType.Ordinary ? b.RequestAmount : 0) +
                                 g.Sum(b => b.Type == AmountType.BalanceIn ? b.RequestAmount : 0) - g.Sum(b => b.Type == AmountType.BalanceIn ? b.PaymentAmount : 0),
               END = g.Key.AnnualBudgetAmount + g.Key.FinalBudgetAmount - g.Sum(b => b.Type == AmountType.Ordinary ? b.RequestAmount : 0) - g.Sum(b => b.Type == AmountType.BalanceOut ? b.RequestAmount : 0) + g.Sum(b => b.Type == AmountType.BalanceIn ? b.RequestAmount : 0)
           })
           .ToList();

            //List<BudgetAmountViewModel> budgetAmountViewModels = _mapper.Map<List<BudgetAmountViewModel>>(results);
            return Ok(results);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }


    /// <summary>
    /// Fund執行表資料
    /// </summary>
    /// <returns>查詢結果</returns>
    [HttpGet("ExportFundForExcel")]
    public IActionResult ExportFund(int year, int startMonth, int endMonth, string requestPerson, string sectionCode)
    {
        // 將民國年轉換為西元年
        int westernYear = year + 1911;

        Expression<Func<BudgetAmount, bool>> condition = item => true;
        //condition = condition.And(item => item.Status != null && item.Status.Trim() == "O");
        condition = condition.And(item => item.Reconciled == true);
        condition = condition.And(item => item.PaymentDate >= new DateTime(westernYear, startMonth, 1) && item.PaymentDate <= new DateTime(westernYear, endMonth, DateTime.DaysInMonth(westernYear, endMonth)));
        condition = condition.And(item => item.RequestPerson == requestPerson);
        condition = condition.And(item => item.AmountYear == year);
        condition = condition.And(item => item.Budget!.BudgetName.Substring(0, 2) == sectionCode);
        condition = condition.And(item => item.Budget!.GroupId == 1);

        try
        {
            var results = _budgetAmountRepository.GetByCondition(condition)
                .GroupBy(item => new
                {
                    Name = item.Budget!.BudgetName.Substring(0, 2),
                    item.RequestPerson
                })
                .Select(g => new
                {
                    g.Key.Name,
                    g.Key.RequestPerson,
                    Money = g.Sum(x => (x.Type == AmountType.Ordinary || x.Type == AmountType.BalanceIn) ? x.PaymentAmount : 0)
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
    /// Fund執行表資料
    /// </summary>
    /// <returns>查詢結果</returns>
    [HttpGet("ExportFundPersonForExcel")]
    public IActionResult ExportRequestPerson(int year, int startMonth, int endMonth, UserSystemType system)
    {
        // 將民國年轉換為西元年
        int westernYear = year + 1911;

        Expression<Func<BudgetAmount, bool>> condition = item => true;
        condition = condition.And(item => item.Reconciled == true);
        condition = condition.And(item => item.PaymentDate >= new DateTime(westernYear, startMonth, 1) && item.PaymentDate <= new DateTime(westernYear, endMonth, DateTime.DaysInMonth(westernYear, endMonth)));
        condition = condition.And(item => item.AmountYear == year);

        try
        {    // 取得符合條件的 BudgetAmount 資料
            var budgetAmounts = _budgetAmountRepository.GetByCondition(condition)
                .ToList();

            // 取得所有符合 System 的 User 資料
            var users = _userRepository.GetByCondition(user => user.System == system)
                .ToList();

            // 使用 LINQ Join 進行連接
            var results = budgetAmounts
                .Join(users,
                      budget => budget.RequestPerson,
                      user => user.Name,
                      (budget, user) => new
                      {
                          user.Name
                          // 可以選擇其他需要的欄位
                      })
                 .Select( u => u.Name)
                .Distinct() // 確保結果不重複
                .ToList();

            return Ok(results);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

}




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
using AirportBudget.Server.ViewModels;
using System.Text.RegularExpressions;
using MathNet.Numerics.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using LinqKit;



namespace AirportBudget.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BudgetController(IGenericRepository<Budget> budgetRepository, IMapper mapper) : ControllerBase
{
    private readonly IGenericRepository<Budget> _budgetRepository = budgetRepository;
    private readonly IMapper _mapper = mapper;

    [HttpGet("GetBudgetId")]
    public IActionResult GetBudgetId(int GroupId, string Subject6, string Subject7, string? Subject8, int CreatedYear) 
    {
        try
        {
            Expression<Func<Budget, bool>> condition = item => true;
            condition = condition.And(Budget => Budget.GroupId == GroupId && Budget.Subject6 == Subject6);
            condition = condition.And(Budget => Budget.Subject7 == Subject7 && Budget.CreatedYear == CreatedYear);
            if (!string.IsNullOrEmpty(Subject8))
            {
                condition = condition.And(Budget => Budget.Subject8 == Subject8);
            }


            var result = _budgetRepository.GetByCondition(condition).FirstOrDefault();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

    [HttpGet("GetSubjects6")]
    public IActionResult GetSubject6(int groupId, int year, string subject6)
    {
        try
        {
            Expression<Func<Budget, bool>> condition = item => true;
            condition = condition.And(b => b.GroupId == groupId && b.CreatedYear == year && b.Subject6 == subject6);

            // 使用 LINQ 查詢篩選條件
            var subject6s = _budgetRepository.GetByCondition(condition)
                                            //.Where(b => b.Subject6.StartsWith(subject6.Substring(0, 2)))  // 加入前兩位字元篩選條件
                                            .Select(b => b.Subject6)  // 只選取 Subject6 欄位
                                            .Distinct()               // 去重複
                                            .ToList();                // 轉換為 List
                                                                      // 檢查結果是否為空
            if (!subject6s.Any())
            {
                return Ok("這個組室沒有指定科目!");
            }

            return Ok(subject6s);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

    [HttpGet("GetSubjects7")]
    public IActionResult GetSubject7(int groupId, int year, string subject6)
    {
        try
        {
            Expression<Func<Budget, bool>> condition = item => true;
            condition = condition.And(b => b.GroupId == groupId && b.CreatedYear == year && b.Subject6 == subject6);

            // 使用 LINQ 查詢篩選條件
            var subject6s = _budgetRepository.GetByCondition(condition)
                                            .Select(b => b.Subject7)  // 只選取 Subject7 欄位
                                            .Distinct()               // 去重複
                                            .ToList();                // 轉換為 List

            return Ok(subject6s);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

    [HttpGet("GetSubjects8")]
    public IActionResult GetSubject8(int groupId, int year, string subject7)
    {
        try
        {
            Expression<Func<Budget, bool>> condition = item => true;
            condition = condition.And(b => b.GroupId == groupId && b.CreatedYear == year && b.Subject7 == subject7);

            // 使用 LINQ 查詢篩選條件
            // 使用 LINQ 查詢並篩選條件，排除空字串
            var subject6s = _budgetRepository.GetByCondition(condition)
                                             .Where(b => !string.IsNullOrEmpty(b.Subject8))  // 排除空字串
                                             .Select(b => b.Subject8)  // 只選取 Subject8 欄位
                                             .Distinct()               // 去重複
                                             .ToList();                // 轉換為 List
            return Ok(subject6s);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

    [HttpGet("BudgetIdForExcel")]
    public IActionResult GetBudgetId(int GroupId, int Year, string Subject6)
    {
        try
        {
            Expression<Func<Budget, bool>> condition = item => true;
            condition = condition.And(b => b.GroupId == GroupId && b.CreatedYear == Year && b.Subject6 == Subject6);

            var budgetId = _budgetRepository.GetByCondition(condition)
                                            .Select(b => b.BudgetId)  
                                            .ToList();               

            return Ok(budgetId);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }
}




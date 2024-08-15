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

    [HttpGet("GetSubject6")]
    public IActionResult GetSubject6(int GroupId, int Year)
    {
        try
        {
            Expression<Func<Budget, bool>> condition = item => true;
            condition = condition.And(b => b.GroupId == GroupId && b.CreatedYear == Year);

            // 使用 LINQ 查詢篩選條件
            var subject6s = _budgetRepository.GetByCondition(condition)
                                            .Select(b => b.Subject6)  // 只選取 Subject6 欄位
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




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
            condition = condition.And(Budget => Budget.GroupId == GroupId && Budget.Subject6.Contains(Subject6));
            condition = condition.And(Budget => Budget.Subject7.Contains(Subject7) && Budget.CreatedYear == CreatedYear);
            if (!string.IsNullOrEmpty(Subject8))
            {
                condition = condition.And(Budget => Budget.Subject8.Contains(Subject8));
            }


            var result = _budgetRepository.GetByCondition(condition).FirstOrDefault();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

}




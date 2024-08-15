using Microsoft.AspNetCore.Mvc;
using AirportBudget.Server.Interfaces.Repositorys;
using AirportBudget.Server.Models;
using AutoMapper;
using AirportBudget.Server.Datas;
using Microsoft.EntityFrameworkCore;
using NPOI.XSSF.UserModel; // NPOI 的 Excel 套件
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NPOI.SS.Formula.Functions;
using AirportBudget.Server.Mappings;
using AutoMapper.QueryableExtensions;
using AirportBudget.Server.DTOs;
using AirportBudget.Server.ViewModels;
using System.Text.RegularExpressions;
using AirportBudget.Server.Repositorys;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Expressions;
using LinqKit;



namespace AirportBudget.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class Subject7Controller(IGenericRepository<Subject7> subject7, IMapper mapper) : ControllerBase
{
    private readonly IGenericRepository<Subject7> _subject7 = subject7;
    private readonly IMapper _mapper = mapper;

    [HttpGet("Subjects7")]
    public async Task<IActionResult> GetSubject7(int subject6Id)
    {
        try
        {
            Expression<Func<Subject7, bool>> condition = item => true;
            condition = condition.And(s => s.Subject6Id != 0 && s.Subject6Id == subject6Id);
            if (subject6Id == 0)
            {
                return NotFound("傳輸的subject6為0");
            }
            var Subjects7 = await _subject7.GetByCondition(condition).ToListAsync();
            return Ok(Subjects7);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
        }
    }

}




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



namespace AirportBudget.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class Subject6Controller(IGenericRepository<Subject6> Subject6, IMapper mapper) : ControllerBase
{
    private readonly IGenericRepository<Subject6> _Subject6 = Subject6;
    private readonly IMapper _mapper = mapper;
    [HttpGet("Subjects6")]
    public async Task<IActionResult> GetSubject6(int? groupId)
    {
        try
        {
            var Subjects6 = await _Subject6.GetByCondition(t => t.GroupId == groupId).ToListAsync();
            return Ok(Subjects6);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
        }
    }


    [HttpGet("Subjects6_1")]
    public async Task<IActionResult> GetSubject6_1(int? groupId, string? id)
    {
        try
        {
            if (groupId == null || string.IsNullOrEmpty(id))
            {
                return Ok("這個組室沒有指定科目!");
            }
            var Subjects6_1 = await _Subject6.GetByCondition(s => s.GroupId == groupId && s.Subject6SerialCode != null && s.Subject6SerialCode == id).ToListAsync();

            if (Subjects6_1 == null || !Subjects6_1.Any())
            {
                return Ok("這個組室沒有指定科目!");
            }
            return Ok(Subjects6_1);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
        }
    }

}




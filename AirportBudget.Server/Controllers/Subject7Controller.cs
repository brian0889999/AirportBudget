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
public class Subject7Controller(IGenericRepository<Subject7> Subject7, IMapper mapper) : ControllerBase
{
    private readonly IGenericRepository<Subject7> _Subject7 = Subject7;
    private readonly IMapper _mapper = mapper;

    [HttpGet("Subjects7")]
    public async Task<IActionResult> GetSubject7(int? groupId, string? id)
    {
        try
        {
            if (groupId == null || string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var Subjects7 = await _Subject7.GetByCondition(s => s.GroupId == groupId && s.Subject7SerialCode != null && s.Subject7SerialCode.Substring(0, id.Length) == id).ToListAsync();
            return Ok(Subjects7);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
        }
    }

}




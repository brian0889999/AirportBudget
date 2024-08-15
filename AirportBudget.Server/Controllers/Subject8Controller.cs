﻿using Microsoft.AspNetCore.Mvc;
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
public class Subject8Controller(IGenericRepository<Subject8> subject8, IMapper mapper) : ControllerBase
{
    private readonly IGenericRepository<Subject8> _subject8 = subject8;
    private readonly IMapper _mapper = mapper;

    [HttpGet("Subjects8")]
    public async Task<IActionResult> GetTypes3(int subject7Id)
    {
        Expression<Func<Subject8, bool>> condition = item => true;
        condition = condition.And(s => s.Subject7Id == subject7Id);

        try
        {
            //if(string.IsNullOrEmpty(group) || string.IsNullOrEmpty(id))
            //{
            //    return NotFound();
            //}
            var Subjects8 = await _subject8.GetByCondition(condition).ToListAsync();
            return Ok(Subjects8);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
        }
    }

}




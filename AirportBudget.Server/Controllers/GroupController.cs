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
public class GroupController(IGenericRepository<Models.Group> groupRepository, IMapper mapper) : ControllerBase
{
    private readonly IGenericRepository<Models.Group> _groupRepository = groupRepository;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// 取得 群組
    /// </summary>
    /// <returns>查詢結果</returns>
    [HttpGet]
    public IActionResult GetGroup() 
    {
        try
        {
            //Expression<Func<Budget, bool>> condition = item => true;
            //condition = condition.And();

            var result = _groupRepository.GetAll().ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

    /// <summary>
    /// 取得 群組選單
    /// </summary>
    /// <returns>查詢結果</returns>
    [HttpGet("SelectedOption")]
    public IActionResult GetSelectedOption()
    {
        try
        {
            var groups = _groupRepository.GetAll().ToList();

            var selectedOptionViewModels = groups.Select(group => new SelectedOptionViewModel
            {
                text = group.GroupName,
                value = group.GroupId
            });
            return Ok(selectedOptionViewModels);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }
}




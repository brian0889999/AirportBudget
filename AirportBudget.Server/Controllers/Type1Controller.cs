//using Microsoft.AspNetCore.Mvc;
//using AirportRenovate.Server.Interfaces.Repositorys;
//using AirportRenovate.Server.Models;
//using AutoMapper;
//using AirportRenovate.Server.Datas;
//using Microsoft.EntityFrameworkCore;
//using NPOI.XSSF.UserModel; // NPOI 的 Excel 套件
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using NPOI.SS.Formula.Functions;
//using AirportRenovate.Server.Mappings;
//using AutoMapper.QueryableExtensions;
//using AirportRenovate.Server.DTOs;
//using AirportRenovate.Server.ViewModels;
//using System.Text.RegularExpressions;
//using AirportRenovate.Server.Repositorys;
//using Microsoft.AspNetCore.Authorization;



//namespace AirportRenovate.Server.Controllers;

//[Authorize]
//[ApiController]
//[Route("api/[controller]")]
//public class Type1Controller(IGenericRepository<Type1> Type1, IMapper mapper) : ControllerBase
//{
//    private readonly IGenericRepository<Type1> _Type1 = Type1;
//    private readonly IMapper _mapper = mapper;
//    [HttpGet("Subjects6")]
//    public async Task<IActionResult> GetTypes1(string? group)
//    {
//        try
//        {
//            var Subjects6 = await _Type1.GetByCondition(t => t.Group == group).ToListAsync();
//            return Ok(Subjects6);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
//        }
//    }


//    [HttpGet("Subjects6_1")]
//    public async Task<IActionResult> GetTypes1_1(string? group, string? id)
//    {
//        try
//        {
//            if (string.IsNullOrEmpty(group) || string.IsNullOrEmpty(id))
//            {
//                return Ok("這個組室沒有指定科目!");
//            }
//            var Subjects6_1 = await _Type1.GetByCondition(t => t.Group == group && t.ID != null && t.ID == id).ToListAsync();

//            if (Subjects6_1 == null || !Subjects6_1.Any())
//            {
//                return Ok("這個組室沒有指定科目!");
//            }
//            return Ok(Subjects6_1);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
//        }
//    }

//}




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
//public class Type2Controller(IGenericRepository<Type2> Type2, IMapper mapper) : ControllerBase
//{
//    private readonly IGenericRepository<Type2> _Type2 = Type2;
//    private readonly IMapper _mapper = mapper;
    
//    [HttpGet("Subjects7")]
//    public async Task<IActionResult> GetTypes2(string? group, string? id)
//    {
//        try
//        {
//            if(string.IsNullOrEmpty(group) || string.IsNullOrEmpty(id))
//            {
//                return NotFound();
//            }
//            var Subjects7 = await _Type2.GetByCondition(t => t.Group == group && t.ID != null && t.ID.Substring(0, id.Length) == id).ToListAsync();
//            return Ok(Subjects7);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
//        }
//    }

//}




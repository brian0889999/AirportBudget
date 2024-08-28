using Microsoft.AspNetCore.Mvc;
using AirportBudget.Server.Interfaces.Repositorys;
using AirportBudget.Server.Models;
using AutoMapper;
using AirportBudget.Server.Datas;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AirportBudget.Server.Mappings;
using AutoMapper.QueryableExtensions;
using AirportBudget.Server.DTOs;
using AirportBudget.Server.ViewModels;
using AirportBudget.Server.Repositorys;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Expressions;
using LinqKit;
using Newtonsoft.Json;
using AirportBudget.Server.Enums;



namespace AirportBudget.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EntityLogController(IGenericRepository<EntityLog> entityLog, IMapper mapper) : ControllerBase
{
    private readonly IGenericRepository<EntityLog> _entityLog = entityLog;
    private readonly IMapper _mapper = mapper;
   

    [HttpGet]
    public IActionResult GetLogs(int? entityId, MyEntityType? myEntityType)
    {
        try
        {
            Expression<Func<EntityLog, bool>> condition = item => true;

            // 如果 entityId 有值，加入條件
            if (entityId.HasValue)
            {
                condition = condition.And(item => item.EntityId == entityId.Value);
            }

            // 如果 myEntityType 有值，加入條件
            if (myEntityType.HasValue)
            {   // 將 myEntityType 枚舉轉換為 int 進行比較
                condition = condition.And(item => (int)item.EntityType == (int)myEntityType.Value);
            }

            var logs = _entityLog.GetByCondition(condition);

            var deserializedValuesList = logs
                .Select(log => JsonConvert.DeserializeObject<Dictionary<string, object>>(log.Values))
                .ToList();

            return Ok(deserializedValuesList);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
        }
    }

}




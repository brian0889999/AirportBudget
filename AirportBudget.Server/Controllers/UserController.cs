using Microsoft.AspNetCore.Mvc;
using AirportBudget.Server.Datas;
using AirportBudget.Server.Utilities;
using AirportBudget.Server.ViewModels;
using AirportBudget.Server.Models;
using AirportBudget.Server.Interfaces.Repositorys;
using Microsoft.AspNetCore.Authorization;
using AirportBudget.Server.DTOs;
using AirportBudget.Server.Services;
using AutoMapper;
using NPOI.SS.Formula.Functions;
using NPOI.OpenXmlFormats.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using LinqKit;

namespace AirportBudget.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly IGenericRepository<User> _userRepository;
    private readonly DESEncryptionUtility _dESEncryptionUtility;
    private readonly IMapper _mapper;
    public UserController(TokenService tokenService, IGenericRepository<User> userRepository, DESEncryptionUtility dESEncryptionUtility, IMapper mapper)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
        _dESEncryptionUtility = dESEncryptionUtility;
        _mapper = mapper;
    }

    /// <summary>
    /// 取得 使用者
    /// </summary>
    /// <returns>查詢結果</returns>
    [HttpGet]
    public ActionResult<IEnumerable<User>> FetchUsers()
    {
        var usersData = _userRepository.GetAll().ToList();
        List<User> users = _mapper.Map<List<User>>(usersData);

        foreach (var user in users)
        {
            if (!string.IsNullOrEmpty(user.Password))
            {
                user.Password = _dESEncryptionUtility.DecryptDES(user.Password.Trim());
            }
        }

        return Ok(users);
    }

    /// <summary>
    /// 新增 使用者
    /// </summary>
    /// <returns>新增結果</returns>
    [HttpPost]
    public IActionResult AddUser([FromBody] UserViewModel request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest("沒有user資料");
            }
            else
            {
                // 如果 No 欄位的值是 0，則從資料庫中查找最大值並加 1
                //if (request.No == 0)
                //{
                //    Expression<Func<User, bool>> condition = u => true;
                //    var maxNo = await _userRepository.GetByCondition(condition)
                //                                .OrderByDescending(u => u.UserId)
                //                                .Select(u => u.UserId)
                //                                .FirstOrDefaultAsync();
                //    request.No = maxNo + 1;
                //}
                if (!string.IsNullOrEmpty(request.Password))
                {
                    request.Password = _dESEncryptionUtility.EncryptDES(request.Password.Trim());
                }

                User newUser = _mapper.Map<User>(request);

                _userRepository.Add(newUser);
                return Ok(newUser);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// 更新 使用者
    /// </summary>
    /// <returns>更新結果</returns>
    [HttpPut]
    public IActionResult UpdateUser([FromBody] UserViewModel request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest("沒有user資料");
            }
            else
            {
                // 根據UserId去找資料
                Expression<Func<User, bool>> condition = u => true;
                condition = condition.And(u => u.UserId == request.UserId);

                var existingUser = _userRepository.GetByCondition(condition).Include(u => u.RolePermission).FirstOrDefault();

                if (existingUser == null)
                {
                    return NotFound("沒有找到user");
                }

                if (!string.IsNullOrEmpty(request.Password))
                {
                    request.Password = _dESEncryptionUtility.EncryptDES(request.Password.Trim());
                }

                _mapper.Map(request, existingUser);
                _userRepository.Update(existingUser);

                return Ok(existingUser);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    /// <summary>
    /// 取得 使用者
    /// </summary>
    /// <returns>查詢結果</returns>
    [HttpGet("Current")]
    public IActionResult GetCurrent()
    {
        try
        {
            UserDTO userDTO = _tokenService.GetUser(User);

            var userInfo = _userRepository.GetByCondition(item => item.UserId == userDTO.UserId)
                .Include(u => u.RolePermission)
                .Include(u => u.Group)
                .FirstOrDefault();

            //if (user.Password != null)
            //{
            //    user.Password = _dESEncryptionUtility.DecryptDES(user.Password);
            //}
            User formatUserInfo = _mapper.Map<User>(userInfo);

            return Ok(formatUserInfo);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }


}

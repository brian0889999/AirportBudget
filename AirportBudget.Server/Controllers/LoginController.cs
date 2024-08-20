using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AirportBudget.Server.Datas;
using AirportBudget.Server.Utilities;
using AirportBudget.Server.ViewModels;
using AirportBudget.Server.Models;
using AirportBudget.Server.Interfaces.Repositorys;
using System.Linq.Expressions;
using AirportBudget.Server.Services;
using LinqKit;

namespace AirportBudget.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController(IGenericRepository<User> userRepository, DESEncryptionUtility dESEncryptionUtility, TokenService tokenService) : ControllerBase
{
    private readonly IGenericRepository<User> _userRepository = userRepository;
    private readonly DESEncryptionUtility _dESEncryptionUtility = dESEncryptionUtility;
    private readonly TokenService _tokenService = tokenService;

    //[HttpPost]
    //public IActionResult LoginTest([FromBody] LoginViewModel loginData)
    //{  /* var isAuthenticated = */

    //    if (!string.IsNullOrEmpty(loginData.Account) && !string.IsNullOrEmpty(loginData.Password))
    //    {

    //        return Ok("ok"); // 回傳成功的訊息及用戶相關資訊
    //    }
    //    else
    //    {
    //        return Unauthorized("登入失敗"); // 登入失敗，回傳未授權的訊息
    //    }
    //}

    //[HttpGet]

    //public async Task<IActionResult> GetUsers()
    //{
    //    try
    //    {
    //        var users = await _users.GetAllAsync();
    //        foreach (var user in users)
    //        {
    //            if (user.Password != null)
    //            {
    //                user.Password = _dESEncryptionUtility.DecryptDES(user.Password);
    //            }
    //        }
    //        return Ok(users);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex}");
    //    }
    //}


    /// <summary>
    /// 使用者登入
    /// </summary>
    /// <returns>登入結果</returns>
    [HttpPost]

    public IActionResult Login([FromBody] LoginViewModel loginForm)
    {
        if (!string.IsNullOrEmpty(loginForm.Account) && !string.IsNullOrEmpty(loginForm.Password))
        {
            // 區分大小寫
            Expression<Func<User, bool>> condition = item =>
                EF.Functions.Collate(item.Account, "SQL_Latin1_General_CP1_CS_AS") == loginForm.Account &&
                EF.Functions.Collate(item.Password, "SQL_Latin1_General_CP1_CS_AS") == loginForm.Password;

            var password = _dESEncryptionUtility.EncryptDES(loginForm.Password);
            var user = _userRepository.GetByCondition(x => x.Account == loginForm.Account).FirstOrDefault();
            if (user != null && user.Password == password)
            {   if(user.Status == false)
                {
                    return StatusCode(201, "使用者已停用");
                }

                // 檢查密碼最後更改日期是否超過半年
                if (user.LastPasswordChangeDate.HasValue)
                {
                    var sixMonthsAgo = DateTime.Now.AddMonths(-6);
                    if (user.LastPasswordChangeDate.Value < sixMonthsAgo)
                    {   
                        var userId = _userRepository.GetByCondition(u => u.Account == loginForm.Account && u.Password == password)
                                           .Select( u => u.UserId )
                                           .FirstOrDefault();
                        //return StatusCode(202, "需要更改密碼，因為超過半年未更改密碼");
                        return StatusCode(202, new
                        {
                            message = "需要更改密碼，因為超過半年未更改密碼",
                            UserId = userId,
                        });
                    }
                }

                // 生成 JWT Token
                var jwtToken = _tokenService.GenerateJwtToken(user);

                return Ok(jwtToken);
            }
            else
            {

                return Unauthorized("登入失敗");
            }

        }
        else
        {
            return Unauthorized("未收到帳號資訊"); // 回傳登入失敗
        }
    }

    /// <summary>
    /// 更新密碼
    /// </summary>
    /// <returns>更新結果</returns>
    [HttpPut("ChangePassword")]
    public IActionResult ChangePassword([FromBody] ChangePasswordViewModel request)
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

                var existingUser = _userRepository.GetByCondition(condition)
                    .Include(u => u.Group)
                    .Include(u => u.RolePermission)
                    .FirstOrDefault();

                if (existingUser == null)
                {
                    return NotFound("沒有找到user");
                }

                // 檢查舊密碼是否正確
                if (!string.IsNullOrEmpty(request.OldPassword))
                {
                    var decryptedPassword = _dESEncryptionUtility.DecryptDES(existingUser.Password.Trim());

                    if (request.OldPassword != decryptedPassword)
                    {
                        return BadRequest("舊密碼不正確");
                    }
                }

                // 檢查新密碼和確認密碼是否相同
                if (request.NewPassword != request.ConfirmPassword)
                {
                    return BadRequest("新密碼和確認密碼不一致");
                }

                existingUser.Password = _dESEncryptionUtility.EncryptDES(request.NewPassword); // 更新密碼並加密
                existingUser.LastPasswordChangeDate = DateTime.Now; // 更新LastPasswordChangeDate

                _userRepository.Update(existingUser);

                return Ok("密碼更新成功");
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }



}

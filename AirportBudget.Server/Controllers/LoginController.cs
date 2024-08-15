﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AirportBudget.Server.Datas;
using AirportBudget.Server.Utilities;
using AirportBudget.Server.ViewModels;
using AirportBudget.Server.Models;
using AirportBudget.Server.Interfaces.Repositorys;
using System.Linq.Expressions;
using AirportBudget.Server.Services;

namespace AirportBudget.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController(IGenericRepository<User> users, DESEncryptionUtility dESEncryptionUtility, TokenService tokenService) : ControllerBase
{
    private readonly IGenericRepository<User> _users = users;
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
            var user = _users.GetByCondition(x => x.Account == loginForm.Account).FirstOrDefault();
            if (user != null && user.Password == password)
            {   if(user.Status == false)
                {
                    return StatusCode(201, "使用者已停用");
                }
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
}

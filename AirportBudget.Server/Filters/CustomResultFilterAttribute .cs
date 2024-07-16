using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AirportBudget.Server.DTOs;

namespace AirportBudget.Server.Filters;

public class CustomResultFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errorMessages = new List<string>();

            foreach (var error in context.ModelState.Values.SelectMany(v => v.Errors))
            {
                errorMessages.Add(error.ErrorMessage);
            }

            context.Result = new ObjectResult(new ApiResponse<object>
            {
                StatusCode = 400,
                Data = null,
                Message = string.Join(",", errorMessages)
            });
        }

        base.OnActionExecuting(context);
    }

    public override void OnResultExecuting(ResultExecutingContext context)
    {
        // 在執行結果之後的邏輯
        if (context.Result is ObjectResult objectResult)
        {
            // 修改或替換回傳的結果物件
            objectResult.Value = new ApiResponse<object>
            {
                StatusCode = objectResult.StatusCode,
                Data = objectResult.Value,
                Message = GetMessage(objectResult.StatusCode)
            };
        }
        else if (context.Result is StatusCodeResult statusCodeResult)
        {
            context.Result = new ObjectResult(new ApiResponse<object>
            {
                StatusCode = statusCodeResult.StatusCode,
                Data = null,
                Message = GetMessage(statusCodeResult.StatusCode)
            });
        }
        base.OnResultExecuting(context);
    }

    private static string GetMessage(int? statusCode)
    {
        if (statusCode.HasValue)
        {
            return statusCode.Value switch
            {
                200 => "成功",
                201 => "已建立",
                204 => "無內容",
                400 => "請求錯誤",
                401 => "未授權",
                403 => "禁止存取",
                404 => "找不到資源",
                408 => "操作逾時",
                500 => "內部伺服器錯誤",
                _ => "未知",
            };
        }

        return "未知";
    }
}

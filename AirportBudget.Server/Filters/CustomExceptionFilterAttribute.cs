using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AirportBudget.Server.DTOs;

namespace AirportBudget.Server.Filters;

public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        var statusCode = 500; // 設中斷點看錯誤訊息
        var errorMessage = "發生錯誤";

        // 根據異常類型或其他條件自定義錯誤訊息
        if (exception is ArgumentException)
        {
            statusCode = 400; // Bad Request
            errorMessage = "請求錯誤";
        }
        else if (exception is UnauthorizedAccessException)
        {
            statusCode = 401; // Unauthorized
            errorMessage = "未授權";
        }
        else if (exception is FileNotFoundException fileNotFoundException)
        {
            statusCode = 404; // Not Found
            errorMessage = $"找不到資源：{fileNotFoundException.FileName}";
        }
        else if (exception is TimeoutException)
        {
            statusCode = 408; // Request Timeout
            errorMessage = "操作逾時";
        }
        // 可以根據需要添加其他異常類型的處理邏輯

        // 記錄異常或執行其他額外處理
        var errorResponse = new ApiResponse<object>
        {
            StatusCode = statusCode,
            Data = null,
            Message = errorMessage
        };

        context.Result = new ObjectResult(errorResponse)
        {
            StatusCode = statusCode
        };

        base.OnException(context);
    }
}

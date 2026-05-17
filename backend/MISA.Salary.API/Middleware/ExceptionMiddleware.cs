using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MISA.Salary.Common.DTOs;
using MISA.Salary.Common.Exceptions;

namespace MISA.Salary.API.Middleware
{
    /// <summary>
    /// Middleware xử lý exception tập trung cho toàn bộ ứng dụng
    /// Bắt tất cả exception và trả về response chuẩn ServiceResult
    /// TUYỆT ĐỐI KHÔNG try/catch lẻ tẻ trong Controller
    /// Author: MISA (10/05/2026)
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Chuyển request đến middleware/controller tiếp theo
                await _next(context);
            }
            catch (ValidateException ex)
            {
                // Lỗi validate → HTTP 400 Bad Request
                await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest);
            }
            catch (NotFoundException ex)
            {
                // Không tìm thấy → HTTP 404 Not Found
                await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                // Lỗi không xác định → HTTP 500 Internal Server Error
                await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Xử lý exception và ghi response chuẩn
        /// </summary>
        private static async Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode statusCode)
        {
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = (int)statusCode;

            var result = new ServiceResult();

            if (ex is ValidateException validateEx)
            {
                // Trả về chi tiết lỗi validate (danh sách tên trường + thông báo)
                result = ServiceResult.Failure(
                    devMsg: validateEx.Message,
                    userMsg: "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại thông tin.",
                    errorCode: "VALIDATE_ERROR",
                    data: validateEx.Errors
                );
            }
            else if (ex is NotFoundException)
            {
                result = ServiceResult.Failure(
                    devMsg: ex.Message,
                    userMsg: "Không tìm thấy dữ liệu yêu cầu.",
                    errorCode: "NOT_FOUND"
                );
            }
            else
            {
                // Lỗi server: chỉ trả DevMsg cho developer, UserMsg chung chung cho user
                result = ServiceResult.Failure(
                    devMsg: ex.Message,
                    userMsg: "Có lỗi xảy ra, vui lòng liên hệ MISA để được hỗ trợ.",
                    errorCode: "INTERNAL_ERROR"
                );
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(result, jsonOptions));
        }
    }
}

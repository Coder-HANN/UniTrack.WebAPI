using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;

namespace UniTrack.WebAPI.Middleware
{
    public class ValidationExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // ✅ Scoped servis request scope'tan alınır
            var localization =
                context.RequestServices.GetRequiredService<ILocalizationService>();

            try
            {
                await _next(context);
            }
            catch (FluentValidation.ValidationException ex)
            {
                var key = ex.Errors.First().ErrorMessage;
                var message = await localization.Get(key);

                var response = new ServiceResponse<object>
                {
                    IsSuccess = false,
                    Message = message,
                    Data = null
                };

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}

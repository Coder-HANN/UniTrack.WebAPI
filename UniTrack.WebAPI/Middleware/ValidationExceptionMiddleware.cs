using MediatR;
using UniTrack.Application.Abstraction.Services.Localization;
using UniTrack.Application.Common;

namespace UniTrack.WebAPI.Middleware
{
    public class ValidationExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILocalizationService localization;

        public ValidationExceptionMiddleware(
            RequestDelegate next,
            ILocalizationService localization)
        {
            this.next = next;
            this.localization = localization;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
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


                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}

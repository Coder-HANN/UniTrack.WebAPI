namespace UniTrack.Application.Common
{
    public class ServiceResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }


        public static ServiceResponse<T> Fail(string errorCode, string message)
        {
            return new ServiceResponse<T>
            {
                IsSuccess = false,
                ErrorCode = errorCode,
                Message = message
            };
        }

        public static ServiceResponse<T> Success(string message, T data = default)
        {
            return new ServiceResponse<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

    }
}

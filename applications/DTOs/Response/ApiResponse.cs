namespace applications.DTOs.Response
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public ErrorDetail? Error { get; set; }
        public PaginationInfo? Pagination { get; set; }

        public static ApiResponse<T> SuccessResponse(T data)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Error = null
            };
        }

        public static ApiResponse<T> SuccessResponseWithPagination(T data, int page, int limit, int total)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Error = null,
                Pagination = new PaginationInfo
                {
                    Page = page,
                    Limit = limit,
                    Total = total
                }
            };
        }

        public static ApiResponse<T> ErrorResponse(int code, string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = default,
                Error = new ErrorDetail
                {
                    Code = code,
                    Message = message
                }
            };
        }
    }

    public class ErrorDetail
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class PaginationInfo
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public int Total { get; set; }
    }
}

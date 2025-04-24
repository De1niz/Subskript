public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }

    // Method to create a successful response
    public static ApiResponse<T> CreateSuccess(T data, string message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    // Method to create a failed response
    public static ApiResponse<T> CreateFail(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message
        };
    }
}
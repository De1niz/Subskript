public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }

    // Başarılı sonuç oluşturmak için metot
    public static ApiResponse<T> CreateSuccess(T data, string message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    // Hatalı sonuç oluşturmak için metot
    public static ApiResponse<T> CreateFail(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message
        };
    }
}
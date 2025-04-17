namespace ReactBackend.DTOs
{
    public class ErrorResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
    }
}
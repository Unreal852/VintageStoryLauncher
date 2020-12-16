namespace VintageStoryLauncher.Core.Auth
{
    public readonly struct AuthResult
    {
        public AuthResult(bool success, int code, string message = "")
        {
            Success = success;
            Message = message;
            Code = code;
        }

        public bool   Success { get; }
        public string Message { get; }
        public int    Code    { get; }
    }
}
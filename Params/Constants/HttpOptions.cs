namespace webapi.Params.Constants
{
    public class HttpOptions
    {
        public const string INVALID_DATA = "InvalidData";
        public const string INVALID_AUTH = "InvalidAuth";
        public const string UNAUTHORIZED = "Unauthorized";
        public const string LOGOUT = "Logout";
        public const string FORBIDDEN = "Forbidden";
        public const string NOT_FOUND = "NotFound";
        public const int JWT_LIFE_MINUTES = 60 * 24 * 7;
        public const string JWT_KEY = ".AspNetCore.Application.Id";
    }
}

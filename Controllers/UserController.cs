using Microsoft.AspNetCore.Mvc;
using webapi.Services;
using webapi.Entities;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using webapi.Params.HttpResponse;
using webapi.Params.Constants;
using webapi.Params.HttpRequest;

namespace webapi.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IWebHostEnvironment _appEnvironment;

        public UserController(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        [IgnoreAntiforgeryTokenAttribute]
        [HttpGet, Route("/api/auth")]
        public async Task<IActionResult> GetAccessByAuth()
        {
            bool result;
            string message;
            string data;

            string? userid = TryParseToken(HttpContext);

            if (userid == null)
            {
                result = false;
                message = HttpOptions.UNAUTHORIZED;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            User? user = await TryGetUserByUserid(userid);

            if (user == null)
            {
                result = false;
                message = HttpOptions.INVALID_DATA;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            UserService userService = new UserService();
            Role? role = await userService.GetRole(user.RoleId);
            AccessesParams accessesParams = new AccessesParams();

            if (role == null)
            {
                result = false;
                message = HttpOptions.INVALID_DATA;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            accessesParams.Accesses = role.Access.Split(",");

            result = true;
            message = "";
            data = JsonConvert.SerializeObject(accessesParams);
            return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
        }

        [IgnoreAntiforgeryTokenAttribute]
        [HttpPost, Route("/api/login")]
        public async Task<IActionResult> GetAccessByLogin([FromBody] AuthParams authParams)
        {
            bool result;
            string message;
            string data;

            if (authParams == null || string.IsNullOrEmpty(authParams.Login) ||string.IsNullOrEmpty(authParams.Password))
            {
                result = false;
                message = HttpOptions.INVALID_DATA;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            UserService userService = new UserService();
            User? user = await userService.FindUserAuth(authParams.Login);

            if (user == null || !VerifyHashedPassword(user.Password, authParams.Password))
            {
                result = false;
                message = HttpOptions.INVALID_AUTH;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            Role? role = await userService.GetRole(user.RoleId);
            AccessesParams accessesParams = new AccessesParams();

            if (role == null)
            {
                result = false;
                message = HttpOptions.INVALID_AUTH;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            accessesParams.Accesses = role.Access.Split(",");

            GenerateResponseJWT(HttpContext, user.UserId);

            result = true;
            message = "";
            data = JsonConvert.SerializeObject(accessesParams);
            return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
        }

        [HttpGet, Route("/api/logout")]
        public IActionResult DeleteCookies()
        {
            bool result;
            string message;
            string data;

            if (!Request.Cookies.ContainsKey(HttpOptions.JWT_KEY))
            {
                result = false;
                message = "";
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            HttpContext.Response.Cookies.Delete(HttpOptions.JWT_KEY);
            result = true;
            message = "";
            data = "";
            return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
        }

        [HttpGet, Route("/api/account")]
        public async Task<IActionResult> GetUserData()
        {
            bool result;
            string message;
            string data;

            string? userid = TryParseToken(HttpContext);

            if (userid == null)
            {
                result = false;
                message = HttpOptions.LOGOUT;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            User? user = await TryGetUserByUserid(userid);

            if (user == null)
            {
                result = false;
                message = HttpOptions.FORBIDDEN;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }


            result = true;
            message = "";
            UserDataParams userDataParams = new UserDataParams(user);
            userDataParams.Img = Convert.ToBase64String(System.IO.File.ReadAllBytes(userDataParams.Img));
            data = JsonConvert.SerializeObject(userDataParams);

             return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
        }

        [IgnoreAntiforgeryTokenAttribute]
        [HttpPost, Route("/api/account/save")]
        public async Task<IActionResult> SaveChangeUserData([FromBody] UserSaveParams userSaveParams)
        {
            bool result;
            string message;
            string data = "";

            if (userSaveParams == null)
            {
                result = false;
                message = HttpOptions.INVALID_DATA;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            string? userid = TryParseToken(HttpContext);

            if (userid == null)
            {
                result = false;
                message = HttpOptions.LOGOUT;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            User? user = await TryGetUserByUserid(userid);

            if (user == null)
            {
                result = false;
                message = HttpOptions.FORBIDDEN;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            UserService userService = new UserService();

            result = true;
            message = "";
            await userService.SaveUserData(user.UserId, userSaveParams);

            return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
        }

        [HttpGet, Route("/api/username")]
        public async Task<IActionResult> FindUserNameImgByUserid()
        {
            bool result;
            string message;
            string data;

            string? userid = TryParseToken(HttpContext);

            if (userid == null)
            {
                result = false;
                message = HttpOptions.LOGOUT;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            User? user = await TryGetUserByUserid(userid);

            if (user == null)
            {
                result = false;
                message =   HttpOptions.FORBIDDEN;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            UserNameParams userNameParams = new UserNameParams {
                SName = user.SName,
                Name = user.Name,
                MName = user.MName,
                Img = Convert.ToBase64String(System.IO.File.ReadAllBytes(user.Img)),
            };

            result = true;
            message = "";
            data = JsonConvert.SerializeObject(userNameParams);
            return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
        }

        // Users полностью???
        [HttpGet, Route("/api/userid")]
        public async Task<IActionResult> FindUserNameBySName(string sname)
        {
            bool result;
            string message;
            string data;

            string? userid = TryParseToken(HttpContext);

            if (userid == null)
            {
                result = false;
                message = HttpOptions.LOGOUT;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            User? user = await TryGetUserByUserid(userid);

            if (user == null || user.RoleId == 1)
            {
                result = false;
                message =   HttpOptions.FORBIDDEN;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            UserService userService = new UserService();
            User[] users = await userService.GetUsersBySName(user.UserId, sname);

            UserNameIDParams[] userNameIDParams = new UserNameIDParams[users.Length];
            for (int i = 0; i < userNameIDParams.Length; i++)
                userNameIDParams[i] = new UserNameIDParams(users[i]);

            if (userNameIDParams == null || userNameIDParams.Length == 0 || userNameIDParams.Length > 20)
            {
                result = false;
                message = HttpOptions.NOT_FOUND;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            result = true;
            message = "";
            data = JsonConvert.SerializeObject(userNameIDParams);
            return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
        }

        [HttpGet, Route("/api/userimg")]
        public async Task<IActionResult> FindUserImgByUserid( Guid userFoundId)
        {
            bool result;
            string message;
            string data;

            if (userFoundId.Equals(Guid.Empty))
            {
                result = false;
                message = HttpOptions.  INVALID_DATA;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            string? expertid = TryParseToken(HttpContext);

            if (expertid == null)
            {
                result = false;
                message = HttpOptions.LOGOUT;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            User? expert = await TryGetUserByUserid(expertid);

            if (expert == null || expert.RoleId == 1)
            {
                result = false;
                message =   HttpOptions.FORBIDDEN;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            UserService userService = new UserService();
            User? user = await userService.GetUser(userFoundId);

            if (user == null)
            {
                result = false;
                message = HttpOptions.  INVALID_DATA;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            result = true;
            message = "";
            string img = Convert.ToBase64String(System.IO.File.ReadAllBytes(user.Img));
            data = JsonConvert.SerializeObject(img);
            return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
        }

        [HttpGet, Route("/api/push")]
        public async Task<IActionResult> FindPushs()
        {
            bool result;
            string message;
            string data;

            string? userid = TryParseToken(HttpContext);

            if (userid == null)
            {
                result = false;
                message = HttpOptions.LOGOUT;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            User? user = await TryGetUserByUserid(userid);

            if (user == null)
            {
                result = false;
                message = HttpOptions.  FORBIDDEN;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            PushService pushService = new PushService();

            Push[] push = await pushService.GetPushes(user.UserId);

            if (push == null || push.Length == 0)
            {
                result = false;
                message = HttpOptions.NOT_FOUND;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            for (int i = 0; i < push.Length; i++)
                push[i].UserId = Guid.Empty;

            result = true;
            message = "";
            data = JsonConvert.SerializeObject(push);
            return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
        }

        [IgnoreAntiforgeryTokenAttribute]
        [HttpPost, Route("/api/push/checked")]
        public async Task<IActionResult> CheckedPush([FromBody] PushParams pushParams)
        {
            bool result;
            string message;
            string data = "";

            if (pushParams == null || pushParams.Pushid == null || pushParams.Pushid.Length == 0)
            {
                result = false;
                message = HttpOptions.INVALID_DATA;
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            string? userid = TryParseToken(HttpContext);

            if (userid == null)
            {
                result = false;
                message = HttpOptions.LOGOUT;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            User? user = await TryGetUserByUserid(userid);

            if (user == null)
            {
                result = false;
                message = HttpOptions.FORBIDDEN;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            PushService pushService = new PushService();
            await pushService.CheckedPushs(user.UserId, pushParams.Pushid);

            result = true;
            message = "";
            return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
        }

        [IgnoreAntiforgeryTokenAttribute]
        [HttpPost, Route("/account/add")]
        public async Task<IActionResult> AddAccount([FromForm] UserAddParams userForm)
        {
            bool result;
            string message;
            string data = "";

            if (userForm == null || userForm.ExpertId.Equals(Guid.Empty) || userForm.RoleId > 2 || userForm.RoleId < 0 || 
                string.IsNullOrEmpty(userForm.Login) || string.IsNullOrEmpty(userForm.Password) || string.IsNullOrEmpty(userForm.SName) ||
                string.IsNullOrEmpty(userForm.Name) || string.IsNullOrEmpty(userForm.MName) || string.IsNullOrEmpty(userForm.Job))
            {
                result = false;
                message = HttpOptions.INVALID_DATA;
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            UserService userService = new UserService();
            User newUser = new User 
            { 
                UserId = Guid.NewGuid(),
                RoleId = userForm.RoleId,

                Login = userForm.Login,
                Password = HashPassword(userForm.Password),

                SName = userForm.SName,
                MName = userForm.MName,
                Name = userForm.Name,
                Job = userForm.Job,

                Push_Offense_Email = true,
                Push_Offer_Email = true,
                Push_Offense_SMS = false,
            };
            User? expert;

            if (userForm.RoleId == 1)
            {
                if (userForm.ExpertId == null || userForm.ExpertId.Equals(Guid.Empty))
                {
                    result = false;
                    message = HttpOptions.INVALID_DATA;
                    return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
                }

                expert = await userService.GetUser((Guid)userForm.ExpertId);

                if (expert != null)
                {
                    newUser.ExpertId = expert.UserId; 
                }

                else
                {
                    result = false;
                    message = HttpOptions.INVALID_DATA;
                    return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
                }
            }

            if (userForm.Img != null)
            {
                if (!userForm.Img.ContentType.Equals("image/png") && !userForm.Img.ContentType.Equals("image/jpg") && !userForm.Img.ContentType.Equals("image/jpeg"))
                {
                    result = false;
                    message = HttpOptions.INVALID_DATA;
                    return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
                }

                string path = $"{_appEnvironment.ContentRootPath}/src/imgs/account/acc_{DateTimeOffset.Now.ToUnixTimeMilliseconds()}.{userForm.Img.FileName.Split(".")[^1]}";
                
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await userForm.Img.CopyToAsync(fileStream);
                }

                newUser.Img = path;
            }

            else
            {
                newUser.Img = $"{_appEnvironment.ContentRootPath}/src/imgs/account/acc_base_140.jpg";
            }

            await userService.CreateUser(newUser);

            result = true;
            message = "";
            return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
        }

        public static async Task<User?> TryGetUserByUserid(string useridToken)
        {
            if (Guid.TryParse(useridToken, out Guid userid))
            {
                UserService userService = new UserService();
                User? user = await userService.GetUser(userid);

                return user;
            }
            return null;
        }

        public static string? TryParseToken(HttpContext httpContext)
        {
            if (httpContext.Request.Cookies.TryGetValue(HttpOptions.JWT_KEY, out string? token))
            {
                JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

                return jwt.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier))?.Value;
            }
            return null;
        }

        private static void GenerateResponseJWT(HttpContext httpContext, Guid userId)
        {
            httpContext.Response.Cookies.Append(HttpOptions.JWT_KEY, GenerateJWT(userId), new CookieOptions
            {
                MaxAge = TimeSpan.FromMinutes(HttpOptions.JWT_LIFE_MINUTES)
            });
            httpContext.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            httpContext.Response.Headers.Add("X-Xss-Protection", "1");
            httpContext.Response.Headers.Add("X-Frame-Options", "DENY");
        }

        private static string GenerateJWT(Guid userid)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, userid.ToString()),
            };

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(HttpOptions.JWT_LIFE_MINUTES)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;

            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password.ToLower(), 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }

        private static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;
            byte[] src = Convert.FromBase64String(hashedPassword);

            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }

            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);

            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password.ToLower(), dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }

            return buffer3.SequenceEqual(buffer4);
        }
    }
}

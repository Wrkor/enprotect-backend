using Microsoft.AspNetCore.Mvc;
using webapi.Services;
using webapi.Entities;
using Newtonsoft.Json;
using System.Text;
using webapi.Params.HttpResponse;
using webapi.Params.Constants;
using webapi.Params.HttpRequest;

namespace webapi.Controllers
{
    [ApiController]
    public class OffenseController : ControllerBase
    {
        private readonly IWebHostEnvironment _appEnvironment;

        public OffenseController(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        [HttpGet, Route("/api/offense")]
        public async Task<IActionResult> GetOffenses()
        {
            bool result;
            string message;
            string data;

            string? userid = UserController.TryParseToken(HttpContext);

            if (userid == null)
            {
                result = false;
                message = HttpOptions.UNAUTHORIZED;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            User? user = await UserController.TryGetUserByUserid(userid);

            if (user == null)
            {
                result = false;
                message = HttpOptions.INVALID_DATA;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            OffenseService offenseService = new OffenseService();
            Offense[] offenses = Array.Empty<Offense>();

            if (user.RoleId == 1)
            {
                offenses = await offenseService.GetOffensesByEmployee(user.UserId);
            }

            else if (user.RoleId == 0 || user.RoleId == 2)
            {
                offenses = await offenseService.GetOffenses(user.UserId);
            }

            if (offenses == null || offenses.Length == 0)
            {
                result = false;
                message = HttpOptions.NOT_FOUND;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            List<OffensesSendParams> offensesList = new List<OffensesSendParams>();
            for (int i = 0; i < offenses.Length; i++)
                offensesList.Add(new OffensesSendParams(offenses[i]));

            result = true;
            message = "";
            data = JsonConvert.SerializeObject(offensesList);
            return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));

        }

        [HttpGet, Route("/api/offense/{offenseId}")]
        public async Task<IActionResult> GetOffense(int offenseId)
        {
            bool result;
            string message;
            string data;

            string? userid = UserController.TryParseToken(HttpContext);

            if (userid == null)
            {
                result = false;
                message = HttpOptions.UNAUTHORIZED;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            User? user = await UserController.TryGetUserByUserid(userid);

            if (user == null)
            {
                result = false;
                message = HttpOptions.INVALID_DATA;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            OffenseService offenseService = new OffenseService();
            Offense? offense = await offenseService.GetOffense(offenseId);

            if (offense == null)
            {
                result = false;
                message = HttpOptions.NOT_FOUND;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            if (user.RoleId == 1)
            {
                result = false;
                message = HttpOptions.FORBIDDEN;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            UserService userService = new UserService();

            if (!offense.ExpertId.Equals(user.UserId))
            {
                result = false;
                message = HttpOptions.FORBIDDEN;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            if (userService.GetFiles(offense.Imgs, out string[] base64Files))
            {
                result = true;
                message = "";
                offense.Imgs = JsonConvert.SerializeObject(base64Files);
                User? expert = await userService.GetUser(offense.ExpertId);

                if (expert != null)
                {
                    if (userService.GetFiles(expert.Img, out string[] base64FileExpert, 1))
                    {
                        expert.Img = JsonConvert.SerializeObject(base64FileExpert[0]);
                        OffenseSendParams offenseSendParams;

                        if (offense.EmployeeId == null || offense.EmployeeId.Equals(Guid.Empty))
                        {
                            offenseSendParams = new OffenseSendParams(offense, expert);
                            data = JsonConvert.SerializeObject(offenseSendParams);
                            return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
                        }

                        User? employee = await userService.GetUser((Guid)offense.EmployeeId);

                        if (employee != null)
                        {
                            if (userService.GetFiles(employee.Img, out string[] base64FileEmployee, 1))
                            {
                                employee.Img = JsonConvert.SerializeObject(base64FileEmployee[0]);

                                offenseSendParams = new OffenseSendParams(offense, expert, employee);
                                data = JsonConvert.SerializeObject(offenseSendParams);
                                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
                            }
                        }
                    }
                }
            }

            result = false;
            message = HttpOptions.FORBIDDEN;
            data = "";
            return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
        }

        [IgnoreAntiforgeryTokenAttribute]
        [HttpPost, Route("/api/offense/create")]
        public async Task<IActionResult> CreateOffense([FromForm] OffenseParams offense)
        {
            bool result;
            string message;
            string data = "";

            if (offense == null  || string.IsNullOrEmpty(offense.Description) || string.IsNullOrEmpty(offense.Title) || string.IsNullOrEmpty(offense.Category)
                || (offense.Category.Equals("Персонал") && (offense.EmployeeId.Equals(Guid.Empty) || offense.EmployeeId == null)))
            {
                result = false;
                message = HttpOptions.INVALID_DATA;
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            string? userid = UserController.TryParseToken(HttpContext);

            if (userid == null)
            {
                result = false;
                message = HttpOptions.UNAUTHORIZED;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            User? user = await UserController.TryGetUserByUserid(userid);

            if (user == null)
            {
                result = false;
                message = HttpOptions.INVALID_DATA;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            Offense newOffense = new Offense
            {
                ExpertId = user.UserId,
                Title = offense.Title,
                Description = offense.Description,
                Category = offense.Category,
                Status = "Зарегистрировано",
                Date = DateTime.Now
            };

            if (newOffense.Category.Equals("Персонал"))
            {
                newOffense.EmployeeId = offense.EmployeeId;
            }

            else 
            {
                newOffense.EmployeeId = null;
            }


            if (offense.Imgs != null && offense.Imgs.Count > 0)
            {
                foreach (IFormFile uploadedFile in offense.Imgs)
                {
                    if (!uploadedFile.ContentType.Equals("image/png") && !uploadedFile.ContentType.Equals("image/jpg") && !uploadedFile.ContentType.Equals("image/jpeg"))
                    {
                        result = false;
                        message = HttpOptions.INVALID_DATA;
                        return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
                    }
                }

                StringBuilder stringBuilder = new StringBuilder();
                foreach (IFormFile uploadedFile in offense.Imgs)
                {
                    string path = $"{_appEnvironment.ContentRootPath}/src/imgs/offenses/offense_{DateTimeOffset.Now.ToUnixTimeMilliseconds()}.{uploadedFile.FileName.Split(".")[^1]}";
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }

                    stringBuilder.AppendLine(path);
                }

                newOffense.Imgs = stringBuilder.ToString();
            }

            else
            {
                newOffense.Imgs = "";
            }

            OffenseService offenseService = new OffenseService();
            await offenseService.SetOffense(newOffense);

            if (newOffense.EmployeeId != null)
            {
                PushService pushService = new PushService();
                await pushService.SavePushByCreateOffense((Guid)newOffense.EmployeeId, newOffense.Title);
            }
            
            result = true;
            message = "";
            return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
        }

        [IgnoreAntiforgeryTokenAttribute]
        [HttpPost, Route("/api/offense/update")]
        public async Task<IActionResult> UpdateOffense([FromBody] OffenseUpdateParams offense)
        {
            bool result;
            string message;
            string data = "";

            if (offense == null || string.IsNullOrEmpty(offense.Status))
            {
                result = false;
                message = HttpOptions.INVALID_DATA;
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            string? userid = UserController.TryParseToken(HttpContext);

            if (userid == null)
            {
                result = false;
                message = HttpOptions.UNAUTHORIZED;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            User? user = await UserController.TryGetUserByUserid(userid);

            if (user == null)
            {
                result = false;
                message = HttpOptions.INVALID_DATA;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            OffenseService offenseService = new OffenseService();
            Offense? offenseCheck = await offenseService.GetOffense(offense.Offenseid);

            if (offenseCheck == null)
            {
                result = false;
                message = HttpOptions.INVALID_DATA;
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            if (offenseCheck.ExpertId.Equals(user.UserId) && (user.RoleId == 2 || user.RoleId == 0))
            {
                Offense newOffense = new Offense();
                newOffense.Status = offense.Status;

                await offenseService.UpdateOffense(offense.Offenseid, newOffense);

                if (offenseCheck.EmployeeId != null && !offenseCheck.EmployeeId.Equals(Guid.Empty))
                {
                    PushService pushService = new PushService();
                    await pushService.SavePushByChangeStatusOffense((Guid)offenseCheck.EmployeeId, offenseCheck.Title);
                }

                result = true;
                message = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            else
            {
                result = false;
                message = HttpOptions.FORBIDDEN;
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }
        }
    }
}

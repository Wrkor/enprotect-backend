using Microsoft.AspNetCore.Mvc;
using webapi.Services;
using webapi.Entities;
using Newtonsoft.Json;
using webapi.Params.Constants;
using webapi.Params.HttpRequest;
using webapi.Params.HttpResponse;
using System.Text;

namespace webapi.Controllers
{
    [ApiController]
    public class OfferController : ControllerBase
    {
        private readonly IWebHostEnvironment _appEnvironment;

        public OfferController(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        [HttpGet, Route("/api/offer")]
        public async Task<IActionResult> GetOffers()
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

            OfferService offerService = new OfferService();
            Offer[] offers = Array.Empty<Offer>();

            if (user.RoleId == 1)
            {
                offers = await offerService.GetOffersByEmployee(user.UserId);
            }

            else if (user.RoleId == 0 || user.RoleId == 2)
            {
                offers = await offerService.GetOffersByExpert(user.UserId);
            }

            if (offers == null || offers.Length == 0)
            {
                result = false;
                message = HttpOptions.NOT_FOUND;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            UserService userService = new UserService();

            List<OffersSendParams> offerList = new List<OffersSendParams>();

            for (int i = 0; i < offers.Length; i ++)
            {
                if (userService.GetFiles(offers[i].Imgs, out string[]? base64Files, 2))
                {
                    offers[i].Imgs = JsonConvert.SerializeObject(base64Files);
                    offerList.Add(new OffersSendParams(offers[i]));
                }
            }
                
            if (offerList.Count == 0)
            {
                result = false;
                message = HttpOptions.INVALID_DATA;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));

            }

            result = true;
            message = "";
            data = JsonConvert.SerializeObject(offerList);
            return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
        }

        [HttpGet, Route("/api/offer/{offerId}")]
        public async Task<IActionResult> GetOffer(int offerId)
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

            OfferService offerService = new OfferService();
            Offer? offer = await offerService.GetOffer(offerId);

            if (offer == null)
            {
                result = false;
                message = HttpOptions.INVALID_DATA;
                data = "";
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            UserService userService = new UserService();

            if (user.RoleId == 1)
            {
                if (offer.EmployeeId.Equals(user.UserId))
                {
                    if (userService.GetFiles(offer.Imgs, out string[] base64Files))
                    {
                        result = true;
                        message = "";
                        offer.Imgs = JsonConvert.SerializeObject(base64Files);
                        User? employee = await userService.GetUser(offer.EmployeeId);

                        if (employee != null)
                        {
                            if (userService.GetFiles(employee.Img, out string[] base64File, 1)) {
                                employee.Img = JsonConvert.SerializeObject(base64File[0]);

                                OfferSendParams offerSendParams = new OfferSendParams(offer, employee);
                                data = JsonConvert.SerializeObject(offerSendParams);
                                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
                            } 
                        }
                    }

                    result = false;
                    message = HttpOptions.INVALID_DATA;
                    data = "";
                    return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
                }
            }

            else if (user.RoleId == 0 || user.RoleId == 2)
            {
                if (offer.ExpertId.Equals(user.UserId))
                {
                    if (userService.GetFiles(offer.Imgs, out string[] base64Files))
                    {
                        result = true;
                        message = "";
                        offer.Imgs = JsonConvert.SerializeObject(base64Files);
                        User? employee = await userService.GetUser(offer.EmployeeId);

                        if (employee != null)
                        {
                            if (userService.GetFiles(employee.Img, out string[] base64File, 1))
                            {
                                employee.Img = JsonConvert.SerializeObject(base64File);

                                OfferSendParams offerSendParams = new OfferSendParams(offer, employee);
                                data = JsonConvert.SerializeObject(offerSendParams);
                                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
                            }
                        }
                    }

                    result = false;
                    message = HttpOptions.INVALID_DATA;
                    data = "";
                    return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
                }
            }

            result = false;
            message = HttpOptions.FORBIDDEN;
            data = "";
            return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
        }

        [IgnoreAntiforgeryTokenAttribute]
        [HttpPost, Route("/api/offer/create")]
        public async Task<IActionResult> CreateOffer([FromForm] OfferParams offer)
        {
            bool result;
            string message;
            string data = "";

            if (offer == null || string.IsNullOrEmpty(offer.Description) || string.IsNullOrEmpty(offer.Title))
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

            if (user.ExpertId == null || user.ExpertId.Equals(Guid.Empty))
            {
                result = false;
                message = HttpOptions.FORBIDDEN;
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            UserService userService = new UserService();

            User? expert = await userService.GetUser((Guid)user.ExpertId);

            if (expert == null)
            {
                result = false;
                message = HttpOptions.FORBIDDEN;
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            Offer newOffer = new Offer
            {
                EmployeeId = user.UserId,
                Title = offer.Title,
                Description = offer.Description,
                Status = "Зарегистрировано",
                Date = DateTime.Now,
                ExpertId = expert.UserId,
            };

            if (offer.Imgs != null && offer.Imgs.Count > 0)
            {
                foreach (IFormFile uploadedFile in offer.Imgs)
                {
                    if (!uploadedFile.ContentType.Equals("image/png") && !uploadedFile.ContentType.Equals("image/jpg") && !uploadedFile.ContentType.Equals("image/jpeg"))
                    {
                        result = false;
                        message = HttpOptions.INVALID_DATA;
                        return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
                    }
                }

                StringBuilder stringBuilder = new StringBuilder();
                foreach (IFormFile uploadedFile in offer.Imgs)
                {
                    string path = $"{_appEnvironment.ContentRootPath}/src/imgs/offers/offer_{DateTimeOffset.Now.ToUnixTimeMilliseconds()}.{uploadedFile.FileName.Split(".")[^1]}";
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }

                    stringBuilder.AppendLine(path);
                }

                newOffer.Imgs = stringBuilder.ToString();
            }

            else
            {
                newOffer.Imgs = "";
            }

            OfferService offerService = new OfferService();
            await offerService.SetOffer(newOffer);

            PushService pushService = new PushService();
            await pushService.SavePushByCreateOffer(newOffer.ExpertId, newOffer.Title);

            result = true;
            message = "";
            return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
        }

        [IgnoreAntiforgeryTokenAttribute]
        [HttpPost, Route("/api/offer/update")]
        public async Task<IActionResult> UpdateOffer([FromBody] OfferUpdateParams offer)
        {
            bool result;
            string message;
            string data = "";

            if (offer == null || string.IsNullOrEmpty(offer.Status))
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
            
            OfferService offerService = new OfferService();
            Offer? offerCheck = await offerService.GetOffer(offer.Offerid);

            if (offerCheck == null)
            {
                result = false;
                message = HttpOptions.INVALID_DATA;
                return new JsonResult(JsonConvert.SerializeObject(new JsonResponse(result, message, data)));
            }

            if (offerCheck.ExpertId.Equals(user.UserId) && (user.RoleId == 2 || user.RoleId == 0))
            {
                Offer newOffer = new Offer();
                newOffer.Status = offer.Status;

                await offerService.UpdateOffer(offer.Offerid, newOffer);

                PushService pushService = new PushService();
                await pushService.SavePushByChangeStatusOffer(offerCheck.EmployeeId, offerCheck.Title);

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

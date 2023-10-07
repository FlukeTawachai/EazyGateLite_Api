using EazyGateLite_Api.Requests.LineLiff;
using EazyGateLite_Api.Responses.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Text;

namespace EazyGateLite_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineLiffController : Controller
    {
        private readonly string lineApiEndpoint = "https://api.line.me/v2/bot/message/push";
        //Channel access token of Messaging API
        private readonly string accessToken = "NMyT/J8nR1Z74MQRzFzXXbA7osxlismvXUPtkqY8v0dP+LZVU7qNknWvPg4ynqpgfj4T0DPu940RAMacVzJEuq/kkVIROY5peeb9djtXdgReTPPK66Qer+HCpMi1CUwszNV8KoRjLgbBdp6jPZ2VrgdB04t89/1O/w1cDnyilFU=";

        [HttpGet]
        [SwaggerResponse(200, Type = typeof(ApiResponse))]
        [SwaggerResponse(500, Type = typeof(ApiResponse))]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(new ApiResponse() { Success = true, Message = "Success" });
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse() { Success = false, Message = ex.InnerException.Message });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse() { Success = false, Message = ex.Message });
                }
            }
        }

        [HttpPost("SendTextMessage")]
        [SwaggerResponse(200, Type = typeof(ApiResponse))]
        [SwaggerResponse(400, Type = typeof(ApiResponse))]
        [SwaggerResponse(500, Type = typeof(ApiResponse))]
        public async Task<object> SendTextMessageAsync([FromBody] LineLiffReq req)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    object type = new
                    {
                        type = "text",
                        text = req.message
                    };

                    if (req.type == "image")
                    {
                        type = new
                        {
                            type = "image",
                            originalContentUrl = "https://www.ofm.co.th/blog/wp-content/uploads/2022/04/%E0%B8%82%E0%B9%89%E0%B8%B2%E0%B8%A7%E0%B9%80%E0%B8%AB%E0%B8%99%E0%B8%B5%E0%B8%A2%E0%B8%A7%E0%B8%A1%E0%B8%B0%E0%B8%A1%E0%B9%88%E0%B8%A7%E0%B8%87-1.jpg",
                            previewImageUrl = "https://www.ofm.co.th/blog/wp-content/uploads/2022/04/%E0%B8%82%E0%B9%89%E0%B8%B2%E0%B8%A7%E0%B9%80%E0%B8%AB%E0%B8%99%E0%B8%B5%E0%B8%A2%E0%B8%A7%E0%B8%A1%E0%B8%B0%E0%B8%A1%E0%B9%88%E0%B8%A7%E0%B8%87-1.jpg"
                        };
                    }

                    var messageObject = new
                    {
                        //UserId of Messaging API
                        to = req.lineToken,
                        messages = new[]
                            { type }
                    };

                    var jsonString = JObject.FromObject(messageObject).ToString();
                    var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(lineApiEndpoint, content);
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return Ok(new ApiResponse() { Success = true, Message = responseBody });

                    // Handle response as required
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse() { Success = false, Message = ex.InnerException.Message });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse() { Success = false, Message = ex.Message });
                }

            }

        }

        [HttpGet("SendTextMessageByUserId")]
        [SwaggerResponse(200, Type = typeof(ApiResponse))]
        [SwaggerResponse(400, Type = typeof(ApiResponse))]
        [SwaggerResponse(500, Type = typeof(ApiResponse))]
        public async Task<object> SendTextMessageByUserId()
        {
            try
            {
                string apiUrl = "https://api.line.me/v2/bot/followers/ids";
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    var response = await client.GetAsync(apiUrl);
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return Ok(new ApiResponse() { Success = true, Message = responseBody });

                    // Handle response as required
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse() { Success = false, Message = ex.InnerException.Message });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse() { Success = false, Message = ex.Message });
                }

            }

        }

        [HttpPost("LineNotify")]
        [SwaggerResponse(200, Type = typeof(ApiResponse))]
        [SwaggerResponse(400, Type = typeof(ApiResponse))]
        [SwaggerResponse(500, Type = typeof(ApiResponse))]

        public IActionResult LineNotify([FromBody] LineLiffReq req)
        {
            try
            {
                string newMessage = System.Web.HttpUtility.UrlEncode(req.message, Encoding.UTF8);
                var request = (HttpWebRequest)WebRequest.Create("https://notify-api.line.me/api/notify");
                var postData = string.Format("message={0}", newMessage);
                var data = Encoding.UTF8.GetBytes(postData);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                request.Headers.Add("Authorization", "Bearer " + req.lineToken);
                var stream = request.GetRequestStream();
                stream.Write(data, 0, data.Length);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return Ok(new ApiResponse() { Success = true, Message = "Success" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse() { Success = false, Message = ex.Message });
            }
        }
    }
}

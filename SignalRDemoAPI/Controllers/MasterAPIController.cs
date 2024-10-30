using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SignalRDemo.Services.MainServices;
using SignalRDemoAPI.Hubs;
using StudentManagement_API.Models.Models;
using StudentManagement_API.Models.Models.DTO;
using System.Net;
using System.Reflection;

namespace SignalRDemoAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MasterAPIController : ControllerBase
    {

        private APIResponse _response;

        private readonly IStudentServices _studentServices;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IConfiguration _configuration;
        public MasterAPIController(IStudentServices studentServices, IHubContext<NotificationHub> hubContext, IConfiguration configuration)
        {
            _studentServices = studentServices;
            this._response = new();
            _hubContext = hubContext;
            _configuration = configuration;
        }
        public Dictionary<string, Type> controllers = new()
        {
            { "Home", typeof(HomeController) },
        };

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("{controllerName}/{methodName}/Login")]
        public ActionResult<APIResponse> CallExternamLoginMethod(ApiRequest apiRequest)
        {
            try
            {
                object controller = null;
                if (controllers.TryGetValue(apiRequest.ControllerName, out Type controllerType))
                {
                    controller = Activator.CreateInstance(controllerType, _studentServices,_configuration,_hubContext);
                }
                MethodInfo methodInfo = controller.GetType().GetMethod(apiRequest.MethodName);
                if (methodInfo != null)
                {

                    if (apiRequest.DataObject != "null")
                    {
                        object dtoObject = JsonConvert.DeserializeObject<dynamic>(apiRequest.DataObject);
                        //var newobj = ((JObject)dtoObject).ToObject<StudentLoginDto>();
                        var value = _studentServices.GetDynamicData(apiRequest.ControllerName, apiRequest.MethodName, dtoObject);
                        //int intValue = Convert.ToInt32(dtoObject);
                        var result = methodInfo.Invoke(controller, new object[] { value });
                        var actionResult = (ActionResult<APIResponse>)result;
                        _response = actionResult.Value;
                        return _response;
                    }
                    else
                    {
                        var result = methodInfo.Invoke(controller, null);
                        var actionResult = (ActionResult<APIResponse>)result;
                        _response = actionResult.Value;
                        return _response;
                    }

                }
                else
                {
                    _response.ErroMessages = new List<string> { "Method Or Controller Invalid" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return _response;
                }
            }
            catch (Exception ex)
            {
                _response.ErroMessages = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }


    }
}

using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;
using System.Text;
using SignalRDemo.Models;
using Newtonsoft.Json;

namespace SignalRDemo.Controllers
{
    public class MasterController : Controller
    {
        Uri BaseAddress = new("https://localhost:7281/MasterApi");
        private readonly string apiVersionUrl = "https://localhost:7281/MasterApi/Login/GetApiVersion";
        private readonly HttpClient _httpClient;

        public MasterController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = BaseAddress;
        }


        public RoleBaseResponse<T> CallApiWithoutToken<T>(SecondApiRequest apiRequest)
        {
            string url = _httpClient.BaseAddress + "/" + apiRequest.ControllerName + "/" + apiRequest.MethodName + "/Login";
            var serializedData = JsonConvert.SerializeObject(apiRequest);
            var content = new StringContent(serializedData, Encoding.UTF8, "application/json");
            HttpContext.Session.SetString("ApiCallTime", DateTime.Now.ToString());
            HttpResponseMessage response = _httpClient.PostAsync(url, content).Result;
            RoleBaseResponse<T> roleBaseResponse = new();
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content?.ReadAsStringAsync().Result;
                APIResponse<RoleBaseResponse<T>> aPIResponse = JsonConvert.DeserializeObject<APIResponse<RoleBaseResponse<T>>>(data);
                if (aPIResponse.IsSuccess && aPIResponse.result != null)
                {
                    roleBaseResponse = aPIResponse.result;
                }
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                roleBaseResponse.IsAuthorize = false;
            }
            return roleBaseResponse;
        }
    }
}

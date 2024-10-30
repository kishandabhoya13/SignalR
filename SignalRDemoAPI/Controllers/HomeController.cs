using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRDemo.Models;
using SignalRDemo.Models.Models;
using SignalRDemo.Services.MainServices;
using SignalRDemoAPI.Hubs;
using StudentManagement_API.Models.Models;
using StudentManagement_API.Models.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace SignalRDemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private APIResponse _response;
        private readonly IStudentServices _studentServices;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();

        public HomeController(IStudentServices studentServices, IConfiguration configuration, IHubContext<NotificationHub> hubContext)
        {
            this._response = new();
            _studentServices = studentServices;
            _configuration = configuration;
            _hubContext = hubContext;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{studentId:int}", Name = "GetStudent")]
        public ActionResult<APIResponse> GetStudent(int studentId)
        {
            try
            {
                if (studentId == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErroMessages = new List<string> { "Invalid StudentId" };
                    _response.IsSuccess = false;
                    return _response;
                }

                Student student = _studentServices.GetStudent<Student>("[dbo].[Get_Student_Details]", studentId);
                RoleBaseResponse<Student> roleBaseResponse = new()
                {
                    data = student
                };
                if (student.StudentId > 0)
                {
                    _response.result = roleBaseResponse;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                }
                else
                {
                    _response.ErroMessages = new List<string> { "Student Not Fount" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return _response;
                }
                return _response;
            }
            catch (Exception ex)
            {
                _response.ErroMessages = new List<string> { ex.ToString() };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return _response;
            }

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("CheckLoginDetails", Name = "CheckLoginDetails")]
        public ActionResult<APIResponse> CheckLoginDetails([FromQuery] StudentLoginDto studentLoginDto)
        {
            try
            {
                LoginInformationDto loginInformationDto = _studentServices.GetLoginStudentDetails(studentLoginDto);
                if (loginInformationDto != null && loginInformationDto.StudentId != 0)
                {
                    RoleBaseResponse<LoginInformationDto> roleBaseResponse = new()
                    {
                        data = loginInformationDto,
                    };
                    string token = Guid.NewGuid().ToString();
                    loginInformationDto.token = token;
                    _studentServices.AddVerificationRecord(loginInformationDto);
                    _response.result = roleBaseResponse;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                }
                else
                {
                    LoginInformationDto loginInformationDto1= _studentServices.CheckUserNamePassword(studentLoginDto);
                    RoleBaseResponse<LoginInformationDto> roleBaseResponse = new()
                    {
                        data = loginInformationDto1,
                    };
                    _response.result = roleBaseResponse;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                _response.ErroMessages = new List<string> { ex.ToString() };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
            }
            return _response;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("UpdateVerificationStatus", Name = "UpdateVerificationStatus")]
        public ActionResult<APIResponse> UpdateVerificationStatus(string token, int userId)
        {
            try
            {
                bool isSuccess = _studentServices.UpdateVerificationRecord(token, userId);
                if (isSuccess)
                {
                    string connectionId = NotificationHub.GetConnectionId(userId.ToString());
                    _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", "Verification Accepted");
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.Unauthorized;
                }
                return _response;

            }
            catch (Exception ex)
            {
                _response.ErroMessages = new List<string> { ex.ToString() };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.Unauthorized;
                return _response;
            }
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("GetAllStudents")]
        public ActionResult<APIResponse> GetAllStudents(PaginationDto paginationDto)
        {
            if (paginationDto.StartIndex < 0 || paginationDto.PageSize < 0)
            {
                return _response;
            }
            IList<Student> students = _studentServices.GetDataWithPagination<Student>(paginationDto, "[dbo].[Get_All_Students]");
            int totalItems = students.Count > 0 ? students.FirstOrDefault(x => x.StudentId != 0)?.TotalRecords ?? 0 : 0;
            int TotalPages = (int)Math.Ceiling((decimal)totalItems / paginationDto.PageSize);
            RoleBaseResponse<IList<Student>> roleBaseResponse = new()
            {
                data = students,
                StartIndex = paginationDto.StartIndex,
                PageSize = paginationDto.PageSize,
                TotalItems = totalItems,
                TotalPages = TotalPages,
                CurrentPage = (int)Math.Ceiling((double)paginationDto.StartIndex / paginationDto.PageSize),
                searchQuery = paginationDto.searchQuery,
            };

            _response.result = roleBaseResponse;
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("GetAllHods")]
        public ActionResult<APIResponse> GetAllHods(PaginationDto paginationDto)
        {
            if (paginationDto.StartIndex < 0 || paginationDto.PageSize < 0)
            {
                return _response;
            }
            IList<ProfessorHod> professorHods = _studentServices.GetDataWithPagination<ProfessorHod>(paginationDto, "[dbo].[Get_All_Hods]");
            int totalItems = professorHods.Count > 0 ? professorHods.FirstOrDefault(x => x.Id != 0)?.TotalRecords ?? 0 : 0;
            int TotalPages = (int)Math.Ceiling((decimal)totalItems / paginationDto.PageSize);
            RoleBaseResponse<IList<ProfessorHod>> roleBaseResponse = new()
            {
                data = professorHods,
                StartIndex = paginationDto.StartIndex,
                PageSize = paginationDto.PageSize,
                TotalItems = totalItems,
                TotalPages = TotalPages,
                CurrentPage = (int)Math.Ceiling((double)paginationDto.StartIndex / paginationDto.PageSize),
                searchQuery = paginationDto.searchQuery,
            };

            _response.result = roleBaseResponse;
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }
    }
}

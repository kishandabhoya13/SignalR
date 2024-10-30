using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SignalRDemo.Models;
using SignalRDemo.Models.Models;
using StudentManagement_API.DataContext;
using StudentManagement_API.Models.Models.DTO;
using StudentManagement_API.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static DemoApiWithoutEF.Utilities.Enums;

namespace SignalRDemo.Services.MainServices
{
    public class StudentServices : IStudentServices
    {
        private readonly IJwtServices _jwtServices;
        private readonly IConfiguration _configuration;

        public StudentServices(IConfiguration configuration,IJwtServices jwtServices) 
        {
            this._configuration = configuration;
            _jwtServices = jwtServices;
        }

        public T GetStudent<T>(string Procedure, int Id)
        {
            Collection<DbParameters> parameters = new()
            {
                new DbParameters { Name = "@StudentId", Value = Id, DBType = DbType.Int64 }
            };
            T newobj = DbClient.ExecuteOneRecordProcedure<T>(Procedure, parameters);
            return newobj;

        }

        public LoginInformationDto GetLoginStudentDetails(StudentLoginDto studentLoginDto)
        {
            try
            {
                Collection<DbParameters> parameters = new Collection<DbParameters>();
                parameters.Add(new DbParameters() { Name = "@UserName", Value = studentLoginDto.UserName, DBType = DbType.String });
                parameters.Add(new DbParameters() { Name = "@PassWord", Value = studentLoginDto.Password, DBType = DbType.String });
                LoginInformationDto loginInformationDto= DbClient.ExecuteOneRecordProcedure<LoginInformationDto>("[dbo].[Get_UserName_Password]", parameters);
                if(loginInformationDto != null && loginInformationDto.StudentId > 0)
                {
                    JwtClaimsDto jwtClaimsDto = new()
                    {
                        StudentId = loginInformationDto.StudentId,
                        UserName = loginInformationDto.UserName,
                        Email = loginInformationDto.Email,
                        RoleId = 3,
                    };
                    //_jwtServices.GenerateToken(jwtClaimsDto);
                }
                return loginInformationDto;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public LoginInformationDto CheckUserNamePassword(StudentLoginDto studentLoginDto)
        {
            try
            {
                Collection<DbParameters> parameters = new Collection<DbParameters>();
                parameters.Add(new DbParameters() { Name = "@UserName", Value = studentLoginDto.UserName, DBType = DbType.String });
                parameters.Add(new DbParameters() { Name = "@PassWord", Value = studentLoginDto.Password, DBType = DbType.String });
                LoginInformationDto loginInformationDto= DbClient.ExecuteOneRecordProcedure<LoginInformationDto>("[dbo].[Get_ProfessorHod_UserName_Password]", parameters);
                return loginInformationDto;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void AddVerificationRecord(LoginInformationDto loginInformationDto)
        {
            Collection<DbParameters> parameters = new Collection<DbParameters>();
            parameters.Add(new DbParameters() { Name = "@UserId", Value = loginInformationDto.StudentId, DBType = DbType.String });
            parameters.Add(new DbParameters() { Name = "@Token", Value = loginInformationDto.token, DBType = DbType.String });
            DbClient.ExecuteProcedure("[dbo].[Add_CheckStatus_Details]", parameters, ExecuteType.ExecuteNonQuery);
        }

        public static T GetDataModel<T>(object dataObj)
        {
            return ((JObject)dataObj).ToObject<T>();
        }

        public dynamic GetDynamicData(string controllerName, string methodName, object dataObj)
        {
            if (controllerName == "Home" && methodName == "CheckLoginDetails")
            {
                return GetDataModel<StudentLoginDto>(dataObj);
            }else if((controllerName == "Home" && methodName == "GetAllStudents") || (controllerName == "Home" && methodName == "GetAllHods"))
            {
                return GetDataModel<PaginationDto>(dataObj);
            }
            else
            {
                return null;
            }
        }

        public bool UpdateVerificationRecord(string token , int UserId)
        {
            Collection<DbParameters> parameters = new Collection<DbParameters>();
            parameters.Add(new DbParameters() { Name = "@UserId", Value = UserId, DBType = DbType.String });
            parameters.Add(new DbParameters() { Name = "@Token", Value = token, DBType = DbType.String });
            int row = (int)DbClient.ExecuteProcedure("[dbo].[Update_CheckStatus_Details]", parameters, ExecuteType.ExecuteNonQuery);
            if(row == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public IList<T> GetDataWithPagination<T>(PaginationDto paginationDto, string sp)
        {
            try
            {
                Collection<DbParameters> parameters = new();
                parameters.Add(new DbParameters { Name = "@Search_Query", Value = paginationDto.searchQuery ?? "", DBType = DbType.String });
                parameters.Add(new DbParameters { Name = "@Sort_Column_Name", Value = paginationDto.OrderBy ?? "", DBType = DbType.String });
                parameters.Add(new DbParameters { Name = "@Start_index", Value = paginationDto.StartIndex, DBType = DbType.Int64 });
                parameters.Add(new DbParameters { Name = "@Page_Size", Value = paginationDto.PageSize, DBType = DbType.Int64 });
                if (paginationDto.FromDate != null && paginationDto.ToDate != null)
                {
                    parameters.Add(new DbParameters { Name = "@FromDate", Value = paginationDto.FromDate, DBType = DbType.Date });
                    parameters.Add(new DbParameters { Name = "@ToDate", Value = paginationDto.ToDate, DBType = DbType.Date });

                }
                //IList<Book> books = DbClient.ExecuteProcedure<Book>("[dbo].[Get_Books_List]", parameters);
                IList<T> data = DbClient.ExecuteProcedure<T>(sp, parameters);

                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}

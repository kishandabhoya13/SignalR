using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SignalRDemo.Models;
using StudentManagment.Models;
using StudentManagment.Models.DataModels;
using System.Diagnostics;

namespace SignalRDemo.Controllers
{
    public class HomeController : MasterController
    {

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckLogin(AdminStudentViewModel adminViewModel)
        {
            try
            {
                StudentLoginViewModel studentLoginViewModel = new()
                {
                    UserName = adminViewModel.UserName,
                    Password = adminViewModel.Password,
                };
                SecondApiRequest secondApiRequest = new()
                {
                    ControllerName = "Home",
                    MethodName = "CheckLoginDetails",
                    DataObject = JsonConvert.SerializeObject(studentLoginViewModel),

                };
                RoleBaseResponse<JwtClaimsViewModel> roleBaseResponse = CallApiWithoutToken<JwtClaimsViewModel>(secondApiRequest);
                JwtClaimsViewModel jwtClaimsViewModel = roleBaseResponse.data;
                if (jwtClaimsViewModel != null && !string.IsNullOrEmpty(jwtClaimsViewModel.UserName))
                {
                    HttpContext.Session.SetString("FullName", jwtClaimsViewModel.FirstName + " " + jwtClaimsViewModel.LastName);

                    if (jwtClaimsViewModel.Id != 0)
                    {
                        HttpContext.Session.SetInt32("UserId", jwtClaimsViewModel.Id);
                        HttpContext.Session.SetInt32("Id", jwtClaimsViewModel.Id);
                        HttpContext.Session.SetString("HodUserName", jwtClaimsViewModel.UserName);

                        return RedirectToAction("AllStudents", "Home");
                    }
                    else
                    {
                        HttpContext.Session.SetString("UserName", jwtClaimsViewModel.UserName);
                        HttpContext.Session.SetString("Email", jwtClaimsViewModel.Email);
                        HttpContext.Session.SetInt32("UserId", jwtClaimsViewModel.StudentId);
                        return RedirectToAction("AllHods");

                    }

                }
                else
                {
                    TempData["error"] = "Invalid Username/Password";
                    return View("Login");
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw;
            }
        }

        public IActionResult WaitingScreen(JwtClaimsViewModel jwtClaimsViewModel)
        {
            EmailViewModel emailViewModel = new()
            {
                StudentId = jwtClaimsViewModel.StudentId,
                Email = jwtClaimsViewModel.Email,
                FirstName = jwtClaimsViewModel.FirstName,
                LastName = jwtClaimsViewModel.LastName,
            };


            SecondApiRequest secondApiRequest = new()
            {
                ControllerName = "Home",
                MethodName = "SendEmailForVerification",
                DataObject = JsonConvert.SerializeObject(emailViewModel),

            };
            RoleBaseResponse<bool> roleBaseResponse = CallApiWithoutToken<bool>(secondApiRequest);
            return View(jwtClaimsViewModel);
        }

        public IActionResult SuccessLogin()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        public IActionResult AllStudents()
        {
            if (HttpContext.Session.GetInt32("Id") == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        public IActionResult ChatPage(string StudentUserName)
        {
            if (HttpContext.Session.GetInt32("Id") == null)
            {
                return RedirectToAction("Login");
            }

            ChatMessage chatMessage = new()
            {
                ReceiverUsername = StudentUserName,
                Username = HttpContext.Session.GetString("HodUserName")
            };
            return View(chatMessage);
        }

        [HttpPost]
        public IActionResult AdminIndexTableView(SecondApiRequest secondApiRequest)
        {
            secondApiRequest.RoleId = HttpContext.Session.GetInt32("RoleId") ?? 0;
            secondApiRequest.token = HttpContext.Session.GetString("Jwt") ?? "";
            PaginationViewModel paginationViewModel = new()
            {
                PageSize = secondApiRequest.PageSize,
                StartIndex = secondApiRequest.StartIndex,
                OrderBy = secondApiRequest.OrderBy,
                OrderDirection = secondApiRequest.OrderDirection,
                searchQuery = secondApiRequest.searchQuery,
                JwtToken = secondApiRequest.token,
                FromDate = secondApiRequest.FromDate,
                ToDate = secondApiRequest.ToDate
            };

            SecondApiRequest newSecondApiRequest = new()
            {
                ControllerName = "Home",
                MethodName = "GetAllStudents",
                DataObject = JsonConvert.SerializeObject(paginationViewModel),
            };
            RoleBaseResponse<IList<Student>> roleBaseResponse = CallApiWithoutToken<IList<Student>>(newSecondApiRequest);
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return Json(false);
            }
            return Json(roleBaseResponse);
        }

        public IActionResult AllHods()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AllHodData(SecondApiRequest secondApiRequest)
        {
            secondApiRequest.RoleId = HttpContext.Session.GetInt32("RoleId") ?? 0;
            secondApiRequest.token = HttpContext.Session.GetString("Jwt") ?? "";
            PaginationViewModel paginationViewModel = new()
            {
                PageSize = secondApiRequest.PageSize,
                StartIndex = secondApiRequest.StartIndex,
                OrderBy = secondApiRequest.OrderBy,
                OrderDirection = secondApiRequest.OrderDirection,
                searchQuery = secondApiRequest.searchQuery,
                JwtToken = secondApiRequest.token,
                FromDate = secondApiRequest.FromDate,
                ToDate = secondApiRequest.ToDate
            };

            SecondApiRequest newSecondApiRequest = new()
            {
                ControllerName = "Home",
                MethodName = "GetAllHods",
                DataObject = JsonConvert.SerializeObject(paginationViewModel),
            };
            RoleBaseResponse<IList<ProfessorHod>> roleBaseResponse = CallApiWithoutToken<IList<ProfessorHod>>(newSecondApiRequest);
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return Json(false);
            }
            return Json(roleBaseResponse);
        }

        public IActionResult ChatWithHod(string HodUserName)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login");
            }
            ChatMessage chatMessage = new()
            {
                Username = HttpContext.Session.GetString("UserName").ToString(),
                ReceiverUsername = HodUserName,
            };
            return View(chatMessage);
        }
    }
}
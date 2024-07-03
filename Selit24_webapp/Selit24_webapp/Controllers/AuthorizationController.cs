using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Selit24_webapp.Models;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace Selit24_webapp.Controllers
{
    public class AuthorizationController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7273/api");

        private readonly HttpClient _client;
        public AuthorizationController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        // GET: LoginController

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(Login login)
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");

            HttpResponseMessage customerResponse = await _client.PostAsync($"{_client.BaseAddress}/CustomerAutho/CustomerSignUpByEmailAndPassword", jsonContent);
            if (customerResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Authorization");
            }
            return View();
        }
        public IActionResult Login()
        {
            ClaimsPrincipal claimuser = HttpContext.User;
            if(claimuser.Identity.IsAuthenticated)
            {

                var roleClaim = claimuser.FindFirst(ClaimTypes.Role);

                if (roleClaim != null)
                {
                    // Redirect based on the role claim
                    if (roleClaim.Value == "Customer")
                    {
                        return RedirectToAction("CustomerList", "Customer");
                    }
                    else if (roleClaim.Value == "User")
                    {
                        return RedirectToAction("Users", "Users");
                    }
                }

                // Handle the case where the role claim is missing or has an unexpected value
                TempData["ErrorMessage"] = "Invalid user role.";
                return View();
            }

            // Handle unauthenticated users
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            try 
            {
                HttpResponseMessage customerResponse = _client.GetAsync($"{_client.BaseAddress}/CustomerAutho/CustomerLogInByEmailandPassword?Email={login.Email}&Password={login.Password}").Result;
                HttpResponseMessage userResponse = _client.GetAsync($"{_client.BaseAddress}/UserAutho/UserLogInByEmailAndPassword?Email={login.Email}&Password={login.Password}").Result;
                if (customerResponse.IsSuccessStatusCode)
                {
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier,login.Email),
                        new Claim(ClaimTypes.Role,"Customer")
                    };

                    ClaimsIdentity claimidentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = login.KeepLoggedIN
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimidentity), properties);

                    return RedirectToAction("CustomerList","Customer");
                }
                else if (userResponse.IsSuccessStatusCode)
                {
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier,login.Email),
                        new Claim(ClaimTypes.Role,"User")
                    };

                    ClaimsIdentity claimidentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = login.KeepLoggedIN
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(claimidentity),properties);
                    return RedirectToAction("Users","Users");
                }
                else
                {
                    // Both login attempts failed, handle as needed
                    TempData["ErrorMessage"] = "Invalid email or password.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred during login. Please try again later.";
                Console.Error.WriteLine($"Error during login: {ex.Message}");
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Authorization");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(Login login)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new { Email = login.Email }), Encoding.UTF8, "application/json");

            // Make the PUT request to the API endpoint
            HttpResponseMessage response = await _client.PutAsync($"{_client.BaseAddress}/UserAutho/UserOTPOnForgetPasswordByEmail/{login.Email}", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["ForgotPasswordEmail"] = login.Email;
                return RedirectToAction("TestOTP", "Authorization");
            }
            return View();
        }

        

        public IActionResult TestOTP()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TestOTP(Login login)
        {
            string email= TempData["ForgotPasswordEmail"] as string;
            HttpResponseMessage userResponse = _client.GetAsync($"{_client.BaseAddress}/UserAutho/UserConfirmOTPbyEmail?Email={email}&OTP={login.OTP}").Result;
            if (userResponse.IsSuccessStatusCode)
            {
                TempData["Email"] = email;
                return RedirectToAction("ChangePassword", "Authorization");
            }
            return View();
        }


        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(Login login)
        {
            string email = TempData["Email"] as string;
            var content = new StringContent(JsonConvert.SerializeObject(new { Email = email, NewPassword = login.Password }), Encoding.UTF8, "application/json");

            // Make the PUT request to the API endpoint
            HttpResponseMessage response = await _client.PutAsync($"{_client.BaseAddress}/UserAutho/UserResetPasswordByEmailAndNewPassword/?Email={email}&newPassword={login.Password}", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Authorization");
            }
            return View();
        }
    }
}
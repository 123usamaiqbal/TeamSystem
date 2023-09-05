using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamManageSystem.Data;
using TeamManageSystem.Models.Account;
using TeamManageSystem.Models.ViewModel;

namespace TeamManageSystem.Controllers.Account
{
    public class AccountController : Controller
    {
        public AccountController(TeamManageContext context)
        {
            Context = context;
        }

        public TeamManageContext Context { get; }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Login()
        {
            return View();

        }

        [HttpPost]
        public IActionResult Login(LoginSignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                var data = Context.User.Where(e => e.Username == model.Username).SingleOrDefault();
                if (data != null)
                {
                    if (data.Role == "Admin")
                    {
                        //bool isValid = (data.Username == model.Username && data.Password == model.Password);
                        bool isValid = BCrypt.Net.BCrypt.Verify(model.Password, data.Password);
                        if (isValid)
                        {
                            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim(ClaimTypes.NameIdentifier, data.Id.ToString())
                };

                            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var principal = new ClaimsPrincipal(identity);

                            // Set the "Remember Me" option based on the checkbox value
                            var authenticationProperties = new AuthenticationProperties();
                            if (model.IsRemember)
                            {
                                authenticationProperties.IsPersistent = true; // Cookie won't expire after the session ends
                                authenticationProperties.ExpiresUtc = DateTime.UtcNow.AddDays(30); // Set your desired expiration time
                            }

                            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authenticationProperties);

                            HttpContext.Session.SetString("Username", model.Username);
                            return RedirectToAction("ADashboard", "Dashboard");
                        }
                        else
                        {
                            TempData["errorPassword"] = "Invalid Password";
                            return View(model);
                        }
                    }
                    else
                    {
                        //bool isValid = (data.Username == model.Username && data.Password == model.Password);
                        bool isValid = BCrypt.Net.BCrypt.Verify(model.Password, data.Password);
                        if (isValid)
                        {
                            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim(ClaimTypes.NameIdentifier, data.Id.ToString())
                };

                            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var principal = new ClaimsPrincipal(identity);

                            // Set the "Remember Me" option based on the checkbox value
                            var authenticationProperties = new AuthenticationProperties();
                            if (model.IsRemember)
                            {
                                authenticationProperties.IsPersistent = true; // Cookie won't expire after the session ends
                                authenticationProperties.ExpiresUtc = DateTime.UtcNow.AddDays(30); // Set your desired expiration time
                            }

                            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authenticationProperties);

                            HttpContext.Session.SetString("Username", model.Username);
                            return RedirectToAction("TDashboard", "Dashboard");
                        }
                        else
                        {
                            TempData["errorPassword"] = "Invalid Password";
                            return View(model);
                        }
                    }
                }
                else
                {
                    TempData["errorUsername"] = "Username not found!";
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
            /*if (ModelState.IsValid)
            {
                var data = Context.User.Where(e => e.Username == model.Username).SingleOrDefault();
                if(data!=null)
                {
                    bool isValid = (data.Username == model.Username && data.Password == model.Password);
                    if(isValid)
                    {
                        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, model.Username),
                            new Claim(ClaimTypes.NameIdentifier, data.Id.ToString()) }, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        HttpContext.Session.SetString("Username", model.Username);
                        return RedirectToAction("TDashboard", "Dashboard");
                    }
                    else
                    {
                        TempData["errorPassword"] = "Invalid Password";
                        return View(model);
                    }
                }
                else
                {
                    TempData["errorUsername"] = "Username not found!";
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }*/
        }
       

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var storedCookies = Request.Cookies.Keys;
            foreach(var cookies in storedCookies)
            {
                Response.Cookies.Delete(cookies);
            }
            return RedirectToAction("Login", "Account");
        }


        /*[AcceptVerbs("Post","Get")]
        public IActionResult UserNameIsExist(string userName)
        {
            var data = Context.User.Where(e => e.Username == userName).SingleOrDefault();
            if(data!=null)
            {
                return Json($"Username {userName} already exist");
            }
            else
            {
                return Json(true);
            }
        }*/

        public IActionResult FrontPage()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(SignUpUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                var data = new User()
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = hashedPassword,
                    Mobile = model.Mobile,
                    Role = "User",
                };
                Context.User.Add(data);
                Context.SaveChanges();
                TempData["successMessage"] = "You are Registered Successfully";
                return RedirectToAction("Login");
            }
            else
            {
                TempData["errorMessage"] = "Empty form can't be Submitted";
                return View(model);
            }
        }
    }
}

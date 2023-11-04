using IMS_System.Extentions;
using IMS_System.Models.Entities;
using IMS_System.ModelViews;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Drawing.Drawing2D;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace IMS_System.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly ImsSystemContext _context;

		private readonly IToastNotification _toastNotification;

		public AccountsController(ImsSystemContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;

		}

        public static bool isEmail(string inputEmail)
        {
            inputEmail = inputEmail ?? string.Empty;
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }


        public bool IsValidPhone(string Phone)
        {
            try
            {
                if (string.IsNullOrEmpty(Phone))
                    return false;
                var r = new Regex(@"^\(?([0-9]{3})\)?[-.●]?([0-9]{3})[-.●]?([0-9]{4})$");
                return r.IsMatch(Phone);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Login.html", Name = "Login")]
        public IActionResult Login()
        {
            var tkID = HttpContext.Session.GetString("UserId");
            if (tkID != null)
            {
                return RedirectToAction("Dashboard", "Accounts");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login.html", Name = "Login")]
        public async Task<IActionResult> Login(LoginModelView _user)
        {
            if (ModelState.IsValid)
            {
                if (isEmail(_user.UserName) || IsValidPhone(_user.UserName))
                {
                    if (_user.Password.Length >= 6)
                    {
                        var f_password = EncodingSHA256.GetSHA256(_user.Password);
                        var user = _context.Users
                            .Include(u => u.Role)
                            .SingleOrDefault(s => (s.Email.Equals(_user.UserName) || s.Mobile.Equals(_user.UserName)) && s.Password.Equals(f_password));
                        if (user != null)
                        {
                            //add session
                            HttpContext.Session.SetString("UserId", user.UserId.ToString());
                            var UserId = HttpContext.Session.GetString("UserId");
                            _toastNotification.AddSuccessToastMessage("Login successful!");

                            //identity
                            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Role, user.Role.RoleName.Trim().ToString()),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim("UserId", user.UserId.ToString()),
                    };
                            ClaimsIdentity identity = new ClaimsIdentity(claims, "login");
                            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                            await HttpContext.SignInAsync(principal);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ViewBag.Error = "Login failed";
                            _toastNotification.AddErrorToastMessage("Wrong email or password");
                            return RedirectToAction("Login", "Accounts");
                        }
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Password must more than 6 digits ơr letters");
                        return RedirectToAction("Login", "Accounts");
                    }
                }
                else
                {
                    ViewBag.Error = "Login failed";
                    _toastNotification.AddErrorToastMessage("Email wrong format");
                    return RedirectToAction("Login", "Accounts");
                }

            }
            else
            {
                _toastNotification.AddErrorToastMessage("Please enter email and password");
                return RedirectToAction("Login", "Accounts");
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Register.html", Name = "Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Register.html", Name = "Register")]
        public async Task<IActionResult> Register(RegisterModelView newUser)
        {
            if (ModelState.IsValid)
            {
                var check = _context.Users.FirstOrDefault(s => s.Email == newUser.Email || s.Mobile == newUser.PhoneNumber);
                if (check == null)
                {
                    if (newUser.Password != newUser.ConfirmPassword)
                    {
                        _toastNotification.AddErrorToastMessage("Confirm password wrong");
                        return RedirectToAction("Register", "Accounts");
                    }
                    else if (!isEmail(newUser.Email))
                    {
                        _toastNotification.AddErrorToastMessage("Email wrong format");
                        return RedirectToAction("Register", "Accounts");
                    }
                    else if (IsValidPhone(newUser.PhoneNumber) == false)
                    {
                        _toastNotification.AddErrorToastMessage("PhoneNumber wrong format");
                        return RedirectToAction("Register", "Accounts");
                    }
                    else if (newUser.Password.ToString().Length < 6)
                    {
                        _toastNotification.AddErrorToastMessage("Password must more than 6 digits ơr letters");
                        return RedirectToAction("Register", "Accounts");
                    }
                    else
                    {
                        User user = new User
                        {
                            Name = newUser.FullName,
                            Email = newUser.Email,
                            Mobile = newUser.PhoneNumber,
                            Password = EncodingSHA256.GetSHA256(newUser.Password),
                            RoleId = 4,
                            Role = _context.Roles.SingleOrDefault(x => x.RoleId == 4),
                        };
                        _context.Add(user);
                        await _context.SaveChangesAsync();
                        _toastNotification.AddSuccessToastMessage("Register successful!");

                        //add session
                        HttpContext.Session.SetString("UserId", user.UserId.ToString());

                        //identity
                        var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Role, user.Role.RoleName.Trim().ToString()),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim("UserId", user.UserId.ToString()),
                    };
                        ClaimsIdentity identity = new ClaimsIdentity(claims, "login");
                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                        await HttpContext.SignInAsync(principal);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewBag.Error = "Email or phone number is already exists";
                    _toastNotification.AddErrorToastMessage("Email or phone number is already exists");
                    return RedirectToAction("Register", "Accounts");
                }


            }
            else
            {
                _toastNotification.AddErrorToastMessage("Please enter full information");
                return RedirectToAction("Register", "Accounts");
            }
            return RedirectToAction("Register", "Accounts");
        }

        [HttpGet]
        [Route("Logout.html", Name = "Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();//remove session
            HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        [Route("Dashboard.html", Name = "DashBoard")]
        public IActionResult DashBoard()
        {

            var tkID = HttpContext.Session.GetString("UserId");
            if (tkID != null)
            {
                var customer = _context.Users.AsNoTracking().SingleOrDefault(x => x.UserId == Convert.ToInt32(tkID));
                if (customer != null)
                {
                    return View(customer);
                }
                return RedirectToAction("Login", "Accounts");
            }

            return View();
        }


        public IActionResult ChangePassword(string OldPassword, string NewPassword, string ConfirmPassword)
        {
            var uID = HttpContext.Session.GetString("UserId");
            var user = _context.Users.AsNoTracking().SingleOrDefault(u => u.UserId == Convert.ToInt32(uID));
            var f_password = EncodingSHA256.GetSHA256(OldPassword);
            if (user.Password.Trim().Equals(f_password.ToString()))
            {
                if (NewPassword.Length < 6)
                {
                    _toastNotification.AddErrorToastMessage("NewPassword must more than 6 digits ơr letters");
                    RedirectToAction("Dashboard", "Accounts");
                }
                else if (ConfirmPassword != NewPassword)
                {
                    _toastNotification.AddErrorToastMessage("Confirm new password wrong");
                    RedirectToAction("Dashboard", "Accounts");
                }
                else
                {
                    user.Password = EncodingSHA256.GetSHA256(NewPassword);
                    _context.Update(user);
                    _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Change Password successful!");
                    RedirectToAction("Dashboard", "Accounts");
                }
            }
            else
            {
                _toastNotification.AddErrorToastMessage("OldPassword incorrect!");
                RedirectToAction("Dashboard", "Accounts");
            }
            return RedirectToAction("Dashboard", "Accounts");
        }
    }
}

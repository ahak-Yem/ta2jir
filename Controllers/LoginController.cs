using ta2jir.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Diagnostics;
using NToastNotify;
namespace ta2jir.Controllers
{
    //enum for current user
    enum userType { nothing = 0, admin = 1, user = 2 };
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IToastNotification _toastNotification;
        private readonly Ta2jirContext _db;

        /// <summary>
        /// Constructor to assign the db context
        /// </summary>
        /// <param name="db"></param>
        public LoginController(Ta2jirContext db, ILogger<LoginController> logger, IWebHostEnvironment webHostEnvironment, IToastNotification toastNotification)
        {
            _db = db;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _toastNotification = toastNotification;
        }

        //Variable to save current user type
        static userType loggedUserType = userType.nothing;
        public static bool notAuthorized = false;
        static User currentUser = new User();
        /// <summary>
        /// A method to be used in other classes to get current user type
        /// </summary>
        /// <returns>User type</returns>
        public static string GetUserType()
        {
            return loggedUserType.ToString();
        }
        public static User GetCurrentUser()
        {
            return currentUser;
        }


        /// <summary>
        /// A method to check the entered logging data
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="Password"></param>
        /// <returns>The View that should be shown</returns>
        public async Task<IActionResult> Index(string Email, string Password)
        {
            IEnumerable<Admin> admins = _db.Admins;
            IEnumerable<User> users = _db.Users;
            string username="no one";
            if (ModelState.IsValid)
            {
                foreach (User user in users)
                {
                    if (Email == user.Email && Password == user.Password)
                    {
                        loggedUserType = userType.user;
                        notAuthorized = false;
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Name),
                            new Claim(ClaimTypes.Email, user.Email)
                        };
                        currentUser = user;
                        var claimsIdentity = new ClaimsIdentity(claims, "Login");
                        username = user.Name;
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                        break;
                    }
                }
                if (loggedUserType == userType.user)
                {
                    foreach (Admin admin in admins)
                    {
                        if (Email == admin.Email)
                        {
                            loggedUserType = userType.admin;
                            break;
                        }
                    }
                    return RedirectToAction("Dashboard", "User", new {currentUser.UserId});
                }
            }
            if (Email != null && Password != null && loggedUserType == userType.nothing)
            {
                notAuthorized = true;
            }
            return View();
        }

        /// <summary>
        /// (GET)
        /// An Action Method that returns a View to add new User
        /// </summary>
        /// <returns>The register form view</returns>
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// (POST)
        /// An Action that recieves the data filled in the form in our View and save it in the DB
        /// </summary>
        /// <param name="adminData"></param>
        /// <returns>The View from the Index Action Method so the Admin list view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User userData, string RePassword)
        {
            userData.DateJoined =DateTime.Now;
            ModelState.Remove("IsBlocked");
            if (_db.Users.Contains(userData))
            {
                ModelState.AddModelError("Email", "You are already registered with the given email in this platform");
            }
            if (userData.Password != RePassword)
            {
                ModelState.AddModelError("Password", "Passwords do not match");
            }
            if (userData.Birthdate > DateTime.Now)
            {
                ModelState.AddModelError("Birthdate", "You are not born yet");
            }
            else if (userData.Birthdate > DateTime.Now.AddYears(-12))
            {
                ModelState.AddModelError("Birthdate", "You should be older than 12 to use this platform");
            }
            if (userData.Name.Length < 2)
            {
                ModelState.AddModelError("Name", "Your Name should contain at least 2 letters");
            }
            if (userData.ProfilePicFile != null)
            {
                string folder = $"res\\Users\\{userData.Email}\\";
                string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                Directory.CreateDirectory(serverFolder);
                string file= $"{Guid.NewGuid()}_{userData.Name} Profile Picture_{userData.ProfilePicFile.FileName}";
                string path = serverFolder + file;
                await userData.ProfilePicFile.CopyToAsync(new FileStream(path, FileMode.Create));
                userData.ProfilePic = "\\" + folder+file;
            }
            if (ModelState.IsValid)
            {
                await _db.Users.AddAsync(userData);
                await _db.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Registerd, We are happy to welcome you as a member in our platform");
                return RedirectToAction("Index", "Login");
            }
            else
                return View(userData);
        }

        /// <summary>
        /// A method for logout
        /// </summary>
        /// <returns>login page</returns>
        public async Task<IActionResult> Logout()
        {
            loggedUserType = userType.nothing;
            notAuthorized = false;
            currentUser = new User();
            await HttpContext.SignOutAsync();
            _toastNotification.AddErrorToastMessage("You are logged out !");
            return RedirectToAction("Index", "Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

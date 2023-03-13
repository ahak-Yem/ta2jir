using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using ta2jir.Models;
namespace ta2jir.Controllers
{
    public class UserController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IToastNotification _toastNotification;
        private readonly Ta2jirContext _db;
        private readonly string _userType;
        private readonly User _user;
        private static readonly string defaultProfilePicture = "\\res\\default ProfilePic.png";

        public static string ProfilePicture()
        {
            return defaultProfilePicture;
        }
        public UserController(Ta2jirContext db, IToastNotification toastNotification, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _toastNotification = toastNotification;
            _userType = LoginController.GetUserType();
            _user = LoginController.GetCurrentUser();
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Dashboard(uint? userId)
        {
            if (_userType == "admin" || _userType == "user")
            {
                if (_user.Birthdate != null)
                {
                    string? birthdateString = _user.Birthdate.ToString();
                    if (birthdateString != null)
                    {
                        ViewData["birthdate"] = DateTime.Parse(birthdateString).ToString("dd.MM.yyyy");
                    }
                }
                return View(_user);
            }
            else
            {
                return RedirectToAction("Logout", "Login");
            }
        }
        /// <summary>
        /// (Get Method)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Edit User View or Not Found Page</returns>
        [HttpGet]
        public IActionResult EditProfile(uint? userId)
        {
            if (_userType == "admin" || _userType == "user")
            {
                if (userId == null)
                {
                    return NotFound();
                }
                if (userId == 0)
                {
                    return NotFound();
                }
                User? userFromDB = _db.Users.Find(userId);
                if (userFromDB != null)
                {
                    string? birthdateString = userFromDB.Birthdate.ToString();
                    if (!String.IsNullOrWhiteSpace(birthdateString))
                    {
                        ViewData["birthdate"] = DateTime.Parse(birthdateString).ToString("dd.MM.yyyy");
                    }
                    return View(userFromDB);
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Your User data couldn't be found please contact us to solve your issue.");
                    return RedirectToAction("Logout", "Login");
                }
            }
            _toastNotification.AddErrorToastMessage("You are currently not logged in please log in to be able to use our service");
            return RedirectToAction("Logout", "Login");
        }

        /// <summary>
        /// Post Method
        /// </summary>
        /// <param name="userData"></param>
        /// <param name="newProfilePicFile"></param>
        /// <returns>One of different Viewa depending on the algorithm</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(User userData, IFormFile newProfilePicFile)
        {
            if (_userType == "admin" || _userType == "user")
            {
                try
                {
                    if (userData.UserId != 0)
                    {
                        User? userFromDB = await _db.Users.FindAsync(userData.UserId);
                        if (userFromDB != null)
                        {
                            //things that can't be updated
                            userData.DateJoined = userFromDB.DateJoined;
                            userData.UserRating = userFromDB.UserRating;
                            userData.Email = userFromDB.Email;
                            userData.IsBlocked = userFromDB.IsBlocked;
                            ModelState.Remove("Email");
                            ModelState.Remove("IsBlocked");
                            if (string.IsNullOrWhiteSpace(userData.Name))
                            {
                                ModelState.Remove("Name");
                                userData.Name = userFromDB.Name;
                            }
                            await UpdateProfilePic(userData, newProfilePicFile, userFromDB);
                        }
                        else
                        {
                            _toastNotification.AddErrorToastMessage("Your User data couldn't be found please contact us to solve your issue.");
                            return RedirectToAction("Logout", "Login");
                        }
                        CheckPasswordInput(ref userData, userFromDB);
                        if (userData.Name.Length > 50)
                        {
                            ModelState.AddModelError("Name", "This Name is long because it has more than 50 letters");
                        }
                        if (userData.Birthdate != null)
                        {
                            string? birthdateString = userData.Birthdate.ToString();
                            if (!string.IsNullOrWhiteSpace(birthdateString))
                            {
                                if (DateTime.Parse(birthdateString).Year > DateTime.Now.AddYears(-13).Year)
                                {
                                    ModelState.AddModelError("Birthdate", $"You need to be born before {DateTime.Now.AddYears(-12).Year}");
                                }
                            }
                        }
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("You are currently not logged in please log in to be able to use our service");
                        return RedirectToAction("Logout", "Login");
                    }
                    if (ModelState.IsValid)
                    {
                        _db.Users.Update(userData);
                        await _db.SaveChangesAsync();
                        _toastNotification.AddSuccessToastMessage("Your data has been updated successfully !");
                        return View(userData);
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Your data was invalid check the error messages");
                        return View(userData);
                    }
                }
                catch (Exception)
                {
                    _toastNotification.AddErrorToastMessage("There was an unexpected error please help us by reporting this error and try again");
                    return RedirectToAction("MyProfile", "User");
                }
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Unauthorized forbbiden action please login again or contact us if you think there is a mistake");
                return RedirectToAction("Logout", "Login");
            }
        }

        private async Task UpdateProfilePic(User userData, IFormFile newProfilePicFile, User? userFromDB)
        {
            if (newProfilePicFile != null && newProfilePicFile.Length > 0)
            {
                string folder = $"res\\Users\\{userData.Email}\\";
                string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                Directory.CreateDirectory(serverFolder);
                string file = $"{Guid.NewGuid()}_{userData.Name} Profile Picture_{newProfilePicFile.FileName}";
                string path = serverFolder + file;
                await newProfilePicFile.CopyToAsync(new FileStream(path, FileMode.Create));
                userData.ProfilePic = "\\" + folder + file;
            }
            else if (newProfilePicFile == null && String.IsNullOrWhiteSpace(userData.ProfilePic) && userData.ProfilePicFile == null)
            {
                ModelState.Remove("newProfilePicFile");
                userData.ProfilePic = userFromDB?.ProfilePic;
            }
        }

        private void CheckPasswordInput(ref User userData, User userFromDB)
        {
            if (userData.Password == null)
            {
                if (userData.NewPassword == null && userData.RepeatPassword == null)
                {
                    ModelState.Remove("Password");
                    userData.Password = userFromDB.Password;
                }
                else
                {
                    ModelState.AddModelError("Password", "Please enter your old password");
                }
            }
            else if (userData.Password != userFromDB.Password)
            {
                ModelState.AddModelError("Password", "The entered password is incorrect");
            }
            else if (userData.NewPassword == userFromDB.Password)
            {
                ModelState.AddModelError("NewPassword", "The new password is the same as your old password");
            }
            else if (userData.NewPassword == null)
            {
                ModelState.AddModelError("NewPassword", "This field is mandatory");
            }
            else if (userData.NewPassword.Length < 4)
            {
                ModelState.AddModelError("NewPassword", "Your password should contain at least 4 characters");
            }
            else
            {
                userData.Password = userData.NewPassword;
            }
        }

        public async Task<IActionResult> RemovePicture(uint? userId)
        {
            if (_userType == "admin" || _userType == "user")
            {
                try
                {
                    if (userId != 0)
                    {
                        User? userFromDB = await _db.Users.FindAsync(userId);
                        if (userFromDB != null)
                        {
                            userFromDB.ProfilePic = null;
                            userFromDB.ProfilePicFile = null;
                            if (ModelState.IsValid)
                            {
                                _db.Update(userFromDB);
                                await _db.SaveChangesAsync();
                                _toastNotification.AddSuccessToastMessage("Your profile picture is removed successfully");
                                return RedirectToAction("EditProfile", "User", userFromDB);
                            }
                            else
                            {
                                _toastNotification.AddErrorToastMessage("Your photo couldn't be removed. We are working on solving this issue");
                                return RedirectToAction("EditProfile", "User", userFromDB);
                            }
                        }
                        else
                        {
                            _toastNotification.AddErrorToastMessage("Your User data couldn't be found please contact us to solve your issue.");
                            return RedirectToAction("Logout", "Login");
                        }
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("You are currently not logged in please log in to be able to use our service");
                        return RedirectToAction("Logout", "Login");
                    }
                }
                catch (Exception)
                {
                    _toastNotification.AddErrorToastMessage("There was an unexpected error please help us by reporting this error and try again");
                    return RedirectToAction("MyProfile", "User");
                }
            }
            else
            {
                return RedirectToAction("Logout", "Login");
            }
        }
        [HttpGet]
        public async Task<IActionResult> MyProfile(uint? userId)
        {
            if (_userType == "admin" || _userType == "user")
            {
                User? user = await _db.Users.FindAsync(userId);
                if (_user.Birthdate != null)
                {
                    string? birthdateString = user?.Birthdate.ToString();
                    if (birthdateString != null)
                    {
                        ViewData["birthdate"] = DateTime.Parse(birthdateString).ToString("dd.MM.yyyy");
                    }
                }
                return View(_user);
            }
            else
            {
                return RedirectToAction("Logout", "Login");
            }
        }
        [HttpGet]
        public async Task<IActionResult> DeleteAccount(uint? userId)
        {
            if (_userType == "admin" || _userType == "user")
            {
                if (userId == null)
                {
                    return NotFound();
                }
                if (userId == 0)
                {
                    return NotFound();
                }
                User? userData = await _db.Users.FindAsync(userId);
                if (_userType == "user")
                {
                    if (userData != null)
                    {
                        userData.IsBlocked = "deleted";
                        userData.deletedOn = DateTime.Now.Date;
                        _db.Update(userData);
                        _db.SaveChangesAsync();
                        _toastNotification.AddInfoToastMessage(" We are sorry to say goodbye \U0001F622 Your account is deactivated and it will be deleted after 6 months");
                        return RedirectToAction("Logout", "Login");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Any error has occured please login and retry the process");
                        return RedirectToAction("Logout", "Login");
                    }
                }
                else if (_userType == "admin")
                {
                    if (userData != null)
                    {
                        _toastNotification.AddAlertToastMessage("Admins can not delete their own account");
                        return RedirectToAction("MyProfile", "User", userData.UserId);
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Any error has occured please login and retry the process");
                        return RedirectToAction("Logout", "Login");
                    }
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("You are currently not logged in please check your internet connection and log in");
                    return RedirectToAction("Logout", "Login");
                }

            }
            else
            {
                return RedirectToAction("Logout", "Login");
            }
        }
    }
}
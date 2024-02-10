using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ExamSystem.Models.Admin;
using System.Security.Claims;
using ExamSystem.Models;

namespace ExamSystem.Controllers.Admin
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        
            private readonly UserManager<User> _userManager;
            private readonly SignInManager<User> _signInManager;
            private readonly RoleManager<IdentityRole> _roleManager;

            public AccountController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager,
                SignInManager<User> signInManager)
            {
                this._userManager = userManager;
                this._signInManager = signInManager;
                _roleManager = roleManager;
            }


            [AllowAnonymous]
            [HttpGet]
            public IActionResult Login()
            {
                Login login = new Login();
                return View(login);

            }

            [AllowAnonymous]
            [HttpPost]
            public async Task<IActionResult> Login(Login login)
            {
                if (ModelState.IsValid)
                {
                    User appUser = await _userManager.FindByEmailAsync(login.Email);
                    if (appUser != null)
                    {
                        await _signInManager.SignOutAsync();
                        Microsoft.AspNetCore.Identity.SignInResult result =
                            await _signInManager.PasswordSignInAsync(appUser, login.Password, false, false);

                        if (result.Succeeded)
                        {
                            var user = await _userManager.FindByEmailAsync(login.Email);

                            bool roles = await _userManager.IsInRoleAsync(user, "User");

                            if (!roles)
                            {
                                // Redirect to the teacher page
                                return RedirectToAction("Index", "Examiner");
                            }
                            else
                            {
                                // Redirect to the user's dashboard or a default page
                                return RedirectToAction("User_Home", "Examinee");
                            }
                        }

                    }
                    ModelState.AddModelError(nameof(login.Email), "Invalid Email or Password");
                }
                return View(login);
            }

            [AllowAnonymous]
            public async Task<IActionResult> Logout()
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            [HttpGet]
            public IActionResult Create(RegisterUser registerUser)
            {
                ModelState.Clear();
                return View(registerUser);
            }

            [AllowAnonymous]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> CreateUser(RegisterUser user)
            {

                if (ModelState.IsValid)
                {
                    User appUser = new User
                    {
                        UserName = user.Username,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                    };

                    IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);

                    if (result.Succeeded)
                    {
                        string roleName = user.Role;

                        // Check if the role exists
                        var roleExists = await _roleManager.RoleExistsAsync(roleName);

                        if (!roleExists)
                        {
                            // Role doesn't exist, create it
                            result = await _roleManager.CreateAsync(new IdentityRole(roleName));

                            if (!result.Succeeded)
                            {
                                // Handle the error, e.g., log or return an error response
                                return StatusCode(500, "role is not created");
                            }
                        }

                        // add to the "User" role
                        await _userManager.AddToRoleAsync(appUser, roleName);

                        await _signInManager.RefreshSignInAsync(appUser);
                    if(roleName=="User")
                        return RedirectToAction("User_Home", "Examinee");
                    else
                        return RedirectToAction("Index", "Examiner");
                }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                        // If there are errors, return the same view with the model
                        return View("Create", user);
                    }
                }

                // If ModelState is not valid, return the same view with the model
                return View("Create", user);
            }

            [Authorize]
            public async Task<IActionResult> ViewAccount()
            {
                var currentUser = HttpContext.User;

                if (currentUser.Identity.IsAuthenticated)
                {
                    // User is logged in
                    var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (userId != null)
                    {
                        User appUser = await _userManager.FindByIdAsync(userId);
                        return View(appUser);
                    }
                }

                return View();
            }
            [Authorize]
            [HttpPost]
            public async Task<IActionResult> UpdateAccount(User updatedUser)
            {
                // Retrieve the existing user from the database
                var userid = updatedUser.Id;
                User existingUser = await _userManager.FindByIdAsync(userid);

                if (existingUser == null)
                {
                    // Handle the case where the user doesn't exist
                    return NotFound();
                }

                // Check if specific fields have been modified
                if (existingUser.UserName != updatedUser.UserName)
                {
                    // UserName has been modified
                    existingUser.UserName = updatedUser.UserName;
                }

                if (existingUser.FirstName != updatedUser.FirstName)
                {
                    // Email has been modified
                    existingUser.FirstName = updatedUser.FirstName;
                }

                if (existingUser.LastName != updatedUser.LastName)
                {
                    // Email has been modified
                    existingUser.LastName = updatedUser.LastName;
                }

                if (existingUser.Email != updatedUser.Email)
                {
                    // Email has been modified
                    existingUser.Email = updatedUser.Email;
                }

                // Check if the password field in updatedUser is provided
                if (!string.IsNullOrEmpty(updatedUser.PasswordHash))
                {
                    // Change the user's password'
                    var newPasswordHash = _userManager.PasswordHasher.HashPassword(existingUser, updatedUser.PasswordHash);

                    existingUser.PasswordHash = newPasswordHash;

                }


                // Update the properties of the existing user with the new values


                var result = await _userManager.UpdateAsync(existingUser);

                if (result.Succeeded)
                {
                    // Update was successful
                    await _signInManager.RefreshSignInAsync(existingUser);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Update failed, handle errors
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(updatedUser);
                }


            }

            [Authorize]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteAccount(string id)
            {
                var User = await _userManager.FindByIdAsync(id);

                if (User == null)
                {
                    return NotFound();
                }

            var result = await _userManager.DeleteAsync(User);

            if (result.Succeeded)
            {
                // User deletion successful
                await _signInManager.RefreshSignInAsync(User);
                return RedirectToAction("Index", "Home"); // Redirect to a success page or another appropriate action
            }
            else
            {
                return View(User); // Redirect to an error page or another appropriate action
            }
        }

        }

    }

using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Identity;
using ExamSystem.Models.Admin;
using ExamSystem.Models;
namespace ExamSystem.Controllers.Admin
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly ExamContext _DB;

    public AdminController(ExamContext dB, UserManager<User> userManager,
        SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHostEnvironment)
    {
        _DB = dB;
        this.webHostEnvironment = webHostEnvironment;
        this._userManager = userManager;
        this._signInManager = signInManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        //Retrieve all users using the UserManager by calling the Users property and converting the result to a list.
        var allUsers = _userManager.Users.ToList();

        // Filter the above users based on roles
        var users = allUsers.Where(user => _userManager.IsInRoleAsync(user, "User").Result).ToList();
        var admins = allUsers.Where(user => _userManager.IsInRoleAsync(user, "Admin").Result).ToList();
        var teachers = allUsers.Where(user => _userManager.IsInRoleAsync(user, "Teacher").Result).ToList();

        
        // Pass the lists to the view
        ViewData["Users"] = users;
        ViewData["Admins"] = admins;
        ViewData["Teachers"] = teachers;

        return View();
    }

    public async Task<IActionResult> CreateAdmin()
    {
        return View();

    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAdmin(RegisterUser user)
    {

        if (ModelState.IsValid)
        {
            //creates a new instance of the AppUser class
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
                string roleName = "Admin";

                // check if the "Admin" role exists
                var roleExists = await _roleManager.RoleExistsAsync(roleName);

                if (!roleExists)
                {
                    // Rrole does not exist, it creates the "Admin" role 
                    result = await _roleManager.CreateAsync(new IdentityRole(roleName));

                    if (!result.Succeeded)
                    {
                        // Handle the error, e.g., log or return an error response
                        return StatusCode(500, "Role is not created.");
                    }
                }

                // add to the "Admin" role
                await _userManager.AddToRoleAsync(appUser, roleName);


                return RedirectToAction("Index", "Admin");
            }
            else
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                // If there are errors, return the same view with the model
                return View("CreateAdmin", user);
            }
        }

        // If ModelState is not valid, return the same view with the model
        return View("CreateAdmin", user);
    }



    [HttpGet]
    public async Task<IActionResult> EditUser(string? userid)
    {
        if (userid == null)
        {
            return NotFound();
        }
        var user = await _userManager.FindByIdAsync(userid);


        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }



    [HttpPost]
    public async Task<IActionResult> EditUser(User updatedUser)
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
            return RedirectToAction("Index", "Admin");
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


    [HttpGet]
    public async Task<IActionResult> RemoveUser(string? userid)
    {
        if (userid == null)
        {
            return NotFound();
        }
        var user = await _userManager.FindByIdAsync(userid);


        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveAccount(User? appUser)
    {
        var AdminUser = await _userManager.FindByIdAsync(appUser.Id);

        if (AdminUser == null)
        {
            return NotFound();
        }
        var result = await _userManager.DeleteAsync(AdminUser);

        if (result.Succeeded)
        {
            // User deletion successful
            await _signInManager.RefreshSignInAsync(AdminUser);
            return RedirectToAction("Index", "Admin"); // Redirect to a success page or another appropriate action
        }
        else
        {
            return NotFound(); // Redirect to an error page or another appropriate action
        }


    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FPTBook.Models;
using FPTBook.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
public class UserRolesController : Controller
{
    private readonly UserManager<FPTBookUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserRolesController(UserManager<FPTBookUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index(string searchString)
    {
        var users = await _userManager.Users.ToListAsync();
        var userRolesViewModel = new List<UserRolesViewModel>();
        //var usersearch = userRolesViewModel.Where(p => p.Name == searchString);
        if (!String.IsNullOrEmpty(searchString))
        {
            for (int i = 0; i < users.Count(); i++)
            {
                if (users[i].UserName.Contains(searchString))
                {
                    var thisViewModel = new UserRolesViewModel();
                    thisViewModel.UserId = users[i].Id;
                    thisViewModel.Email = users[i].Email;
                    thisViewModel.Name = users[i].Name;
                    thisViewModel.UserName = users[i].UserName;
                    thisViewModel.Roles = await GetUserRoles(users[i]);
                    userRolesViewModel.Add(thisViewModel);
                }
            }
        }
        else
            {
                foreach (FPTBookUser user in users)
                {
                    var thisViewModel = new UserRolesViewModel();
                    thisViewModel.UserId = user.Id;
                    thisViewModel.Email = user.Email;
                    thisViewModel.Name = user.Name;
                    thisViewModel.UserName = user.UserName;
                    thisViewModel.Roles = await GetUserRoles(user);
                    userRolesViewModel.Add(thisViewModel);
                }
            }
            return View(userRolesViewModel);
        }
        public async Task<IActionResult> Manage(string userId)
        {
            ViewBag.userId = userId;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }
            ViewBag.UserName = user.UserName;
            var model = new List<ManageUserRolesViewModel>();
            foreach (var role in _roleManager.Roles.ToList())
            {
                var userRolesViewModel = new ManageUserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                model.Add(userRolesViewModel);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Manage(List<ManageUserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View();
            }
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
            return RedirectToAction("Index");
        }
        private async Task<List<string>> GetUserRoles(FPTBookUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }
    }
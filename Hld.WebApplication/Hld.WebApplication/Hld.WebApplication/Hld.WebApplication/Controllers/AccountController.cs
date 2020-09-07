using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Hld.WebApplication.Data;
using Hld.WebApplication.Models;
using Hld.WebApplication.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Hld.WebApplication.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private SignInManager<AppUser> signInManager;
        private readonly JwtAppSetting _jwtAppSetting;

        public AccountController(UserManager<AppUser> userMgr, SignInManager<AppUser> signinMgr, IOptions<JwtAppSetting> jwtAppSetting, RoleManager<IdentityRole> _roleManager)
        {
            this._jwtAppSetting = jwtAppSetting.Value;
            userManager = userMgr;
            roleManager = _roleManager;
            signInManager = signinMgr;
        }

        public IActionResult Index()
        {
            return View();
        }

        //[AllowAnonymous]
        //public IActionResult Login(string returnUrl)
        //{
        //    Login login = new Login();
        //    login.ReturnUrl = returnUrl;
        //    return View(login);
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(Login login)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            AppUser appUser = await userManager.FindByEmailAsync(login.Email);
        //            if (appUser != null)
        //            {
        //                await signInManager.SignOutAsync();
        //                Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(appUser, login.Password, false, false);
        //                if (result.Succeeded)
        //                {
        //     //               var roles = await userManager.GetRolesAsync(appUser);
                          
        //     //               IdentityRole identityRole = new IdentityRole();
                           
        //     //               var userRoles = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToArray();
        //     //               identityRole.Name = roles.Select(p => p).FirstOrDefault();
        //     //             IdentityRole identity = await roleManager.FindByNameAsync(identityRole.Name);
        //     //               var userClaims = await userManager.GetClaimsAsync(appUser).ConfigureAwait(false);
        //     //               var roleClaims = await roleManager.GetClaimsAsync(identity).ConfigureAwait(false);
                           
        //     //               var claims = new[]
        //     //                            {
        //     //    new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
        //     //    new Claim(ClaimTypes.Email, appUser.Email),
        //     //    new Claim(ClaimTypes.Name, appUser.UserName)
        //     //}.Union(userClaims).Union(roleClaims).Union(userRoles);

        //     //               var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAppSetting.SigningKey));
        //     //               var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //     //               var token = new JwtSecurityToken(
        //     //                   issuer: _jwtAppSetting.Site,
        //     //                   audience: _jwtAppSetting.Site,
        //     //                   claims: claims,
        //     //                   expires: DateTime.UtcNow.AddYears(1),
        //     //                   signingCredentials: creds);
        //                    return Redirect(login.ReturnUrl ?? "/");
        //                }

        //            }
        //            ModelState.AddModelError(nameof(login.Email), "Login Failed: Invalid Email or password");
        //        }
        //        return View(login);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
           
        //}
    }
}
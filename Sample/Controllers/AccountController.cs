using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Sample.Models;
using AspNetCore.MongoDB.JWTIdentity;

namespace Sample.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        MongoJWT<ApplicationUser> _mongoJWT;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            MongoJWT<ApplicationUser> mongoJWT
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mongoJWT = mongoJWT;
        }

        [HttpPost("token")]
        public async Task<JsonResult> Login([FromBody] LoginModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _mongoJWT.SetTokenExpired(DateTime.Now.AddHours(2));

                    var user = await _userManager.FindByNameAsync(model.UserName);

                    Dictionary<string, object> customPayload = new Dictionary<string, object>();
                    customPayload.Add("CustomText", "Custom payload Here");

                    _mongoJWT.SetCustomPayload(customPayload);

                    return await _mongoJWT.GetAuthTokenAsync(model.UserName);
                }

                return new JsonResult("UserName or Password is incorrect") { StatusCode = 401 };
            }

            return new JsonResult(ModelState.Values.SelectMany(x => x.Errors)) { StatusCode = 401 };
        }

        [HttpPost("register")]
        public async Task<JsonResult> Register([FromBody] RegisterModel model)
        {
            if(ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if(result.Succeeded)
                {
                    return new JsonResult("Successfully Register");
                }

                return GetErrors(result);
            }

            return new JsonResult(ModelState.Values.SelectMany(x => x.Errors)) { StatusCode = 401 };
        }

        private JsonResult GetErrors(IdentityResult result)
        {
            Dictionary<string, string> errorResult = new Dictionary<string, string>();

            foreach (var error in result.Errors)
            {
                errorResult.Add("", error.Description);
            }

            return new JsonResult(errorResult);
        }
    }
}
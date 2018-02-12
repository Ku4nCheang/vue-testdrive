using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using netcore.Models;
using netcore.Core.Extensions;
using netcore.Models.ViewModels.AuthViewModels;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using netcore.Core.Contants;
using netcore.Core.ErrorDescribers;
using netcore.Core.Services;
using netcore.Models.ViewModels.SharedViewModels;

namespace netcore.Controllers
{
    [Route("api/v1/[controller]")]
    public class AuthController : ApiController<AuthController>
    {   
        public AuthController (IServiceProvider serviceProvider, UserManager<User> userManager, IOptionsSnapshot<JwtBearerOptions> jwtBearerSnapshot):
            base(serviceProvider) 
        {
            UserManager = userManager;
            JwtBearerSnapshot = jwtBearerSnapshot;
        }

        protected readonly UserManager<User> UserManager;
        protected readonly IOptionsSnapshot<JwtBearerOptions> JwtBearerSnapshot;

        //
        // POST: /Account/Register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<JsonResult> Register([FromBody]RegisterViewModel model)
        {
            if (!ModelState.IsValid) {
                Logger.LogWarning(EventIds.RegisterError, "Registration was aborted since invalid model state.");
                return this.JsonInvalidModelState(ModelState);
            }

            var user = new User { UserName = model.Email, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.TransformIdentityErrors();
                return this.JsonError(errors.FirstOrDefault());
            }

            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
            // Send an email with this link
            //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
            //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
            //    "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
            // await _signInManager.SignInAsync(user, isPersistent: false);
            // _logger.LogInformation(3, "User created a new account with password.");
            // return RedirectToAction(nameof(HomeController.Index), "Home");
            
            Logger.LogInformation(EventIds.Register, $"User ({user.Email}) has been created");
            return this.JsonSuccess(Mapper.Map<UserViewModel>(user));
        }
    }
}

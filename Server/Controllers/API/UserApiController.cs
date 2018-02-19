using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using netcore.Core.Configurations;
using netcore.Core.Constants;
using netcore.Core.Contants;
using netcore.Core.ErrorDescribers;
using netcore.Core.Extensions;
using netcore.Core.Repositories;
using netcore.Core.Services;
using netcore.Core.Utilities;
using netcore.Models;
using netcore.Models.ViewModels.AccountApiViewModels;
using netcore.Models.ViewModels.SharedViewModels;
using netcore.Models.ViewModels.UserApiViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace netcore.Controllers.API
{
    // make sure the authorization schema is using jwt bearer, otherwise cookie authentication will be used.
    [Route("api/v1/users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserApiController : ApiController<AccountApiController>
    {
        public UserApiController(IServiceProvider serviceProvider, IUserRepository userRepository) : base(serviceProvider)
        {
            ErrorDescriber = new UserErrorDescriber();
            UserRepository = userRepository;
        }

        protected readonly UserErrorDescriber ErrorDescriber;
        protected readonly IUserRepository UserRepository;

        //
        // ─── GET USERS API ───────────────────────────────────────────────
        //

        [HttpGet]
        [Authorize(Roles="SystemUser,Administrator")]
        public async Task<JsonResult> GetUsers(int count = 30, int page = 1, UserOrderBy orderBy = UserOrderBy.UserName, UserOrderDir dir = UserOrderDir.Ascending)
        {
            page = (page < 1)? 1 : page;
            var users = await UserRepository.GetUsersAsync(count, page, orderBy, dir);
            var usersDTO = Mapper.Map<List<UserViewModel>>(users);
            // return sucess response
            return this.JsonSuccess( new {
                Data = usersDTO,
                Total = await UserRepository.CountUsersAsync()
            });
        }

        //
        // ─── GET USER API ────────────────────────────────────────────────
        //

        [HttpGet("{id}")]
        [Authorize(Policy="OwnerOrInternalUser", AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
        public async Task<JsonResult> GetUser(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            // return 404 if user not found
            if (user == null || user.Deleted)
                return this.JsonError(ErrorDescriber.UserNotFound());

            var usersDTO = Mapper.Map<UserViewModel>(user);
            // return sucess response
            return this.JsonSuccess( usersDTO );
        }

        //
        // ─── UPDATE USER API ─────────────────────────────────────────────
        //

        [HttpPut("{id}")]
        [Authorize(Policy="OwnerOrInternalUser", AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
        public async Task<JsonResult> UpdateProfile(string id, [FromBody] UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Logger.LogInformation(EventIds.UpdateProfileError, "Update profile action was aborted since invalid data.");
                return this.JsonInvalidModelState(ModelState);
            }

            var user = await UserManager.FindByIdAsync(id);

            // return 404 if user not found
            if (user == null || user.Deleted)
                return this.JsonError(ErrorDescriber.UserNotFound());

            var updatedFields = new Dictionary<string, string>();

            if (model.DateOfBirth != null) 
            {
                updatedFields.Add(nameof(user.DateOfBirth), $"({user.DateOfBirth.ToString()} ) -> ({user.DateOfBirth.ToString()})");
                user.DateOfBirth = model.DateOfBirth;
            }

            if (model.DisplayName != null) 
            {
                updatedFields.Add(nameof(user.DisplayName), $"({user.DisplayName.ToString()}) -> ({model.DisplayName.ToString()})");
                user.DisplayName = model.DisplayName;
            }

            if (model.Email != null) 
            {
                updatedFields.Add(nameof(user.Email), $"({user.Email.ToString()}) -> ({model.Email.ToString()})");
                user.Email = model.Email;
                // just set email or using usermanager to update the email via sending token to confirm changes.
            }

            if (model.PhoneNumber != null) 
            {
                updatedFields.Add(nameof(user.PhoneNumber), $"({user.PhoneNumber.ToString()}) -> ({model.PhoneNumber.ToString()})");
                user.PhoneNumber = model.PhoneNumber;
                // just set phone number or using usermanager to update the phone number via sending token to confirm changes.
            }

            if (model.Gender != null)
            {
                updatedFields.Add(nameof(user.Gender), $"({user.Gender.ToString()}) -> ({model.Gender.ToString()})");
                user.Gender = model.Gender.Value;
            }
            
            var result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = result.Errors.TransformIdentityErrors();
                return this.JsonError(errors.FirstOrDefault());
            }

            var values = updatedFields
                .Select( field => $"{field.Key}: {field.Value}")
                .ToArray();

            // log the updated field value for the user
            Logger.LogDebug(EventIds.UpdateProfile , $"Fields have changed: {String.Join(", ", values)}");

            var usersDTO = Mapper.Map<UserViewModel>(user);
            // return sucess response
            Logger.LogInformation(EventIds.UpdateProfile , $"User ({user.Id}) profile has been updated.");
            return this.JsonAccepted( usersDTO );
        }

        //
        // ─── DEACTIVATE USER API ─────────────────────────────────────────
        //

        [HttpDelete("{id}")]
        [Authorize(Policy="OwnerOrInternalUser", AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
        public async Task<JsonResult> DeactivateUser(string id, [FromBody] DeactivateUserViewModel model)
        {
            var user = await UserManager.FindByIdAsync(id);

            // return 404 if user not found
            if (user == null)
                return this.JsonError(ErrorDescriber.UserNotFound());
            else if (user.Deleted && !model.Hard)
                return this.JsonError(ErrorDescriber.UserAlreadyDeactivated());

            IdentityResult result = null;

            if (!model.Hard)
            {
                // set user is deleted
                user.Deleted = true;
                result = await UserManager.UpdateAsync(user);   
            }
            else
            {
                result = await UserManager.DeleteAsync(user);
            }

            if (result != null && result.Succeeded)
            {
                var message = model.Hard ? $"User ({user.Id}) has been deleted." : $"User ({user.Id}) has been deactiviated.";
                Logger.LogInformation(EventIds.DeactivateAccount, message);
                // return sucess response
                return this.JsonAccepted(new {});
            }

            // no need to log warning message since usermanage has handled logging when errors occured.
            var errors = result?.Errors.TransformIdentityErrors();
            return this.JsonError(errors.FirstOrDefault());
        }



    }


}

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
using netcore.Models.ViewModels.AccountViewModels;
using netcore.Models.ViewModels.SharedViewModels;
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
        public async Task<JsonResult> GetUsers(int count = 30, int page = 1)
        {
            var u = User;
            var u1 = await UserManager.GetUserAsync(User);
            var users = await UserRepository.GetAllUsersAsync();
            var usersDTO = Mapper.Map<List<UserViewModel>>(users);
            // return sucess response
            return this.JsonSuccess( new {
                Data = usersDTO,
                Total = await UserRepository.CountUsersAsync()
            });
        }
    }
}

using AutoMapper;
using HotelListing.Data;
using HotelListing.Models;
using HotelListing.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApiUser> userManager;
        //private readonly SignInManager<ApiUser> signInManager;
        private readonly ILogger<CountryController> logger;
        private readonly IMapper mapper;
        private readonly IAuthManager authManager;

        public AccountController(UserManager<ApiUser> userManager, ILogger<CountryController> logger, IMapper mapper, IAuthManager authManager)
        {
            this.userManager = userManager;
            //this.signInManager = signInManager;
            this.logger = logger;
            this.mapper = mapper;
            this.authManager = authManager;
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            this.logger.LogInformation($"Registration Attempt for {userDTO.Email}");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try//including ErrorHandler we don't need try catch anymore
            {
                ApiUser user = this.mapper.Map<ApiUser>(userDTO);
                user.UserName = userDTO.Email;
                var result = await this.userManager.CreateAsync(user, userDTO.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(error.Code, error.Description);

                    return BadRequest(ModelState);
                }

                await userManager.AddToRolesAsync(user, userDTO.Roles);
                return Accepted();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Something went wrong in the {nameof(Register)}");
                return Problem($"Something went wrong in the {nameof(Register)}", statusCode: 500);
            }
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LogIn([FromBody] LogInUserDTO userDTO)
        {
            this.logger.LogInformation($"LogIn Attempt for {userDTO.Email}");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (!await authManager.ValidateUser(userDTO))
                    return Unauthorized();

                return Accepted(new { Token = await authManager.CreateToken() });

                //var result = await this.signInManager.PasswordSignInAsync(userDTO.Email, userDTO.Password, false, false);
                //if (!result.Succeeded)
                //    return Unauthorized(userDTO);

                //return Accepted();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Something went wrong in the {nameof(LogIn)}");
                return Problem($"Something went wrong in the {nameof(LogIn)}", statusCode: 500);
            }
        }
    }
}

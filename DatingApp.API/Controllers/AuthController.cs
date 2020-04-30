using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;
        public IConfiguration _config { get; set; }

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(IAuthRepository authRepository, IMapper mapper, IConfiguration config,
        UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _authRepository = authRepository;
            _mapper = mapper;
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // [HttpPost("register")]
        // public async Task<IActionResult> Register(UserForRegistrationDTO userForRegistrationDTO)
        // {
        //     userForRegistrationDTO.Username = userForRegistrationDTO.Username.ToLower();

        //     if (await _authRepository.DoesUserExist(userForRegistrationDTO.Username))
        //     {
        //         return BadRequest("Username already exists.");
        //     }

        //     var userToCreate = _mapper.Map<User>(userForRegistrationDTO);

        //     var createdUser = await _authRepository.Register(userToCreate, userForRegistrationDTO.Password);

        //     var userToReturn = _mapper.Map<UserForAddDTO>(createdUser);

        //     return CreatedAtRoute("GetUser", new { controller = "Users", id = createdUser.Id }, userToReturn);
        // }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegistrationDTO userForRegistrationDTO)
        {
            var userToCreate = _mapper.Map<User>(userForRegistrationDTO);

            var result = await _userManager.CreateAsync(userToCreate, userForRegistrationDTO.Password); 

            var userToReturn = _mapper.Map<UserForAddDTO>(userToCreate);

            if (result.Succeeded)
            {
                return CreatedAtRoute("GetUser", new { controller = "Users", id = userToCreate.Id }, userToReturn);
            }

            return BadRequest(result.Errors);
        }

        // [HttpPost("login")]
        // public async Task<IActionResult> Login(UserForLoginDTO userForLoginDTO)
        // {
        //     var userFromRepo = await _authRepository.Login(userForLoginDTO.Username.ToLower(), userForLoginDTO.Password);

        //     if (userFromRepo == null)
        //     {
        //         return Unauthorized();
        //     }

        //     var user = _mapper.Map<UserForUseDTO>(userFromRepo);

        //     return Ok(new
        //     {
        //         token = GenerateJwtToken(userFromRepo),
        //         userFromRepo
        //     });
        // }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO userForLoginDTO)
        {
            var user = await _userManager.FindByNameAsync(userForLoginDTO.Username);

            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDTO.Password, false);

            if (result.Succeeded)
            {
                var appUser = _mapper.Map<UserForUseDTO>(user);

                return Ok(new
                {
                    token = GenerateJwtToken(user).Result,
                    user = appUser
                });
            }

            return Unauthorized();
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}
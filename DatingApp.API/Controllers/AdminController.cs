using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public AdminController(DataContext context, UserManager<User> userManager, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _context = context;
            _userManager = userManager;
            _cloudinaryConfig = cloudinaryConfig;

            Account account = new Account(_cloudinaryConfig.Value.CloudName, _cloudinaryConfig.Value.ApiKey, _cloudinaryConfig.Value.ApiSecret);

            _cloudinary = new Cloudinary(account);
        }

        [Authorize(Policy = "ModeratorPolicy")]
        [HttpGet("getPhotosForModeration")]
        public async Task<IActionResult> GetPhotosForModeration()
        {
            var photos = await _context.Photos.Include(i => i.User).IgnoreQueryFilters()
                                                                   .Where(w => w.IsApproved == false)
                                                                   .Select(s => new
                                                                   {
                                                                       Id = s.Id,
                                                                       UserName = s.User.UserName,
                                                                       Url = s.Url,
                                                                       IsApproved = s.IsApproved
                                                                   }).ToListAsync();

            return Ok(photos);
        }

        [Authorize(Policy = "ModeratorPolicy")]
        [HttpPost("approvePhoto/{photoId}")]
        public async Task<IActionResult> ApprovePhoto(int photoId)
        {
            var photo = await _context.Photos.IgnoreQueryFilters()
                                             .FirstOrDefaultAsync(x => x.Id == photoId);

            photo.IsApproved = true;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Policy = "ModeratorPolicy")]
        [HttpPost("rejectPhoto/{photoId}")]
        public async Task<IActionResult> RejectPhoto(int photoId)
        {
            var photo = await _context.Photos.IgnoreQueryFilters()
                                             .FirstOrDefaultAsync(x => x.Id == photoId);

            if (photo.IsMain)
            {
                return BadRequest("Main photo can't be rejected.");
            }

            if (photo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photo.PublicId);

                var result = _cloudinary.Destroy(deleteParams);

                if (result.Result == "ok")
                {
                    _context.Photos.Remove(photo);
                }
            }

            if (photo.PublicId == null)
            {
                _context.Photos.Remove(photo);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var users = await _context.Users
                                      .OrderBy(o => o.UserName)
                                      .Select(s => new
                                      {
                                          Id = s.Id,
                                          UserName = s.UserName,
                                          Roles = (from userRole in s.UserRole
                                                   join role in _context.Roles
                                                   on userRole.RoleId equals role.Id
                                                   select role.Name).ToList()
                                      }).ToListAsync();

            return Ok(users);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleForUpdateDTO roleRequest)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var userRoles = await _userManager.GetRolesAsync(user);

            var selectedRoles = roleRequest.RoleNames;

            // selectedRoles = selectedRoles != null ? selectedRoles : new string[] {};
            selectedRoles = selectedRoles ?? new string[] { };

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
            {
                return BadRequest("Failed to add roles.");
            }

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
            {
                return BadRequest("Failed to remove roles.");
            }

            return Ok(await _userManager.GetRolesAsync(user));
        }
    }
}
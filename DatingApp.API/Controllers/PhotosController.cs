using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _options;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository datingRepository, IMapper mapper, IOptions<CloudinarySettings> options)
        {
            _datingRepository = datingRepository;
            _mapper = mapper;
            _options = options;

            Account account = new Account(_options.Value.CloudName, _options.Value.ApiKey, _options.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _datingRepository.GetPhoto(id);

            var photo = _mapper.Map<PhotoForReturnDTO>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForAddDTO photoRequest)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var uploadResults = new ImageUploadResult();
            var userFromRepo = await _datingRepository.GetUser(userId);
            var file = photoRequest.File;

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResults = _cloudinary.Upload(uploadParams);
                }
            }

            photoRequest.Url = uploadResults.Uri.ToString();
            photoRequest.PublicId = uploadResults.PublicId;

            var photo = _mapper.Map<Photo>(photoRequest);

            if (!userFromRepo.Photos.Any(a => a.IsMain))
            {
                photo.IsMain = true;
            }

            userFromRepo.Photos.Add(photo);

            if (await _datingRepository.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDTO>(photo);

                return CreatedAtRoute("GetPhoto", new { userId = userId, id = photo.Id }, photoToReturn);
            }

            return BadRequest("Could not add the photo.");
        }

        [HttpPost("{id}/setMainPhoto")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userFromRepo = await _datingRepository.GetUser(userId);

            if (!userFromRepo.Photos.Any(a => a.Id == id))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _datingRepository.GetPhoto(id);

            if (photoFromRepo.IsMain)
            {
                return BadRequest("This is already set as main photo.");
            }

            var currentMainPhoto = await _datingRepository.GetUserMainPhoto(userId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if (await _datingRepository.SaveAll())
            {
                return NoContent();
            }

            return BadRequest("Could not set photo as main.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhotoForUser(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userFromRepo = await _datingRepository.GetUser(userId);

            if (!userFromRepo.Photos.Any(a => a.Id == id))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _datingRepository.GetPhoto(id);

            if (photoFromRepo.IsMain)
            {
                return BadRequest("You can not delete your main photo.");
            }

            if (photoFromRepo.PublicId == null)
            {
                _datingRepository.Remove(photoFromRepo);
            }
            else
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);

                if (result.Result == "ok")
                {
                    _datingRepository.Remove(photoFromRepo);
                }
            }

            if (await _datingRepository.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Could not delete the photo.");
        }

    }
}
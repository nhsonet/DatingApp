using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    // [Authorize]
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository datingRepository, IMapper mapper)
        {
            _datingRepository = datingRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await _datingRepository.GetUser(currentUserId, true);

            userParams.UserId = currentUserId;

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }

            var users = await _datingRepository.GetUsers(userParams);
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDTO>>(users);

            Response.AddPagination(users.PageNumber, users.PageSize, users.TotalItem, users.TotalPage);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var isCurrentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == id;

            var user = await _datingRepository.GetUser(id, isCurrentUser);
            var userToReturn = _mapper.Map<UserForDetailDTO>(user);

            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDTO userRequest)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            var userFromRepo = await _datingRepository.GetUser(id, true);

            _mapper.Map(userRequest, userFromRepo);

            if (await _datingRepository.SaveAll()) {
                return NoContent();
            }

            throw new Exception($"Updating user with {id} failed on save.");
        }

        [HttpPost("{id}/Like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            var like = await _datingRepository.GetLike(id, recipientId);

            if (like != null)
            {
                return BadRequest("You already like this user.");
            }

            if (await _datingRepository.GetUser(recipientId, false) == null)
            {
                return NotFound();
            }

            like = new Like
            {
                LikerId = id,
                LikeeId = recipientId
            };

            _datingRepository.Add<Like>(like);

            if (await _datingRepository.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Failed to like user.");
        }

    }
}
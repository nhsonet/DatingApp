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
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _mapper;

        public MessagesController(IDatingRepository datingRepository, IMapper mapper)
        {
            _datingRepository = datingRepository;
            _mapper = mapper;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            var messageFromRepo = await _datingRepository.GetMessage(id);

            if (messageFromRepo == null)
            {
                return NotFound();
            }

            return Ok(messageFromRepo);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery]MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            messageParams.UserId = userId;

            var messagesFromRepo = await _datingRepository.GetMessagsForUser(messageParams);

            var messages = _mapper.Map<IEnumerable<MessageForReturnDTO>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.PageNumber, messagesFromRepo.PageSize, messagesFromRepo.TotalItem, messagesFromRepo.TotalPage);

            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            var messagesFromRepo = await _datingRepository.GetMessageThread(userId, recipientId);

            var messageThread = _mapper.Map<IEnumerable<MessageForReturnDTO>>(messagesFromRepo);

            return Ok(messageThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForAddDTO messageRequest)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            var sender = await _datingRepository.GetUser(userId, false);

            if (sender == null)
            {
                return BadRequest("Could not find sender.");
            }

            messageRequest.SenderId = userId;

            var recipient = await _datingRepository.GetUser(messageRequest.RecipientId, false);

            if (recipient == null)
            {
                return BadRequest("Could not find recipient.");
            }

            var message = _mapper.Map<Message>(messageRequest);

            _datingRepository.Add(message);

            if (await _datingRepository.SaveAll())
            {
                var messageToReturn = _mapper.Map<MessageForReturnDTO>(message);
                return CreatedAtRoute("GetMessage", new {userId, id = message.Id}, messageToReturn);
            }

            throw new Exception("Creating message failed on save.");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            var message = await _datingRepository.GetMessage(id);

            if (message.RecipientId != userId)
            {
                return Unauthorized();
            }

            message.IsRead = true;
            message.ReadAt = DateTime.Now;

            await _datingRepository.SaveAll();

            return NoContent();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            var messagesFromRepo = await _datingRepository.GetMessage(id);

            if (messagesFromRepo.SenderId == userId)
            {
                messagesFromRepo.IsDeletedBySender = true;
            }

            if (messagesFromRepo.RecipientId == userId)
            {
                messagesFromRepo.IsDeletedByRecipient = true;
            }

            if (messagesFromRepo.IsDeletedBySender && messagesFromRepo.IsDeletedByRecipient)
            {
                _datingRepository.Remove(messagesFromRepo);
            }

            if (await _datingRepository.SaveAll())
            {
                return NoContent();
            }

            throw new Exception("Deleting message failed.");
        }

    }
}
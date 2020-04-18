using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.DTOs
{
    public class PhotoForAddDTO
    {
        public PhotoForAddDTO()
        {
            CreatedAt = DateTime.Now;
        }

        public string PublicId { get; set; }
        public string Url { get; set; }
        public IFormFile File { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Remove<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            // eager loading
            var user = await _context.User.Include(i => i.Likers).Include(i => i.Likees).FirstOrDefaultAsync(x => x.Id == id);

            // lazy loading
            // var user = await _context.User.FirstOrDefaultAsync(x => x.Id == id);

            if (likers)
            {
                return user.Likers.Where(w => w.LikeeId == id).Select(s => s.LikerId);
            }
            else
            {
                return user.Likees.Where(w => w.LikerId == id).Select(s => s.LikeeId);
            }
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            // eager loading
            // var users = await _context.User.Include(i => i.Photos).ToListAsync();
            var users = _context.User.Include(i => i.Photos).OrderByDescending(o => o.LastActive).AsQueryable();

            // lazy loading
            // var users = _context.User.OrderByDescending(o => o.LastActive).AsQueryable();

            users = users.Where(w => w.Id != userParams.UserId);
            users = users.Where(w => w.Gender == userParams.Gender);

            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(w => userLikers.Contains(w.Id));
            }

            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(w => userLikees.Contains(w.Id));
            }

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(- userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(- userParams.MinAge);

                users = users.Where(w => w.DateOfBirth >= minDob && w.DateOfBirth <= maxDob);
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "createdAt":
                        users = users.OrderByDescending(o => o.CreatedAt);
                        break;
                    default:
                        users = users.OrderByDescending(o => o.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageSize, userParams.PageNumber);
        }

        public async Task<User> GetUser(int id)
        {
            // eager loading
            var user = await _context.User.Include(i => i.Photos).FirstOrDefaultAsync(x => x.Id == id);

            // lazy loading
            // var user = await _context.User.FirstOrDefaultAsync(x => x.Id == id);

            return user;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photo.FirstOrDefaultAsync(x => x.Id == id);

            return photo;
        }

        public async Task<Photo> GetUserMainPhoto(int userId)
        {
            return await _context.Photo.Where(w => w.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Like.FirstOrDefaultAsync(x => x.LikerId == userId && x.LikeeId == recipientId);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Message.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagsForUser(MessageParams messageParams)
        {
            // eager loading
            var messages = _context.Message
                .Include(i => i.Sender).ThenInclude(i => i.Photos).Include(i => i.Recipient).ThenInclude(i => i.Photos)
                .AsQueryable();

            // lazy loading
            // var messages = _context.Message.AsQueryable();

            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(w => w.RecipientId == messageParams.UserId && w.IsDeletedByRecipient == false);
                    break;

                case "Outbox":
                    messages = messages.Where(w => w.SenderId == messageParams.UserId && w.IsDeletedBySender == false);
                    break;

                default:
                    messages = messages.Where(w => w.RecipientId == messageParams.UserId &&
                                                   w.IsDeletedByRecipient == false && w.IsRead == false);
                    break;
            }

            messages = messages.OrderByDescending(o => o.SentAt);
            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            // eager loading
            var messages = await _context.Message
                .Include(i => i.Sender).ThenInclude(i => i.Photos).Include(i => i.Recipient).ThenInclude(i => i.Photos)
                .Where(w => w.RecipientId == userId && w.IsDeletedByRecipient == false && w.SenderId == recipientId ||
                            w.RecipientId == recipientId && w.SenderId == userId && w.IsDeletedBySender == false)
                .OrderByDescending(o => o.SentAt)
                .ToListAsync();

            // lazy loading
            // var messages = await _context.Message
            //     .Where(w => w.RecipientId == userId && w.IsDeletedByRecipient == false && w.SenderId == recipientId ||
            //                 w.RecipientId == recipientId && w.SenderId == userId && w.IsDeletedBySender == false)
            //     .OrderByDescending(o => o.SentAt)
            //     .ToListAsync();

            return messages;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        
    }
}
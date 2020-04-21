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

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            // var users = await _context.User.Include(i => i.Photos).ToListAsync();

            var users = _context.User.Include(i => i.Photos).OrderByDescending(o => o.LastActive).AsQueryable();

            users = users.Where(w => w.Id != userParams.UserId);
            users = users.Where(w => w.Gender == userParams.Gender);

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
            var user = await _context.User.Include(i => i.Photos).FirstOrDefaultAsync(x => x.Id == id);

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

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
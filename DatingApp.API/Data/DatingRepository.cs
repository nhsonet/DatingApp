using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _context.User.Include(i => i.Photos).ToListAsync();

            return users;
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
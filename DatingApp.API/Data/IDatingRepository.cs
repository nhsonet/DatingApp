using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T: class;
        void Remove<T>(T entity) where T: class;
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(int id);
        Task<Photo> GetPhoto(int id);
        Task<Photo> GetUserMainPhoto(int userId);
        Task<bool> SaveAll();
    }
}
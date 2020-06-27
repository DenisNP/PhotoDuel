using System.Linq;
using PhotoDuel.Models;
using PhotoDuel.Models.Web;

namespace PhotoDuel.Services
{
    public class UserService
    {
        private readonly IDbService _dbService;

        public UserService(IDbService dbService)
        {
            _dbService = dbService;
        }

        public User Load(InitRequest request)
        {
            var user = _dbService.Collection<User>("users").FirstOrDefault(u => u.Id == request.UserId);

            if (user == null)
            {
                user = new User
                {
                    Id = request.UserId,
                    Rating = 0
                };
            }

            user.Name = request.UserName;
            user.Photo = request.UserPhoto;
            
            _dbService.UpdateAsync("users", user);

            return user;
        }
    }
}
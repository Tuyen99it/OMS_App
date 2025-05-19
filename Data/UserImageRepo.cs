
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using OMS_App.Models;

namespace OMS_App.Data
{
    public class UserImageRepo : IUserImageRepo
    {
        private readonly OMSDBContext _context;
        public UserImageRepo(OMSDBContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateUserImageAsync(UserImage userImage)
        {
            if (userImage == null)
            {
                throw new ArgumentNullException("UserImage is null");
            }
            await _context.UserImages.AddAsync(userImage);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        public async Task<UserImage> GetUserImageAsync(string userId, string userImageId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException("UserId is null");

            }
            return await _context.UserImages
                    .Where(u => u.AppUserId == userId && u.Id == Convert.ToInt32(userImageId))
                    .FirstOrDefaultAsync();
        }

        public async Task<List<UserImage>> GetUserImagesAsync(string userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException("UserId is null");
            }
            return await _context.UserImages
                    .Where(u => u.AppUserId == userId)
                    .ToListAsync();
        }



        public async Task<string> GetUserProfileUrlAsync(string userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException("UserId is null");
            }
            var userImage = await _context.UserImages
                    .Where(u => u.AppUserId == userId && u.IsActive == true).FirstOrDefaultAsync();

            return userImage?.ImagePath;
        }
    }
}
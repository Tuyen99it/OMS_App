namespace OMS_App.Data
{
    public interface IUserImageRepo
    {
        Task<bool> CreateUserImageAsync(UserImage userImage);

        Task<List<UserImage>> GetUserImagesAsync(string userId);
        Task<UserImage> GetUserImageAsync(string userId, string userImageId);
        Task<string> GetUserProfileUrlAsync(string userId);

    }
}
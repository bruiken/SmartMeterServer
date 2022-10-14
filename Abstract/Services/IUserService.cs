namespace Abstract.Services
{
    public interface IUserService
    {
        Models.User? GetById(int id);

        bool TryLoginWithRefreshToken();

        bool TryLogin(string username, string password, bool rememberLogin);

        int CreateUser(Models.CreateUser user);

        void CreateAdminUser();
    }
}

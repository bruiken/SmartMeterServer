namespace Abstract.Services
{
    public interface IUserService
    {
        Models.User? GetById(int id);

        bool TryLoginWithRefreshToken();

        void Login(string username, string password, bool rememberLogin);

        void Logout();

        int CreateUser(Models.CreateUser user);

        void CreateAdminUser();
    }
}

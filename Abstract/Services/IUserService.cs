namespace Abstract.Services
{
    public interface IUserService
    {
        Models.User? GetById(int id);

        bool TryLoginWithRefreshToken();

        void Login(string username, string password, bool rememberLogin);

        int CreateUser(Models.CreateUser user);

        void CreateAdminUser();
    }
}

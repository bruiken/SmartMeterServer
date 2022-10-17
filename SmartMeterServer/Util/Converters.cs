namespace Rotom.Util
{
    public static class Converters
    {
        public static Abstract.Models.CreateUser Convert(Models.CreateUserModel model)
        {
            return new Abstract.Models.CreateUser
            {
                Username = model.Username,
                Password = model.Password,
            };
        }

        public static Models.UserModel Convert(Abstract.Models.User model)
        {
            return new Models.UserModel
            {
                Id = model.Id,
                Username = model.Username,
            };
        }
    }
}

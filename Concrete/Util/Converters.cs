namespace Concrete.Util
{
    static class Converters
    {
        public static Abstract.Models.User Convert(Data.Models.User user)
        {
            return new Abstract.Models.User
            {
                Id = user.Id,
                Username = user.Username,
            };
        }
    }
}

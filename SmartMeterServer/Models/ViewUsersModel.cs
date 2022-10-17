namespace Rotom.Models
{
    public class ViewUsersModel : ErrorViewModel
    {
        public IEnumerable<UserModel> Users { get; set; }
    }
}

namespace Abstract.Models
{
    public class User
    {
        public int Id { get; set; }
        
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string Username { get; set; }
    }
}

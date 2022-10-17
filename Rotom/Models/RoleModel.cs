namespace Rotom.Models
{
    public class RoleModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public PermissionsModel Permissions { get; set; }
    }
}

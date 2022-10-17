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
                RoleId = model.RoleId,
            };
        }

        public static Models.UserModel Convert(Abstract.Models.User model)
        {
            return new Models.UserModel
            {
                Id = model.Id,
                Username = model.Username,
                RoleId = model.RoleId,
                RoleName = model.RoleName,
            };
        }

        public static Models.RoleModel Convert(Abstract.Models.Role model)
        {
            return new Models.RoleModel
            {
                Id = model.Id,
                Name = model.Name,
                Permissions = Convert(model.Permissions),
            };
        }

        public static Models.PermissionsModel Convert(Abstract.Models.Permissions model)
        {
            return new Models.PermissionsModel
            {
                CanModifyUsers = model.CanModifyUsers,
            };
        }
    }
}

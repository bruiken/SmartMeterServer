namespace Abstract.Services
{
    public interface IRoleService
    {
        Models.Role? GetById(int id);

        Models.Role? GetByUser(int userId);

        bool RoleIsAllowedPermission(Models.Role role, Models.EPermission permission);

        int SeedRoles();

        IEnumerable<Models.Role> GetRoles();
    }
}

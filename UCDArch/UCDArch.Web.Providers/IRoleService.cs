namespace UCDArch.Web.Providers
{
    /// <summary>
    /// Role Service for providing ASP.NET Role Provider data
    /// </summary>
    /// <remarks>
    /// Role Provider Essential Methods:
    /// IsUserInRole
    /// GetAllRoles
    /// GetRoleForUser
    /// GetUsersInRole
    /// RoleExists
    /// </remarks>
    public interface IRoleService
    {
        bool IsUserInRole(string application, string user, string role);

        string[] GetAllRoles(string application);

        string[] GetRolesForUser(string application, string user);

        string[] GetUsersInRole(string application, string roleName);

        bool RoleExists(string application, string role);
    }
}
using System.ServiceModel;

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
    [ServiceContract(Namespace = "https://secure.caes.ucdavis.edu/Catbert4")]
    public interface IRoleService
    {
        [OperationContract]
        bool IsUserInRole(string application, string user, string role);

        [OperationContract]
        string[] GetAllRoles(string application);

        [OperationContract]
        string[] GetRolesForUser(string application, string user);

        [OperationContract]
        string[] GetUsersInRole(string application, string roleName);

        [OperationContract]
        bool RoleExists(string application, string role);
    }
}
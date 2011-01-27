using System.ServiceModel;

namespace UCDArch.Web.Providers
{
    public class RoleServiceClient : ClientBase<IRoleService>, IRoleService
    {
        public RoleServiceClient(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public bool IsUserInRole(string application, string user, string role)
        {
            return base.Channel.IsUserInRole(application, user, role);
        }

        public string[] GetAllRoles(string application)
        {
            return base.Channel.GetAllRoles(application);
        }

        public string[] GetRolesForUser(string application, string user)
        {
            return base.Channel.GetRolesForUser(application, user);
        }

        public string[] GetUsersInRole(string application, string roleName)
        {
            return base.Channel.GetUsersInRole(application, roleName);
        }

        public bool RoleExists(string application, string role)
        {
            return base.Channel.RoleExists(application, role);
        }
    }
}
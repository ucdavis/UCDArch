using System;
using System.Collections.Specialized;
using System.ServiceModel;
using System.Web.Configuration;
using System.Web.Security;

namespace UCDArch.Web.Providers
{
    /// <summary>
    /// CatbertServiceRoleProvider inherits from the generic RoleProvider provided by ASP.NET that is part
    /// of their provider model (which includes Membership, Role and Profile Providers)
    /// </summary>
    /// <see cref="http://msdn.microsoft.com/asp.net/downloads/providers/"/>
    public class CatbertServiceRoleProvider : RoleProvider
    {
        //Statics
        private const string StrUseCatbertWeb = "Use the Catbert Web UI for role modification";
        private const string StrServiceDataNotReadable = "Service Data Could Not Be Read";

        //private vars
        private string _description;

        //public accessors
        public override string Description
        {
            get { return _description; }
        }

        private string ServiceUrl { get; set; }

        private string AuthToken { get; set; }

        public override string ApplicationName { get; set; }

        #region Methods

        /// <summary>
        /// Just set the appname and connection string
        /// </summary>
        public void InitWithoutConfig(string appName, string serviceUrl, string authToken)
        {
            ApplicationName = appName;
            ServiceUrl = serviceUrl;
            AuthToken = authToken;
        }

        /// <summary>
        /// Initialized from the web.config file when the application loads for the first time.  
        /// </summary>
        /// <param name="name">The name of the role provider</param>
        /// <param name="config">Collection of keys.  They need to include ApplicationName, ConnectionString.
        /// Description is optional</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            //Make sure we have a valid config collection
            if (config == null)
                throw new ArgumentNullException("config");

            //If no name was given, we'll give it a generic name
            if (string.IsNullOrEmpty(name))
                name = "CatbertServiceRoleProvider";

            //If no description is given, we'll give it a generic one
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Catbert Service Role Provider");
            }

            //Initialize the RoleProvider base
            base.Initialize(name, config);

            //Loop through the config collection and set our private variables
            foreach (string key in config.Keys)
            {
                switch (key.ToLower())
                {
                    case "applicationname":
                        ApplicationName = config[key];
                        break;
                    case "serviceurlkey":
                        ServiceUrl = WebConfigurationManager.AppSettings[config[key]];
                        break;
                    case "authtokenkey":
                        AuthToken = WebConfigurationManager.AppSettings[config[key]];
                        break;
                    case "description":
                        _description = config[key];
                        break;
                }
            }

            //The Application Name, ServiceUrl and AuthToken are required
            if (string.IsNullOrEmpty(ApplicationName)) throw new ArgumentException("Application Name Is Required");
            if (string.IsNullOrEmpty(ServiceUrl)) throw new ArgumentException("A Valid Service Url Is Required (Use ServiceUrlKey)");
            if (string.IsNullOrEmpty(AuthToken)) throw new ArgumentException("A Valid Authorization Token Is Required (Use AuthTokenKey)");
        }

        private RoleServiceClient GetClient()
        {
            var binding = new BasicHttpBinding
            {
                SendTimeout = TimeSpan.FromMinutes(1),
                Security =
                {
                    Mode = BasicHttpSecurityMode.TransportWithMessageCredential,
                    Message = { ClientCredentialType = BasicHttpMessageCredentialType.UserName }
                }
            };
            var endpointAddress = new EndpointAddress(ServiceUrl);

            var client = new RoleServiceClient(binding, endpointAddress);
            client.ClientCredentials.UserName.UserName = ApplicationName;
            client.ClientCredentials.UserName.Password = AuthToken;

            return client;
        }

        /// <summary>
        /// Determine if a user is in the supplied role in this application.
        /// </summary>
        /// <param name="username">LoginID</param>
        /// <param name="roleName">RoleName</param>
        /// <returns>True if the user is in the role, else false</returns>
        public override bool IsUserInRole(string username, string roleName)
        {
            using (var client = GetClient())
            {
                return client.IsUserInRole(ApplicationName, username, roleName);
            }
        }

        /// <summary>
        /// Gets all roles in the application context
        /// </summary>
        /// <returns>Roles</returns>
        public override string[] GetAllRoles()
        {
            using (var client = GetClient())
            {
                return client.GetAllRoles(ApplicationName);
            }
        }

        /// <summary>
        /// Gets all roles for the user in the context of this application
        /// </summary>
        /// <param name="username">LoginID to get the roles for</param>
        public override string[] GetRolesForUser(string username)
        {
            using (var client = GetClient())
            {
                return client.GetRolesForUser(ApplicationName, username);
            }
        }

        /// <summary>
        /// Gets all users that are in the current role in the application context
        /// </summary>
        public override string[] GetUsersInRole(string roleName)
        {
            using (var client = GetClient())
            {
                return client.GetUsersInRole(ApplicationName, roleName);
            }
        }

        /// <summary>
        /// Find out if a role exists within an application context
        /// </summary>
        /// <param name="roleName">RoleName</param>
        /// <returns>True if the role exists in the application context, else false</returns>
        /// <remarks>A return of false doesn't mean that the role doesn't exist in the db, just that it doesn't
        /// exists withing the context of this application</remarks>
        public override bool RoleExists(string roleName)
        {
            using (var client = GetClient())
            {
                return client.RoleExists(ApplicationName, roleName);
            }
        }

        #endregion

        #region Non-Implemented Methods

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException(StrUseCatbertWeb);
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException(StrUseCatbertWeb);
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException(StrUseCatbertWeb);
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException(StrUseCatbertWeb);
        }

        #endregion

    }
}
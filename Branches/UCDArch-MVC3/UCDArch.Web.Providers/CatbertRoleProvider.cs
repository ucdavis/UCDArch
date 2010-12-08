using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Web.Security;

namespace UCDArch.Web.Providers
{
    /// <summary>
    /// CAESDORoleProvider inherits from the generic RoleProvider provided by ASP.NET that is part
    /// of their provider model (which includes Membership, Role and Profile Providers)
    /// </summary>
    /// <remarks>Could also inherit from the SqlRoleProvider -- maybe worth looking at</remarks>
    /// <see cref="http://msdn.microsoft.com/asp.net/downloads/providers/"/>
    public class CatbertRoleProvider : RoleProvider
    {
        //Statics
        private const string StrUseCatbertWeb = "Use the Catbert Web UI for role modification";
        private const string StrSqlDataNotReadable = "Sql Data Could Not Be Read";

        //private vars
        private string _description;

        //public accessors
        public override string Description
        {
            get { return _description; }
        }

        private string ConnectionString { get; set; }

        private string ConnectionStringKey { get; set; }

        public override string ApplicationName { get; set; }

        #region Methods

        /// <summary>
        /// Just set the appname and connection string
        /// </summary>
        public void InitWithoutConfig(string appName, string connectionString)
        {
            ApplicationName = appName;
            ConnectionString = connectionString;
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
                name = "CAESDORoleProvider";

            //If no description is given, we'll give it a generic one
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "CAESDO Role Provider");
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
                    case "connectionstring":
                        ConnectionStringKey = config[key];
                        break;
                    case "description":
                        _description = config[key];
                        break;
                }
            }

            //Pull the connection string out of the DB through the given connection string key
            ConnectionString = WebConfigurationManager.ConnectionStrings[ConnectionStringKey].ToString();

            //The Application Name and Connection String are required
            if (string.IsNullOrEmpty(ApplicationName)) throw new ArgumentException("Application Name Is Required");
            if (string.IsNullOrEmpty(ConnectionString)) throw new ArgumentException("A Valid Connection Is Required");
        }

        /// <summary>
        /// Determine if a user is in the supplied role in this application.
        /// </summary>
        /// <param name="username">LoginID</param>
        /// <param name="roleName">RoleName</param>
        /// <returns>True if the user is in the role, else false</returns>
        public override bool IsUserInRole(string username, string roleName)
        {
            var cmd = new SqlCommand("usp_getRolesInAppByLoginID")
                          {
                              Connection = new SqlConnection(ConnectionString),
                              CommandType = CommandType.StoredProcedure
                          };

            cmd.Parameters.AddWithValue("@AppName", ApplicationName);
            cmd.Parameters.AddWithValue("@LoginID", username);

            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();

            if (dataReader == null) throw new Exception(StrSqlDataNotReadable);

            while (dataReader.Read())
            {
                //If the roleName is found then return true
                if (dataReader.GetString(0) == roleName) return true;
            }
            cmd.Connection.Close();

            //The reader never found a match
            return false;
        }

        /// <summary>
        /// Gets all roles in the application context
        /// </summary>
        /// <returns>Roles</returns>
        public override string[] GetAllRoles()
        {
            var cmd = new SqlCommand("usp_getAllRolesInApp")
                          {
                              Connection = new SqlConnection(ConnectionString),
                              CommandType = CommandType.StoredProcedure
                          };

            cmd.Parameters.AddWithValue("@AppName", ApplicationName);

            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();

            if (dataReader == null) throw new Exception(StrSqlDataNotReadable);

            var roles = new List<string>();

            while (dataReader.Read())
            {
                roles.Add(dataReader.GetString(0));
            }

            cmd.Connection.Close();

            return roles.ToArray();
        }

        /// <summary>
        /// Gets all roles for the user in the context of this application
        /// </summary>
        /// <param name="username">LoginID to get the roles for</param>
        public override string[] GetRolesForUser(string username)
        {
            var cmd = new SqlCommand("usp_getRolesInAppByLoginID")
                          {
                              Connection = new SqlConnection(ConnectionString),
                              CommandType = CommandType.StoredProcedure
                          };

            cmd.Parameters.AddWithValue("@AppName", ApplicationName);
            cmd.Parameters.AddWithValue("@LoginID", username);

            cmd.Connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();

            if (dataReader == null) throw new Exception(StrSqlDataNotReadable);

            var roles = new List<string>();

            while (dataReader.Read())
            {
                roles.Add(dataReader.GetString(0));
            }

            cmd.Connection.Close();

            return roles.ToArray();
        }

        /// <summary>
        /// Gets all users that are in the current role in the application context
        /// </summary>
        public override string[] GetUsersInRole(string roleName)
        {
            var cmd = new SqlCommand("usp_getUsersInRole")
                          {
                              Connection = new SqlConnection(ConnectionString),
                              CommandType = CommandType.StoredProcedure
                          };

            cmd.Parameters.AddWithValue("@AppName", ApplicationName);
            cmd.Parameters.AddWithValue("@RoleName", roleName);

            cmd.Connection.Open();

            SqlDataReader dataReader = cmd.ExecuteReader();

            if (dataReader == null) throw new Exception(StrSqlDataNotReadable);

            var users = new List<string>();

            while (dataReader.Read())
            {
                users.Add(dataReader.GetString(0));
            }

            cmd.Connection.Close();

            return users.ToArray();
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
            var roles = new List<string>(GetAllRoles());

            return roles.Contains(roleName);
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
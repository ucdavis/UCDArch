using System;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.RegressionTests.SampleMappings;

namespace UCDArch.RegressionTests.Repository
{
    internal static class DomainObjectDataHelper
    {
        public static void LoadDomainDataUsers(IRepository<User> userRepository)
        {
            string[] names = { "Scott", "John", "James", "Bob", "Larry", "Joe", "Pete", "Adam", "Alan", "Ken" };
            string[] loginIDs = { "postit", "aaaaa", "bbbbb", "ccccc", "ddddd", "eeeee", "fffff", "ggggg", "hhhhh", "iiiii" };

            //using (var ts = new TransactionScope())
            //{
            NHibernateSessionManager.Instance.BeginTransaction();
            for (int i = 0; i < 10; i++)
            {
                var user = new User
                {
                    Email = "fake@ucdavis.edu",
                    EmployeeID = "999999999",
                    FirstName = names[i],
                    LastName = "Last",
                    LoginID = loginIDs[i],
                    UserKey = Guid.NewGuid()
                };

                userRepository.EnsurePersistent(user); //Save
            }

            NHibernateSessionManager.Instance.CommitTransaction();
        }

        /// <summary>
        /// Create a basic Valid User.
        /// </summary>
        /// <returns>A valid user</returns>
        public static User CreateValidUser()
        {
            return new User
            {
                Email = "test@ucdavis.edu",
                EmployeeID = "123456789",
                FirstName = "FName",
                Inactive = false,
                LastName = "LName",
                LoginID = "LoginId",
                Phone = "(530) 752-7664",
                StudentID = "1234567",
                UserKey = Guid.NewGuid()
            };
        }
    }
}

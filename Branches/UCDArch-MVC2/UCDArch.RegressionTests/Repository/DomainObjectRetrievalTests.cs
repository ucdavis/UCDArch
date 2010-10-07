using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.RegressionTests.SampleMappings;
using UCDArch.Testing;

namespace UCDArch.RegressionTests.Repository
{
    [TestClass]
    public class DomainObjectRetrievalTests : RepositoryTestBase
    {
        private readonly IRepository<User> _userRepository = new Repository<User>();
        protected override void LoadData()
        {
            DomainObjectDataHelper.LoadDomainDataUsers(_userRepository);
        }

        /// <summary>
        /// Determines whether this instance [can get existing user].
        /// </summary>
        [TestMethod]
        public void CanGetExistingUser()
        {
            var user = _userRepository.GetNullableById(2);
            Assert.IsNotNull(user);
            Assert.AreEqual("aaaaa", user.LoginID);
            Assert.AreEqual("John", user.FirstName);
            Assert.AreEqual(2, user.Id);
        }

        /// <summary>
        /// Gets the user that does not exist returns proxy.
        /// </summary>
        [TestMethod]
        public void GetUserThatDoesNotExistReturnsNull()
        {
            var user = _userRepository.GetNullableById(99); //99 does not exist
            Assert.IsNull(user);
        }

        /// <summary>
        /// Determines whether this instance [can get existing user with get nullable].
        /// </summary>
        [TestMethod]
        public void CanGetExistingUserWithGetNullable()
        {
            var user = _userRepository.GetNullableById(2);
            Assert.IsNotNull(user);
            Assert.AreEqual("aaaaa", user.LoginID);
            Assert.AreEqual("John", user.FirstName);
            Assert.AreEqual(2, user.Id);
        }

        /// <summary>
        /// Gets the user with get nullable that does not exist returns null.
        /// </summary>
        [TestMethod]
        public void GetUserWithGetNullableThatDoesNotExistReturnsNull()
        {
            var user = _userRepository.GetNullableById(99); //99 does not exist
            Assert.IsNull(user);
        }

        /// <summary>
        /// Gets all users returns all users.
        /// </summary>
        [TestMethod]
        public void GetAllUsersReturnsAllUsers()
        {
            var users = _userRepository.GetAll();
            Assert.IsNotNull(users);
            Assert.AreEqual(10, users.Count);
            Assert.AreEqual("John", users[1].FirstName);
            Assert.AreEqual("aaaaa", users[1].LoginID);
            Assert.AreEqual(2, users[1].Id);
        }

        /// <summary>
        /// Gets all users sort by login ascending returns correct order.
        /// </summary>
        [TestMethod]
        public void GetAllUsersSortByLoginAscendingReturnsCorrectOrder()
        {
            var orderedUsers = new List<OrderedUsers>
                                   {
                                       new OrderedUsers {Name = "John", Login = "aaaaa", Id = 2},
                                       new OrderedUsers {Name = "James", Login = "bbbbb", Id = 3},
                                       new OrderedUsers {Name = "Bob", Login = "ccccc", Id = 4},
                                       new OrderedUsers {Name = "Larry", Login = "ddddd", Id = 5},
                                       new OrderedUsers {Name = "Joe", Login = "eeeee", Id = 6},
                                       new OrderedUsers {Name = "Pete", Login = "fffff", Id = 7},
                                       new OrderedUsers {Name = "Adam", Login = "ggggg", Id = 8},
                                       new OrderedUsers {Name = "Alan", Login = "hhhhh", Id = 9},
                                       new OrderedUsers {Name = "Ken", Login = "iiiii", Id = 10},
                                       new OrderedUsers {Name = "Scott", Login = "postit", Id = 1}
                                   };

            var users = _userRepository.GetAll("LoginID", true);
            Assert.IsNotNull(users);
            Assert.AreEqual(10, users.Count);

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(orderedUsers[i].Name, users[i].FirstName);
                Assert.AreEqual(orderedUsers[i].Login, users[i].LoginID);
                Assert.AreEqual(orderedUsers[i].Id, users[i].Id);
            }
        }


        /// <summary>
        /// Gets all users sort by login decending returns correct order.
        /// </summary>
        [TestMethod]
        public void GetAllUsersSortByLoginDecendingReturnsCorrectOrder()
        {
            var orderedUsers = new List<OrderedUsers>
                                   {
                                       new OrderedUsers {Name = "Scott", Login = "postit", Id = 1},
                                       new OrderedUsers {Name = "Ken", Login = "iiiii", Id = 10},
                                       new OrderedUsers {Name = "Alan", Login = "hhhhh", Id = 9},
                                       new OrderedUsers {Name = "Adam", Login = "ggggg", Id = 8},
                                       new OrderedUsers {Name = "Pete", Login = "fffff", Id = 7},
                                       new OrderedUsers {Name = "Joe", Login = "eeeee", Id = 6},
                                       new OrderedUsers {Name = "Larry", Login = "ddddd", Id = 5},
                                       new OrderedUsers {Name = "Bob", Login = "ccccc", Id = 4},
                                       new OrderedUsers {Name = "James", Login = "bbbbb", Id = 3},
                                       new OrderedUsers {Name = "John", Login = "aaaaa", Id = 2}
                                   };


            var users = _userRepository.GetAll("LoginID", false);
            Assert.IsNotNull(users);
            Assert.AreEqual(10, users.Count);

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(orderedUsers[i].Name, users[i].FirstName);
                Assert.AreEqual(orderedUsers[i].Login, users[i].LoginID);
                Assert.AreEqual(orderedUsers[i].Id, users[i].Id);
            }
        }

        /// <summary>
        /// Gets all users sort by first name ascending returns correct order.
        /// </summary>
        [TestMethod]
        public void GetAllUsersSortByFirstNameAscendingReturnsCorrectOrder()
        {
            var orderedUsers = new List<OrderedUsers>
                                   {
                                       new OrderedUsers {Name = "Adam", Login = "ggggg", Id = 8},
                                       new OrderedUsers {Name = "Alan", Login = "hhhhh", Id = 9},
                                       new OrderedUsers {Name = "Bob", Login = "ccccc", Id = 4},
                                       new OrderedUsers {Name = "James", Login = "bbbbb", Id = 3},
                                       new OrderedUsers {Name = "Joe", Login = "eeeee", Id = 6},
                                       new OrderedUsers {Name = "John", Login = "aaaaa", Id = 2},
                                       new OrderedUsers {Name = "Ken", Login = "iiiii", Id = 10},
                                       new OrderedUsers {Name = "Larry", Login = "ddddd", Id = 5},
                                       new OrderedUsers {Name = "Pete", Login = "fffff", Id = 7},
                                       new OrderedUsers {Name = "Scott", Login = "postit", Id = 1}
                                   };

            var users = _userRepository.GetAll("FirstName", true);
            Assert.IsNotNull(users);
            Assert.AreEqual(10, users.Count);

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(orderedUsers[i].Name, users[i].FirstName);
                Assert.AreEqual(orderedUsers[i].Login, users[i].LoginID);
                Assert.AreEqual(orderedUsers[i].Id, users[i].Id);
            }
        }

        /// <summary>
        /// Wrongs the property name throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.QueryException))]
        public void WrongPropertyNameThrowsException()
        {
            try
            {
                if (_userRepository != null)
                {
                    IList<User> users = _userRepository.GetAll("bbb", true);
                    Assert.IsNull(users);
                }
            }
            catch (Exception message)
            {
                Assert.AreEqual("could not resolve property: bbb of: UCDArch.RegressionTests.SampleMappings.User", message.Message );
                throw;
            }
        }

        /// <summary>
        /// Queryable the first name is larry returns expected result.
        /// </summary>
        [TestMethod]
        public void QueryableWhereFirstNameIsLarryReturnsExpectedResult()
        {
            IQueryable<User> users = _userRepository.Queryable.Where(u => u.FirstName == "Larry");

            var usersList = users.ToList();
            Assert.IsNotNull(usersList);
            Assert.AreEqual(1, usersList.Count);
            Assert.AreEqual("ddddd", usersList[0].LoginID);                      
        }

        /// <summary>
        /// Queryables the where first name starts with J returns expected results.
        /// </summary>
        [TestMethod]
        public void QueryableWhereFirstNameStartsWithJReturnsExpectedResults()
        {
            var users = _userRepository.Queryable.Where(u => u.FirstName.StartsWith("J")).OrderBy(u => u.FirstName) .ToList();
            var names = new List<string>{"James", "Joe", "John"};

            Assert.IsNotNull(users);
            Assert.AreEqual(3, users.Count);
            foreach (var user in users)
            {
                Assert.IsTrue(names.Contains(user.FirstName));
            }
        }

        /// <summary>
        /// Easier to reorder the ordered fields that are checked.
        /// </summary>
        public struct OrderedUsers
        {
            public string Name;
            public string Login;
            public int Id;
        }
    }
}

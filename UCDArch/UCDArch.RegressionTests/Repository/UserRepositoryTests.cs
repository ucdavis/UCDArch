using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.RegressionTests.SampleMappings;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;


namespace UCDArch.RegressionTests.Repository
{
    [TestClass]
    public class UserRepositoryTests : FluentRepositoryTestBase<UserMap>
    {
        private readonly IRepository<User> _repository = new Repository<User>();
        private readonly IRepository<Unit> _unitRepository = new Repository<Unit>();
        private readonly IRepository<UnitAssociation> _unitAssociationRepository = new Repository<UnitAssociation>();

        /// <summary>
        /// An invalid name. Too long. 51 characters.
        /// </summary>
        public readonly string InvalidName = "123456789 123456789 123456789 123456789 12345678901";

        protected override void LoadData()
        {
            CreateUsers();
            CreateUnits();
        }

        #region Repository Tests

        [TestMethod]
        public void CanSaveValidUserUsingGenericRepository()
        {
            var user = new User {FirstName = "Valid", LastName = "Valid", LoginID = "Valid"};

            Assert.AreEqual(true, user.IsTransient());

            _repository.EnsurePersistent(user);

            Assert.AreEqual(false, user.IsTransient());
        }

        [TestMethod]
        public void CanSaveValidUserUsingNonGenericRepository()
        {
            var user = new User {FirstName = "Valid", LastName = "Valid", LoginID = "Valid"};

            Assert.AreEqual(true, user.IsTransient());

            Repository.OfType<User>().EnsurePersistent(user);

            Assert.AreEqual(false, user.IsTransient());
        }

        #endregion Repository Tests

        #region Validation Tests

        [TestMethod]
        public void WillNotSaveEmptyUser()
        {
            var user = new User();

            try
            {
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected ApplicationException");
            }
            catch (ApplicationException)
            {
            }
        }

        #region First Name Tests

        /// <summary>
        /// Does not save the User with null first name.
        /// Expected Error messages are found.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithNullFirstName()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.FirstName = null;
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected ApplicationException");
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre("The FirstName field is required.");
            }
        }

        /// <summary>
        /// Does not save user with spaces only in first name.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithSpacesOnlyInFirstName()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.FirstName = "  ";
                _repository.EnsurePersistent(user);
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre("The FirstName field is required.");
            }
        }


        /// <summary>
        /// Does not save user with an empty first name.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithEmptyFirstName()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.FirstName = "";
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected ApplicationException");
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre("The FirstName field is required.");
            }
        }


        /// <summary>
        /// User does not save with first name too long.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithFirstNameTooLong()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.FirstName = InvalidName;
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected ApplicationException");
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre(
                    "The field FirstName must be a string with a maximum length of 50.");
            }
        }

        #endregion First Name Tests

        #region Last Name Tests

        /// <summary>
        /// Does not save the User with null last name.
        /// Expected Error messages are found.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithNullLastName()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.LastName = null;
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected ApplicationException");
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre("The LastName field is required.");
            }
        }

        /// <summary>
        /// Does not save user with spaces only in last name.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithSpacesOnlyInLastName()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.LastName = "  ";
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected ApplicationException");
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre("The LastName field is required.");
            }
        }


        /// <summary>
        /// Does not save user with an empty last name.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithEmptyLastName()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.LastName = "";
                _repository.EnsurePersistent(user);
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre("The LastName field is required.");
            }
        }


        /// <summary>
        /// User does not save with last name too long.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithLastNameTooLong()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.LastName = InvalidName;
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected ApplicationException");
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre(
                    "The field LastName must be a string with a maximum length of 50.");
            }
        }

        #endregion Last Name Tests

        #region LoginId Tests

        /// <summary>
        /// Does not save the User with null loginID.
        /// Expected Error messages are found.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithNullLoginId()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.LoginID = null;
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected ApplicationException");
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre("The LoginID field is required.");
            }
        }

        /// <summary>
        /// Does not save user with spaces only in loginID.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithSpacesOnlyInLoginId()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.LoginID = "  ";
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected ApplicationException");
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre("The LoginID field is required.");
            }
        }


        /// <summary>
        /// Does not save user with an empty loginID.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithEmptyLoginId()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.LoginID = "";
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected ApplicationException");
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre("The LoginID field is required.");
            }
        }


        /// <summary>
        /// User does not save with loginID too long.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithLoginIdTooLong()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.LoginID = "12345678901"; //Max ten characters
                _repository.EnsurePersistent(user);
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre(
                    "The field LoginID must be a string with a maximum length of 10.");
            }
        }

        #endregion LoginId Tests

        #region Email Tests

        /// <summary>
        /// Does not save the User with null email.
        /// Expected Error messages are found.
        /// </summary>
        [TestMethod]
        public void UserSavesWithNullEmail()
        {
            User user = CreateValidUser();
            user.Email = null;
            _repository.EnsurePersistent(user);
        }

        /// <summary>
        /// Does save the User with null email.
        /// Expected Error messages are found.
        /// </summary>
        [TestMethod]
        public void UserSavesWithEmptyEmail()
        {
            User user = CreateValidUser();
            user.Email = "";
            _repository.EnsurePersistent(user);
        }

        /// <summary>
        /// Does not save the User with null email.
        /// Expected Error messages are found.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithSpacesOnlyEmail()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.Email = " ";
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected ApplicationException");
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre("Not a well-formed email address.");
            }
        }

        /* Sample invalid email formats
            * Abc.example.com (character @ is missing)
            * Abc.@example.com (character dot(.) is last in local part)
            * Abc..123@example.com (character dot(.) is double)
            * A@b@c@example.com (only one @ is allowed outside quotations marks)
            * ()[]\;:,<>@example.com (none of the characters before the @ in this example are allowed outside quotation marks)
         */

        /// <summary>
        /// Users the does not save with invalid email format 1.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithInvalidEmailFormat1()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.Email = "Abc.example.com";
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected ApplicationException");
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre("Not a well-formed email address.");
            }
        }

        /// <summary>
        /// Users the does not save with invalid email format 2.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithInvalidEmailFormat2()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.Email = "Abc.@example.com";
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected ApplicationException");
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre("Not a well-formed email address.");
            }
        }

        /// <summary>
        /// Users the does not save with invalid email format 3.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithInvalidEmailFormat3()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.Email = "Abc..123@example.com";
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected ApplicationException");
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre("Not a well-formed email address.");
            }
        }

        /// <summary>
        /// Users the does not save with invalid email format 4.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithInvalidEmailFormat4()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.Email = "A@b@c@example.com";
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected ApplicationException");
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre("Not a well-formed email address.");
            }
        }

        /// <summary>
        /// Users the does not save with invalid email format 5.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithInvalidEmailFormat5()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.Email = "()[]\\;:,<>@example.com";
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected ApplicationException");
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre("Not a well-formed email address.");
            }
        }

        #endregion Email Tests

        #region EmployeeID Tests

        /// <summary>
        /// Users saves with null employee id.
        /// </summary>
        [TestMethod]
        public void UserSavesWithNullEmployeeId()
        {
            User user = CreateValidUser();
            user.EmployeeID = null;
            _repository.EnsurePersistent(user);
        }

        /// <summary>
        /// Users saves with empty employee id.
        /// </summary>
        [TestMethod]
        public void UserSavesWithEmptyEmployeeId()
        {
            User user = CreateValidUser();
            user.EmployeeID = "";
            _repository.EnsurePersistent(user);
        }

        /// <summary>
        /// Users saves with spaces only employee id.
        /// </summary>
        [TestMethod]
        public void UserSavesWithSpacesOnlyEmployeeId()
        {
            User user = CreateValidUser();
            user.EmployeeID = " ";
            _repository.EnsurePersistent(user);
        }

        /// <summary>
        /// Users saves with valid length employee id.
        /// </summary>
        [TestMethod]
        public void UserSavesWithValidLengthEmployeeId()
        {
            User user = CreateValidUser();
            user.EmployeeID = "123456789"; //Nine characters
            _repository.EnsurePersistent(user);
        }

        /// <summary>
        /// Users does not save with employee id too long.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithEmployeeIdTooLong()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.EmployeeID = "1234567890"; //Max nine characters
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected Exception");
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre(
                    "The field EmployeeID must be a string with a maximum length of 9.");
            }
        }

        #endregion EmployeeID Tests

        #region StudentID Tests

        /// <summary>
        /// Users saves with null Student id.
        /// </summary>
        [TestMethod]
        public void UserSavesWithNullStudentId()
        {
            User user = CreateValidUser();
            user.StudentID = null;
            _repository.EnsurePersistent(user);
        }

        /// <summary>
        /// Users saves with empty Student id.
        /// </summary>
        [TestMethod]
        public void UserSavesWithEmptyStudentId()
        {
            User user = CreateValidUser();
            user.StudentID = "";
            _repository.EnsurePersistent(user);
        }

        /// <summary>
        /// Users saves with spaces only Student id.
        /// </summary>
        [TestMethod]
        public void UserSavesWithSpacesOnlyStudentId()
        {
            User user = CreateValidUser();
            user.StudentID = " ";
            _repository.EnsurePersistent(user);
        }

        /// <summary>
        /// Users saves with valid length Student id.
        /// </summary>
        [TestMethod]
        public void UserSavesWithValidLengthStudentId()
        {
            User user = CreateValidUser();
            user.StudentID = "123456789"; //Nine characters
            _repository.EnsurePersistent(user);
        }

        /// <summary>
        /// Users does not save with Student id too long.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithStudentIdTooLong()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.StudentID = "1234567890"; //Max nine characters
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected ApplicationException");
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre(
                    "The field StudentID must be a string with a maximum length of 9.");
            }
        }

        #endregion StudentID Tests

        #region Test multiple errors

        /// <summary>
        /// User does not save with first and last name too long.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithFirstAndLastNameTooLong()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.FirstName = InvalidName;
                user.LastName = InvalidName;
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected ApplicationException");
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre(
                    "The field FirstName must be a string with a maximum length of 50.",
                    "The field LastName must be a string with a maximum length of 50.");
            }
        }

        /// <summary>
        /// Users does not save with many errors.
        /// </summary>
        [TestMethod]
        public void UserDoesNotSaveWithManyErrors()
        {
            User user = null;
            try
            {
                user = CreateValidUser();
                user.FirstName = null; //Required
                user.LastName = null; //Required
                user.LoginID = null; //Required
                user.Email = "abc@@bce.edg"; //two @ characters
                user.Phone = "(123)123-1234"; //No area code starts with 1
                user.EmployeeID = "1234567890"; //Too long
                user.StudentID = "1234567890"; //Too long
                _repository.EnsurePersistent(user);
                Assert.Fail("Expected ApplicationException");
            }
            catch (ApplicationException)
            {
                Assert.IsNotNull(user);
                user.ValidationResults().AsMessageList().AssertErrorsAre(
                    "The FirstName field is required.", "The LastName field is required.",
                    "The LoginID field is required.",
                    @"The field Phone must match the regular expression '^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$'.",
                    "Not a well-formed email address.",
                    "The field EmployeeID must be a string with a maximum length of 9.",
                    "The field StudentID must be a string with a maximum length of 9.");
            }
        }

        #endregion Test multiple errors

        #endregion Validation Tests

        #region CRUD Tests

        //TODO: Move Crud tests into a different test class so LoadData is only run for this?
        /// <summary>
        /// CRUD Tests
        /// Determines whether this instance [can get all users].
        /// </summary>
        [TestMethod]
        public void CanGetAllUsers()
        {
            IList<User> allUsers = _repository.GetAll();
            Assert.AreEqual(10, allUsers.Count, "Did not find the ten expected users.");
        }

        /// <summary>
        /// CRUD tests
        /// Determines whether this instance [can save new user].
        /// </summary>
        [TestMethod]
        public void CanSaveNewUser()
        {
            var user = CreateValidUser();
            user.LoginID = "745293HBFE"; //Just a random login id. Used lower in code to check that we get it.

            Assert.IsTrue(user.IsTransient());

            NHibernateSessionManager.Instance.BeginTransaction();
            _repository.EnsurePersistent(user);
            NHibernateSessionManager.Instance.CommitTransaction();

            Assert.IsFalse(user.IsTransient()); //Make sure the user is saved

            NHibernateSessionManager.Instance.GetSession().Evict(user); //get the user out of the local cache

            //Now get the user back out
            var userId = user.Id;

            var retrievedUser = _repository.GetNullableById(userId);

            Assert.IsNotNull(retrievedUser);
            Assert.AreEqual("745293HBFE", retrievedUser.LoginID); //Make sure it is the correct user
            Assert.AreEqual(11, _repository.GetAll().Count);
        }

        //TODO: These tests
        //TODO: Create Units and link them to a user. Test that it works as expected.

        /// <summary>
        /// CRUD Tests
        /// Determines whether this instance [can modify user].
        /// </summary>
        [TestMethod]
        public void CanModifyUser()
        {
            User firstUser = _repository.GetNullableById(1); //Just get the first user

            Assert.AreEqual("Scott", firstUser.FirstName); //First user is scott

            NHibernateSessionManager.Instance.BeginTransaction();
            firstUser.FirstName = "Tiny";
            _repository.EnsurePersistent(firstUser);
            NHibernateSessionManager.Instance.CommitTransaction();

            NHibernateSessionManager.Instance.GetSession().Evict(firstUser);
                //Evict the user from the cache so we can retrieve it

            firstUser = _repository.GetNullableById(1); //Get the user back out

            Assert.AreEqual("Tiny", firstUser.FirstName); //the name should now be tiny
        }

        /// <summary>
        /// Determines whether this instance [can remove user].
        /// </summary>
        [TestMethod]
        public void CanRemoveUser()
        {
            User firstUser = _repository.GetNullableById(1); //Just get the first user

            Assert.AreEqual("Scott", firstUser.FirstName); //First user is scott

            NHibernateSessionManager.Instance.BeginTransaction();
            _repository.Remove(firstUser);
            NHibernateSessionManager.Instance.CommitTransaction();

            Assert.AreEqual(9, _repository.GetAll().Count); //There should be 9 users now

            User shouldNotExistUser = _repository.GetNullableById(1);

            Assert.IsNull(shouldNotExistUser);
        }

        /// <summary>
        /// Create a User
        /// Create UserAssociations to that user and existing Units
        /// Get the user and assert that the UserAssociations exist as expected.
        /// </summary>
        [TestMethod]
        public void CanGetUserWithSavedAssociations()
        {
            #region Create a User

            var user = CreateValidUser();
            user.LoginID = "XXX123HBFE";
                //Just a random login id. Used lower in code to check that we get it.            

            Assert.IsTrue(user.IsTransient());

            NHibernateSessionManager.Instance.BeginTransaction();
            _repository.EnsurePersistent(user);
            NHibernateSessionManager.Instance.CommitTransaction();

            Assert.IsFalse(user.IsTransient()); //Make sure the user is saved

            NHibernateSessionManager.Instance.GetSession().Evict(user); //get the user out of the local cache

            //Now get the user back out
            var userId = user.Id;

            var retrievedUser = _repository.GetNullableById(userId);

            Assert.IsNotNull(retrievedUser);
            Assert.AreEqual("XXX123HBFE", retrievedUser.LoginID); //Make sure it is the correct user
            Assert.AreEqual(11, _repository.GetAll().Count);

            #endregion Create a User

            #region Create UnitAssociations

            NHibernateSessionManager.Instance.BeginTransaction();
            var allUnits = _unitRepository.GetAll();
            foreach (var unit in allUnits)
            {
                var userAssociation = new UnitAssociation
                                          {
                                              Inactive = false,
                                              User = _repository.GetNullableById(userId),
                                              Unit = unit
                                          };
                _unitAssociationRepository.EnsurePersistent(userAssociation);
            }
            NHibernateSessionManager.Instance.CommitTransaction();

            #endregion Create UnitAssociations

            var allUnitAssociations = _unitAssociationRepository.GetAll();
            var filteredUnitAssociations = new List<UnitAssociation>();
            foreach (var unitAssociation in allUnitAssociations)
            {
                if (unitAssociation.User.Id == userId)
                {
                    filteredUnitAssociations.Add(unitAssociation);
                }
            }

            #region Modify user

            User modifyUser = _repository.GetNullableById(userId);

            Assert.AreEqual("XXX123HBFE", modifyUser.LoginID);

            Assert.IsNotNull(modifyUser.UnitAssociations);
            Assert.AreEqual(5, modifyUser.UnitAssociations.Count);
            foreach (var list in filteredUnitAssociations)
            {
                Assert.IsTrue(modifyUser.UnitAssociations.Contains(list));
            }


            //NHibernateSessionManager.Instance.BeginTransaction();
            //modifyUser.UnitAssociations = filteredUnitAssociations;
            //_repository.EnsurePersistent(modifyUser);
            //NHibernateSessionManager.Instance.CommitTransaction();

            //NHibernateSessionManager.Instance.GetSession().Evict(modifyUser); //Evict the user from the cache so we can retrieve it

            //modifyUser = _repository.GetById(userId); //Get the user back out

            //Assert.AreEqual("XXX123HBFE", modifyUser.FirstName);

            //Assert.IsNotNull(modifyUser.UnitAssociations);
            //Assert.AreEqual(5, modifyUser.UnitAssociations.Count);
            //foreach (var list in filteredUnitAssociations)
            //{
            //    Assert.IsTrue(modifyUser.UnitAssociations.Contains(list));
            //}

            #endregion Modify user
        }

        #endregion CRUD Tests

        #region Helper methods

        /// <summary>
        /// Create a basic Valid User.
        /// </summary>
        /// <returns>A valid user</returns>
        private static User CreateValidUser()
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


        /// <summary>
        /// Creates 5 units for DB Tests.
        /// </summary>
        private void CreateUnits()
        {
            NHibernateSessionManager.Instance.BeginTransaction();
            for (int i = 1; i <= 5; i++)
            {
                var unit = CreateValidUnit();
                unit.FullName += i.ToString();
                unit.ShortName += i.ToString();

                _unitRepository.EnsurePersistent(unit); //Save
            }
            NHibernateSessionManager.Instance.CommitTransaction();
        }

        /// <summary>
        /// Creates the valid unit.
        /// </summary>
        /// <returns></returns>
        private static Unit CreateValidUnit()
        {
            return new Unit
                       {
                           FullName = "FullName",
                           ShortName = "ShortName",
                           FISCode = "1234",
                           PPSCode = "123456"
                       };
        }

        /// <summary>
        /// Creates the users for DB Tests.
        /// </summary>
        private void CreateUsers()
        {
            string[] names = {"Scott", "John", "James", "Bob", "Larry", "Joe", "Pete", "Adam", "Alan", "Ken"};
            string[] loginIDs = {
                                    "postit", "aaaaa", "bbbbb", "ccccc", "ddddd", "eeeee", "fffff", "ggggg", "hhhhh",
                                    "iiiii"
                                };

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

                _repository.EnsurePersistent(user); //Save
            }

            NHibernateSessionManager.Instance.CommitTransaction();
        }

        #endregion Helper methods
    }
}
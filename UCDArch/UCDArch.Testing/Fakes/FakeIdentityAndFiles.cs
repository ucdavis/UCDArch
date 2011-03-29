using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace UCDArch.Testing.Fakes
{
    #region mocks
    /// <summary>
    /// Mock the HTTPContext. Used for getting the current user name
    /// </summary>
    public class MockHttpContext : HttpContextBase
    {
        private IPrincipal _user;
        private readonly int _count;
        public string[] UserRoles { get; set; }
        private string _userName;
        private string _fileContentType;

        public MockHttpContext(int fileCount, string[] userRoles, string userName = "UserName", string fileContentType = "application/pdf")
        {
            _count = fileCount;
            UserRoles = userRoles;
            _userName = userName;
            _fileContentType = fileContentType;
        }

        public override IPrincipal User
        {
            get { return _user ?? (_user = new MockPrincipal(UserRoles, _userName)); }
            set
            {
                _user = value;
            }
        }

        public override HttpRequestBase Request
        {
            get
            {
                return new MockHttpRequest(_count, _fileContentType);
            }
        }
    }

    /// <summary>
    /// Mock the Principal. Used for getting the current user name
    /// </summary>
    public class MockPrincipal : IPrincipal
    {
        IIdentity _identity;
        public bool RoleReturnValue { get; set; }
        public string[] UserRoles { get; set; }
        private string _userName;

        public MockPrincipal(string[] userRoles, string userName = "UserName")
        {
            UserRoles = userRoles;
            _userName = userName;
        }

        public IIdentity Identity
        {
            get { return _identity ?? (_identity = new MockIdentity(_userName)); }
        }

        public bool IsInRole(string role)
        {
            if (UserRoles.Contains(role))
            {
                return true;
            }
            return false;
        }

    }

    /// <summary>
    /// Mock the Identity. Used for getting the current user name
    /// </summary>
    public class MockIdentity : IIdentity
    {
        private string _userName;


        public MockIdentity(string userName = "UserName")
        {
            _userName = userName;
        }

        public string AuthenticationType
        {
            get
            {
                return "MockAuthentication";
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return true;
            }
        }

        public string Name
        {
            get
            {
                return _userName;
            }
        }
    }





    public class MockHttpRequest : HttpRequestBase
    {
        MockHttpFileCollectionBase Mocked { get; set; }

        public MockHttpRequest(int count, string contentType = "application/pdf")
        {
            Mocked = new MockHttpFileCollectionBase(count, contentType);
        }
        public override HttpFileCollectionBase Files
        {
            get
            {
                return Mocked;
            }
        }
    }

    public class MockHttpFileCollectionBase : HttpFileCollectionBase
    {
        public int Counter { get; set; }
        private string _contentType;

        public MockHttpFileCollectionBase(int count, string contentType = "application/pdf")
        {
            _contentType = contentType;
            Counter = count;
            for (int i = 0; i < count; i++)
            {
                BaseAdd("Test" + (i + 1), new byte[] { 4, 5, 6, 7, 8 });
            }

        }

        public override int Count
        {
            get
            {
                return Counter;
            }
        }
        public override HttpPostedFileBase Get(string name)
        {
            return new MockHttpPostedFileBase(_contentType);
        }
        public override HttpPostedFileBase this[string name]
        {
            get
            {
                return new MockHttpPostedFileBase(_contentType);
            }
        }
        public override HttpPostedFileBase this[int index]
        {
            get
            {
                return new MockHttpPostedFileBase(_contentType);
            }
        }
    }

    public class MockHttpPostedFileBase : HttpPostedFileBase
    {
        private string _contentType;
        public MockHttpPostedFileBase(string contentType)
        {
            _contentType = contentType;
        }
        public override int ContentLength
        {
            get
            {
                return 5;
            }
        }
        public override string FileName
        {
            get
            {
                return "Mocked File Name";
            }
        }
        public override Stream InputStream
        {
            get
            {
                var memStream = new MemoryStream(new byte[] { 4, 5, 6, 7, 8 });
                return memStream;
            }
        }
        public override string ContentType
        {
            get
            {
                return _contentType;
            }
        }
    }

    #endregion
}

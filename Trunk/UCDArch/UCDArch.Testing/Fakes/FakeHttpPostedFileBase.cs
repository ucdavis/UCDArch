using System.IO;
using System.Web;

namespace UCDArch.Testing.Fakes
{
    /// <summary>
    /// Fake to return Filename, ContentType, and Contents
    /// </summary>
    public class FakeHttpPostedFileBase : HttpPostedFileBase
    {
        private readonly string _fileName;
        private readonly string _contentType;
        private readonly byte[] _contents;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeHttpPostedFileBase"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="contents">The contents.</param>
        public FakeHttpPostedFileBase(string fileName, string contentType, byte[] contents)
        {
            _fileName = fileName;
            _contentType = contentType;
            _contents = contents;
        }

        /// <summary>
        /// When overridden in a derived class, gets the fully qualified name of the file on the client.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The name of the file on the client, which includes the directory path.
        /// </returns>
        /// <exception cref="T:System.NotImplementedException">
        /// Always.
        /// </exception>
        public override string FileName
        {
            get
            {
                return _fileName;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the MIME content type of an uploaded file.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The MIME content type of the file.
        /// </returns>
        /// <exception cref="T:System.NotImplementedException">
        /// Always.
        /// </exception>
        public override string ContentType
        {
            get
            {
                return _contentType;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a <see cref="T:System.IO.Stream"/> object that points to an uploaded file to prepare for reading the contents of the file.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An object for reading a file.
        /// </returns>
        /// <exception cref="T:System.NotImplementedException">
        /// Always.
        /// </exception>
        public override Stream InputStream
        {
            get
            {
                return new MemoryStream(_contents);
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the size of an uploaded file, in bytes.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The length of the file, in bytes.
        /// </returns>
        /// <exception cref="T:System.NotImplementedException">
        /// Always.
        /// </exception>
        public override int ContentLength
        {
            get
            {
                return _contents.Length;
            }
        }
    }
}

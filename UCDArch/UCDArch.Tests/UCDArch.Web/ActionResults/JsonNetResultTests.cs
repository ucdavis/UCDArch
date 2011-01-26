using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using UCDArch.Web.ActionResults;

namespace UCDArch.Tests.UCDArch.Web.ActionResults
{
    [TestClass]
    public class JsonNetResultTests
    {
        #region JsonResultString Tests
        /// <summary>
        /// Result Returns Data Was Null For NullData.
        /// </summary>
        [TestMethod]
        public void ResultReturnsDataWasNullForNullData()
        {
            var result = new JsonNetResult(null);

            Assert.AreEqual("[Data was Null]", result.JsonResultString);
        }

        /// <summary>
        /// Result Return JsonString When Data Has One Property.
        /// </summary>
        [TestMethod]
        public void ResultReturnJsonStringWhenDataHasOneProperty()
        {
            var sampleClass = new SampleClassWithOneJsonProperty();

            var result = new JsonNetResult(sampleClass);

            Assert.AreEqual("{\"Name\":\"SampleName\"}", result.JsonResultString);

        }

        /// <summary>
        /// Result Return JsonString When Data has Multiple Properties .
        /// </summary>
        [TestMethod]
        public void ResultReturnJsonStringWhenDataHasMultipleProperties()
        {
            var sampleClass = new SampleClassWithMultipleJsonProperties();

            var result = new JsonNetResult(sampleClass);

            Assert.AreEqual("{\"Name\":\"SampleName\",\"FavoriteNumber\":42}", result.JsonResultString);
        }

        /// <summary>
        /// Result Return JsonString When Data Has Sub Values
        /// </summary>
        [TestMethod]
        public void ResultReturnJsonStringWhenDataHasSubValues()
        {
            var sampleClass = new SampleClassWithSubJsonPropert();

            var result = new JsonNetResult(sampleClass);

            Assert.AreEqual("{\"Country\":\"USA\",\"FavoriteCity\":\"SampleCity\"}", result.JsonResultString);
        }

        #endregion JsonResultString Tests

        #region Sample Classes
        internal class SampleClassWithOneJsonProperty : BaseClass
        {
            [JsonProperty]
            public override string Name { get; set; }
        }

        internal class SampleClassWithMultipleJsonProperties : BaseClass
        {
            [JsonProperty]
            public override string Name { get; set; }

            [JsonProperty]
            public override int FavoriteNumber { get; set; }
        }

        [JsonObject(MemberSerialization.OptIn)]
        internal class BaseClass
        {
            public BaseClass()
            {
                Name = "SampleName";
                FavoriteCity = "SampleCity";
                FavoriteNumber = 42;
            }

            public virtual string Name { get; set; }

            public virtual string FavoriteCity { get; set; }

            public virtual int FavoriteNumber { get; set; }
        }

        internal class SampleClassWithSubJsonPropert : BaseClassWithJsonProperty
        {
            public SampleClassWithSubJsonPropert()
            {
                State = "California";
                Country = "USA";
            }

            public string State{ get; set;}
            [JsonProperty]
            public string Country{ get; set;}
        }

        [JsonObject(MemberSerialization.OptIn)]
        internal class BaseClassWithJsonProperty
        {
            public BaseClassWithJsonProperty()
            {
                Name = "SampleName";
                FavoriteCity = "SampleCity";
                FavoriteNumber = 42;
            }

            public virtual string Name { get; set; }

            [JsonProperty]
            public virtual string FavoriteCity { get; set; }

            public virtual int FavoriteNumber { get; set; }
        }
        #endregion Sample Classes
    }
}
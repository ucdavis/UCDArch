using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.DomainModel;
using UCDArch.Testing;
using Newtonsoft.Json;

namespace UCDArch.Tests.UCDArch.Core.JsonNet
{
    [TestClass]
    public class JsonSerializerTests
    {
        [TestMethod]
        public void DomainObjectReturnsJustId()
        {
            var obj = new SimpleDomainObject();

            obj.SetIdTo(42);

            var result = JsonConvert.SerializeObject(obj);

            const string jsonString = "{\"Id\":42}";

            Assert.AreEqual(jsonString, result);
        }

        [TestMethod]
        public void AddingSimplePropertyReturnsJustId()
        {
            var obj = new NamedDomainObject {Name = "TestName"};

            obj.SetIdTo(42);

            var result = JsonConvert.SerializeObject(obj);

            const string jsonString = "{\"Id\":42}";

            Assert.AreEqual(jsonString, result);
        }

        [TestMethod]
        public void AddingSimplePropertyWithJsonAttributeReturnsIdAndName()
        {
            var obj = new NamedDomainObjectWithPropertyIncluded { Name = "TestName" };

            obj.SetIdTo(42);

            var result = JsonConvert.SerializeObject(obj);

            const string jsonString = "{\"Id\":42,\"Name\":\"TestName\"}";

            Assert.AreEqual(jsonString, result);
        }
    }

    public class SimpleDomainObject : DomainObject{}

    public class NamedDomainObject : DomainObject
    {
        public virtual string Name { get; set; }
    }

    public class NamedDomainObjectWithPropertyIncluded : DomainObject
    {
        [JsonProperty]
        public virtual string Name { get; set; }
    }
}
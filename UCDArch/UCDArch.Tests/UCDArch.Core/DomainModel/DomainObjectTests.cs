using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.DomainModel;
using UCDArch.Testing;

namespace UCDArch.Tests.UCDArch.Core.DomainModel
{
    [TestClass]
    public class DomainObjectTests
    {
        [TestMethod]
        public void CanHaveEntityWithoutDomainSignatureProperties()
        {
            var invalidEntity =
                new ObjectWithNoDomainSignatureProperties();

            invalidEntity.GetSignatureProperties();
        }

        [TestMethod]
        public void TransientEntityWithoutDomainSignatureShouldReturnConsistentHashcode()
        {
            var sut = new ObjectWithNoDomainSignatureProperties();

            Assert.AreEqual(sut.GetHashCode(), sut.GetHashCode());
        }

        [TestMethod]
        public void TwoTransientEntitiesWithoutSignaturePropertiesGenerateDifferentHashcodes()
        {
            var sut1 = new ObjectWithNoDomainSignatureProperties();
            var sut2 = new ObjectWithNoDomainSignatureProperties();

            Assert.AreNotEqual(sut1.GetHashCode(), sut2.GetHashCode());
        }

        [TestMethod]
        public void
            EntityWithNoSignaturePropertiesPreservesHashcodeWhenTransitioningFromTransientToPersistent()
        {
            var sut = new ObjectWithNoDomainSignatureProperties();

            Assert.IsTrue(sut.IsTransient());

            int hashcodeWhenTransient = sut.GetHashCode();

            sut.SetIdTo(1);

            Assert.IsFalse(sut.IsTransient());
            Assert.AreEqual(sut.GetHashCode(), hashcodeWhenTransient);
        }

        [TestMethod]
        public void TwoPersistentEntitiesWithNoSignaturePropertiesAndDifferentIdsGenerateDifferentHashcodes()
        {
            IDomainObjectWithTypedId<int> sut1 = new ObjectWithNoDomainSignatureProperties().SetIdTo(1);
            IDomainObjectWithTypedId<int> sut2 = new ObjectWithNoDomainSignatureProperties().SetIdTo(2);

            Assert.AreNotEqual(sut1.GetHashCode(), sut2.GetHashCode());
        }

        [TestMethod]
        public void TwoPersistentEntitiesWithNoSignaturePropertiesAndEqualIdsGenerateEqualHashcodes()
        {
            IDomainObjectWithTypedId<int> sut1 = new ObjectWithNoDomainSignatureProperties().SetIdTo(1);
            IDomainObjectWithTypedId<int> sut2 = new ObjectWithNoDomainSignatureProperties().SetIdTo(1);

            Assert.AreEqual(sut1.GetHashCode(), sut2.GetHashCode());
        }

        [TestMethod]
        public void TransientEntityWithDomainSignatureShouldReturnConsistentHashcode()
        {
            var sut = new ObjectWithOneDomainSignatureProperty {Age = 1};

            Assert.AreEqual(sut.GetHashCode(), sut.GetHashCode());
        }

        [TestMethod]
        public void TwoTransientEntitiesWithDifferentValuesOfDomainSignatureGenerateDifferentHashcodes()
        {
            var sut1 = new ObjectWithOneDomainSignatureProperty {Age = 1};
            var sut2 = new ObjectWithOneDomainSignatureProperty {Age = 2};

            Assert.AreNotEqual(sut1.GetHashCode(), sut2.GetHashCode());
        }

        [TestMethod]
        public void TwoTransientEntititesWithEqualValuesOfDomainSignatureGenerateEqualHashcodes()
        {
            var sut1 = new ObjectWithOneDomainSignatureProperty {Age = 1};
            var sut2 = new ObjectWithOneDomainSignatureProperty {Age = 1};

            Assert.AreEqual(sut1.GetHashCode(), sut2.GetHashCode());
        }

        [TestMethod]
        public void
            TransientEntityWithDomainSignaturePreservesHashcodeTemporarilyWhenItsDomainSignatureChanges()
        {
            var sut = new ObjectWithOneDomainSignatureProperty {Age = 1};

            int initialHashcode = sut.GetHashCode();

            sut.Age = 2;

            Assert.AreEqual(sut.GetHashCode(), initialHashcode);
        }

        [TestMethod]
        public void EntityWithDomainSignaturePreservesHashcodeWhenTransitioningFromTransientToPersistent()
        {
            var sut = new ObjectWithOneDomainSignatureProperty {Age = 1};

            Assert.IsTrue(sut.IsTransient());

            int hashcodeWhenTransient = sut.GetHashCode();

            sut.SetIdTo(1);

            Assert.IsFalse(sut.IsTransient());
            Assert.AreEqual(sut.GetHashCode(), hashcodeWhenTransient);
        }

        [TestMethod]
        public void TwoPersistentEntitiesWithEqualDomainSignatureAndDifferentIdsGenerateDifferentHashcodes()
        {
            IDomainObjectWithTypedId<int> sut1 = new ObjectWithOneDomainSignatureProperty {Age = 1}.SetIdTo(1);
            IDomainObjectWithTypedId<int> sut2 = new ObjectWithOneDomainSignatureProperty {Age = 1}.SetIdTo(2);

            Assert.AreNotEqual(sut1.GetHashCode(), sut2.GetHashCode());
        }

        [TestMethod]
        public void TwoPersistentEntitiesWithDifferentDomainSignatureAndEqualIdsGenerateEqualHashcodes()
        {
            IDomainObjectWithTypedId<int> sut1 = new ObjectWithOneDomainSignatureProperty {Age = 1}.SetIdTo(1);
            IDomainObjectWithTypedId<int> sut2 = new ObjectWithOneDomainSignatureProperty {Age = 2}.SetIdTo(1);

            Assert.AreEqual(sut1.GetHashCode(), sut2.GetHashCode());
        }

        [TestMethod]
        public void KeepsConsistentHashThroughLifetimeOfTransientObject()
        {
            var object1 = new ObjectWithOneDomainSignatureProperty();
            int initialHash = object1.GetHashCode();

            object1.Age = 13;
            object1.Name = "Foo";

            Assert.AreEqual(initialHash, object1.GetHashCode());

            object1.Age = 14;
            Assert.AreEqual(initialHash, object1.GetHashCode());
        }

        [TestMethod]
        public void KeepsConsistentHashThroughLifetimeOfPersistentObject()
        {
            var object1 = new ObjectWithOneDomainSignatureProperty();
            EntityIdSetter.SetIdOf(object1, 1);
            int initialHash = object1.GetHashCode();

            object1.Age = 13;
            object1.Name = "Foo";

            Assert.AreEqual(initialHash, object1.GetHashCode());

            object1.Age = 14;
            Assert.AreEqual(initialHash, object1.GetHashCode());
        }

        [TestMethod]
        public void CanCompareDomainObjectsWithOnlySomePropertiesBeingPartOfDomainSignature()
        {
            var object1 = new ObjectWithOneDomainSignatureProperty();
            var object2 = new ObjectWithOneDomainSignatureProperty();
            Assert.AreEqual(object1, object2);

            object1.Age = 13;
            object2.Age = 13;
            // Name property isn't included in comparison
            object1.Name = "Foo";
            object2.Name = "Bar";
            Assert.AreEqual(object1, object2);

            object1.Age = 14;
            Assert.AreNotEqual(object1, object2);
        }

        [TestMethod]
        public void CanCompareDomainObjectsWithAllPropertiesBeingPartOfDomainSignature()
        {
            var object1 = new ObjectWithAllDomainSignatureProperty();
            var object2 = new ObjectWithAllDomainSignatureProperty();
            Assert.AreEqual(object1, object2);

            object1.Age = 13;
            object2.Age = 13;
            object1.Name = "Foo";
            object2.Name = "Foo";
            Assert.AreEqual(object1, object2);

            object1.Name = "Bar";
            Assert.AreNotEqual(object1, object2);

            object1.Name = null;
            Assert.AreNotEqual(object1, object2);

            object2.Name = null;
            Assert.AreEqual(object1, object2);
        }

        [TestMethod]
        public void CanCompareInheritedDomainObjects()
        {
            var object1 =
                new InheritedObjectWithExtraDomainSignatureProperty();
            var object2 =
                new InheritedObjectWithExtraDomainSignatureProperty();
            Assert.AreEqual(object1, object2);

            object1.Age = 13;
            object1.IsLiving = true;
            object2.Age = 13;
            object2.IsLiving = true;
            // Address property isn't included in comparison
            object1.Address = "123 Oak Ln.";
            object2.Address = "Nightmare on Elm St.";
            Assert.AreEqual(object1, object2);

            object1.IsLiving = false;
            Assert.AreNotEqual(object1, object2);
        }

        [TestMethod]
        public void WontGetConfusedWithOutsideCases()
        {
            var object1 =
                new ObjectWithIdenticalTypedProperties();
            var object2 =
                new ObjectWithIdenticalTypedProperties();

            object1.Address = "Henry";
            object1.Name = "123 Lane St.";
            object2.Address = "123 Lane St.";
            object2.Name = "Henry";
            Assert.AreNotEqual(object1, object2);

            object1.Address = "Henry";
            object1.Name = null;
            object2.Address = "Henri";
            object2.Name = null;
            Assert.AreNotEqual(object1, object2);

            object1.Address = null;
            object1.Name = "Supercalifragilisticexpialidocious";
            object2.Address = null;
            object2.Name = "Supercalifragilisticexpialidocious";
            Assert.AreEqual(object1, object2);

            object1.Name = "Supercalifragilisticexpialidocious";
            object2.Name = "Supercalifragilisticexpialidociouz";
            Assert.AreNotEqual(object1, object2);
        }

        [TestMethod]
        public void CanCompareObjectsWithComplexProperties()
        {
            var object1 = new ObjectWithComplexProperties();
            var object2 = new ObjectWithComplexProperties();

            Assert.AreEqual(object1, object2);

            object1.Address = new AddressBeingDomainSignatureComparble
                                  {
                                      Address1 = "123 Smith Ln.",
                                      Address2 = "Suite 201",
                                      ZipCode = 12345
                                  };
            Assert.AreNotEqual(object1, object2);

            // Set the address of the 2nd to be different to the address of the first
            object2.Address = new AddressBeingDomainSignatureComparble
                                  {
                                      Address1 = "123 Smith Ln.",
                                      // Address2 isn't marked as being part of the domain signature; 
                                      // therefore, it WON'T be used in the equality comparison
                                      Address2 = "Suite 402",
                                      ZipCode = 98765
                                  };
            Assert.AreNotEqual(object1, object2);

            // Set the address of the 2nd to be the same as the first
            object2.Address.ZipCode = 12345;
            Assert.AreEqual(object1, object2);

            object1.Phone = new PhoneBeingNotDomainObject
                                {
                                    PhoneNumber = "(555) 555-5555"
                                };
            Assert.AreNotEqual(object1, object2);

            // IMPORTANT: Note that even though the phone number below has the same value as the 
            // phone number on object1, they're not domain signature comparable; therefore, the
            // "out of the box" equality will be used which shows them as being different objects.
            object2.Phone = new PhoneBeingNotDomainObject
                                {
                                    PhoneNumber = "(555) 555-5555"
                                };
            Assert.AreNotEqual(object1, object2);

            // Observe as we replace the object1.Phone with an object that isn't domain-signature
            // comparable, but DOES have an overridden Equals which will return true if the phone
            // number properties are equal.
            object1.Phone = new PhoneBeingNotDomainObjectButWithOverriddenEquals
                                {
                                    PhoneNumber = "(555) 555-5555"
                                };
            Assert.AreEqual(object1, object2);
        }

        #region Nested type: InheritedObjectWithExtraDomainSignatureProperty

        private class InheritedObjectWithExtraDomainSignatureProperty : ObjectWithOneDomainSignatureProperty
        {
            public string Address { get; set; }

            [DomainSignature]
            public bool IsLiving { get; set; }
        }

        #endregion

        #region Nested type: ObjectWithAllDomainSignatureProperty

        private class ObjectWithAllDomainSignatureProperty : DomainObject
        {
            [DomainSignature]
            public string Name { get; set; }

            [DomainSignature]
            public int Age { get; set; }
        }

        #endregion

        #region Nested type: ObjectWithIdenticalTypedProperties

        private class ObjectWithIdenticalTypedProperties : DomainObject
        {
            [DomainSignature]
            public string Name { get; set; }

            [DomainSignature]
            public string Address { get; set; }
        }

        #endregion

        #region ObjectWithComplexProperties

        #region Nested type: AddressBeingDomainSignatureComparble

        private class AddressBeingDomainSignatureComparble : DomainObject
        {
            [DomainSignature]
            public string Address1 { get; set; }

            public string Address2 { get; set; }

            [DomainSignature]
            public int ZipCode { get; set; }
        }

        #endregion

        #region Nested type: ObjectWithComplexProperties

        private class ObjectWithComplexProperties : DomainObject
        {
            [DomainSignature]
            public string Name { get; set; }

            [DomainSignature]
            public AddressBeingDomainSignatureComparble Address { get; set; }

            [DomainSignature]
            public PhoneBeingNotDomainObject Phone { get; set; }
        }

        #endregion

        #region Nested type: PhoneBeingNotDomainObject

        private class PhoneBeingNotDomainObject
        {
            public string PhoneNumber { get; set; }
            public string Extension { get; set; }
        }

        #endregion

        #region Nested type: PhoneBeingNotDomainObjectButWithOverriddenEquals

        private class PhoneBeingNotDomainObjectButWithOverriddenEquals : PhoneBeingNotDomainObject
        {
            public override bool Equals(object obj)
            {
                var compareTo =
                    obj as PhoneBeingNotDomainObject;

                return (compareTo != null &&
                        PhoneNumber.Equals(compareTo.PhoneNumber));
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        #endregion

        #endregion

        #region Carry-Over tests from when Entity was split from an object called PersistentObject

        [TestMethod]
        public void CanCompareEntitys()
        {
            var object1 = new ObjectWithIntId {Name = "Acme"};
            var object2 = new ObjectWithIntId {Name = "Anvil"};

            Assert.AreNotEqual(object1, null);
            Assert.AreNotEqual(object1, object2);

            EntityIdSetter.SetIdOf(object1, 10);
            EntityIdSetter.SetIdOf(object2, 10);

            // Even though the "business value signatures" are different, the persistent Ids 
            // were the same.  Call me crazy, but I put that much trust into persisted Ids.
            Assert.AreEqual(object1, object2);
            Assert.AreEqual(object1.GetHashCode(), object2.GetHashCode());

            var object3 = new ObjectWithIntId {Name = "Acme"};

            // Since object1 has an Id but object3 doesn't, they won't be equal
            // even though their signatures are the same
            Assert.AreNotEqual(object1, object3);

            var object4 = new ObjectWithIntId {Name = "Acme"};

            // object3 and object4 are both transient and share the same signature
            Assert.AreEqual(object3, object4);
        }

        [TestMethod]
        public void CanCompareEntitiesWithAssignedIds()
        {
            var object1 = new ObjectWithAssignedId {Name = "Acme"};
            var object2 = new ObjectWithAssignedId {Name = "Anvil"};

            Assert.AreNotEqual(object1, null);
            Assert.AreNotEqual(object1, object2);

            object1.SetAssignedIdTo("AAAAA");
            object2.SetAssignedIdTo("AAAAA");

            // Even though the "business value signatures" are different, the persistent Ids 
            // were the same.  Call me crazy, but I put that much trust into persisted Ids.
            Assert.AreEqual(object1, object2);

            var object3 = new ObjectWithAssignedId {Name = "Acme"};

            // Since object1 has an Id but object3 doesn't, they won't be equal
            // even though their signatures are the same
            Assert.AreNotEqual(object1, object3);

            var object4 = new ObjectWithAssignedId {Name = "Acme"};

            // object3 and object4 are both transient and share the same signature
            Assert.AreEqual(object3, object4);
        }

        [TestMethod]
        public void CannotEquateObjectsWithSameIdButDifferentTypes()
        {
            var object1Type = new Object1();
            var object2Type = new Object2();

            EntityIdSetter.SetIdOf(object1Type, 1);
            EntityIdSetter.SetIdOf(object2Type, 1);

            Assert.AreNotEqual(object1Type, object2Type);
        }

        #region Nested type: Object1

        private class Object1 : DomainObject
        {
        }

        #endregion

        #region Nested type: Object2

        private class Object2 : DomainObject
        {
        }

        #endregion

        #region Comprehensive unit tests provided by Brian Nicoloff

        private MockEntityObjectWithDefaultId _diffObj;
        private MockEntityObjectWithSetId _diffObjWithId;
        private MockEntityObjectWithDefaultId _obj;
        private MockEntityObjectWithSetId _objWithId;
        private MockEntityObjectWithDefaultId _sameObj;
        private MockEntityObjectWithSetId _sameObjWithId;

        [TestInitialize]
        public void Setup()
        {
            _obj = new MockEntityObjectWithDefaultId
                       {
                           FirstName = "FName1",
                           LastName = "LName1",
                           Email = "testus...@mail.com"
                       };
            _sameObj = new MockEntityObjectWithDefaultId
                           {
                               FirstName = "FName1",
                               LastName = "LName1",
                               Email = "testus...@mail.com"
                           };
            _diffObj = new MockEntityObjectWithDefaultId
                           {
                               FirstName = "FName2",
                               LastName = "LName2",
                               Email = "testuse...@mail.com"
                           };
            _objWithId = new MockEntityObjectWithSetId
                             {
                                 FirstName = "FName1",
                                 LastName = "LName1",
                                 Email = "testus...@mail.com"
                             };
            _sameObjWithId = new MockEntityObjectWithSetId
                                 {
                                     FirstName = "FName1",
                                     LastName = "LName1",
                                     Email = "testus...@mail.com"
                                 };
            _diffObjWithId = new MockEntityObjectWithSetId
                                 {
                                     FirstName = "FName2",
                                     LastName = "LName2",
                                     Email = "testuse...@mail.com"
                                 };
        }

        [TestCleanup]
        public void Teardown()
        {
            _obj = null;
            _sameObj = null;
            _diffObj = null;
        }

        [TestMethod]
        public void DoesDefaultEntityEqualsOverrideWorkWhenNoIdIsAssigned()
        {
            Assert.IsTrue(_obj.Equals(_sameObj));
            Assert.IsTrue(!_obj.Equals(_diffObj));
            Assert.IsTrue(!_obj.Equals(new MockEntityObjectWithDefaultId()));
        }

        [TestMethod]
        public void DoEqualDefaultEntitiesWithNoIdsGenerateSameHashCodes()
        {
            Assert.IsTrue(_obj.GetHashCode().Equals(_sameObj.GetHashCode()));
        }

        [TestMethod]
        public void DoEqualDefaultEntitiesWithMatchingIdsGenerateDifferentHashCodes()
        {
            Assert.IsTrue(!_obj.GetHashCode().Equals(_diffObj.GetHashCode()));
        }

        [TestMethod]
        public void DoDefaultEntityEqualsOverrideWorkWhenIdIsAssigned()
        {
            _obj.SetAssignedIdTo(1);
            _diffObj.SetAssignedIdTo(1);
            Assert.IsTrue(_obj.Equals(_diffObj));
        }

        [TestMethod]
        public void DoesEntityEqualsOverrideWorkWhenNoIdIsAssigned()
        {
            Assert.IsTrue(_objWithId.Equals(_sameObjWithId));
            Assert.IsTrue(!_objWithId.Equals(_diffObjWithId));
            Assert.IsTrue(!_objWithId.Equals(new MockEntityObjectWithSetId()));
        }

        [TestMethod]
        public void DoEqualEntitiesWithNoIdsGenerateSameHashCodes()
        {
            Assert.IsTrue(_objWithId.GetHashCode().Equals(_sameObjWithId.GetHashCode()));
        }

        [TestMethod]
        public void DoEqualEntitiesWithMatchingIdsGenerateDifferentHashCodes()
        {
            Assert.IsTrue(!_objWithId.GetHashCode().Equals(_diffObjWithId.GetHashCode()));
        }

        [TestMethod]
        public void DoEntityEqualsOverrideWorkWhenIdIsAssigned()
        {
            _objWithId.SetAssignedIdTo("1");
            _diffObjWithId.SetAssignedIdTo("1");
            Assert.IsTrue(_objWithId.Equals(_diffObjWithId));
        }

        #region Nested type: MockEntityObjectBase

        private class MockEntityObjectBase :
            MockEntityObjectBase<int>
        {
        }

        public class MockEntityObjectBase<T> :
            DomainObjectWithTypedId<T>
        {
            [DomainSignature]
            public string FirstName { get; set; }

            [DomainSignature]
            public string LastName { get; set; }

            public string Email { get; set; }
        }

        #endregion

        #region Nested type: MockEntityObjectWithDefaultId

        private class MockEntityObjectWithDefaultId :
            MockEntityObjectBase, IHasAssignedId<int>
        {
            #region IHasAssignedId<int> Members

            public void SetAssignedIdTo(int assignedId)
            {
                Id = assignedId;
            }

            #endregion
        }

        #endregion

        #region Nested type: MockEntityObjectWithSetId

        private class MockEntityObjectWithSetId :
            MockEntityObjectBase<string>, IHasAssignedId<string>
        {
            #region IHasAssignedId<string> Members

            public void SetAssignedIdTo(string assignedId)
            {
                Id = assignedId;
            }

            #endregion
        }

        #endregion

        #endregion

        #region Nested type: ObjectWithAssignedId

        private class ObjectWithAssignedId : DomainObjectWithTypedId<string>, IHasAssignedId<string>
        {
            [DomainSignature]
            public string Name { get; set; }

            #region IHasAssignedId<string> Members

            public void SetAssignedIdTo(string assignedId)
            {
                Id = assignedId;
            }

            #endregion
        }

        #endregion

        #region Nested type: ObjectWithIntId

        private class ObjectWithIntId : DomainObject
        {
            [DomainSignature]
            public string Name { get; set; }
        }

        #endregion

        #endregion

        #region Nested type: ObjectWithNoDomainSignatureProperties

        /// <summary>
        /// This is a nonsense object; i.e., it doesn't make sense to have 
        /// an entity without a domain signature.
        /// </summary>
        private class ObjectWithNoDomainSignatureProperties : DomainObject
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        #endregion

        #region Nested type: ObjectWithOneDomainSignatureProperty

        public class ObjectWithOneDomainSignatureProperty : DomainObject
        {
            public string Name { get; set; }

            [DomainSignature]
            public int Age { get; set; }
        }

        #endregion
    }
}
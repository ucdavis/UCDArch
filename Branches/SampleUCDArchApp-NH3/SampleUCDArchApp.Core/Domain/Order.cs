using System;
using Newtonsoft.Json;
using UCDArch.Core.DomainModel;
using UCDArch.Core.Utils;
using NHibernate.Validator.Constraints;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace SampleUCDArchApp.Core.Domain
{
    public class Order : DomainObject
    {
        /// <summary>
        /// This is a placeholder constructor for NHibernate.
        /// A no-argument constructor must be avilable for NHibernate to create the object.
        /// </summary>
        public Order() { }

        public Order(Customer orderedBy)
        {
            Check.Require(orderedBy != null, "orderedBy may not be null");

            OrderedBy = orderedBy;
        }

        [JsonProperty]
        public virtual DateTime OrderDate { get; set; }

        [Required]
        [Length(12)]
        [JsonProperty]
        public virtual string ShipAddress { get; set; }

        [NotNull]
        public virtual Customer OrderedBy { get; set; }
    }
}
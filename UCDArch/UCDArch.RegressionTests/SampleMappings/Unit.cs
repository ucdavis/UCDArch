using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace UCDArch.RegressionTests.SampleMappings
{
    public class Unit : DomainObject
    {
        [Length(50)]
        [Required]
        public virtual string FullName { get; set; }

        [Length(50)]
        public virtual string ShortName { get; set; }

        [Length(6)]
        public virtual string PPSCode { get; set; }

        [Length(4)]
        [Required]
        public virtual string FISCode { get; set; }

        /*
        [Required]
        public virtual School School { get; set; }
        */
    }
}
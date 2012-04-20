using System.ComponentModel.DataAnnotations;
using UCDArch.Core.DomainModel;

namespace UCDArch.RegressionTests.SampleMappings
{
    public class Unit : DomainObject
    {
        [StringLength(50)]
        [Required]
        public virtual string FullName { get; set; }

        [StringLength(50)]
        public virtual string ShortName { get; set; }

        [StringLength(6)]
        public virtual string PPSCode { get; set; }

        [StringLength(4)]
        [Required]
        public virtual string FISCode { get; set; }

        /*
        [Required]
        public virtual School School { get; set; }
        */
    }
}
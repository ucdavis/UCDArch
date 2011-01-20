using System;
using System.Collections.Generic;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace UCDArch.RegressionTests.SampleMappings
{
    public class User : DomainObject
    {
        [Length(50)]
        [Required]
        public virtual string FirstName { get; set; }
        
        [Length(50)]
        [Required]
        public virtual string LastName { get; set; }
        
        [Length(10)]
        [Required]
        public virtual string LoginID { get; set; }

        //[StringLengthValidator(50)]
        //[RegexValidator(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")]
        //[IgnoreNulls]
        [Email]
        public virtual string Email { get; set; }

        //[StringLengthValidator(50)]
        //[RegexValidator(@"((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}")]
        //[IgnoreNulls]
        //[Pattern(@"((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}")]
        //The patern below would match US phone numbers, but may fail international phone numbers so only use in test.
        [Pattern(@"^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$")] 
        [Length(50)]
        public virtual string Phone { get; set; }

        [Length(9)]
        public virtual string EmployeeID { get; set; }
        
        [Length(9)]
        public virtual string StudentID { get; set; }

        public virtual Guid UserKey { get; set; }

        public virtual bool Inactive { get; set; }

        public virtual IList<UnitAssociation> UnitAssociations { get; set; }

        //public virtual IList<Permission> Permissions { get; set; }

        public User()
        {
            UnitAssociations = new List<UnitAssociation>();
        }
    }
}
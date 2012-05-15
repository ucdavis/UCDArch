using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace UCDArch.RegressionTests.SampleMappings
{
    public class User : DomainObject
    {
        [StringLength(50)]
        [Required]
        public virtual string FirstName { get; set; }
        
        [StringLength(50)]
        [Required]
        public virtual string LastName { get; set; }
        
        [StringLength(10)]
        [Required]
        public virtual string LoginID { get; set; }

        //[StringStringLengthValidator(50)]
        //[IgnoreNulls]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Not a well-formed email address.")]
        public virtual string Email { get; set; }

        //[StringStringLengthValidator(50)]
        //[RegexValidator(@"((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}")]
        //[IgnoreNulls]
        //[Pattern(@"((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}")]
        //The patern below would match US phone numbers, but may fail international phone numbers so only use in test.
        [RegularExpression(@"^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$")] 
        [StringLength(50)]
        public virtual string Phone { get; set; }

        [StringLength(9)]
        public virtual string EmployeeID { get; set; }
        
        [StringLength(9)]
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

    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Id(x => x.Id).Column("UserID");
            Map(x => x.FirstName);
            Map(x => x.LastName);

            Map(x => x.LoginID);
            Map(x => x.Phone);
            Map(x => x.Email);

            Map(x => x.EmployeeID);
            Map(x => x.StudentID);
            Map(x => x.UserKey);
            Map(x => x.Inactive);

            HasManyToMany(x => x.UnitAssociations).Table("UnitAssociations")
                .ParentKeyColumn("UserID").ChildKeyColumn("UnitID").Not.Inverse().Cascade.None();
        }
    }
}
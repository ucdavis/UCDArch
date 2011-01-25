using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UCDArch.Core.DomainModel;
using UCDArch.Core.Utils;
using NHibernate.Validator.Constraints;
using UCDArch.Core.NHibernateValidator.Extensions;


namespace SampleUCDArchApp.Core.Domain
{
    public class Product : DomainObject
    {
        public Product() { }

        [Required]
        [Length(40)]
        public virtual string ProductName { get; set; }

        [Length(20)]
        public virtual string QuantityPerUnit { get; set; }
        public virtual double UnitPrice { get; set; }
        public virtual int UnitsInStock { get; set; }
        public virtual int UnitsOnOrder { get; set; }
        public virtual int ReorderLevel { get; set; }
        public virtual bool Discontinued { get; set; }

    
    }
}

using FluentNHibernate.Mapping;
using SampleUCDArchApp.Core.Domain;

namespace SampleUCDArchApp.Core.Mappings
{
    public class CustomerMap : ClassMap<Customer>
    {
        public CustomerMap()
        {
            Id(x => x.Id, "CustomerID");

            Map(x => x.CompanyName);
            Map(x => x.ContactName);
            Map(x => x.Country);
            Map(x => x.Fax);

            HasMany(x => x.Orders).Cascade.All();
        }
    }
}
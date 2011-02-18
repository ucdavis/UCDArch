using FluentNHibernate.Mapping;
using SampleUCDArchApp.Core.Domain;

namespace SampleUCDArchApp.Core.Mappings
{
    public class OrderMap : ClassMap<Order>
    {
        public OrderMap()
        {
            Id(x => x.Id, "OrderID");

            Map(x => x.OrderDate);
            Map(x => x.ShipAddress);

            References(x => x.OrderedBy);
        }
    }
}
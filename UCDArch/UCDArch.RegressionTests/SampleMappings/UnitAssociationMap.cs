using FluentNHibernate.Mapping;

namespace UCDArch.RegressionTests.SampleMappings
{
    public class UnitAssociationMap : ClassMap<UnitAssociation>
    {
        public UnitAssociationMap()
        {
            Id(x => x.Id).Column("UnitAssociationID");
            Map(x => x.Inactive);

            References(x => x.Unit).Column("UnitID").Fetch.Join().Not.Nullable();
            References(x => x.User).Column("UserID").Not.Nullable();
        }
    }
}
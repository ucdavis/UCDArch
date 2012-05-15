using FluentNHibernate.Mapping;
using UCDArch.RegressionTests.SampleMappings;

namespace UCDArch.RegressionTests.Fluent
{
    public class UnitMap : ClassMap<Unit>
    {
        public UnitMap()
        {
            Table("Unit");
            Id(x => x.Id).Column("UnitID");

            Map(x => x.FISCode);
            Map(x => x.ShortName);
            Map(x => x.FullName);
            Map(x => x.PPSCode);
        }
    }
}

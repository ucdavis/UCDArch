using FluentNHibernate.Mapping;

namespace UCDArch.RegressionTests.SampleMappings
{
    public class UnitMap : ClassMap<Unit>
    {
        public UnitMap()
        {
            Id(x => x.Id).Column("UnitID");
            Map(x => x.FullName);
            Map(x => x.ShortName);
            Map(x => x.PPSCode);
            Map(x => x.FISCode);
        }
    }
}
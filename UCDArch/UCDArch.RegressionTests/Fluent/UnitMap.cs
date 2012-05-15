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

            Map(x => x.FISCode).Column("FIS_Code");
            Map(x => x.PPSCode).Column("PPS_Code");
            Map(x => x.ShortName);
            Map(x => x.FullName);
        }
    }
}

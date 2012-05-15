using UCDArch.Core.DomainModel;

namespace UCDArch.RegressionTests.SampleMappings
{
    public class UnitAssociation : DomainObject
    {
        public virtual bool Inactive { get; set; }

        public virtual User User { get; set; }
        /*
        public virtual Application Application { get; set; }
         */ 
        public virtual Unit Unit { get; set; }
    }
}

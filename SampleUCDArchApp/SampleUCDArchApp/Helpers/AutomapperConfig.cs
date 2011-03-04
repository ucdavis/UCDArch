using AutoMapper;
using SampleUCDArchApp.Core.Domain;

namespace SampleUCDArchApp.Helpers
{
    public class AutomapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg => cfg.AddProfile<ViewModelProfile>());
        }
    }


    public class ViewModelProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Order, Order>();

            CreateMap<Customer, Customer>();
        }
    }
}
using AutoMapper;
using Production.Api.Models;

namespace Production.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductUpdateModel, Product>()
                .ForMember(dest => dest.Name, opt => opt.Condition(src => src.Name != null))
                .ForMember(dest => dest.MakeFlag, opt => opt.Condition(src => src.MakeFlag != null))
                .ForMember(dest => dest.FinishedGoodsFlag, opt => opt.Condition(src => src.FinishedGoodsFlag != null))
                .ForMember(dest => dest.Color, opt => opt.Condition(src => src.Color != null));

            CreateMap<ProductCreateModel, Product>();
        }
    }
}

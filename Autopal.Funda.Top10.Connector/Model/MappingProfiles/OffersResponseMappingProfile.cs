using AutoMapper;
using Autopal.Funda.Top10.Connector.Client.Model;

namespace Autopal.Funda.Top10.Connector.Model.MappingProfiles
{
    public class OffersResponseMappingProfile : Profile
    {
        /// <summary>
        ///     Default constructor of AutoMapper profile for OffersResponse
        /// </summary>
        public OffersResponseMappingProfile()
        {
            CreateMap<GetOffersResponse, OffersResponse>()
                .ForMember(dst => dst.TotalCount, opt => opt.MapFrom(src => src.TotaalAantalObjecten))
                .ForMember(dst => dst.TotalPage, opt => opt.MapFrom(src => src.Paging.AantalPaginas))
                .ForMember(dst => dst.Offers, opt => opt.MapFrom(src => src.Objects));

            CreateMap<Object, Offer>()
                .ForMember(dst => dst.AgentId, opt => opt.MapFrom(src => src.MakelaarId))
                .ForMember(dst => dst.AgentName, opt => opt.MapFrom(src => src.MakelaarNaam))
                .ForMember(dst => dst.HouseType, opt => opt.MapFrom(src => src.SoortAanbodString))
                .ForMember(dst => dst.Address, opt => opt.MapFrom(src => src.Adres));
        }
    }
}
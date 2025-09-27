using Admin.Infrastructure.Shared.Models; // Source types
using AutoMapper;
using Admin.MVCWebUI.Models;              // Destination types


namespace Admin.MVCWebUI.MappingProfiles
{
  
        public class SlabsNTariffsAutoMappingProfile : Profile
        {
            public SlabsNTariffsAutoMappingProfile()
            {
                //CreateMap<CategoryListDto, CategoryListDto>();
                CreateMap<SlabsDetailsModel, SlabsDetailsModel>();
            }
        }
    }

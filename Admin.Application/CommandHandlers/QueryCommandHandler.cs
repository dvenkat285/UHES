//using Admin.Application.Features;
using Admin.Infrastructure.Shared.Models;
using MediatR;
using Dapper;
//using Admin.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Admin.Application.CommandQueries.Queries;
using Admin.Infrastructure.Repositories;

namespace Admin.Application.CommandHandlers
{
    public class QueryCommandHandler :
            IRequestHandler<GetAllSubcategoryWiseSlabTariffDetailsQuery, List<SlabsDetailsModel>>,
            IRequestHandler<GetAllCategoriesQuery, List<CategoryListDto>>
    {
        private readonly Repositories uhesRepository;
        //private readonly IMapper mapper;

        public QueryCommandHandler(Repositories uhesRepository
            //, IMapper mapper
            )
        {
            this.uhesRepository = uhesRepository;
            //this.mapper = mapper;
        }

        public async Task<List<SlabsDetailsModel>> Handle(GetAllSubcategoryWiseSlabTariffDetailsQuery request, CancellationToken cancellationToken)
        {
            var response = await this.uhesRepository.GetAllSubcategoryWiseSlabTariffDetails();
            return response.ToList();
        }

        public async Task<List<CategoryListDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await uhesRepository.GetAllCategoriesAsync(cancellationToken);
        }
    }
}


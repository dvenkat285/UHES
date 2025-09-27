using Admin.Infrastructure.Shared.Models;
using MediatR;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Admin.Application.CommandQueries.Queries;
using Admin.Infrastructure.Repositories;
//using Admin.API;
//using Admin.Application.Repositories;



namespace Admin.Application.CommandQueries
{
    public class GetSlabByCategorySubcategoryAndLinesQueryHandler
    : IRequestHandler<GetSlabByCategorySubcategoryAndLinesQuery, SlabsDetailsModel?>
    {
        private readonly Repositories _uhesRepository;

        public GetSlabByCategorySubcategoryAndLinesQueryHandler(Repositories uhesRepository)
        {
            _uhesRepository = uhesRepository;
        }

        public async Task<SlabsDetailsModel?> Handle(GetSlabByCategorySubcategoryAndLinesQuery request, CancellationToken cancellationToken)
        {
            var slab = await _uhesRepository.GetSlabByCategorySubcategoryAndLinesAsync(
                request.CategoryId, request.SubCategoryId, request.Lines, cancellationToken);

            return slab;
        }
    }


}

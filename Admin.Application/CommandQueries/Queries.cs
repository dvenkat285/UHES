//using Admin.Application.Features;
using Admin.Infrastructure.Shared.Models;
using MediatR;

namespace Admin.Application.CommandQueries
{
    public class Queries
    {
        public record GetAllSubcategoryWiseSlabTariffDetailsQuery() : IRequest<List<SlabsDetailsModel>>;

        public record GetAllCategoriesQuery : IRequest<List<CategoryListDto>>;
        public class GetSlabByCategorySubcategoryAndLinesQuery : IRequest<SlabsDetailsModel?>
        {
            public int CategoryId { get; }
            public int SubCategoryId { get; }
            public string Lines { get; }

            public GetSlabByCategorySubcategoryAndLinesQuery(int categoryId, int subCategoryId, string lines)
            {
                CategoryId = categoryId;
                SubCategoryId = subCategoryId;
                Lines = lines;
            }
        }

    }


}


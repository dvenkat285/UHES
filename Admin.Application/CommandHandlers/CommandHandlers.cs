//using Admin.Application.Features;
using MediatR;
using Dapper;
using Admin.Infrastructure.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Admin.Infrastructure.Repositories;
//using Admin.Application.Repositories;


namespace Admin.Application.CommandHandlers
{

    public record CreateSlabCommand(CategoryCreateDto Dto) : IRequest<CategoryCreateResultDto>;
    public record UpdateSlabCommand(CategoryUpdateDto Dtos) : IRequest<CategoryUpdateResultDto>;

    // The actual handler class
    public class CommandHandler :
        IRequestHandler<CreateSlabCommand, CategoryCreateResultDto>,
        IRequestHandler<UpdateSlabCommand, CategoryUpdateResultDto>
    {
        private readonly Repositories _repository;

        public CommandHandler(Repositories repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<CategoryCreateResultDto> Handle(CreateSlabCommand request, CancellationToken cancellationToken)
        {
            return await _repository.CreateSlabWithCategoryAndSubcategoryAsync(request.Dto, cancellationToken);
        }

        public async Task<CategoryUpdateResultDto> Handle(UpdateSlabCommand request, CancellationToken cancellationToken)
        {
            return await _repository.UpdateSlabWithCategoryAndSubcategoryAsync(request.Dtos, cancellationToken);
        }
    }
}

using Application.Abstractions.Messaging;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using Mapster;

namespace Application.UseCases.Categories.Commands.Update;

public class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand>
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(IRepository<Category> categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetAsync(
            filter: new CategoryFilter { Id = request.Id },
            asNoTracking: false,
            cancellationToken);

        request.Adapt(category);

        _categoryRepository.Update(category!);

        await _unitOfWork.SaveChangesAsync();
    }
}

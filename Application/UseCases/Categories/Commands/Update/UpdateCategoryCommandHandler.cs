using Application.Abstractions.Messaging;
using Application.Exceptions;
using Application.Filters;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;

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

        if (category == null)
            throw new NotFoundByIdException<Category>(request.Id);

        if (request.Name != null)
        {
            category.Name = request.Name;
        }

        if (request.ParentCategoryId != null)
        {
            if (request.ParentCategoryId == category.Id)
                throw new CircularDependencyException<Category>();

            category.ParentCategoryId = request.ParentCategoryId;
        }

        _categoryRepository.Update(category);

        await _unitOfWork.SaveChangesAsync();
    }
}

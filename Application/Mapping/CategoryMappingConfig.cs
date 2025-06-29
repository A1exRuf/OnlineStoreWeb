using Application.Dtos.Category;
using Domain.Entities;
using Mapster;

namespace Application.Mapping;

public class CategoryMappingConfig
{
    public static void Configure()
    {
        var config = TypeAdapterConfig.GlobalSettings;

        config.ForType<Category, CategoryWithChildrenDto>()
            .ConstructUsing(src => new CategoryWithChildrenDto(
                src.Id,
                src.Name,
                src.ParentCategoryId));
    }
}

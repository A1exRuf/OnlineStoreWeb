using Mapster;

namespace Application.Mapping;

public static class MappingConfig
{
    public static void Configure()
    {
        CartMappingConfig.Configure();
        OrderMappingConfig.Configure();
        CategoryMappingConfig.Configure();

        var config = TypeAdapterConfig.GlobalSettings;

        config.Default.IgnoreNullValues(true);
    }
}

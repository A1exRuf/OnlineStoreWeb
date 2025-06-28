using Application.Dtos.OrderItem;
using Application.Dtos.Product;
using Application.UseCases.Orders.Queries.GetDetailed;
using Domain.Entities;
using Mapster;

namespace Application.Mapping;

public static class OrderMappingConfig
{
    public static void Configure()
    {
        var config = TypeAdapterConfig.GlobalSettings;

        config.ForType<Order, GetDetailedResponse>()
            .ConstructUsing(src => new GetDetailedResponse(
                src.Id,
                src.Status,
                src.OrderDate,
                src.PaidDate,
                src.ShippingDate,
                src.CompletedDate,
                src.Address,
                src.Items.Adapt<List<OrderItemDto>>()));

        config.ForType<OrderItem, OrderItemDto>()
            .ConstructUsing(src => new OrderItemDto(
                src.Id,
                src.Product.Adapt<ProductDto>(),
                src.Quantity,
                src.UnitPrice));
    }
}

namespace Application.Dtos;

public record EntityIdDto(Guid Id)
{
    public static implicit operator Guid(EntityIdDto dto) => dto.Id;
}

using Application.Dtos;

namespace Application.Abstractions;

public interface IBlobService
{
    Task<Guid> UploadAsync(
        Stream stream,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<Uri> GetUriAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id, 
        CancellationToken cancellationToken = default);
}

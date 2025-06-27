using Domain.Common;

namespace Application.Abstractions;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    UserRole? Role { get; }
    Guid GuestCartId { get; }
}

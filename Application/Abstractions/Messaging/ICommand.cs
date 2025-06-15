using MediatR;

namespace Application.Abstractions.Messaging;

public interface IBaseCommand
{
}

public interface ICommand : IBaseCommand, IRequest
{
}

public interface ICommand<TResponse> : IBaseCommand, IRequest<TResponse>
{
}
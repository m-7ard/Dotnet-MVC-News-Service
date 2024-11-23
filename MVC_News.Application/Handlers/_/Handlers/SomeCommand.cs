using MediatR;
using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Handlers._.Handlers;

public class SomeCommand : IRequest<OneOf<CreateSubscriptionResult, List<ApplicationError>>>
{
}
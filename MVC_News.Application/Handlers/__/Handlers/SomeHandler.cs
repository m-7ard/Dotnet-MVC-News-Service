using MediatR;
using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Handlers.__.Handlers;

public class SomeHandler : IRequestHandler<SomeCommand, OneOf<CreateSubscriptionResult, List<ApplicationError>>>
{
    // private readonly IProductHistoryRepository _productHistoryRepository;

    public SomeHandler()
    {
    }

    public async Task<OneOf<CreateSubscriptionResult, List<ApplicationError>>> Handle(SomeCommand request, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}
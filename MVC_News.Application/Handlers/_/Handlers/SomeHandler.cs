using MediatR;
using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Handlers._.Handlers;

public class SomeHandler : IRequestHandler<SomeCommand, OneOf<SomeResult, List<PlainApplicationError>>>
{
    // private readonly IProductHistoryRepository _productHistoryRepository;

    public SomeHandler()
    {
    }

    public async Task<OneOf<SomeResult, List<PlainApplicationError>>> Handle(SomeCommand request, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}
using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Domain.Entities;
using OneOf;

namespace MVC_News.Application.Handlers.Authors.Read;

public class ReadAuthorHandler : IRequestHandler<ReadAuthorQuery, OneOf<ReadAuthorResult, List<ApplicationError>>>
{
    private readonly IUserRepository _userRepository;

    public ReadAuthorHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<OneOf<ReadAuthorResult, List<ApplicationError>>> Handle(ReadAuthorQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserById(request.Id);
        if (user is null)
        {
            return new List<ApplicationError>()
            {
                new ApplicationError(
                    message: $"Author of id \"{request.Id}\" does not exist.",
                    path: ["_"],
                    code: ApplicationErrorCodes.ModelDoesNotExist
                )
            };
        }

        return new ReadAuthorResult(author: new Author(id: request.Id, displayName: user.DisplayName));
    }
}
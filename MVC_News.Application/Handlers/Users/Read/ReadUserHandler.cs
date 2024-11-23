using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using OneOf;

namespace MVC_News.Application.Handlers.Users.Read;

public class ReadUserHandler : IRequestHandler<ReadUserQuery, OneOf<ReadUserResult, List<ApplicationError>>>
{
    private readonly IUserRepository _userRepository;

    public ReadUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<OneOf<ReadUserResult, List<ApplicationError>>> Handle(ReadUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserById(request.Id);
        if (user is null)
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: $"User of id \"{request.Id}\" does not exist.",
                path: ["_"],
                code: ApplicationErrorCodes.ModelDoesNotExist
            );
        }

        return new ReadUserResult(user);
    }
}
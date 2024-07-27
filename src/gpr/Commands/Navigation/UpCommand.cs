using System.CommandLine;
using GitPullRequest.Services;

namespace GitPullRequest.Commands.Navigation;

public class UpCommand()
    : Command<EmptyCommandOptions, UpCommandHandler>(
        "up",
        "Move up your current stack closer to the top."
    );

public class UpCommandHandler(IAnsiConsole console, INavigation navigation)
    : ICommandOptionsHandler<EmptyCommandOptions>
{
    public async Task<int> HandleAsync(
        EmptyCommandOptions options,
        CancellationToken cancellationToken
    )
    {
        switch (navigation.Up())
        {
            case NavigationSuccess(var (commit, message)):
                console.WriteLine($"[{commit}] {message}");
                return 0;

            case NavigationFailure(var error):
                console.WriteLine("Command failed: " + error.Message);
                return -1;

            default:
                throw new InvalidOperationException("Invalid navigation result");
        }
    }
}

using System.CommandLine;
using GitPullRequest.Services;
using Spectre.Console;

namespace GitPullRequest.Commands.Navigation;

public class TopCommand()
    : Command<EmptyCommandOptions, TopCommandHandler>(
        "top",
        "Move to the top of your current stack."
    );

public class TopCommandHandler(IAnsiConsole console, INavigation navigation)
    : ICommandOptionsHandler<EmptyCommandOptions>
{
    public async Task<int> HandleAsync(
        EmptyCommandOptions options,
        CancellationToken cancellationToken
    )
    {
        switch (navigation.Top())
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

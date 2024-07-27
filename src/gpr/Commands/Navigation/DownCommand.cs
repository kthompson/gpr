using System.CommandLine;
using GitPullRequest.Services;

namespace GitPullRequest.Commands.Navigation;

public class DownCommand()
    : Command<EmptyCommandOptions, DownCommandHandler>(
        "down",
        "Move down your current stack closer to the bottom."
    );

public class DownCommandHandler(IAnsiConsole console, INavigation navigation)
    : ICommandOptionsHandler<EmptyCommandOptions>
{
    public async Task<int> HandleAsync(
        EmptyCommandOptions options,
        CancellationToken cancellationToken
    )
    {
        switch (navigation.Down())
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

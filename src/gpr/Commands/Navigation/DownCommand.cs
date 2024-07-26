using System.CommandLine;
using GitPullRequest.Services;
using LibGit2Sharp;

namespace GitPullRequest.Commands.Navigation;

public class DownCommand()
    : Command<System.CommandLine.EmptyCommandOptions, DownCommandHandler>(
        "down",
        "Move down your current stack."
    );

public class DownCommandHandler(IConsole console, INavigation navigation)
    : ICommandOptionsHandler<System.CommandLine.EmptyCommandOptions>
{
    public async Task<int> HandleAsync(
        System.CommandLine.EmptyCommandOptions options,
        CancellationToken cancellationToken
    )
    {
        switch (navigation.Down())
        {
            case NavigationSuccess(var (_, commit, message)):
                console.WriteLine($"[{commit}] {message}");
                return 0;

            case NavigationFailure(var error):
                console.WriteLine("Command failed: " + error.Message);
                return -1;

            default:
                throw new InvalidOperationException("Invalid navigation result");
        }

        return 0;
    }
}

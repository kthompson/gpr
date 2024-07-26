using System.CommandLine;
using GitPullRequest.Services;

namespace GitPullRequest.Commands.Navigation;

public class PrevCommand()
    : Command<System.CommandLine.EmptyCommandOptions, PrevCommandHandler>(
        "prev",
        "Switch to the previous stack."
    );

public class PrevCommandHandler(IConsole console, INavigation navigation)
    : ICommandOptionsHandler<System.CommandLine.EmptyCommandOptions>
{
    public async Task<int> HandleAsync(
        System.CommandLine.EmptyCommandOptions options,
        CancellationToken cancellationToken
    )
    {
        switch (navigation.Previous())
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
    }
}

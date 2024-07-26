using System.CommandLine;
using System.CommandLine.IO;
using GitPullRequest.Services;

namespace GitPullRequest.Commands.Navigation;

public class BottomCommand()
    : Command<BottomCommandOptions, BottomCommandOptionsHandler>(
        "bottom",
        "Move to the bottom of your current stack."
    );

public class BottomCommandOptions : ICommandOptions;

public class BottomCommandOptionsHandler(IAnsiConsole console, INavigation navigation)
    : ICommandOptionsHandler<BottomCommandOptions>
{
    public async Task<int> HandleAsync(
        BottomCommandOptions options,
        CancellationToken cancellationToken
    )
    {
        switch (navigation.Bottom())
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

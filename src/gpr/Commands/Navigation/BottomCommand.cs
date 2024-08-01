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
    public Task<int> HandleAsync(BottomCommandOptions options, CancellationToken cancellationToken)
    {
        switch (navigation.Bottom())
        {
            case NavigationSuccess(var (commit, message)):
                console.WriteLine($"[{commit}] {message}");
                return Task.FromResult(0);

            case NavigationFailure(var error):
                console.WriteLine("Command failed: " + error.Message);
                return Task.FromResult(-1);

            default:
                throw new InvalidOperationException("Invalid navigation result");
        }
    }
}

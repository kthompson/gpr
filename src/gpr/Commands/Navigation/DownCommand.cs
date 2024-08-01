using System.CommandLine;
using GitPullRequest.Services;

namespace GitPullRequest.Commands.Navigation;

public class DownCommand()
    : Command<EmptyCommandOptions, DownCommandHandler>(
        "down",
        "Move down your current stack closer to the bottom."
    );

// ReSharper disable once ClassNeverInstantiated.Global
public class DownCommandHandler(IAnsiConsole console, INavigation navigation)
    : ICommandOptionsHandler<EmptyCommandOptions>
{
    public Task<int> HandleAsync(EmptyCommandOptions options, CancellationToken cancellationToken)
    {
        switch (navigation.Down())
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

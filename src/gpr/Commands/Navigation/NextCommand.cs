using System.CommandLine;
using GitPullRequest.Services;

namespace GitPullRequest.Commands.Navigation;

public class NextCommand()
    : Command<EmptyCommandOptions, NextCommandHandler>("next", "Switch to the next stack.");

public class NextCommandHandler(IAnsiConsole console, INavigation navigation)
    : ICommandOptionsHandler<EmptyCommandOptions>
{
    public Task<int> HandleAsync(EmptyCommandOptions options, CancellationToken cancellationToken)
    {
        switch (navigation.Next())
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

using System.CommandLine;
using GitPullRequest.Services;

namespace GitPullRequest.Commands.Navigation;

public class TopCommand() : Command<System.CommandLine.EmptyCommandOptions, TopCommandHandler>("top", "Move to the top of your current stack.");

public class TopCommandHandler(IConsole console, INavigation navigation) : ICommandOptionsHandler<System.CommandLine.EmptyCommandOptions>
{
    public async Task<int> HandleAsync(System.CommandLine.EmptyCommandOptions options, CancellationToken cancellationToken)
    {
        switch (navigation.Top())
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
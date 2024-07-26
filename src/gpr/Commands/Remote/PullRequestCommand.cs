using System.CommandLine;
using LibGit2Sharp;

namespace GitPullRequest.Commands.Remote;

public class PullRequestCommand()
    : Command<EmptyCommandOptions, PullRequestCommandHandler>(
        "pr",
        "Submit pull requests for the current stack"
    );

public class PullRequestCommandHandler(IConsole console, IRepository repo)
    : ICommandOptionsHandler<EmptyCommandOptions>
{
    public async Task<int> HandleAsync(
        EmptyCommandOptions options,
        CancellationToken cancellationToken
    )
    {
        // PullRequest changes from the remote


        return 0;
    }
}

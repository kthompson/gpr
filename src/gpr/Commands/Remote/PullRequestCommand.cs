using System.CommandLine;

namespace GitPullRequest.Commands.Remote;

public class PullRequestCommand()
    : Command<EmptyCommandOptions, PullRequestCommandHandler>(
        "pr",
        "Submit pull requests for the current stack"
    );

public class PullRequestCommandHandler : ICommandOptionsHandler<EmptyCommandOptions>
{
    public Task<int> HandleAsync(EmptyCommandOptions options, CancellationToken cancellationToken)
    {
        // PullRequest changes from the remote


        return Task.FromResult(0);
    }
}

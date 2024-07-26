using System.CommandLine;
using LibGit2Sharp;

namespace GitPullRequest.Commands;

public class StatusCommand : Command<EmptyCommandOptions, StatusCommandHandler>
{
    public StatusCommand()
        : base("status", "Show the current status")
    {
        AddAlias("st");
    }
}

public class StatusCommandHandler(IAnsiConsole console, IRepository repo)
    : ICommandOptionsHandler<EmptyCommandOptions>
{
    public async Task<int> HandleAsync(
        EmptyCommandOptions options,
        CancellationToken cancellationToken
    )
    {
        // show the current file listing
        return 0;
    }
}

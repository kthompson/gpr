using System.CommandLine;
using LibGit2Sharp;

namespace GitPullRequest.Commands;

public class CommitCommand : Command<CommitCommandOptions, CommitCommandOptionsHandler>
{
    public CommitCommand() : base("commit", "Commit the current changes")
    {
        AddOption(new Option<string>("-m", "The commit message"));
    }
}

public class CommitCommandOptions : ICommandOptions
{
    public required string Message { get; set; }
}

public class CommitCommandOptionsHandler(IConsole console, IRepository repo) : ICommandOptionsHandler<CommitCommandOptions>
{
    public async Task<int> HandleAsync(CommitCommandOptions options, CancellationToken cancellationToken)
    {
        // show the current file listing
        return 0;
    }
}
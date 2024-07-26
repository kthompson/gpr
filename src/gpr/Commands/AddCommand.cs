using System.CommandLine;
using LibGit2Sharp;

namespace GitPullRequest.Commands;

public class AddCommand : Command<AddCommandOptions, AddCommandOptionsHandler>
{
    public AddCommand()
        : base("add", "Show the current status")
    {
        AddArgument(new Argument<FileInfo>("untrackedFile", "The file to add"));
    }
}

public class AddCommandOptions : ICommandOptions
{
    public required FileInfo UntrackedFile { get; set; }
}

public class AddCommandOptionsHandler(IAnsiConsole console, IRepository repo)
    : ICommandOptionsHandler<AddCommandOptions>
{
    public async Task<int> HandleAsync(
        AddCommandOptions options,
        CancellationToken cancellationToken
    )
    {
        // Add an untracked file to the staging area
        return 0;
    }
}

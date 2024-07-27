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
        var status = repo.RetrieveStatus(
            new StatusOptions
            {
                Show = StatusShowOption.IndexAndWorkDir,
                DetectRenamesInIndex = true,
                DetectRenamesInWorkDir = true,
            }
        );

        foreach (var entry in status)
        {
            var state = entry.State;

            var ignored = state == FileStatus.Ignored;
            if (ignored)
                continue;

            var renamed = (state & (FileStatus.RenamedInIndex | FileStatus.RenamedInWorkdir)) != 0;
            if (renamed)
            {
                console.MarkupLine($"[red3_1]R {entry.FilePath}[/]");
                continue;
            }

            var modified =
                (state & (FileStatus.ModifiedInIndex | FileStatus.ModifiedInWorkdir)) != 0;
            if (modified)
            {
                console.MarkupLine($"[dodgerblue2]M {entry.FilePath}[/]");
                continue;
            }

            var untracked = (state & (FileStatus.NewInWorkdir)) != 0;
            if (untracked)
            {
                console.MarkupLine($"[hotpink2][underline]? {entry.FilePath}[/][/]");
                continue;
            }

            var added = (state & FileStatus.NewInIndex) == FileStatus.NewInIndex;
            if (added)
            {
                console.MarkupLine($"[green]A {entry.FilePath}[/]");
                continue;
            }

            throw new ArgumentOutOfRangeException(entry.State + ": " + entry.FilePath);
        }
        return 0;
    }
}

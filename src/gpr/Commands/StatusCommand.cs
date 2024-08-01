using System.CommandLine;
using GitPullRequest.Services;
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

public class StatusCommandHandler(IAnsiConsole console, IGetStatus getStatus)
    : ICommandOptionsHandler<EmptyCommandOptions>
{
    public Task<int> HandleAsync(EmptyCommandOptions options, CancellationToken cancellationToken)
    {
        var status = getStatus.GetStatus();

        foreach (var (filePath, state) in status.Entries)
        {
            switch (state)
            {
                case GprFileStatus.Renamed:
                    console.MarkupLine($"[red3_1]R {filePath}[/]");
                    continue;
                case GprFileStatus.Modified:
                    console.MarkupLine($"[dodgerblue2]M {filePath}[/]");
                    continue;
                case GprFileStatus.New:
                    console.MarkupLine($"[hotpink2][underline]? {filePath}[/][/]");
                    continue;
                case GprFileStatus.Added:
                    console.MarkupLine($"[green]A {filePath}[/]");
                    continue;
                default:
                    throw new ArgumentOutOfRangeException(state + ": " + filePath);
            }
        }
        return Task.FromResult(0);
    }
}

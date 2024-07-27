using System.CommandLine;
using GitPullRequest.Services;
using LibGit2Sharp;

namespace GitPullRequest.Commands;

public class CloneCommand : Command<CloneCommandOptions, CloneCommandOptionsHandler>
{
    // Keep the hard dependency on System.CommandLine here
    public CloneCommand()
        : base("clone", "Clone a repository")
    {
        AddArgument(new Argument<Uri>("repo", "The URL of the repository to clone"));
        AddOption(new Option<string>("--to", "The person to say hello to"));
    }
}

public class CloneCommandOptions : ICommandOptions
{
    public required Uri Repo { get; set; }
}

public class CloneCommandOptionsHandler(IAnsiConsole console, IO io)
    : ICommandOptionsHandler<CloneCommandOptions>
{
    public async Task<int> HandleAsync(
        CloneCommandOptions options,
        CancellationToken cancellationToken
    )
    {
        var url = options.Repo;
        var dir = url.AbsolutePath.Split('/').Last().Replace(".git", "");
        
        await console.Progress()
            .AutoRefresh(true)
            .HideCompleted(true)
            .Columns([
                new SpinnerColumn(Spinner.Known.Dots2),
                new TaskDescriptionColumn(),
            ])
            .StartAsync(async context =>
            {
                var task = context.AddTask($"Cloning {url} to {dir}");
                await Task.Run(
                    () =>
                        Repository.Clone(
                            url.ToString(),
                            Path.Combine(io.GetCurrentDirectory(), dir)
                        ),
                    cancellationToken
                );
                task.StopTask();
            });
        
        return 0;
    }
}

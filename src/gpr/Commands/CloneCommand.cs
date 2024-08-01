using System.CommandLine;
using GitPullRequest.Services;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace GitPullRequest.Commands;

public class CloneCommand : Command<CloneCommandOptions, CloneCommandOptionsHandler>
{
    // Keep the hard dependency on System.CommandLine here
    public CloneCommand()
        : base("clone", "Clone a repository")
    {
        AddArgument(new Argument<Uri>("repo", "The URL of the repository to clone"));
    }
}

public class CloneCommandOptions : ICommandOptions
{
    public required Uri Repo { get; set; }
}

public class CloneCommandOptionsHandler(IAnsiConsole console, IO io, IGitHub gh)
    : ICommandOptionsHandler<CloneCommandOptions>
{
    public async Task<int> HandleAsync(
        CloneCommandOptions options,
        CancellationToken cancellationToken
    )
    {
        var url = options.Repo;
        var dir = url.AbsolutePath.Split('/').Last().Replace(".git", "");
        var token = await gh.GetTokenAsync();
        CredentialsHandler? handler =
            !IsGitHubUrl(url) || string.IsNullOrWhiteSpace(token)
                ? null
                : (s, fromUrl, types) =>
                    new UsernamePasswordCredentials { Username = token, Password = "" };

        await console
            .Progress()
            .AutoRefresh(true)
            .HideCompleted(true)
            .Columns([new SpinnerColumn(Spinner.Known.Dots2), new TaskDescriptionColumn()])
            .StartAsync(async context =>
            {
                var task = context.AddTask($"Cloning {url} to {dir}");
                await Task.Run(
                    () =>
                        Repository.Clone(
                            url.ToString(),
                            Path.Combine(io.GetCurrentDirectory(), dir),
                            new CloneOptions { FetchOptions = { CredentialsProvider = handler, } }
                        ),
                    cancellationToken
                );
                task.StopTask();
            });

        return 0;
    }

    private static bool IsGitHubUrl(Uri url) =>
        url.ToString().Contains("github.com", StringComparison.InvariantCultureIgnoreCase);
}

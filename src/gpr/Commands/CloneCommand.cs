using System.CommandLine;
using Kurukuru;
using LibGit2Sharp;

namespace GitPullRequest.Commands;

public class CloneCommand : Command<CloneCommandOptions, CloneCommandOptionsHandler>
{
    // Keep the hard dependency on System.CommandLine here
    public CloneCommand()
        : base("clone", "Clone a repository")
    {
        AddArgument(new Argument<Uri>("repo", "The URL of the repository to clone"));
        this.AddOption(new Option<string>("--to", "The person to say hello to"));
    }
}

public class CloneCommandOptions : ICommandOptions
{
    public required Uri Repo { get; set; }
}

public class CloneCommandOptionsHandler(IConsole console, IO io) : ICommandOptionsHandler<CloneCommandOptions>
{

    public async Task<int> HandleAsync(CloneCommandOptions options, CancellationToken cancellationToken)
    {
        var url = options.Repo;
        var dir = url.AbsolutePath.Split('/').Last().Replace(".git", "");
        console.WriteLine($"Cloning {url} to {dir}");
            
        await Spinner.StartAsync($"Cloning {url} to {dir}", async spinner =>
        {
            var repo = await Task.Run(() => Repository.Clone(url.ToString(), Path.Combine(io.GetCurrentDirectory(), dir)), cancellationToken);
            spinner.Stop($"Cloned to {repo}");
        });
        return (0);
    }
}
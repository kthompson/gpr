using System.CommandLine;
using System.Reflection;
using GitPullRequest.Commands.Navigation;
using GitPullRequest.Commands.Remote;
using GitPullRequest.Services;
using LibGit2Sharp;

namespace GitPullRequest.Commands;

public class GitPullRequestCommand : Command<EmptyCommandOptions, GitPullRequestCommandHandler>
{
    private static Assembly? _assembly;
    private static string? _executablePath;
    private static string? _executableName;

    public GitPullRequestCommand()
        : base(ExecutableName, "gpr")
    {
        // repo-less commands
        Add(new CloneCommand());

        // Navigation commands
        Add(new PrevCommand());
        Add(new NextCommand());
        Add(new TopCommand());
        Add(new BottomCommand());

        // stack commands
        // absorb,

        // commit commands
        Add(new AddCommand());
        Add(new ForgetCommand());
        Add(new CommitCommand());
        Add(new AmendCommand());
        Add(new StatusCommand());

        // remote commands
        Add(new PullRequestCommand());
        Add(new PullCommand());
    }

    internal static Assembly GetAssembly() =>
        _assembly ??= (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());

    /// <summary>
    /// The name of the currently running executable.
    /// </summary>
    private static string ExecutableName =>
        _executableName ??= Path.GetFileNameWithoutExtension(ExecutablePath).Replace(" ", "");

    /// <summary>
    /// The path to the currently running executable.
    /// </summary>
    private static string ExecutablePath => _executablePath ??= Environment.GetCommandLineArgs()[0];
}

public class GitPullRequestCommandHandler(IAnsiConsole console, IO io)
    : ICommandOptionsHandler<EmptyCommandOptions>
{
    public Task<int> HandleAsync(EmptyCommandOptions options, CancellationToken cancellationToken)
    {
        var cd = io.GetCurrentDirectory();
        var repoPath = Repository.Discover(cd);

        if (repoPath == null)
        {
            console.WriteLine("Not a git repository.");
            return Task.FromResult(-1);
        }

        var repo = new Repository(repoPath);
        foreach (var branch in repo.Branches)
        {
            console.WriteLine(branch.FriendlyName);
        }

        return Task.FromResult(0);
    }
}

using System.CommandLine;
using LibGit2Sharp;

namespace GitPullRequest.Commands;

public class ForgetCommand : Command<ForgetCommandOptions, ForgetCommandOptionsHandler>
{
    public ForgetCommand()
        : base("forget", "Stop tracking the specified file")
    {
        AddArgument(new Argument<FileInfo>("untrackedFile", "The file to stop tracking"));
        AddAlias("unadd");
    }
}

public class ForgetCommandOptions : ICommandOptions
{
    public required FileInfo UntrackedFile { get; set; }
}

public class ForgetCommandOptionsHandler(IRepository repo)
    : ICommandOptionsHandler<ForgetCommandOptions>
{
    public Task<int> HandleAsync(ForgetCommandOptions options, CancellationToken cancellationToken)
    {
        // get relative path of the file from the repo root
        var relativePath = options.UntrackedFile.FullName.Substring(
            repo.Info.WorkingDirectory.Length
        );
        repo.Index.Remove(relativePath);
        repo.Index.Write();
        return Task.FromResult(0);
    }
}

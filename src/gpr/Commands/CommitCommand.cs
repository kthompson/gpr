using System.CommandLine;
using System.ComponentModel.DataAnnotations;
using GitPullRequest.Services;
using LibGit2Sharp;

namespace GitPullRequest.Commands;

public class CommitCommand : Command<CommitCommandOptions, CommitCommandOptionsHandler>
{
    public CommitCommand()
        : base("commit", "Commit the current changes")
    {
        AddOption(
            new Option<string>(["-m", "--message"], "The commit message") { IsRequired = true }
        );
    }
}

public class CommitCommandOptions : ICommandOptions
{
    [Required]
    public required string Message { get; set; }
}

public class CommitCommandOptionsHandler(IRepository repo, IGetStatus getStatus)
    : ICommandOptionsHandler<CommitCommandOptions>
{
    public Task<int> HandleAsync(CommitCommandOptions options, CancellationToken cancellationToken)
    {
        var status = getStatus.GetStatus();

        foreach (var (filePath, state) in status.Entries)
        {
            if (state == GprFileStatus.New)
                continue;

            repo.Index.Add(filePath);
        }

        repo.Index.Write();

        var sig = repo.Config.BuildSignature(DateTimeOffset.Now);
        var commit = repo.Commit(options.Message, sig, sig, new CommitOptions());

        // show the current file listing
        return Task.FromResult(0);
    }
}

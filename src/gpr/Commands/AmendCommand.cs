using System.CommandLine;
using GitPullRequest.Services;

namespace GitPullRequest.Commands;

public class AmendCommand()
    : Command<EmptyCommandOptions, AmendCommandHandler>(
        "amend",
        "meld pending changes into the current commit"
    );

public class AmendCommandHandler(IGetStatus getStatus) : ICommandOptionsHandler<EmptyCommandOptions>
{
    public Task<int> HandleAsync(EmptyCommandOptions options, CancellationToken cancellationToken)
    {
        var status = getStatus.GetStatus();

        // foreach (var (filePath, state) in status.Entries)
        // {
        //     if (state == GprFileStatus.New)
        //         continue;
        //
        //     repo.Index.Add(filePath);
        // }

        // repo.Index.Write();
        // repo.Head.TrackingDetails.CommonAncestor

        // var sig = repo.Config.BuildSignature(DateTimeOffset.Now);
        // var commit = repo.Commit(options.Message, sig, sig, new CommitOptions());

        // show the current file listing
        return Task.FromResult(0);
    }
}

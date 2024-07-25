using System.CommandLine;
using LibGit2Sharp;

namespace GitPullRequest.Commands;

public class AmendCommand()
    : Command<EmptyCommandOptions, AmendCommandHandler>("amend", "meld pending changes into the current commit");

public class AmendCommandHandler(IConsole console, IRepository repo) : ICommandOptionsHandler<EmptyCommandOptions>
{
    public async Task<int> HandleAsync(EmptyCommandOptions options, CancellationToken cancellationToken)
    {
        // show the current file listing
        return 0;
    }
}
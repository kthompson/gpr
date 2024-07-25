using System.CommandLine;
using LibGit2Sharp;

namespace GitPullRequest.Commands.Remote;

public class PullCommand()
    : Command<EmptyCommandOptions, PullCommandHandler>("pull", "Pull changes from the remote repository");


public class PullCommandHandler(IConsole console, IRepository repo) : ICommandOptionsHandler<EmptyCommandOptions>
{
    public async Task<int> HandleAsync(EmptyCommandOptions options, CancellationToken cancellationToken)
    {
        // fetch changes from the remote origin
        var remote = repo.Network.Remotes.First();
            
        console.WriteLine($"Pulling from {remote.Url}");
        await Task.Run(() => repo.Network.Fetch(remote.Name, [], new FetchOptions
        {
            // CredentialsProvider = 
        }), cancellationToken);
        
        return 0;
    }
}
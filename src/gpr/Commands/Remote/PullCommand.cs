using System.CommandLine;
using GitPullRequest.Services;
using LibGit2Sharp;

namespace GitPullRequest.Commands.Remote;

public class PullCommand()
    : Command<EmptyCommandOptions, PullCommandHandler>(
        "pull",
        "Pull changes from the remote repository"
    );

public class PullCommandHandler(IAnsiConsole console, IGitHub gh, IRepository repo)
    : ICommandOptionsHandler<EmptyCommandOptions>
{
    public async Task<int> HandleAsync(
        EmptyCommandOptions options,
        CancellationToken cancellationToken
    )
    {
        // fetch changes from the remote origin
        var remote = repo.Network.Remotes.First();

        console.WriteLine($"Pulling from {remote.Url}");
        var token = await gh.GetTokenAsync();

        await Task.Run(
            () =>
                repo.Network.Fetch(
                    remote.Name,
                    [],
                    new FetchOptions
                    {
                        OnProgress = serverProgressOutput =>
                        {
                            console.WriteLine(serverProgressOutput);
                            return !cancellationToken.IsCancellationRequested;
                        },
                        OnTransferProgress = progress =>
                        {
                            console.WriteLine($"Indexed objects: {progress.IndexedObjects}");
                            console.WriteLine($"Received objects: {progress.ReceivedObjects}");
                            console.WriteLine($"Total objects: {progress.TotalObjects}");
                            console.WriteLine($"Received bytes: {progress.ReceivedBytes}");

                            return !cancellationToken.IsCancellationRequested;
                        },
                        CredentialsProvider = (url, fromUrl, types) =>
                            new UsernamePasswordCredentials { Username = token, Password = "", },
                    }
                ),
            cancellationToken
        );

        return 0;
    }
}

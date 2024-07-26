using System.CommandLine;
using LibGit2Sharp;

namespace GitPullRequest.Commands.Remote;

public class PullCommand()
    : Command<EmptyCommandOptions, PullCommandHandler>(
        "pull",
        "Pull changes from the remote repository"
    );

public class PullCommandHandler(IConsole console, IRepository repo)
    : ICommandOptionsHandler<EmptyCommandOptions>
{
    async Task<string> Exec(string cmd, string args)
    {
        var proc = new System.Diagnostics.Process();
        proc.StartInfo.FileName = cmd;
        proc.StartInfo.Arguments = args;
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.Start();
        var result = await proc.StandardOutput.ReadToEndAsync();
        await proc.WaitForExitAsync();
        return result;
    }

    public async Task<int> HandleAsync(
        EmptyCommandOptions options,
        CancellationToken cancellationToken
    )
    {
        // fetch changes from the remote origin
        var remote = repo.Network.Remotes.First();

        console.WriteLine($"Pulling from {remote.Url}");
        var token = (await Exec("gh", "auth token")).Trim();

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

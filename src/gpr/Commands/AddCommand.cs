using System.CommandLine;
using LibGit2Sharp;

namespace GitPullRequest.Commands;

public class AddCommand : Command<AddCommandOptions, AddCommandOptionsHandler>
{
    public AddCommand()
        : base("add", "Track the specified file")
    {
        AddArgument(new Argument<FileInfo>("untrackedFile", "The file to add"));
    }
}

public class AddCommandOptions : ICommandOptions
{
    public required FileInfo UntrackedFile { get; set; }
}

public class AddCommandOptionsHandler(IRepository repo) : ICommandOptionsHandler<AddCommandOptions>
{
    public async Task<int> HandleAsync(
        AddCommandOptions options,
        CancellationToken cancellationToken
    )
    {
        // get relative path of the file from the repo root
        var relativePath = options.UntrackedFile.FullName.Substring(
            repo.Info.WorkingDirectory.Length
        );
        repo.Index.Add(relativePath);
        repo.Index.Write();
        return 0;
    }
}

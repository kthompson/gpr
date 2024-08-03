using LibGit2Sharp;

namespace GitPullRequest.Services;

public interface IBottomProvider
{
    Branch GetBottom();
}

public class BottomProvider(IRepository repository) : IBottomProvider
{
    public Branch GetBottom() =>
        repository.Branches.First(b => b.IsRemote && b.FriendlyName == "origin/HEAD");
}

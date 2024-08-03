using System.Collections.Immutable;
using LibGit2Sharp;

namespace GitPullRequest.Services;

public record Location(string Commit, string Message);

public abstract record NavigationResult;

public sealed record NavigationSuccess(Commit Location) : NavigationResult;

public sealed record NavigationFailureOptions(ImmutableArray<Commit> Options) : NavigationResult;

public sealed record NavigationFailureNoParent : NavigationResult;

public sealed record NavigationFailureNoChildren : NavigationResult;

public interface INavigation
{
    Branch CurrentLocation { get; }

    NavigationResult Goto(Commit commit) => Goto(commit.Id);
    NavigationResult Goto(ObjectId id);
    NavigationResult Next();
    NavigationResult Previous();
    NavigationResult Top();
    NavigationResult Bottom();
}

/**
 * Tracking stacks:
 *  - each stack is the tip/top of a branch in the repository with a name like `stack/1`, `stack/2`, etc.
 *  - the current commit is tracked with another branch named `stack/current`
  */
public class Navigation(IRepository repository, IBottomProvider bottomProvider) : INavigation
{
    public Branch CurrentLocation { get; } =
        repository.Branches.FirstOrDefault(b => b.FriendlyName == "current")
        ?? repository.CreateBranch("current", bottomProvider.GetBottom().Tip);

    public NavigationResult Next() => throw new NotImplementedException();

    // next child
    public NavigationResult Previous()
    {
        // next parent
        var parents = CurrentLocation.Tip.Parents.ToImmutableArray();
        return parents.Length switch
        {
            0 => new NavigationFailureNoParent(),
            1 => Goto(parents[0]),
            _ => new NavigationFailureOptions(parents)
        };
    }

    public NavigationResult Goto(Commit commit) => Goto(commit.Id);

    public NavigationResult Goto(ObjectId id) => throw new NotImplementedException();

    public NavigationResult Top() => throw new NotImplementedException();

    public NavigationResult Bottom() => throw new NotImplementedException();
}

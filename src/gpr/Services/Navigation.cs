using LibGit2Sharp;

namespace GitPullRequest.Services;

public record Location(string Stack, string Commit, string Message);

public abstract record NavigationResult;
public sealed record NavigationSuccess(Location Location) : NavigationResult;
public sealed record NavigationFailure(Exception Exception) : NavigationResult;

public interface INavigation
{
    Location CurrentLocation { get; }
    NavigationResult Next();
    NavigationResult Previous();
    NavigationResult Top();
    NavigationResult Bottom();
    NavigationResult Up();
    NavigationResult Down();
}

public class Navigation : INavigation
{
    public Navigation(IRepository repository)
    {

    }

    public Location CurrentLocation { get; private set; }

    public NavigationResult Next()
    {
        return new NavigationFailure(new NotImplementedException());
    }

    public NavigationResult Previous()
    {
        return new NavigationFailure(new NotImplementedException());
    }

    public NavigationResult Top()
    {
        return new NavigationFailure(new NotImplementedException());
    }

    public NavigationResult Bottom()
    {
        return new NavigationFailure(new NotImplementedException());
    }

    public NavigationResult Up()
    {
        return new NavigationFailure(new NotImplementedException());
    }

    public NavigationResult Down()
    {
        return new NavigationFailure(new NotImplementedException());
    }
}
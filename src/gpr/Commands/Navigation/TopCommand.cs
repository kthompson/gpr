using System.CommandLine;
using GitPullRequest.Services;

namespace GitPullRequest.Commands.Navigation;

public class TopCommand()
    : Command<EmptyCommandOptions, TopCommandHandler>(
        "top",
        "Move to the top of your current stack."
    );

public class TopCommandHandler(IAnsiConsole console, INavigation navigation)
    : NavigationCommandHandler("current stack has multiple tops:", console, navigation)
{
    protected override NavigationResult TryNavigate() => Navigation.Top();
}

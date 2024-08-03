using System.CommandLine;
using GitPullRequest.Services;

namespace GitPullRequest.Commands.Navigation;

public class PrevCommand()
    : Command<EmptyCommandOptions, PrevCommandHandler>("prev", "Switch to the previous stack.");

public class PrevCommandHandler(IAnsiConsole console, INavigation navigation)
    : NavigationCommandHandler("current stack has multiple parents:", console, navigation)
{
    protected override NavigationResult TryNavigate() => Navigation.Previous();
}

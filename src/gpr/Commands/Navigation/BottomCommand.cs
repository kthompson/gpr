using System.CommandLine;
using GitPullRequest.Services;

namespace GitPullRequest.Commands.Navigation;

public class BottomCommand()
    : Command<EmptyCommandOptions, BottomCommandOptionsHandler>(
        "bottom",
        "Move to the bottom of your current stack."
    );

public class BottomCommandOptionsHandler(IAnsiConsole console, INavigation navigation)
    : NavigationCommandHandler("current stack has multiple parents:", console, navigation)
{
    protected override NavigationResult TryNavigate() => Navigation.Bottom();
}

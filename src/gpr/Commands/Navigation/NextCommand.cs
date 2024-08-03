using System.CommandLine;
using GitPullRequest.Services;

namespace GitPullRequest.Commands.Navigation;

public class NextCommand()
    : Command<EmptyCommandOptions, NextCommandHandler>("next", "Switch to the next stack.");

public class NextCommandHandler(IAnsiConsole console, INavigation navigation)
    : NavigationCommandHandler("current stack has multiple children:", console, navigation)
{
    protected override NavigationResult TryNavigate() => Navigation.Next();
}

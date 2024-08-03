using System.CommandLine;
using GitPullRequest.Models;
using GitPullRequest.Services;
using LibGit2Sharp;

namespace GitPullRequest.Commands.Navigation;

public abstract class NavigationCommandHandler(
    string promptMessage,
    IAnsiConsole console,
    INavigation navigation
) : ICommandOptionsHandler<EmptyCommandOptions>
{
    protected readonly INavigation Navigation = navigation;

    protected abstract NavigationResult TryNavigate();

    public Task<int> HandleAsync(EmptyCommandOptions options, CancellationToken cancellationToken)
    {
        var result = TryNavigate();
        while (true)
        {
            switch (result)
            {
                case NavigationSuccess(var commit):
                    console.WriteLine($"[{commit.Id}] {commit.MessageShort}");
                    return Task.FromResult(0);

                case NavigationFailureNoChildren:
                    console.MarkupLine("[red]abort:[/] current commit has no children");
                    return Task.FromResult(-1);

                case NavigationFailureNoParent:
                    console.MarkupLine("[red]abort:[/] current commit has no parent");
                    return Task.FromResult(-1);

                case NavigationFailureOptions(var commits):
                    var selection = console.Prompt(Prompts.CommitSelection(promptMessage, commits));

                    if (selection is PromptOption<Commit>.CancelOption)
                    {
                        console.MarkupLine("[red]abort:[/] cancelling");
                        return Task.FromResult(-1);
                    }

                    result = Navigation.Goto(((PromptOption<Commit>.Item)selection).Value);
                    continue;

                default:
                    throw new InvalidOperationException("Invalid navigation result");
            }
        }
    }
}

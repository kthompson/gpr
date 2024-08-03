using LibGit2Sharp;

namespace GitPullRequest.Models;

public record PromptOption<T>
{
    public record Item(T Value) : PromptOption<T>;

    public record CancelOption : PromptOption<T>;

    public static PromptOption<T> Of(T item) => new Item(item);

    public static PromptOption<T> Cancel => new CancelOption();
}

public class Prompts
{
    public static SelectionPrompt<PromptOption<Commit>> CommitSelection(
        string title,
        IEnumerable<Commit> commits,
        bool allowCancel = true
    ) =>
        new SelectionPrompt<PromptOption<Commit>>()
            .Title(title)
            .UseConverter(option =>
                option switch
                {
                    PromptOption<Commit>.Item(var item) => $"[{item.Id}] {item.MessageShort}",
                    PromptOption<Commit>.CancelOption => "cancel",
                    _ => throw new ArgumentOutOfRangeException(nameof(option))
                }
            )
            .AddChoices(
                commits
                    .Select(PromptOption<Commit>.Of)
                    .Concat(allowCancel ? [PromptOption<Commit>.Cancel] : [])
            );
}

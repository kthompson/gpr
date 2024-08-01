using System.Collections.Immutable;
using LibGit2Sharp;

namespace GitPullRequest.Services;

public enum GprFileStatus
{
    Renamed,
    Modified,
    New,
    Added,
}

public record GprStatusEntry(string FilePath, GprFileStatus State);

public record GprStatus(ImmutableArray<GprStatusEntry> Entries);

public interface IGetStatus
{
    GprStatus GetStatus();
}

class GetStatusHandler(IRepository repo) : IGetStatus
{
    private IEnumerable<GprStatusEntry> GprStatusEntries(RepositoryStatus status)
    {
        foreach (var entry in status)
        {
            var state = entry.State;

            var ignored = state == FileStatus.Ignored;
            if (ignored)
                continue;

            var renamed = (state & (FileStatus.RenamedInIndex | FileStatus.RenamedInWorkdir)) != 0;
            if (renamed)
            {
                yield return new GprStatusEntry(entry.FilePath, GprFileStatus.Renamed);
                continue;
            }

            var modified =
                (state & (FileStatus.ModifiedInIndex | FileStatus.ModifiedInWorkdir)) != 0;
            if (modified)
            {
                yield return new GprStatusEntry(entry.FilePath, GprFileStatus.Modified);
                continue;
            }

            var untracked = (state & (FileStatus.NewInWorkdir)) != 0;
            if (untracked)
            {
                yield return new GprStatusEntry(entry.FilePath, GprFileStatus.New);
                continue;
            }

            var added = (state & FileStatus.NewInIndex) == FileStatus.NewInIndex;
            if (added)
            {
                yield return new GprStatusEntry(entry.FilePath, GprFileStatus.Added);
                continue;
            }

            throw new ArgumentOutOfRangeException(entry.State + ": " + entry.FilePath);
        }
    }

    public GprStatus GetStatus()
    {
        var status = repo.RetrieveStatus(
            new StatusOptions
            {
                Show = StatusShowOption.IndexAndWorkDir,
                DetectRenamesInIndex = true,
                DetectRenamesInWorkDir = true,
            }
        );

        return new GprStatus(GprStatusEntries(status).ToImmutableArray());
    }
}

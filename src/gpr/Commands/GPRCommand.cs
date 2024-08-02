using System.CommandLine;
using System.Reflection;
using GitPullRequest.Commands.Navigation;
using GitPullRequest.Commands.Remote;
using GitPullRequest.Models;
using GitPullRequest.Services;
using LibGit2Sharp;
using Spectre.Console.Advanced;

namespace GitPullRequest.Commands;

public class GitPullRequestCommand : Command<EmptyCommandOptions, GitPullRequestCommandHandler>
{
    private static Assembly? _assembly;
    private static string? _executablePath;
    private static string? _executableName;

    public GitPullRequestCommand()
        : base(ExecutableName, "gpr")
    {
        // repo-less commands
        Add(new CloneCommand());

        // Navigation commands
        Add(new PrevCommand());
        Add(new NextCommand());
        Add(new TopCommand());
        Add(new BottomCommand());

        // stack commands
        // absorb,

        // commit commands
        Add(new AddCommand());
        Add(new ForgetCommand());
        Add(new CommitCommand());
        Add(new AmendCommand());
        Add(new StatusCommand());

        // remote commands
        Add(new PullRequestCommand());
        Add(new PullCommand());
    }

    internal static Assembly GetAssembly() =>
        _assembly ??= (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());

    /// <summary>
    /// The name of the currently running executable.
    /// </summary>
    private static string ExecutableName =>
        _executableName ??= Path.GetFileNameWithoutExtension(ExecutablePath).Replace(" ", "");

    /// <summary>
    /// The path to the currently running executable.
    /// </summary>
    private static string ExecutablePath => _executablePath ??= Environment.GetCommandLineArgs()[0];
}

public class GitPullRequestCommandHandler(IAnsiConsole console, IO io)
    : ICommandOptionsHandler<EmptyCommandOptions>
{
    public Task<int> HandleAsync(EmptyCommandOptions options, CancellationToken cancellationToken)
    {
        // manually load the git repository since this command supports running outside a git repository
        var cd = io.GetCurrentDirectory();
        var repoPath = Repository.Discover(cd);

        if (repoPath == null)
        {
            // TODO: ideally we want to show the help message here but not sure how it access it at the moment
            console.WriteLine("Not a git repository.");
            return Task.FromResult(-1);
        }

        var repo = new Repository(repoPath);

        var originHead = repo.Branches.First(b => b.IsRemote && b.FriendlyName == "origin/HEAD");

        var graph = new Graph<ObjectId, Commit>();
        // add base node
        graph.AddNode(originHead.Tip.Id, originHead.Tip, []);

        // add all the branches
        foreach (var branch in repo.Branches)
        {
            if (branch.IsRemote)
                continue;

            // make sure the branch has a common ancestor with the origin

            if (repo.ObjectDatabase.FindMergeBase(branch.Tip, originHead.Tip) == null)
                continue;

            var commits = new List<Commit> { branch.Tip };
            while (commits.Count > 0)
            {
                var commit = commits.First();
                commits.RemoveAt(0);
                if (commit.Id == originHead.Tip.Id)
                    continue;

                if (graph.Contains(commit.Id))
                    continue;

                commits.AddRange(commit.Parents);
                graph.AddNode(commit.Id, commit, commit.Parents.Select(p => p.Id).ToList());
            }
        }

        var (layers, detached) = graph.TopologicalSort();
        var layeredCommits = layers
            .Select(layer => layer.Select(id => new { Id = id, Commit = graph[id] }))
            .ToList();
        // var commitToBranches = (
        //     from b in repo.Branches
        //     let tip = b.Tip
        //     group b by tip into g
        //     select new { Commit = g.Key, Branches = g.Select(b => b.FriendlyName).ToList() }
        // ).ToDictionary(x => x.Commit, x => x.Branches);

        // var branch = repo.Head;
        // var commit = branch.Tip;
        // var commonAncestor = branch.TrackedBranch.Tip;
        //
        // while (commit != commonAncestor)
        // {
        //     PrintCommit(commit, null, commit == branch.Tip);
        //     commit = commit.Parents.First();
        // }
        //
        // PrintCommit(
        //     commonAncestor,
        //     branch.TrackedBranch.FriendlyName,
        //     commonAncestor == branch.Tip
        // );
        console.WriteLine("~");

        // 0f3b17a4a origin/main
        // 268ff92ee main

        // var origin = repo.Lookup<Commit>("0f3b17a4a");
        // PrintCommit(origin, "origin/main", false);
        // var main = repo.Lookup<Commit>("268ff92ee");
        // PrintCommit(main, "main", false);


        // var mergeBase = repo.ObjectDatabase.FindMergeBase(origin, main);

        // PrintCommit(mergeBase, "merge-base", false);

        PrintCommit(originHead.Tip, "origin/HEAD", false);
        return Task.FromResult(0);
        // var remotes = repo.Branches.Where(b => b.IsRemote).ToList();
        //
        //
        //
        // var commits = from b in repo.Branches
        //     where !b.IsRemote
        //     select new
        //     {
        //         Branch = b,
        //         PossibleBases = remotes
        //             .Select(r => new { Base = repo.ObjectDatabase.FindMergeBase(r.Tip, b.Tip), Remote = r })
        //             .Where(r => r.Base != null)
        //             .ToList()
        //     };
        //
        // foreach (var commit in commits)
        // {
        //     console.WriteLine();
        //     console.WriteLine($"{commit.Branch.FriendlyName} ({commit.PossibleBases.Count})");
        //     foreach (var possibleBase in commit.PossibleBases)
        //     {
        //         PrintCommit(possibleBase.Base, possibleBase.Remote.FriendlyName, false);
        //     }
        // }
        // //
        //
        //
        // // .CalculateHistoryDivergence(branch.Tip, branch.TrackedBranch.Tip)
        //
        // // var commitsToProcess = new List<CommitTree>(tips);
        // // CommitTree? lastParent = null;
        // // while (commitsToProcess.Count > 0)
        // // {
        // //     // get first job
        // //     var tree = commitsToProcess[0];
        // //     commitsToProcess.RemoveAt(0);
        // //
        // //     // look for parent of first commit, if we dont find it, in the cache add it to the tree
        // //     var firstParent = tree.Commit.Parents.FirstOrDefault();
        // //     if (firstParent == null)
        // //         continue;
        // //
        // //     if (treeLookup.TryGetValue(firstParent, out var existing))
        // //     {
        // //         existing.Children.Add(tree);
        // //         lastParent = existing;
        // //     }
        // //     else
        // //     {
        // //         var parent = new CommitTree(firstParent, null, [tree]);
        // //         treeLookup[firstParent] = parent;
        // //         commitsToProcess.Add(parent);
        // //     }
        // // }
        //
        // // print the tree
        //
        //
        //
        //
        // // foreach (var branch in repo.Branches)
        // // {
        // //     // console.WriteLine($"{branch.FriendlyName}, {branch.Tip}");
        // //     var commit = branch.Tip;
        // //
        // //     var branchName = branch.FriendlyName;
        // //
        // //     PrintCommit(commit, branchName);
        // // }
        //
        // return Task.FromResult(0);
    }

    private void PrintCommit(Commit commit, string? branchName, bool selected)
    {
        var remote = string.IsNullOrEmpty(branchName) ? "" : $" [green]{branchName}[/]";
        var dateTime = commit.Author.When.ToLocalTime().LocalDateTime;
        var date =
            dateTime.Year == DateTime.Now.Year
                ? dateTime.ToString("MMM dd")
                : dateTime.ToString("yyyy MMM dd");

        var icon = selected ? "@" : "o";
        var startSelection = selected ? "[plum3]" : "";
        var endSelection = selected ? "[/]" : "";
        console.MarkupLine(
            $"{icon}  [yellow]{commit.Id.ToString(9)}[/] {startSelection}{date} at {dateTime:HH:mm} {Markup.Escape(commit.Author.Name)}{endSelection}{remote}"
        );
        console.MarkupLine(
            $"\u2502  {startSelection}{Markup.Escape(commit.MessageShort)}{endSelection}"
        );
    }
}

/**
 *


// inline

o  53a0165e0  Wednesday at 23:04  kevin
│  hi




│ o  53a0165e0  Wednesday at 23:04  kevin
│ │  hi


│ o  53a0165e0  Wednesday at 23:04  kevin
├─╯  hi

 *
 *
 *
 *
 */

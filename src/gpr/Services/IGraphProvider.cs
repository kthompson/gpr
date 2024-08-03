using GitPullRequest.Models;
using LibGit2Sharp;

namespace GitPullRequest.Services;

public interface IGraphProvider
{
    Graph<ObjectId, Commit> GetGraph();
}

public class GraphProvider(IRepository repository, IBottomProvider bottomProvider) : IGraphProvider
{
    public Graph<ObjectId, Commit> GetGraph()
    {
        var graph = new Graph<ObjectId, Commit>();
        var originHead = bottomProvider.GetBottom();

        // add base node
        graph.AddNode(originHead.Tip.Id, originHead.Tip, []);

        // add all the branches
        foreach (var branch in repository.Branches)
        {
            if (branch.IsRemote)
                continue;

            // make sure the branch has a common ancestor with the origin
            if (repository.ObjectDatabase.FindMergeBase(branch.Tip, originHead.Tip) == null)
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

        return graph;
    }
}

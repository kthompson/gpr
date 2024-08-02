using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;

namespace GitPullRequest.Models;

//
// public enum NodeType
// {
//     Leaf,
//     Branch,
//     Root,
// }
//
// public record Node(ObjectId Id, NodeType Type, string? Name, string Message, Signature Author);
// public record Link(ObjectId From, ObjectId To);
//
// public class Graph
// {
//     public List<Node> Nodes { get; } = [];
//     public Dictionary<Node, HashSet<Node>> Children { get; } = new();
//
//     public void AddNode(ObjectId id, NodeType type, string message, Signature author) =>
//         AddNode(id, type, null, message, author);
//
//     public void AddNode(ObjectId id, NodeType type, string? name, string message, Signature author) =>
//         Nodes.Add(new Node(id, type, name, message, author));
//
//     public void AddLink(ObjectId parent, ObjectId child)
//     {
//         var fromNode = Nodes.First(n => n.Id == parent);
//         var toNode = Nodes.First(n => n.Id == child);
//
//         if (!Children.TryGetValue(fromNode, out var value))
//         {
//             value = new();
//             Children[fromNode] = value;
//         }
//
//         value.Add(toNode);
//     }
// }



public class Graph<TKey, TData> : IEnumerable<KeyValuePair<TKey, TData>>
    where TKey : notnull
{
    public Graph() { }

    private Graph(
        Dictionary<TKey, TData> data,
        Dictionary<TKey, HashSet<TKey>> incomingEdges,
        Dictionary<TKey, HashSet<TKey>> outgoingEdges
    )
    {
        Data = data;
        IncomingEdges = incomingEdges;
        OutgoingEdges = outgoingEdges;
    }

    private Dictionary<TKey, TData> Data { get; } = new();

    private Dictionary<TKey, HashSet<TKey>> IncomingEdges { get; } = new();

    private Dictionary<TKey, HashSet<TKey>> OutgoingEdges { get; } = new();

    public int Count => OutgoingEdges.Count;

    public TData this[TKey key] => Data[key];

    /// <summary>
    /// Creates a shallow copy of the graph, referencing the same keys and data as the original.
    /// </summary>
    /// <returns>A shallow copy of the original.</returns>
    public Graph<TKey, TData> Clone() =>
        new(
            new Dictionary<TKey, TData>(Data),
            IncomingEdges.ToDictionary(kvp => kvp.Key, kvp => new HashSet<TKey>(kvp.Value)),
            OutgoingEdges.ToDictionary(kvp => kvp.Key, kvp => new HashSet<TKey>(kvp.Value))
        );

    /// <summary>
    /// Adds a node with the specified key, data, and outgoing edges to the graph.
    /// </summary>
    public void AddNode(TKey key, TData data, IList<TKey> outgoing)
    {
        if (OutgoingEdges.ContainsKey(key))
            throw new ArgumentException("Node with the provided key already exists.");
        if (CausesCycle(key, outgoing))
            throw new ArgumentException("Adding this node causes a cycle.");

        Data.Add(key, data);
        AddEdges(key, outgoing);
    }

    /// <summary>
    /// Removes a node and its outgoing edges.
    /// </summary>
    /// <param name="key"></param>
    public void RemoveNode(TKey key)
    {
        if (!OutgoingEdges.ContainsKey(key))
            throw new ArgumentException("Node does not exist.");

        Data.Remove(key);
        foreach (var edge in OutgoingEdges[key])
        {
            IncomingEdges[edge].Remove(key);
        }
        OutgoingEdges.Remove(key);
    }

    /// <summary>
    /// Removes all nodes that are not reachable from any of the given nodes.
    /// </summary>
    /// <param name="topKeys">Keys to keep.</param>
    public void Trim(IEnumerable<TKey> topKeys)
    {
        if (topKeys == null)
            throw new ArgumentNullException(nameof(topKeys));

        var unsearched = new Queue<TKey>(topKeys);
        var unconnected = new HashSet<TKey>(OutgoingEdges.Keys);
        while (unsearched.Count != 0)
        {
            var key = unsearched.Dequeue();
            if (!unconnected.Contains(key))
                continue; // We have already found this key.

            // We've found the key, so we can remove it from the unconnected list.
            unconnected.Remove(key);
            foreach (var outgoing in OutgoingEdges[key])
                unsearched.Enqueue(outgoing);
        }

        foreach (var key in unconnected)
        {
            RemoveNode(key);
        }
    }

    /// <summary>
    /// Checks if adding the node causes a cycle.
    /// </summary>
    private bool CausesCycle(TKey key, IList<TKey> outgoing)
    {
        if (outgoing.Contains(key))
            return true; // Self cycle

        if (!IncomingEdges.TryGetValue(key, out var incoming) || incoming.Count == 0)
            return false; // No incoming edges, so we can't have a cycle.

        // If a path exists from any outgoing edge to any incoming edge, adding the node will cause a cycle.
        return (
            from start in outgoing
            from end in incoming
            where PathExists(start, end)
            select start
        ).Any();
    }

    /// <summary>
    /// Checks if a path exists between the given start and end nodes.
    /// </summary>
    public bool PathExists(TKey start, TKey end)
    {
        var tested = new HashSet<TKey> { start };
        var queued = new Queue<TKey>(tested);

        while (queued.Count > 0)
        {
            var current = queued.Dequeue();
            if (current.Equals(end)) // A path exists
                return true;

            if (!OutgoingEdges.TryGetValue(current, out HashSet<TKey> destinations))
                continue; // No out edges for current

            foreach (var destination in destinations)
            {
                if (tested.Contains(destination))
                    continue;

                tested.Add(destination);
                queued.Enqueue(destination);
            }
        }

        return false;
    }

    public bool Contains(TKey key) => OutgoingEdges.ContainsKey(key);

    /// <summary>
    /// Gets the incoming edges to a key, regardless if the node has been added or not.
    /// </summary>
    public List<TKey> GetIncoming(TKey key)
    {
        if (!IncomingEdges.TryGetValue(key, out var edges))
            return new List<TKey>();
        return edges.ToList();
    }

    /// <summary>
    /// Gets the incoming eges to a key, regardless if the node has been added or not.
    /// </summary>
    public List<TKey> GetOutgoing(TKey key)
    {
        if (!OutgoingEdges.TryGetValue(key, out var edges))
            return new List<TKey>();
        return edges.ToList();
    }

    /// <summary>
    /// Returns the nodes topologically sorted into layers. Nodes with no outgoing edges are in the first layer,
    /// while nodes that only point to nodes in the first layer are in the second layer, and so on. Any node that
    /// points to a node that has not been added to the graph is considered detached.
    /// </summary>
    /// <returns>A tuple containing a list of layers and a list of detached keys.</returns>
    public (List<List<TKey>> layers, List<TKey> detached) TopologicalSort()
    {
        // This method could probably be optimized significantly.
        // Now that I know it's called a topological sort, could use Kahn's algorithm.
        // However, we'll still need to take into account detached nodes where the nodes they point
        // to don't actually exist.

        var layers = new List<List<TKey>> { new() };
        var detached = new List<TKey>();
        foreach (var kvp in OutgoingEdges)
        {
            var key = kvp.Key;
            var destinations = OutgoingEdges[key];
            if (OutgoingEdges[key].Count == 0)
                layers[0].Add(key); // If a key has no outgoing edges, it's added to the first layer
            else if (destinations.Any(dest => !OutgoingEdges.ContainsKey(dest)))
                detached.Add(key); // If a key has any outgoing edges that are not in the graph, it is considered detached.
        }

        var satisfiedKeys = new HashSet<TKey>(layers[0]);
        var unsatisfiedKeys = new HashSet<TKey>();

        while (layers[^1].Count > 0)
        {
            var candidates = layers[^1]
                .SelectMany(previous =>
                    IncomingEdges.TryGetValue(previous, out var edge) ? edge : new HashSet<TKey>()
                )
                .Where(key => OutgoingEdges.ContainsKey(key))
                .Concat(unsatisfiedKeys)
                .Distinct();

            unsatisfiedKeys.Clear();

            var currentLevel = new List<TKey>();
            foreach (var candidate in candidates)
            {
                var satisfied = true;
                foreach (var outgoing in OutgoingEdges[candidate])
                {
                    // Check if each of the outgoing keys have been set already.
                    if (satisfiedKeys.Contains(outgoing))
                        continue;

                    // If not, then the candidate gets bumped up to the next level.
                    satisfied = false;
                    break;
                }

                if (!satisfied)
                {
                    unsatisfiedKeys.Add(candidate);
                    continue;
                }

                currentLevel.Add(candidate);
            }

            layers.Add(currentLevel);
            foreach (var key in currentLevel)
                satisfiedKeys.Add(key);
        }

        layers.RemoveAt(layers.Count - 1);
        detached.AddRange(unsatisfiedKeys);
        return (layers, detached);
    }

    private void AddEdges(TKey key, IList<TKey> outgoing)
    {
        OutgoingEdges.Add(key, [.. outgoing]);
        foreach (var dest in outgoing)
        {
            if (!IncomingEdges.TryGetValue(dest, out var incoming))
                IncomingEdges[dest] = [key];
            else
                incoming.Add(key);
        }
    }

    public IEnumerator<KeyValuePair<TKey, TData>> GetEnumerator() => Data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

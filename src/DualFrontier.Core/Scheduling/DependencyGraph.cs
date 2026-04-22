using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Core.ECS;

namespace DualFrontier.Core.Scheduling;

/// <summary>
/// System-dependency graph built from <c>[SystemAccess]</c> declarations.
/// An edge <c>A → B</c> means "B depends on A" — B must run after A because
/// B reads a component that A writes. <see cref="Build"/> performs Kahn's
/// topological sort and groups mutually independent systems into
/// <see cref="SystemPhase"/> instances consumable by
/// <c>ParallelSystemScheduler</c>.
///
/// Write-write collisions on the same component type are reported as errors
/// at <see cref="Build"/> time. Cycles are detected after the topological
/// sort drains and are likewise reported as errors.
/// Diagnostic messages follow the format documented in <c>docs/THREADING.md</c>.
/// </summary>
internal sealed class DependencyGraph
{
    private readonly List<SystemBase> _systems = new();
    private readonly Dictionary<SystemBase, HashSet<SystemBase>> _edges = new();
    private readonly List<SystemPhase> _phases = new();
    private bool _built;

    /// <summary>
    /// Registers a system with the graph. Verifies the concrete type carries
    /// <see cref="SystemAccessAttribute"/> and is not already registered.
    /// Must be called before <see cref="Build"/>.
    /// </summary>
    public void AddSystem(SystemBase system)
    {
        if (system is null)
            throw new ArgumentNullException(nameof(system));
        if (_built)
            throw new InvalidOperationException(
                "graph already built — call Reset() to rebuild");

        Type systemType = system.GetType();
        foreach (SystemBase existing in _systems)
        {
            if (existing.GetType() == systemType)
                throw new InvalidOperationException(
                    $"duplicate system: {systemType.FullName}");
        }

        SystemAccessAttribute? access =
            systemType.GetCustomAttribute<SystemAccessAttribute>(inherit: false);
        if (access is null)
        {
            throw new InvalidOperationException(
                $"[SCHEDULER ERROR] System '{systemType.FullName}' has no [SystemAccess] attribute.{Environment.NewLine}" +
                "Add: [SystemAccess(reads: new[]{typeof(...)}, writes: new[]{typeof(...)}, bus: nameof(IGameServices.X))]");
        }

        _systems.Add(system);
        _edges[system] = new HashSet<SystemBase>();
    }

    /// <summary>
    /// Builds the graph: reads every <c>[SystemAccess]</c> declaration,
    /// detects write-write conflicts, creates write-to-read edges, and runs
    /// Kahn's topological sort into ordered phases. Throws
    /// <see cref="InvalidOperationException"/> with a formatted diagnostic on
    /// write conflicts or cycles. Must be called exactly once after all
    /// systems have been registered via <see cref="AddSystem"/>.
    /// </summary>
    public void Build()
    {
        if (_built)
            throw new InvalidOperationException(
                "graph already built — call Reset() to rebuild");

        var reads = new Dictionary<SystemBase, HashSet<Type>>(_systems.Count);
        var writes = new Dictionary<SystemBase, HashSet<Type>>(_systems.Count);
        foreach (SystemBase system in _systems)
        {
            SystemAccessAttribute access = system.GetType()
                .GetCustomAttribute<SystemAccessAttribute>(inherit: false)!;

            var readSet = new HashSet<Type>();
            foreach (Type t in access.Reads)
                readSet.Add(t);

            var writeSet = new HashSet<Type>();
            foreach (Type t in access.Writes)
                writeSet.Add(t);

            reads[system] = readSet;
            writes[system] = writeSet;
        }

        for (int i = 0; i < _systems.Count; i++)
        {
            SystemBase a = _systems[i];
            HashSet<Type> writesA = writes[a];
            if (writesA.Count == 0)
                continue;

            for (int j = i + 1; j < _systems.Count; j++)
            {
                SystemBase b = _systems[j];
                HashSet<Type> writesB = writes[b];
                if (writesB.Count == 0)
                    continue;

                Type? shared = FindIntersection(writesA, writesB);
                if (shared is not null)
                    throw BuildWriteConflictException(a, b, shared);
            }
        }

        foreach (SystemBase a in _systems)
        {
            HashSet<Type> writesA = writes[a];
            if (writesA.Count == 0)
                continue;

            foreach (SystemBase b in _systems)
            {
                if (ReferenceEquals(a, b))
                    continue;

                HashSet<Type> readsB = reads[b];
                if (readsB.Count == 0)
                    continue;

                if (HasIntersection(writesA, readsB))
                    _edges[a].Add(b);
            }
        }

        var inDegree = new Dictionary<SystemBase, int>(_systems.Count);
        foreach (SystemBase s in _systems)
            inDegree[s] = 0;
        foreach (KeyValuePair<SystemBase, HashSet<SystemBase>> edge in _edges)
        {
            foreach (SystemBase target in edge.Value)
                inDegree[target]++;
        }

        while (true)
        {
            var ready = new List<SystemBase>();
            foreach (KeyValuePair<SystemBase, int> pair in inDegree)
            {
                if (pair.Value == 0)
                    ready.Add(pair.Key);
            }

            if (ready.Count == 0)
                break;

            _phases.Add(new SystemPhase(ready));

            foreach (SystemBase s in ready)
            {
                inDegree.Remove(s);
                foreach (SystemBase target in _edges[s])
                {
                    if (inDegree.ContainsKey(target))
                        inDegree[target]--;
                }
            }
        }

        if (inDegree.Count > 0)
            throw BuildCycleException(inDegree);

        _built = true;
    }

    /// <summary>
    /// Returns the phases produced by <see cref="Build"/>, in execution order.
    /// Throws <see cref="InvalidOperationException"/> if <see cref="Build"/>
    /// has not yet been called.
    /// </summary>
    public IReadOnlyList<SystemPhase> GetPhases()
    {
        if (!_built)
            throw new InvalidOperationException("call Build() first");
        return _phases;
    }

    /// <summary>
    /// Clears all registered systems, edges, and phases. Allows the graph to
    /// be repopulated and rebuilt. Used by tests and future mod hot-reload.
    /// </summary>
    public void Reset()
    {
        _systems.Clear();
        _edges.Clear();
        _phases.Clear();
        _built = false;
    }

    private static bool HasIntersection(HashSet<Type> a, HashSet<Type> b)
    {
        HashSet<Type> smaller = a.Count <= b.Count ? a : b;
        HashSet<Type> larger = ReferenceEquals(smaller, a) ? b : a;
        foreach (Type t in smaller)
        {
            if (larger.Contains(t))
                return true;
        }
        return false;
    }

    private static Type? FindIntersection(HashSet<Type> a, HashSet<Type> b)
    {
        HashSet<Type> smaller = a.Count <= b.Count ? a : b;
        HashSet<Type> larger = ReferenceEquals(smaller, a) ? b : a;
        foreach (Type t in smaller)
        {
            if (larger.Contains(t))
                return t;
        }
        return null;
    }

    private static InvalidOperationException BuildWriteConflictException(
        SystemBase a, SystemBase b, Type component)
    {
        var sb = new StringBuilder();
        sb.Append("[SCHEDULER ERROR] Write conflict:").Append(Environment.NewLine);
        sb.Append("  ").Append(a.GetType().FullName)
          .Append(" writes ").Append(component.FullName).Append(Environment.NewLine);
        sb.Append("  ").Append(b.GetType().FullName)
          .Append(" writes ").Append(component.FullName).Append(Environment.NewLine);
        sb.Append("Resolve: одна из систем должна читать, не писать, или разнести по фазам через [Deferred] событие.");
        return new InvalidOperationException(sb.ToString());
    }

    private InvalidOperationException BuildCycleException(
        Dictionary<SystemBase, int> remaining)
    {
        var sb = new StringBuilder();
        sb.Append("[SCHEDULER ERROR] Cyclic dependency detected:").Append(Environment.NewLine);
        foreach (KeyValuePair<SystemBase, int> pair in remaining)
        {
            SystemBase source = pair.Key;
            foreach (SystemBase target in _edges[source])
            {
                if (!remaining.ContainsKey(target))
                    continue;
                sb.Append("  ")
                  .Append(source.GetType().FullName)
                  .Append(" → ")
                  .Append(target.GetType().FullName)
                  .Append(Environment.NewLine);
            }
        }
        sb.Append("Resolve: разорви цикл через [Deferred] событие.");
        return new InvalidOperationException(sb.ToString());
    }
}

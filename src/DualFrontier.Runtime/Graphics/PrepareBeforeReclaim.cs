namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// All-or-nothing rebuild of a resource list -- the ELT §2.5 <c>prepare</c> step under
/// ELT §1.1 corollary 1 ("reclaim MUST NOT begin before commit succeeds"). Builds the new
/// resources alongside the old so the caller can COMMIT them in one infallible assignment
/// and only then RECLAIM the old set. Device-independent: the concrete resource creation and
/// teardown are supplied as delegates, so the transaction ordering is unit-testable without a GPU.
/// </summary>
internal static class PrepareBeforeReclaim
{
    /// <summary>
    /// Builds a candidate array from <paramref name="sources"/> via <paramref name="build"/>.
    /// If any build throws, every resource built so far IN THIS CALL is rolled back via
    /// <paramref name="reclaim"/> (in construction order) and the original exception propagates
    /// with NO array returned -- so a caller's commit step is unreachable and the caller's OLD
    /// state is never touched. On full success returns the new array; the caller commits it, then
    /// reclaims its own old state.
    /// </summary>
    public static T[] Build<TSource, T>(
        IReadOnlyList<TSource> sources,
        Func<TSource, T> build,
        Action<T> reclaim)
    {
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(build);
        ArgumentNullException.ThrowIfNull(reclaim);

        var built = new T[sources.Count];
        int i = 0;
        try
        {
            for (; i < sources.Count; i++)
            {
                built[i] = build(sources[i]);
            }
        }
        catch
        {
            // Roll back only what THIS call built (indices 0..i-1); index i never completed.
            for (int j = 0; j < i; j++)
            {
                reclaim(built[j]);
            }
            throw;
        }
        return built;
    }
}

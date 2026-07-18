namespace DualFrontier.Core.Interop;

/// <summary>
/// Outcome of <see cref="NativeWorld.DisposeChecked"/> — the checked world
/// teardown (native <c>df_world_destroy_checked</c>; EQ_A3 / RESOURCE_OWNERSHIP_AND_LIFETIME
/// §6.2 wait-for-zero; К-L20). On a WORLD_BUSY refusal <see cref="Destroyed"/> is
/// <see langword="false"/> and the world is left intact, with
/// <see cref="ActiveSpans"/> / <see cref="ActiveBatches"/> reporting the live
/// borrow counts that blocked teardown.
/// </summary>
public readonly record struct WorldTeardownResult(bool Destroyed, int ActiveSpans, int ActiveBatches);

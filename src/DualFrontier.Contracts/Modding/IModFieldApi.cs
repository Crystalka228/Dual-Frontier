namespace DualFrontier.Contracts.Modding;

/// <summary>
/// Field-storage sub-API per <c>MOD_OS_ARCHITECTURE.md</c> v1.7 §4.6. Mods
/// register named dense 2D grids (one cell type per field) and access them
/// via point read/write or zero-copy span lease. The K9 brief defines the
/// field storage contract (see <c>docs/architecture/FIELDS.md</c>).
/// </summary>
/// <remarks>
/// Capability gating happens at the boundary: every <see cref="RegisterField{T}"/>
/// or <see cref="GetField{T}"/> call validates the mod's manifest against
/// the required <c>mod.&lt;provider&gt;.field.&lt;verb&gt;:&lt;field-id&gt;</c>
/// token before forwarding to the underlying registry. Per-cell reads and
/// writes through the returned handle are not re-checked — the handle was
/// already capability-gated at acquisition.
///
/// Returns are typed as <see cref="IFieldHandle"/> rather than
/// <c>FieldHandle&lt;T&gt;</c> because the concrete type lives in
/// <c>DualFrontier.Core.Interop</c>, which already references Contracts.
/// Modders downcast at the call site.
/// </remarks>
public interface IModFieldApi
{
    /// <summary>
    /// Registers a new field in the calling mod's namespace.
    /// </summary>
    /// <typeparam name="T">Cell element type. Must be unmanaged (blittable).</typeparam>
    /// <param name="id">
    /// Field id. Must start with the mod's id followed by a dot (e.g.
    /// <c>"vanilla.magic.mana"</c> for mod <c>vanilla.magic</c>).
    /// </param>
    /// <param name="width">Grid width (positive).</param>
    /// <param name="height">Grid height (positive).</param>
    IFieldHandle RegisterField<T>(string id, int width, int height) where T : unmanaged;

    /// <summary>
    /// Retrieves a previously registered field. Cross-mod access requires
    /// a <c>field.read</c> capability declared against the foreign mod's
    /// namespace; own-namespace access requires <c>field.read</c> against
    /// the calling mod's own namespace.
    /// </summary>
    IFieldHandle GetField<T>(string id) where T : unmanaged;

    /// <summary>
    /// Returns <c>true</c> if a field with the given id is currently registered.
    /// Does not require any capability.
    /// </summary>
    bool IsRegistered(string id);
}

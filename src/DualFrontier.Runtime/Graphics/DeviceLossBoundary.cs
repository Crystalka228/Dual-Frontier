namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// The render-loop device-loss boundary (M9 / D1; resolves ELT OQ-3 as fail-fast v1 -- the ELT §4
/// device-lost class "recover" limb). Runs one frame's work and, if it surfaces a
/// <see cref="DeviceLostException"/>, composes the structured <see cref="DeviceLostDiagnostic"/>
/// (adding the loop-owned frame index) and routes it to a deliberate fail-fast. Production constructs
/// it with a null hook (real <see cref="Environment.FailFast(string, Exception)"/>); tests inject a
/// recorder to observe the route WITHOUT terminating the host -- mirroring EngineSession's
/// ShutdownTransactionHooks.OnAbort. No recovery in v1: device re-creation is future work with its own
/// design -- a half-recovery here would be a kostyl.
/// </summary>
public sealed class DeviceLossBoundary
{
    private readonly Action<DeviceLostDiagnostic>? _onDeviceLost;

    /// <param name="onDeviceLost">
    /// Test seam: replaces the default fail-fast so a device-loss route is observable without killing
    /// the host. Production passes <see langword="null"/> (fail-fast).
    /// </param>
    public DeviceLossBoundary(Action<DeviceLostDiagnostic>? onDeviceLost = null)
    {
        _onDeviceLost = onDeviceLost;
    }

    /// <summary>
    /// Runs <paramref name="frame"/>. A <see cref="DeviceLostException"/> it throws is converted to a
    /// structured diagnostic (with <paramref name="frameIndex"/>) and fail-fasted (or handed to the
    /// injected recorder). Any other exception propagates unchanged.
    /// </summary>
    public void RunGuarded(long frameIndex, Action frame)
    {
        ArgumentNullException.ThrowIfNull(frame);
        try
        {
            frame();
        }
        catch (DeviceLostException ex)
        {
            var diagnostic = new DeviceLostDiagnostic(ex.Context, frameIndex);
            if (_onDeviceLost is { } onLost)
            {
                onLost(diagnostic);
                return;
            }

            // Default (D1): fail-fast with the structured diagnostic. Environment.FailFast routes the
            // message to WER / the event log and terminates -- never console output, so the src
            // console-write census stays pinned. Matches the EngineSession.Abort precedent (К-L20).
            Environment.FailFast(diagnostic.Describe(), ex);
        }
    }
}

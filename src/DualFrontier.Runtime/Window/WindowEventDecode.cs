namespace DualFrontier.Runtime.Window;

/// <summary>
/// Pure decode helpers shared by every windowing backend. No OS handle, no state, no side
/// effect — so the resize law below is testable without a live window on any platform, which
/// is what lets its pins run ungated.
/// </summary>
internal static class WindowEventDecode
{
    /// <summary>
    /// The resize law, stated once for all backends: a reported size is accepted only when it
    /// is non-degenerate AND actually different from what the window already believes.
    ///
    /// <para>Both halves are load-bearing. A 0×0 report is what minimize produces (Win32
    /// WM_SIZE, and an XCB CONFIGURE_NOTIFY on an unmapped window), and treating it as a real
    /// size would drive a zero-extent swapchain. An unchanged report is noise — X11 in
    /// particular sends CONFIGURE_NOTIFY for moves as well as resizes — and letting it through
    /// would publish a resize event per window drag.</para>
    /// </summary>
    /// <returns>True when the caller should adopt <paramref name="newWidth"/>/<paramref name="newHeight"/>
    /// and publish a resize; false when the report must be ignored.</returns>
    internal static bool TrySize(
        int candidateWidth, int candidateHeight,
        int currentWidth, int currentHeight,
        out int newWidth, out int newHeight)
    {
        if (candidateWidth > 0 && candidateHeight > 0
            && (candidateWidth != currentWidth || candidateHeight != currentHeight))
        {
            newWidth = candidateWidth;
            newHeight = candidateHeight;
            return true;
        }

        newWidth = currentWidth;
        newHeight = currentHeight;
        return false;
    }

    /// <summary>
    /// Unpacks a Win32 <c>WM_SIZE</c> <c>lParam</c>: LOWORD = new client width, HIWORD = new
    /// client height (Win32 docs). Unsigned — a client area is never negative, unlike the
    /// cursor coordinates in <c>WM_MOUSEMOVE</c>, which do need the signed short cast.
    /// </summary>
    internal static void UnpackSizeLParam(long lParam, out int width, out int height)
    {
        width = (int)(lParam & 0xFFFF);
        height = (int)((lParam >> 16) & 0xFFFF);
    }
}

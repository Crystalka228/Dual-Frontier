using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Native.Win32;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct WNDCLASSEX
{
    internal uint cbSize;
    internal uint style;
    internal IntPtr lpfnWndProc;
    internal int cbClsExtra;
    internal int cbWndExtra;
    internal IntPtr hInstance;
    internal IntPtr hIcon;
    internal IntPtr hCursor;
    internal IntPtr hbrBackground;
    [MarshalAs(UnmanagedType.LPWStr)]
    internal string? lpszMenuName;
    [MarshalAs(UnmanagedType.LPWStr)]
    internal string lpszClassName;
    internal IntPtr hIconSm;
}

[StructLayout(LayoutKind.Sequential)]
internal struct MSG
{
    internal IntPtr hwnd;
    internal uint message;
    internal IntPtr wParam;
    internal IntPtr lParam;
    internal uint time;
    internal POINT pt;
    internal uint lPrivate;
}

[StructLayout(LayoutKind.Sequential)]
internal struct POINT
{
    internal int x;
    internal int y;
}

[StructLayout(LayoutKind.Sequential)]
internal struct RECT
{
    internal int left;
    internal int top;
    internal int right;
    internal int bottom;
}

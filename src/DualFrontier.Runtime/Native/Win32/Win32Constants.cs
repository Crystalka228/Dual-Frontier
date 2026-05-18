namespace DualFrontier.Runtime.Native.Win32;

internal static class Win32Constants
{
    // Window messages — see Microsoft Win32 docs
    internal const uint WM_CREATE = 0x0001;
    internal const uint WM_DESTROY = 0x0002;
    internal const uint WM_SIZE = 0x0005;
    internal const uint WM_SETFOCUS = 0x0007;
    internal const uint WM_KILLFOCUS = 0x0008;
    internal const uint WM_CLOSE = 0x0010;
    internal const uint WM_QUIT = 0x0012;
    internal const uint WM_KEYDOWN = 0x0100;
    internal const uint WM_KEYUP = 0x0101;
    internal const uint WM_MOUSEMOVE = 0x0200;
    internal const uint WM_LBUTTONDOWN = 0x0201;
    internal const uint WM_LBUTTONUP = 0x0202;

    // Window styles
    internal const uint WS_OVERLAPPEDWINDOW = 0x00CF0000;
    internal const uint WS_VISIBLE = 0x10000000;

    // Class styles
    internal const uint CS_OWNDC = 0x0020;
    internal const uint CS_HREDRAW = 0x0002;
    internal const uint CS_VREDRAW = 0x0001;

    // Cursor + icon defaults (MAKEINTRESOURCE values)
    internal const int IDC_ARROW = 32512;
    internal const int IDI_APPLICATION = 32512;

    // ShowWindow nCmdShow values
    internal const int SW_HIDE = 0;
    internal const int SW_SHOW = 5;

    // PeekMessage flags
    internal const uint PM_NOREMOVE = 0x0000;
    internal const uint PM_REMOVE = 0x0001;

    // CreateWindowEx default position/size — CW_USEDEFAULT
    internal const int CW_USEDEFAULT = unchecked((int)0x80000000);
}

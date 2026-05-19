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
    internal const uint WM_SYSKEYDOWN = 0x0104;
    internal const uint WM_SYSKEYUP = 0x0105;
    internal const uint WM_MOUSEMOVE = 0x0200;
    internal const uint WM_LBUTTONDOWN = 0x0201;
    internal const uint WM_LBUTTONUP = 0x0202;
    internal const uint WM_RBUTTONDOWN = 0x0204;
    internal const uint WM_RBUTTONUP = 0x0205;
    internal const uint WM_MBUTTONDOWN = 0x0207;
    internal const uint WM_MBUTTONUP = 0x0208;
    internal const uint WM_MOUSEWHEEL = 0x020A;

    // Virtual key codes — see Microsoft Win32 docs (Keyboard Input → Virtual-Key Codes)
    internal const int VK_BACK = 0x08;
    internal const int VK_TAB = 0x09;
    internal const int VK_RETURN = 0x0D;
    internal const int VK_SHIFT = 0x10;
    internal const int VK_CONTROL = 0x11;
    internal const int VK_MENU = 0x12;  // Alt
    internal const int VK_ESCAPE = 0x1B;
    internal const int VK_SPACE = 0x20;
    internal const int VK_PRIOR = 0x21;  // PageUp
    internal const int VK_NEXT = 0x22;   // PageDown
    internal const int VK_END = 0x23;
    internal const int VK_HOME = 0x24;
    internal const int VK_LEFT = 0x25;
    internal const int VK_UP = 0x26;
    internal const int VK_RIGHT = 0x27;
    internal const int VK_DOWN = 0x28;
    internal const int VK_DELETE = 0x2E;
    internal const int VK_F1 = 0x70;
    internal const int VK_F2 = 0x71;
    internal const int VK_F3 = 0x72;
    internal const int VK_F4 = 0x73;
    internal const int VK_F5 = 0x74;
    internal const int VK_F6 = 0x75;
    internal const int VK_F7 = 0x76;
    internal const int VK_F8 = 0x77;
    internal const int VK_F9 = 0x78;
    internal const int VK_F10 = 0x79;
    internal const int VK_F11 = 0x7A;
    internal const int VK_F12 = 0x7B;

    // WHEEL_DELTA — Win32 documents standard mouse wheel scroll = 120 units per notch.
    internal const int WHEEL_DELTA = 120;

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

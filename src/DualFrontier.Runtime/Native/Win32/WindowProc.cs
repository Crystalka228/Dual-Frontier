using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Native.Win32;

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal delegate IntPtr WindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

using System;
using System.Runtime.InteropServices;

namespace Windows
{
    internal static class User32
    {
        [DllImport("user32.dll")]
        public static extern bool PeekMessage(out Msg lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

        [DllImport("user32.dll")]
        public static extern bool TranslateMessage([In] ref Msg lpMsg);

        [DllImport("user32.dll")]
        public static extern IntPtr DispatchMessage([In] ref Msg lpmsg);
    }
}

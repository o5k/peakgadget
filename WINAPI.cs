using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace PeakGadget {
    class WINAPI {
        public static IntPtr[] SearchForWindow() {
            SearchData sd = new SearchData();
            EnumWindows(new EnumWindowsProc(EnumProc), ref sd);
            return sd.hWnds.ToArray();
        }

        public static bool EnumProc(IntPtr hWnd, ref SearchData data) {
            data.hWnds.Add(hWnd);
            return true;
        }

        public class SearchData {
            public List<IntPtr> hWnds = new List<IntPtr>();
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, ref SearchData data);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, ref SearchData data);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        public static string GetWindowProcessName(IntPtr hWnd) {
            uint pid = 0;
            GetWindowThreadProcessId(hWnd, out pid);

            try {
                return Process.GetProcessById((int)pid).MainModule.FileName;
            } catch {
                return null;
            }
        }

        public static string GetWindowClassName(IntPtr hWnd) {
            StringBuilder cname = new StringBuilder(1024);
            GetClassName((IntPtr)hWnd, cname, cname.Capacity);

            return cname.ToString();
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [Flags()]
        public enum SetWindowPosFlags : uint {
            AsynchronousWindowPosition = 0x4000,
            DeferErase = 0x2000,
            DrawFrame = 0x0020,
            FrameChanged = 0x0020,
            HideWindow = 0x0080,
            DoNotActivate = 0x0010,
            DoNotCopyBits = 0x0100,
            IgnoreMove = 0x0002,
            DoNotChangeOwnerZOrder = 0x0200,
            DoNotRedraw = 0x0008,
            DoNotReposition = 0x0200,
            DoNotSendChangingEvent = 0x0400,
            IgnoreResize = 0x0001,
            IgnoreZOrder = 0x0004,
            ShowWindow = 0x0040,
        }

        public static bool MakeWindowTopmost(IntPtr hWnd) {
            return SetWindowPos(hWnd, (IntPtr)(-1), 0, 0, 0, 0, SetWindowPosFlags.IgnoreMove | SetWindowPosFlags.IgnoreResize | SetWindowPosFlags.DoNotRedraw);
        }
    }
}

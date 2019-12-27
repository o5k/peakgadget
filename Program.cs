using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PeakGadget {
    class Program {
        const string TARGET_PROCESS = @"\windows sidebar\sidebar.exe";
        const string TARGET_CLASS = "BasicWindow";
        static void Main() {
            while (true) {
                IntPtr[] hWnds = WINAPI.SearchForWindow();

                foreach (IntPtr hWnd in hWnds) {
                    if (WINAPI.GetWindowClassName(hWnd) == TARGET_CLASS
                        && WINAPI.GetWindowProcessName(hWnd).ToLower().EndsWith(TARGET_PROCESS)) {
                        // make it topmost
                        WINAPI.MakeWindowTopmost(hWnd);
                    }
                }
                Thread.Sleep(250);
            }
        }
    }
}

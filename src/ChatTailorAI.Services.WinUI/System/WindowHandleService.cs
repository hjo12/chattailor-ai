using ChatTailorAI.Shared.Services.Common;
using System;

namespace ChatTailorAI.Services.WinUI.System
{
    public class WindowHandleService : IWindowHandleService
    {
        private IntPtr _windowHandle = IntPtr.Zero;

        public void SetWindowHandle(IntPtr handle)
        {
            _windowHandle = handle;
        }

        public IntPtr GetWindowHandle() => _windowHandle;
    }
}
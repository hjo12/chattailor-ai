using System;

namespace ChatTailorAI.Shared.Services.Common
{
    public interface IWindowHandleService
    {
        IntPtr GetWindowHandle();
        void SetWindowHandle(IntPtr handle);
    }
}

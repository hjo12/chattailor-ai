using Microsoft.UI.Xaml;
using Microsoft.UI;
using System;
using WinRT.Interop;
using Microsoft.UI.Windowing;

namespace ChatTailorAI.WinUI.Helpers
{
    public static class TitleBarHelper
    {
        public static void ExpandViewIntoTitleBar(Window window, FrameworkElement titleBar)
        {
            IntPtr hwnd = WindowNative.GetWindowHandle(window);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            if (appWindow != null)
            {
                appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            }

            // Set the drag regions for the custom title bar (if needed)
            window.SetTitleBar(titleBar);
        }

        public static void SetupTitleBar(Window window)
        {
            IntPtr hwnd = WindowNative.GetWindowHandle(window);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            if (appWindow != null)
            {
                var titleBar = appWindow.TitleBar;

                // Active window colors
                titleBar.ForegroundColor = Microsoft.UI.Colors.White;
                titleBar.BackgroundColor = Microsoft.UI.Colors.Transparent;
                titleBar.ButtonForegroundColor = Microsoft.UI.Colors.White;
                titleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
                titleBar.ButtonHoverForegroundColor = Microsoft.UI.Colors.White;
                titleBar.ButtonHoverBackgroundColor = Microsoft.UI.Colors.SlateGray;
                titleBar.ButtonPressedForegroundColor = Microsoft.UI.Colors.White;
                titleBar.ButtonPressedBackgroundColor = Microsoft.UI.Colors.DimGray;

                // Inactive window colors
                titleBar.InactiveForegroundColor = Microsoft.UI.Colors.Gray;
                titleBar.InactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Microsoft.UI.Colors.Gray;
                titleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
            }
        }
    }
}

using Microsoft.UI.Windowing;
using Windows.Graphics;
using ChatTailorAI.Shared.Services.Common;
using Microsoft.UI;
using System;

namespace ChatTailorAI.Services.WinUI.System
{
    /// <summary>
    /// Service to manage the window state
    /// </summary>
    public class WindowManagerService : IWindowManagerService
    {
        private readonly IWindowHandleService _windowHandleService;
        private bool isCompactOverlayMode = false;

        public WindowManagerService(IWindowHandleService windowHandleService)
        {
            _windowHandleService = windowHandleService;
        }

        public void ToggleCompactOverlayMode()
        {
            var hWnd = _windowHandleService.GetWindowHandle();
            if (hWnd == IntPtr.Zero)
            {
                throw new InvalidOperationException("Window handle is not set. Ensure SetWindowHandle is called.");
            }

            var appWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(hWnd));
            isCompactOverlayMode = !isCompactOverlayMode;

            if (isCompactOverlayMode)
            {
                // Enter CompactOverlay-like mode using OverlappedPresenter
                if (appWindow.Presenter is OverlappedPresenter presenter)
                {
                    presenter.IsResizable = true;  // Allow resizing
                    presenter.IsAlwaysOnTop = true;  // Mimic CompactOverlay behavior
                }

                // Resize to larger CompactOverlay-like dimensions
                var newSize = new SizeInt32(500, 500);  // New dimensions for the compact mode window
                appWindow.Resize(newSize);

                // Get the display area of the current monitor
                var displayArea = DisplayArea.GetFromWindowId(appWindow.Id, DisplayAreaFallback.Primary);

                // Adjust position to ensure the window stays at the top-right corner of the current monitor
                var x = Math.Max(displayArea.WorkArea.Width - newSize.Width - 10, displayArea.WorkArea.X); // Prevent moving off-screen
                var y = displayArea.WorkArea.Y + 10;  // 10px margin from the top
                appWindow.Move(new PointInt32(x, y));
            }
            else
            {
                // Exit CompactOverlay-like mode
                if (appWindow.Presenter is OverlappedPresenter presenter)
                {
                    presenter.IsResizable = true;  // Allow resizing in default mode
                    presenter.IsAlwaysOnTop = false;  // Disable always-on-top
                }

                // Resize to default dimensions
                var newSize = new SizeInt32(800, 600);
                appWindow.Resize(newSize);

                // Get the current display area
                var displayArea = DisplayArea.GetFromWindowId(appWindow.Id, DisplayAreaFallback.Primary);

                // Reposition the window to avoid splitting between monitors
                var x = Math.Max(displayArea.WorkArea.X, displayArea.WorkArea.X + (displayArea.WorkArea.Width - newSize.Width) / 2); // Center on the monitor
                var y = Math.Max(displayArea.WorkArea.Y, displayArea.WorkArea.Y + (displayArea.WorkArea.Height - newSize.Height) / 2); // Center vertically
                appWindow.Move(new PointInt32(x, y));

            }
        }

        public bool IsCompactOverlayMode()
        {
            return isCompactOverlayMode;
        }
    }
}

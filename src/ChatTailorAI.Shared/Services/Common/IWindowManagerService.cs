using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTailorAI.Shared.Services.Common
{
    public interface IWindowManagerService
    {
        void ToggleCompactOverlayMode();
        bool IsCompactOverlayMode();
    }
}

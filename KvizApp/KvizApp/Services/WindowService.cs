#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
#endif

namespace KvizApp.Services
{

    public interface IWindowService
    {
        void SetWindowSize(double width, double height);
    }

}

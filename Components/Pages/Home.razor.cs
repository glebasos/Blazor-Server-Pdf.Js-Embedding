using Microsoft.JSInterop;
using PdfSharpTest.Services;

namespace PdfSharpTest.Components.Pages
{
    public partial class Home
    {
        private int selectedTabIndex = 0;
        private void OnTabChanged(int newIndex)
        {
            selectedTabIndex = newIndex;
            InvokeAsync(StateHasChanged);
        }
    }
}

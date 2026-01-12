using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;

namespace NitroHttp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

#if WINDOWS
            EntryHandler.Mapper.AppendToMapping("NoBorder", (handler, view) =>
            {
                var textBox = handler.PlatformView;
                textBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                textBox.FocusVisualPrimaryThickness = new Microsoft.UI.Xaml.Thickness(0);
                textBox.FocusVisualSecondaryThickness = new Microsoft.UI.Xaml.Thickness(0);
                
                textBox.Resources["TextControlBorderThemeThickness"] = new Microsoft.UI.Xaml.Thickness(0);
                textBox.Resources["TextControlBorderThemeThicknessFocused"] = new Microsoft.UI.Xaml.Thickness(0);
            });

            EditorHandler.Mapper.AppendToMapping("NoBorder", (handler, view) =>
            {
                var textBox = handler.PlatformView;
                textBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                textBox.FocusVisualPrimaryThickness = new Microsoft.UI.Xaml.Thickness(0);
                textBox.FocusVisualSecondaryThickness = new Microsoft.UI.Xaml.Thickness(0);
                
                textBox.Resources["TextControlBorderThemeThickness"] = new Microsoft.UI.Xaml.Thickness(0);
                textBox.Resources["TextControlBorderThemeThicknessFocused"] = new Microsoft.UI.Xaml.Thickness(0);
            });

            PickerHandler.Mapper.AppendToMapping("NoBorder", (handler, view) =>
            {
                var comboBox = handler.PlatformView;
                comboBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
                comboBox.FocusVisualPrimaryThickness = new Microsoft.UI.Xaml.Thickness(0);
                comboBox.FocusVisualSecondaryThickness = new Microsoft.UI.Xaml.Thickness(0);
                
                comboBox.Resources["ComboBoxBorderThemeThickness"] = new Microsoft.UI.Xaml.Thickness(0);
                comboBox.Resources["ComboBoxBorderThemeThicknessFocused"] = new Microsoft.UI.Xaml.Thickness(0);
            });
#endif

            return builder.Build();
        }
    }
}

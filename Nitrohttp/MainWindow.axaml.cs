using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaApplication1.Helpers;

namespace NitroHttp;

public partial class MainWindow : Window
{
    private readonly HttpClient _httpClient = new();
    private string _activeTab = "Body";
    private string _responseActiveTab = "Response";

    public MainWindow()
    {
        InitializeComponent();
        UpdateTabVisibility();
    }

    private void OnTabTapped(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.Tag is string tabName)
        {
            _activeTab = tabName;
            _responseActiveTab = tabName;

            UpdateTabStyles();
            UpdateTabVisibility();
        }
    }

    private void UpdateTabStyles()
    {
        var transparent = Brushes.Transparent;
        var mutedBrush = this.FindResource("TextMuted") as IBrush ?? new SolidColorBrush(Color.Parse("#A0A0A0"));
        var accentBrush = this.FindResource("AccentPrimary") as IBrush ?? new SolidColorBrush(Color.Parse("#FF4757"));

        TabBody.Background = transparent;
        TabBodyLabel.Foreground = mutedBrush;
        TabParams.Background = transparent;
        TabParamsLabel.Foreground = mutedBrush;
        TabHeaders.Background = transparent;
        TabHeadersLabel.Foreground = mutedBrush;
        TabAuth.Background = transparent;
        TabAuthLabel.Foreground = mutedBrush;

        switch (_activeTab)
        {
            case "Body":
                TabBody.Background = accentBrush;
                TabBodyLabel.Foreground = Brushes.White;
                break;
            case "Params":
                TabParams.Background = accentBrush;
                TabParamsLabel.Foreground = Brushes.White;
                break;
            case "Headers":
                TabHeaders.Background = accentBrush;
                TabHeadersLabel.Foreground = Brushes.White;
                break;
            case "Auth":
                TabAuth.Background = accentBrush;
                TabAuthLabel.Foreground = Brushes.White;
                break;
        }
    }

    private void UpdateTabVisibility()
    {
        if (ContentParams == null) return;

        ContentParams.IsVisible = _activeTab == "Params";
        ContentHeaders.IsVisible = _activeTab == "Headers";
        ContentBody.IsVisible = _activeTab == "Body";
        ContentAuth.IsVisible = _activeTab == "Auth";
    }

    private void OnResponseTabTapped(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.Tag is string tabName)
        {
            _responseActiveTab = tabName;
            UpdateResponseTabStyles();
            UpdateResponseTabVisibility();
        }
    }
    private void UpdateResponseTabStyles()
    {
        var mutedBrush = this.FindResource("TextMuted") as IBrush ?? new SolidColorBrush(Color.Parse("#A0A0A0"));
        var accentBrush = this.FindResource("AccentPrimary") as IBrush ?? new SolidColorBrush(Color.Parse("#FF4757"));

        ResponseContentTab.Foreground = mutedBrush;
        ResponseHeadersTab.Foreground = mutedBrush;
        ResponseCookiesTab.Foreground = mutedBrush;

        switch (_responseActiveTab)
        {
            case "Content":
                ResponseContentTab.Foreground = accentBrush;
                break;
            case "Headers":
                ResponseHeadersTab.Foreground = accentBrush;
                break;
            case "Cookies":
                ResponseCookiesTab.Foreground = accentBrush;
                break;
        }
    }

    private void UpdateResponseTabVisibility()
    {
        if (ResponseContent == null) return;

        ResponseContent.IsVisible = _responseActiveTab == "Content";
        ResponseHeaders.IsVisible = _responseActiveTab == "Headers";
        ResponseCookies.IsVisible = _responseActiveTab == "Cookies";
    }

    private void SetLoading(bool isLoading)
    {
        LoadingOverlay.IsVisible = isLoading;
        ResponseContent.IsVisible = !isLoading;
    }

    private void DisplayResponse(string responseText, int statusCode, long elapsedMs)
    {
        ResponseLabel.Text = TryFormatJson(responseText);
        ResponseTime.Text = $"{elapsedMs}ms";
        ResponseSize.Text = FormatBytes.Format(Encoding.UTF8.GetByteCount(responseText));
        UpdateStatus(statusCode);
    }

    private void UpdateStatus(int statusCode)
    {
        Status.Foreground = new SolidColorBrush(statusCode switch
        {
            _ when HttpStatusHelper.IsSuccess(statusCode) => Color.Parse("#2ED573"),
            _ when HttpStatusHelper.IsRedirect(statusCode) => Color.Parse("#5B8CFF"),
            _ when HttpStatusHelper.IsClientError(statusCode) => Color.Parse("#FF4757"),
            _ when HttpStatusHelper.IsServerError(statusCode) => Color.Parse("#FFA502"),
            _ => Color.Parse("#6B6B6B")
        });
        Status.Text = HttpStatusHelper.GetStatusText(statusCode);
    }

    private static string TryFormatJson(string text)
    {
        try
        {
            using var doc = JsonDocument.Parse(text);
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        }
        catch
        {
            return text;
        }
    }

    private async void SendButton_Click(object? sender, RoutedEventArgs e)
    {
        SetLoading(true);

        try
        {
            var method = (MethodPicker.SelectedItem as ComboBoxItem)?.Content?.ToString();
            switch (method)
            {
                case "GET":
                    await GetAsync();
                    break;
                case "POST":
                    await PostAsync();
                    break;
                case "PUT":
                    await PutAsync();
                    break;
                case "PATCH":
                    await PatchAsync();
                    break;
                case "DELETE":
                    await DeleteAsync();
                    break;
            }
        }
        finally
        {
            SetLoading(false);
        }
    }

    private async Task GetAsync()
    {
        if (string.IsNullOrEmpty(ApiUrl.Text))
        {
            ResponseLabel.Text = "Error: URL is empty";
            return;
        }

        try
        {
            var stopwatch = Stopwatch.StartNew();
            HttpResponseMessage responseMessage = await _httpClient.GetAsync(ApiUrl.Text);
            stopwatch.Stop();

            string responseText = await responseMessage.Content.ReadAsStringAsync();
            DisplayResponse(responseText, (int)responseMessage.StatusCode, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            ResponseLabel.Text = $"Error: {ex.Message}";
        }
    }

    private async Task PostAsync()
    {
        if (string.IsNullOrEmpty(ApiUrl.Text))
        {
            ResponseLabel.Text = "Error: URL is empty";
            return;
        }

        try
        {
            var stopwatch = Stopwatch.StartNew();
            string jsonBody = RequestBodyEditor?.Text ?? "{}";
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = await _httpClient.PostAsync(ApiUrl.Text, content);
            stopwatch.Stop();

            string responseBody = await responseMessage.Content.ReadAsStringAsync();
            DisplayResponse(responseBody, (int)responseMessage.StatusCode, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            ResponseLabel.Text = $"Error: {ex.Message}";
        }
    }

    private async Task PutAsync()
    {
        if (string.IsNullOrEmpty(ApiUrl.Text))
        {
            ResponseLabel.Text = "Error: URL is empty";
            return;
        }

        try
        {
            var stopwatch = Stopwatch.StartNew();
            string jsonBody = RequestBodyEditor?.Text ?? "{}";
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = await _httpClient.PutAsync(ApiUrl.Text, content);
            stopwatch.Stop();

            string responseBody = await responseMessage.Content.ReadAsStringAsync();
            DisplayResponse(responseBody, (int)responseMessage.StatusCode, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            ResponseLabel.Text = $"Error: {ex.Message}";
        }
    }

    private async Task PatchAsync()
    {
        if (string.IsNullOrEmpty(ApiUrl.Text))
        {
            ResponseLabel.Text = "Error: URL is empty";
            return;
        }

        try
        {
            var stopwatch = Stopwatch.StartNew();
            string jsonBody = RequestBodyEditor?.Text ?? "{}";
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = await _httpClient.PatchAsync(ApiUrl.Text, content);
            stopwatch.Stop();

            string responseBody = await responseMessage.Content.ReadAsStringAsync();
            DisplayResponse(responseBody, (int)responseMessage.StatusCode, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            ResponseLabel.Text = $"Error: {ex.Message}";
        }
    }

    private async Task DeleteAsync()
    {
        if (string.IsNullOrEmpty(ApiUrl.Text))
        {
            ResponseLabel.Text = "Error: URL is empty";
            return;
        }

        try
        {
            var stopwatch = Stopwatch.StartNew();
            HttpResponseMessage responseMessage = await _httpClient.DeleteAsync(ApiUrl.Text);
            stopwatch.Stop();

            string responseBody = await responseMessage.Content.ReadAsStringAsync();
            DisplayResponse(responseBody, (int)responseMessage.StatusCode, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            ResponseLabel.Text = $"Error: {ex.Message}";
        }
    }
}

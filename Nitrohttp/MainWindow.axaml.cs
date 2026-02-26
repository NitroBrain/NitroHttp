using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
    private const int MaxHistoryItems = 50;
    private readonly HttpClient _httpClient = new();
    private readonly string _storePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "NitroHttp",
        "requests.json");

    private string _activeTab = "Body";
    private string _responseActiveTab = "Content";
    private RequestStore _requestStore = new();

    public ObservableCollection<RequestEntry> HistoryItems { get; } = [];
    public ObservableCollection<RequestEntry> CollectionItems { get; } = [];

    public MainWindow()
    {
        InitializeComponent();
        HistoryList.ItemsSource = HistoryItems;
        CollectionsList.ItemsSource = CollectionItems;

        LoadRequestStore();
        UpdateTabVisibility();
        UpdateResponseTabVisibility();
        UpdateStorageButtonStyles();
    }

    private void OnTabTapped(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.Tag is string tabName)
        {
            _activeTab = tabName;

            UpdateTabStyles();
            UpdateTabVisibility();
            UpdateStorageButtonStyles();
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
        ContentHistory.IsVisible = _activeTab == "History";
        ContentCollections.IsVisible = _activeTab == "Collections";
    }

    private void UpdateStorageButtonStyles()
    {
        var inputBrush = this.FindResource("InputBackground") as IBrush ?? new SolidColorBrush(Color.Parse("#252525"));
        var accentBrush = this.FindResource("AccentPrimary") as IBrush ?? new SolidColorBrush(Color.Parse("#FF4757"));
        var mutedBrush = this.FindResource("TextMuted") as IBrush ?? new SolidColorBrush(Color.Parse("#A0A0A0"));

        HistoryButton.Background = _activeTab == "History" ? accentBrush : inputBrush;
        HistoryButton.Foreground = _activeTab == "History" ? Brushes.White : mutedBrush;

        CollectionsButton.Background = _activeTab == "Collections" ? accentBrush : inputBrush;
        CollectionsButton.Foreground = _activeTab == "Collections" ? Brushes.White : mutedBrush;
    }

    private void OnHistoryButtonClick(object? sender, RoutedEventArgs e)
    {
        _activeTab = "History";
        UpdateTabStyles();
        UpdateTabVisibility();
        UpdateStorageButtonStyles();
    }

    private void OnCollectionsButtonClick(object? sender, RoutedEventArgs e)
    {
        _activeTab = "Collections";
        UpdateTabStyles();
        UpdateTabVisibility();
        UpdateStorageButtonStyles();
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
        try
        {
            SetLoading(true);
            try
            {
                var method = GetSelectedMethod();
                var url = ApiUrl.Text?.Trim() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(url))
                {
                    AddHistoryEntry(method, url, RequestBodyEditor?.Text ?? string.Empty);
                }

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
        catch (Exception ex)
        {
            ResponseLabel.Text = $"Error: {ex.Message}";
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

    private string GetSelectedMethod()
    {
        return (MethodPicker.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "GET";
    }

    private void AddHistoryEntry(string method, string url, string body)
    {
        var existing = HistoryItems.FirstOrDefault(item =>
            item.Method.Equals(method, StringComparison.OrdinalIgnoreCase) &&
            item.Url.Equals(url, StringComparison.OrdinalIgnoreCase) &&
            item.Body == body);

        if (existing != null)
        {
            HistoryItems.Remove(existing);
        }

        HistoryItems.Insert(0, new RequestEntry
        {
            Method = method,
            Url = url,
            Body = body,
            Timestamp = DateTimeOffset.UtcNow
        });

        while (HistoryItems.Count > MaxHistoryItems)
        {
            HistoryItems.RemoveAt(HistoryItems.Count - 1);
        }

        PersistRequestStore();
    }

    private void OnSaveToCollectionClick(object? sender, RoutedEventArgs e)
    {
        var method = GetSelectedMethod();
        var url = ApiUrl.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(url))
        {
            ResponseLabel.Text = "Error: URL is empty";
            return;
        }

        var collectionName = CollectionNameBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(collectionName))
        {
            collectionName = $"Request {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }

        var existing = CollectionItems.FirstOrDefault(item =>
            item.CollectionName.Equals(collectionName, StringComparison.OrdinalIgnoreCase));

        if (existing != null)
        {
            CollectionItems.Remove(existing);
        }

        CollectionItems.Insert(0, new RequestEntry
        {
            CollectionName = collectionName,
            Method = method,
            Url = url,
            Body = RequestBodyEditor?.Text ?? string.Empty,
            Timestamp = DateTimeOffset.UtcNow
        });

        CollectionNameBox.Text = string.Empty;
        PersistRequestStore();
    }

    private void OnDeleteCollectionClick(object? sender, RoutedEventArgs e)
    {
        if (CollectionsList.SelectedItem is not RequestEntry entry)
        {
            return;
        }

        CollectionItems.Remove(entry);
        PersistRequestStore();
    }

    private void OnClearHistoryClick(object? sender, RoutedEventArgs e)
    {
        HistoryItems.Clear();
        PersistRequestStore();
    }

    private void OnHistorySelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (HistoryList.SelectedItem is not RequestEntry entry)
        {
            return;
        }

        ApplyRequestEntry(entry);
    }

    private void OnCollectionSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (CollectionsList.SelectedItem is not RequestEntry entry)
        {
            return;
        }

        ApplyRequestEntry(entry);
    }

    private void ApplyRequestEntry(RequestEntry entry)
    {
        ApiUrl.Text = entry.Url;
        RequestBodyEditor.Text = entry.Body;
        SetMethod(entry.Method);
        _activeTab = "Body";
        UpdateTabStyles();
        UpdateTabVisibility();
        UpdateStorageButtonStyles();
    }

    private void SetMethod(string method)
    {
        if (MethodPicker.Items == null)
        {
            MethodPicker.SelectedIndex = 0;
            return;
        }

        var index = 0;
        foreach (var item in MethodPicker.Items)
        {
            if (item is ComboBoxItem comboBoxItem &&
                comboBoxItem.Content?.ToString()?.Equals(method, StringComparison.OrdinalIgnoreCase) == true)
            {
                MethodPicker.SelectedIndex = index;
                return;
            }

            index++;
        }

        MethodPicker.SelectedIndex = 0;
    }

    private void LoadRequestStore()
    {
        try
        {
            if (!File.Exists(_storePath))
            {
                return;
            }

            var json = File.ReadAllText(_storePath);
            var store = JsonSerializer.Deserialize<RequestStore>(json);
            if (store == null)
            {
                return;
            }

            _requestStore = store;
            HistoryItems.Clear();
            CollectionItems.Clear();

            foreach (var item in _requestStore.History)
            {
                HistoryItems.Add(item);
            }

            foreach (var item in _requestStore.Collections)
            {
                CollectionItems.Add(item);
            }
        }
        catch
        {
            _requestStore = new RequestStore();
            HistoryItems.Clear();
            CollectionItems.Clear();
        }
    }

    private void PersistRequestStore()
    {
        try
        {
            _requestStore.History = HistoryItems.ToList();
            _requestStore.Collections = CollectionItems.ToList();

            var directory = Path.GetDirectoryName(_storePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(_requestStore, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_storePath, json);
        }
        catch
        {
        }
    }
}

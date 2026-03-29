using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Rendering;
using AvaloniaApplication1.Helpers;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using Avalonia.Threading;

namespace NitroHttp;

public partial class MainWindow : Window
{
  private const int MaxHistoryItems = 50;
  private readonly HttpClient _httpClient = new();
  private readonly CookieContainer _cookieContainer = new();
  private readonly string _storePath = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
      "NitroHttp",
      "requests.json");

  private string _activeTab = "Body";
  private string _responseActiveTab = "Content";
  private RequestStore _requestStore = new();
  private bool _isSyncingUrlFromParams;
  private string _manualBaseUrl = string.Empty;

  public ObservableCollection<RequestEntry> HistoryItems { get; } = [];
  public ObservableCollection<RequestEntry> CollectionItems { get; } = [];

  public MainWindow()
  {
    var handler = new HttpClientHandler
    {
      CookieContainer = _cookieContainer,
      UseCookies = true
    };

    _httpClient = new HttpClient(handler);

    InitializeComponent();
    ApplyJsonEditorColors();
    HistoryList.ItemsSource = HistoryItems;
    CollectionsList.ItemsSource = CollectionItems;

    LoadRequestStore();
    UpdateTabVisibility();
    UpdateResponseTabVisibility();
    UpdateStorageButtonStyles();

    _manualBaseUrl = ExtractBaseUrl(ApiUrl.Text);
    SyncUrlFromQueryParams();
  }

  private void OnAddQueryParamClick(object? sender, PointerPressedEventArgs e)
  {
    QueryParamsPanel.Children.Add(CreateQueryParamRow());
    SyncUrlFromQueryParams();
  }

  private void OnRemoveQueryParamClick(object? sender, PointerPressedEventArgs e)
  {
    if (sender is not TextBlock removeButton || removeButton.Parent is not Grid row)
    {
      return;
    }

    if (QueryParamsPanel.Children.Count <= 1)
    {
      if (TryGetQueryParamInputs(row, out var keyTextBox, out var valueTextBox))
      {
        keyTextBox.Text = string.Empty;
        valueTextBox.Text = string.Empty;
      }
    }
    else
    {
      QueryParamsPanel.Children.Remove(row);
    }

    SyncUrlFromQueryParams();
  }

  private void OnQueryParamTextChanged(object? sender, TextChangedEventArgs e)
  {
    SyncUrlFromQueryParams();
  }

  private void OnApiUrlTextChanged(object? sender, TextChangedEventArgs e)
  {
    if (_isSyncingUrlFromParams)
    {
      return;
    }

    _manualBaseUrl = ExtractBaseUrl(ApiUrl.Text);
  }

  private Grid CreateQueryParamRow()
  {
    var keyTextBox = new TextBox
    {
      Background = Brushes.Transparent,
      FontSize = 13,
      Watermark = "key",
      Foreground = Brushes.White,
      BorderThickness = new Thickness(0),
      CornerRadius = new CornerRadius(20)
    };
    keyTextBox.TextChanged += OnQueryParamTextChanged;

    var valueTextBox = new TextBox
    {
      Background = Brushes.Transparent,
      FontSize = 13,
      Watermark = "value",
      Foreground = Brushes.White,
      BorderThickness = new Thickness(0),
      CornerRadius = new CornerRadius(20)
    };
    valueTextBox.TextChanged += OnQueryParamTextChanged;

    var row = new Grid
    {
      ColumnDefinitions = new ColumnDefinitions("*,*,Auto")
    };

    var keyBorder = new Border
    {
      Padding = new Thickness(12, 8),
      Background = this.FindResource("InputBackground") as IBrush ?? new SolidColorBrush(Color.Parse("#1F2F27")),
      CornerRadius = new CornerRadius(20),
      Child = keyTextBox
    };
    Grid.SetColumn(keyBorder, 0);
    row.Children.Add(keyBorder);

    var valueBorder = new Border
    {
      Margin = new Thickness(12, 0),
      Padding = new Thickness(12, 8),
      Background = this.FindResource("InputBackground") as IBrush ?? new SolidColorBrush(Color.Parse("#1F2F27")),
      CornerRadius = new CornerRadius(20),
      Child = valueTextBox
    };
    Grid.SetColumn(valueBorder, 1);
    row.Children.Add(valueBorder);

    var removeText = new TextBlock
    {
      FontSize = 20,
      Text = "⨉",
      Foreground = this.FindResource("TextMuted") as IBrush ?? new SolidColorBrush(Color.Parse("#95A79C")),
      VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
      Cursor = new Cursor(StandardCursorType.Hand)
    };
    removeText.PointerPressed += OnRemoveQueryParamClick;
    Grid.SetColumn(removeText, 2);
    row.Children.Add(removeText);

    return row;
  }

  private void SyncUrlFromQueryParams()
  {
    var baseUrl = string.IsNullOrWhiteSpace(_manualBaseUrl)
        ? ExtractBaseUrl(ApiUrl.Text)
        : _manualBaseUrl;

    var queryParts = QueryParamsPanel.Children
        .OfType<Grid>()
        .Select(TryGetQueryParamPair)
        .Where(pair => pair.HasValue)
        .Select(pair => pair!.Value)
        .Select(pair => $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value)}")
        .ToList();

    var nextUrl = baseUrl;
    if (queryParts.Count > 0)
    {
      nextUrl = $"{baseUrl}?{string.Join("&", queryParts)}";
    }

    if (string.Equals(ApiUrl.Text, nextUrl, StringComparison.Ordinal))
    {
      return;
    }

    _isSyncingUrlFromParams = true;
    try
    {
      ApiUrl.Text = nextUrl;
    }
    finally
    {
      _isSyncingUrlFromParams = false;
    }
  }

  private static string ExtractBaseUrl(string? fullUrl)
  {
    if (string.IsNullOrWhiteSpace(fullUrl))
    {
      return string.Empty;
    }

    var text = fullUrl.Trim();
    var questionMarkIndex = text.IndexOf('?');
    return questionMarkIndex >= 0 ? text[..questionMarkIndex] : text;
  }

  private static KeyValuePair<string, string>? TryGetQueryParamPair(Grid row)
  {
    if (!TryGetQueryParamInputs(row, out var keyTextBox, out var valueTextBox))
    {
      return null;
    }

    var key = keyTextBox.Text?.Trim() ?? string.Empty;
    if (string.IsNullOrWhiteSpace(key))
    {
      return null;
    }

    var value = valueTextBox.Text ?? string.Empty;
    return new KeyValuePair<string, string>(key, value);
  }

  private static bool TryGetQueryParamInputs(Grid row, out TextBox keyTextBox, out TextBox valueTextBox)
  {
    keyTextBox = null!;
    valueTextBox = null!;

    var borders = row.Children.OfType<Border>().ToList();
    if (borders.Count < 2)
    {
      return false;
    }

    if (borders[0].Child is not TextBox first || borders[1].Child is not TextBox second)
    {
      return false;
    }

    keyTextBox = first;
    valueTextBox = second;
    return true;
  }

  private void ApplyJsonEditorColors()
  {
    ApplyJsonHighlighting(RequestBodyEditor);
    ApplyJsonHighlighting(ResponseLabel);
  }

  private static void ApplyJsonHighlighting(TextEditor? editor)
  {
    if (editor?.SyntaxHighlighting == null)
    {
      return;
    }

    var keyBrush = new SimpleHighlightingBrush(Color.Parse("#00E5A8"));
    var stringBrush = new SimpleHighlightingBrush(Color.Parse("#FFD166"));
    var numberBrush = new SimpleHighlightingBrush(Color.Parse("#7EE787"));
    var keywordBrush = new SimpleHighlightingBrush(Color.Parse("#86EFAC"));
    var booleanNullBrush = new SimpleHighlightingBrush(Color.Parse("#FBBF24"));
    var punctuationBrush = new SimpleHighlightingBrush(Color.Parse("#E6EDF3"));

    foreach (var color in editor.SyntaxHighlighting.NamedHighlightingColors)
    {
      var name = color.Name;
      if (string.IsNullOrWhiteSpace(name))
      {
        continue;
      }

      if (name.Contains("PropertyName", StringComparison.OrdinalIgnoreCase) ||
          name.Contains("Property", StringComparison.OrdinalIgnoreCase) ||
          name.Contains("Attribute", StringComparison.OrdinalIgnoreCase) ||
          name.Contains("Tag", StringComparison.OrdinalIgnoreCase))
      {
        color.Foreground = keyBrush;
        continue;
      }

      if (name.Contains("String", StringComparison.OrdinalIgnoreCase) ||
          name.Contains("Char", StringComparison.OrdinalIgnoreCase) ||
          name.Contains("Value", StringComparison.OrdinalIgnoreCase))
      {
        color.Foreground = stringBrush;
        continue;
      }

      if (name.Contains("Number", StringComparison.OrdinalIgnoreCase))
      {
        color.Foreground = numberBrush;
        continue;
      }

      if (name.Contains("Keyword", StringComparison.OrdinalIgnoreCase) ||
          name.Contains("Literal", StringComparison.OrdinalIgnoreCase))
      {
        color.Foreground = keywordBrush;
        continue;
      }

      if (name.Contains("Boolean", StringComparison.OrdinalIgnoreCase) ||
          name.Contains("Null", StringComparison.OrdinalIgnoreCase))
      {
        color.Foreground = booleanNullBrush;
        continue;
      }

      if (name.Contains("Bracket", StringComparison.OrdinalIgnoreCase) ||
          name.Contains("Operator", StringComparison.OrdinalIgnoreCase) ||
          name.Contains("Delimiter", StringComparison.OrdinalIgnoreCase) ||
          name.Contains("Punctuation", StringComparison.OrdinalIgnoreCase))
      {
        color.Foreground = punctuationBrush;
      }
    }

    editor.Foreground = new SolidColorBrush(Color.Parse("#FFFFFF"));
    editor.TextArea.TextView.LinkTextForegroundBrush = new SolidColorBrush(Color.Parse("#5ba769"));
    editor.TextArea.TextView.LinkTextUnderline = false;
    editor.TextArea.TextView.CurrentLineBackground = new SolidColorBrush(Color.Parse("#151f1a"));
    editor.TextArea.TextView.CurrentLineBorder = new Pen(new SolidColorBrush(Colors.Transparent), 1);

    if (!editor.TextArea.TextView.LineTransformers.OfType<JsonKeyColorizer>().Any())
    {
      editor.TextArea.TextView.LineTransformers.Add(new JsonKeyColorizer(new SolidColorBrush(Color.Parse("#00E5A8"))));
    }

    if (!editor.TextArea.TextView.LineTransformers.OfType<JsonBooleanNullColorizer>().Any())
    {
      editor.TextArea.TextView.LineTransformers.Add(new JsonBooleanNullColorizer(new SolidColorBrush(Color.Parse("#FBBF24"))));
    }

    editor.Options.ShowTabs = true;
    editor.Options.ShowSpaces = true;
    editor.Options.HighlightCurrentLine = true;
    editor.Options.IndentationSize = 2;
  }

  private sealed class JsonKeyColorizer : DocumentColorizingTransformer
  {
    private static readonly Regex JsonKeyRegex = new("\"(?:\\\\.|[^\"\\\\])*\"\\s*:", RegexOptions.Compiled);
    private readonly IBrush _keyBrush;

    public JsonKeyColorizer(IBrush keyBrush)
    {
      _keyBrush = keyBrush;
    }

    protected override void ColorizeLine(DocumentLine line)
    {
      var lineText = CurrentContext.Document.GetText(line);
      foreach (Match match in JsonKeyRegex.Matches(lineText))
      {
        var colonIndex = lineText.IndexOf(':', match.Index);
        if (colonIndex <= match.Index)
        {
          continue;
        }

        var start = line.Offset + match.Index;
        var end = line.Offset + colonIndex;

        ChangeLinePart(start, end, element =>
        {
          element.TextRunProperties.SetForegroundBrush(_keyBrush);
        });
      }
    }
  }

  private sealed class JsonBooleanNullColorizer : DocumentColorizingTransformer
  {
    private readonly IBrush _valueBrush;

    public JsonBooleanNullColorizer(IBrush valueBrush)
    {
      _valueBrush = valueBrush;
    }

    protected override void ColorizeLine(DocumentLine line)
    {
      var lineText = CurrentContext.Document.GetText(line);
      var inString = false;
      var escaped = false;

      for (var index = 0; index < lineText.Length; index++)
      {
        var ch = lineText[index];

        if (inString)
        {
          if (escaped)
          {
            escaped = false;
            continue;
          }

          if (ch == '\\')
          {
            escaped = true;
            continue;
          }

          if (ch == '"')
          {
            inString = false;
          }

          continue;
        }

        if (ch == '"')
        {
          inString = true;
          continue;
        }

        if (!char.IsLetter(ch))
        {
          continue;
        }

        var start = index;
        while (index < lineText.Length && char.IsLetter(lineText[index]))
        {
          index++;
        }

        var token = lineText[start..index];
        if (token is "true" or "false" or "null")
        {
          var offsetStart = line.Offset + start;
          var offsetEnd = line.Offset + index;
          ChangeLinePart(offsetStart, offsetEnd, element =>
          {
            element.TextRunProperties.SetForegroundBrush(_valueBrush);
          });
        }

        index--;
      }
    }
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
  private static string LimitJsonItems(string jsonText, int maxItems = 500)
  {
    try
    {
      using var doc = JsonDocument.Parse(jsonText);

      if (doc.RootElement.ValueKind == JsonValueKind.Array)
      {
        var items = doc.RootElement
            .EnumerateArray()
            .Take(maxItems)
            .ToList();

        return JsonSerializer.Serialize(items, new JsonSerializerOptions
        {
          WriteIndented = true
        });
      }

      if (doc.RootElement.ValueKind == JsonValueKind.Object)
      {
        return JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions
        {
          WriteIndented = true
        });
      }
    }
    catch
    {
    }

    return jsonText;
  }
  private void DisplayResponse(string responseText, int statusCode, long elapsedMs, string? responseHeaders = "", string? responseCookies = "")
  {
    int count = 0;
    try
    {
      using var doc = JsonDocument.Parse(responseText);
      if (doc.RootElement.ValueKind == JsonValueKind.Array)
      {
        count = doc.RootElement.GetArrayLength();
      }
      else if (doc.RootElement.ValueKind == JsonValueKind.Object)
      {
        count = 1;
      }
    }
    catch
    {
      count = 0;
    }

    if (responseText.Length > 100_000)
    {
      ResponseLabel.Text = LimitJsonItems(responseText, 1_000);
    }
    else
    {
      ResponseLabel.Text = TryFormatJson(responseText);
    }
    ResponseCount.Text = count.ToString();
    ResponseTime.Text = $"{elapsedMs}ms";
    ResponseSize.Text = FormatBytes.Format(Encoding.UTF8.GetByteCount(responseText));

    ResponseHeadersLabel.Text = responseHeaders;
    ResponseCookiesLabel.Text = responseCookies;

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

  private Task<string> GetHeadersAsync(HttpResponseMessage responseMessage)
  {
    var allHeaders = responseMessage.Headers
        .Concat(responseMessage.Content.Headers)
        .Select(h => $"{h.Key}: {string.Join(",", h.Value)}");

    string responseHeaders = string.Join(Environment.NewLine, allHeaders);

    return Task.FromResult(responseHeaders);
  }

  private Task<string> GetCookiesAsync(string url)
  {
    var uri = new Uri(url);

    var cookies = _cookieContainer.GetCookies(uri)
        .Cast<Cookie>()
        .Select(c => $"{c.Name}: {c.Value}")
        .ToList();

    string responseCookies = cookies.Any()
        ? string.Join(Environment.NewLine, cookies)
        : "No Cookies";

    return Task.FromResult(responseCookies);
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

      var responseMessage = await _httpClient.GetAsync(ApiUrl.Text,
          HttpCompletionOption.ResponseHeadersRead);

      var responseText = await responseMessage.Content
          .ReadAsStringAsync();

      stopwatch.Stop();

      // Offload heavy processing
      _ = Task.Run(() =>
      {
        var formatted = TryFormatJson(responseText);

        Dispatcher.UIThread.InvokeAsync(() =>
          {
            DisplayResponse(
                  formatted,
                  (int)responseMessage.StatusCode,
                  stopwatch.ElapsedMilliseconds);
          });
      });
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

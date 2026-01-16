using System.Diagnostics;
using System.Text;
using NitroHttp.Helpers;

namespace NitroHttp
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient = new();
        private string _activeTab = "Body";
        private string _lastJsonResponse = string.Empty;

        public MainPage()
        {
            InitializeComponent();
            MethodPicker.SelectedIndex = MethodPicker.Items.IndexOf("GET");
            UpdateTabVisibility();

            if (Application.Current != null)
            {
                Application.Current.RequestedThemeChanged += OnThemeChanged;
            }
        }

        private void OnThemeChanged(object? sender, AppThemeChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_lastJsonResponse))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    responseLabel.FormattedText = JsonSyntaxHighlighter.Highlight(_lastJsonResponse);
                });
            }

            UpdateTabStyles();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (Application.Current != null)
            {
                Application.Current.RequestedThemeChanged -= OnThemeChanged;
            }
        }

        private void OnTabTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is string tabName)
            {
                _activeTab = tabName;
                UpdateTabStyles();
                UpdateTabVisibility();
            }
        }

        private void UpdateTabStyles()
        {
            var transparent = Colors.Transparent;
            var mutedColor = Colors.Gray;
            var accentColor = Colors.Blue;
            var whiteColor = Colors.White;

            if (Application.Current?.Resources != null)
            {
                if (Application.Current.Resources.TryGetValue("TextMuted", out var mutedObj) && mutedObj is Color muted)
                {
                    mutedColor = muted;
                }
                if (Application.Current.Resources.TryGetValue("AccentPrimary", out var accentObj) && accentObj is Color accent)
                {
                    accentColor = accent;
                }
                if (Application.Current.Resources.TryGetValue("TextPrimary", out var whiteObj) && whiteObj is Color white)
                {
                    whiteColor = white;
                }
            }

            TabParams.BackgroundColor = transparent;
            TabParamsLabel.TextColor = mutedColor;

            TabHeaders.BackgroundColor = transparent;
            TabHeadersLabel.TextColor = mutedColor;

            TabBody.BackgroundColor = transparent;
            TabBodyLabel.TextColor = mutedColor;

            TabAuth.BackgroundColor = transparent;
            TabAuthLabel.TextColor = mutedColor;

            switch (_activeTab)
            {
                case "Params":
                    TabParams.BackgroundColor = accentColor;
                    TabParamsLabel.TextColor = whiteColor;
                    break;
                case "Headers":
                    TabHeaders.BackgroundColor = accentColor;
                    TabHeadersLabel.TextColor = whiteColor;
                    break;
                case "Body":
                    TabBody.BackgroundColor = accentColor;
                    TabBodyLabel.TextColor = whiteColor;
                    break;
                case "Auth":
                    TabAuth.BackgroundColor = accentColor;
                    TabAuthLabel.TextColor = whiteColor;
                    break;
            }
        }

        private void UpdateTabVisibility()
        {
            ContentParams.IsVisible = _activeTab == "Params";
            ContentHeaders.IsVisible = _activeTab == "Headers";
            ContentBody.IsVisible = _activeTab == "Body";
            ContentAuth.IsVisible = _activeTab == "Auth";
        }

        private void DisplayResponse(string responseText, int statusCode, long elapsedMs)
        {
            _lastJsonResponse = responseText;
            responseLabel.FormattedText = JsonSyntaxHighlighter.Highlight(responseText);
            responseTime.Text = $"{elapsedMs}ms";
            responseSize.Text = FormatBytes.Format(Encoding.UTF8.GetByteCount(responseText));
            UpdateStatus(statusCode);
        }

        private async void Button_Click(object sender, EventArgs e)
        {
            switch (MethodPicker.SelectedItem)
            {
                case "GET":
                    await GET(sender, e);
                    break;
                case "POST":
                    await POST(sender, e);
                    break;
                case "PUT":
                    await PUT(sender, e);
                    break;
                case "PATCH":
                    await PATCH(sender, e);
                    break;
                case "DELETE":
                    await DELETE(sender, e);
                    break;
            }
        }

        private async Task GET(object sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender);
            ArgumentNullException.ThrowIfNull(e);

            if (string.IsNullOrEmpty(apiUrl.Text))
            {
                await DisplayAlert("Error", "URL is empty", "OK");
                return;
            }

            try
            {
                var stopwatch = Stopwatch.StartNew();
                HttpResponseMessage responseMessage = await _httpClient.GetAsync(apiUrl.Text);
                stopwatch.Stop();

                string responseText = await responseMessage.Content.ReadAsStringAsync();
                DisplayResponse(responseText, (int)responseMessage.StatusCode, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task POST(object sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender);
            ArgumentNullException.ThrowIfNull(e);

            if (string.IsNullOrEmpty(apiUrl.Text))
            {
                await DisplayAlert("Error", "URL is empty", "OK");
                return;
            }

            try
            {
                var stopwatch = Stopwatch.StartNew();
                string jsonBody = RequestBodyEditor?.Text ?? "{}";
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = await _httpClient.PostAsync(apiUrl.Text, content);
                stopwatch.Stop();

                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                DisplayResponse(responseBody, (int)responseMessage.StatusCode, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task PUT(object sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender);
            ArgumentNullException.ThrowIfNull(e);

            if (string.IsNullOrEmpty(apiUrl.Text))
            {
                await DisplayAlert("Error", "URL is empty", "OK");
                return;
            }

            try
            {
                var stopwatch = Stopwatch.StartNew();
                string jsonBody = RequestBodyEditor?.Text ?? "{}";
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = await _httpClient.PutAsync(apiUrl.Text, content);
                stopwatch.Stop();

                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                DisplayResponse(responseBody, (int)responseMessage.StatusCode, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task PATCH(object sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender);
            ArgumentNullException.ThrowIfNull(e);

            if (string.IsNullOrEmpty(apiUrl.Text))
            {
                await DisplayAlert("Error", "URL is empty", "OK");
                return;
            }

            try
            {
                var stopwatch = Stopwatch.StartNew();
                string jsonBody = RequestBodyEditor?.Text ?? "{}";
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = await _httpClient.PatchAsync(apiUrl.Text, content);
                stopwatch.Stop();

                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                DisplayResponse(responseBody, (int)responseMessage.StatusCode, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task DELETE(object sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender);
            ArgumentNullException.ThrowIfNull(e);

            if (string.IsNullOrEmpty(apiUrl.Text))
            {
                await DisplayAlert("Error", "URL is empty", "OK");
                return;
            }

            try
            {
                var stopwatch = Stopwatch.StartNew();
                HttpResponseMessage responseMessage = await _httpClient.DeleteAsync(apiUrl.Text);
                stopwatch.Stop();

                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                DisplayResponse(responseBody, (int)responseMessage.StatusCode, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void StatusCodeColor(int statusCode)
        {
            status.TextColor = statusCode switch
            {
                _ when HttpStatusHelper.IsSuccess(statusCode) => Color.FromArgb("#2ED573"),
                _ when HttpStatusHelper.IsRedirect(statusCode) => Color.FromArgb("#5B8CFF"),
                _ when HttpStatusHelper.IsClientError(statusCode) => Color.FromArgb("#FF4757"),
                _ when HttpStatusHelper.IsServerError(statusCode) => Color.FromArgb("#FFA502"),
                _ => Color.FromArgb("#6B6B6B")
            };
        }

        private void UpdateStatus(int statusCode)
        {
            StatusCodeColor(statusCode);
            status.Text = HttpStatusHelper.GetStatusText(statusCode);
        }
    }
}

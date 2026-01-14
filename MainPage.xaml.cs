using System.Net.Http.Json;
using System.Text;
using NitroHttp.Helpers;

namespace NitroHttp
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient = new();
        private readonly string _apiUrl;

        public MainPage()
        {
            InitializeComponent();

            MethodPicker.SelectedIndex = MethodPicker.Items.IndexOf("GET");
            
            _apiUrl = apiUrl.Text;
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
                case "DELETE":
                    await DELETE(sender, e);
                    break;
            }
        }
        private async Task GET(object sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender);
            ArgumentNullException.ThrowIfNull(e);

            if (string.IsNullOrEmpty(_apiUrl))
            {
                _ = DisplayAlert("Error", "Url is Empty", "OK");
            }
            else
            {
                try
                {
                    HttpResponseMessage responseMessage = await _httpClient.GetAsync(_apiUrl);
                    string responseText = await responseMessage.Content.ReadAsStringAsync();
                    responseLabel.FormattedText = JsonSyntaxHighlighter.Highlight(responseText);
                    int statusCode = (int)responseMessage.StatusCode;
                    UpdateStatus(statusCode);
                }
                catch (Exception ex)
                {
                    _ = DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }

        private async Task POST(object sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender);
            ArgumentNullException.ThrowIfNull(e);

            if (string.IsNullOrEmpty(_apiUrl))
            {
                _ = DisplayAlert("Error", "Url is Empty", "OK");
            }
            else
            {
                try
                {
                    string jsonBody = "{\"title\": \"learn HttpClient\", \"body\": \"coding is fun\", \"userId\": 1}";
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    HttpResponseMessage responseMessage = await _httpClient.PostAsync(_apiUrl, content);
                    string responseBody = await responseMessage.Content.ReadAsStringAsync();
                    responseLabel.FormattedText = JsonSyntaxHighlighter.Highlight(responseBody);
                    int statusCode = (int)responseMessage.StatusCode;
                    UpdateStatus(statusCode);
                }
                catch (Exception ex)
                {
                    _ = DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }

        private async Task PUT(object sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender);
            ArgumentNullException.ThrowIfNull(e);

            if (string.IsNullOrEmpty(_apiUrl))
            {
                _ = DisplayAlert("Error", "Url is Empty", "OK");
            }
            else
            {
                try
                {
                    string updatedJsonBody = "{\"id\": 1, \"title\": \"learn HttpClient - UPDATED\", \"completed\": true, \"userId\": 1}";
                    var content = new StringContent(updatedJsonBody, Encoding.UTF8, "application/json");
                    HttpResponseMessage responseMessage = await _httpClient.PutAsync(_apiUrl, content);
                    string responseBody = await responseMessage.Content.ReadAsStringAsync();
                    responseLabel.FormattedText = JsonSyntaxHighlighter.Highlight(responseBody);
                    int statusCode = (int)responseMessage.StatusCode;
                    UpdateStatus(statusCode);
                }
                catch (Exception ex)
                {
                    _ = DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }

        private async Task DELETE(object sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender);
            ArgumentNullException.ThrowIfNull(e);
            
            if (string.IsNullOrEmpty(_apiUrl))
            {
                _ = DisplayAlert("Error", "Url is Empty", "OK");
            }
            else
            {
                try
                {
                    HttpResponseMessage responseMessage = await _httpClient.DeleteAsync(_apiUrl);
                    string responseBody = await responseMessage.Content.ReadAsStringAsync();
                    responseLabel.FormattedText = JsonSyntaxHighlighter.Highlight(responseBody);
                    int statusCode = (int)responseMessage.StatusCode;
                    UpdateStatus(statusCode);

                }
                catch(Exception ex)
                {
                    _ = DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }

        private void StatusCodeColor(int statusCode)
        {
            status.TextColor = statusCode switch
            {
                _ when HttpStatusHelper.IsSuccess(statusCode) => Color.FromArgb("#4ADE80"),
                _ when HttpStatusHelper.IsRedirect(statusCode) => Color.FromArgb("#4F7CFF"),
                _ when HttpStatusHelper.IsClientError(statusCode) => Color.FromArgb("#FC4850"),
                _ when HttpStatusHelper.IsServerError(statusCode) => Color.FromArgb("#F59E0B"),
                _ => Color.FromArgb("#6B6D75")
            };
        }

        private void UpdateStatus(int statusCode)
        {
            StatusCodeColor(statusCode);
            status.Text = HttpStatusHelper.GetStatusText(statusCode);
        }
    }
}

using System.Text;

namespace NitroHttp
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient = new();

        public MainPage()
        {
            InitializeComponent();
            MethodPicker.SelectedIndex = MethodPicker.Items.IndexOf("GET");
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
            }
        }
        private async Task GET(object sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender);
            ArgumentNullException.ThrowIfNull(e);

            if (string.IsNullOrEmpty(url.Text))
            {
                _ = DisplayAlert("Error", "Url is Empty", "OK");
            }
            else
            {
                try
                {
                    string respoonseUrl = url.Text;
                    HttpResponseMessage responseMessage = await _httpClient.GetAsync(respoonseUrl);
                    string responseText = await responseMessage.Content.ReadAsStringAsync();
                    response.Text = responseText;
                    int statusCode = (int)responseMessage.StatusCode;
                    status.Text = statusCode.ToString();
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

            if (string.IsNullOrEmpty(url.Text))
            {
                _ = DisplayAlert("Error", "Url is Empty", "OK");
            }
            else
            {
                try
                {
                    string responseUrl = url.Text;
                    string jsonBody = "{\"title\": \"learn HttpClient\", \"body\": \"coding is fun\", \"userId\": 1}";
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    HttpResponseMessage responseMessage = await _httpClient.PostAsync(responseUrl, content);
                    string responseBody = await responseMessage.Content.ReadAsStringAsync();
                    response.Text = responseBody;
                    int statusCode = (int)responseMessage.StatusCode;
                    status.Text = statusCode.ToString();
                }
                catch (Exception ex)
                {
                    _ = DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }
    }
}

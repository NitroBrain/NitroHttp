namespace NitroHttp
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient = new();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, EventArgs e)
        {
            await GET(sender, e);
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
    }
}

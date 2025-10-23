namespace NitroHttp
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient = new();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnSendRequestClicked(object sender, EventArgs e)
        {
            var url = ApiUrlEntry.Text?.Trim();

            if (string.IsNullOrEmpty(url))
            {
                await DisplayAlert("Error", "Please enter a valid API URL.", "OK");
                return;
            }

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                ResponseEditor.Text = response;
            }
            catch (Exception ex)
            {
                ResponseEditor.Text = $"Error:\n{ex.Message}";
            }
        }
    }
}

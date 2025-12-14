namespace NitroHttp
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient = new();

        public MainPage()
        {
            InitializeComponent();
        }

    }
}

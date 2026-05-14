namespace Views.Interfaces
{
    public interface IResponseView
    {
        void Display(string response, int responseStatus, int responseCount, int responseSize);
    }
}

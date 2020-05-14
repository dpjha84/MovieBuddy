namespace MovieBuddy
{
    public interface ILocalDataProvider
    {
        void Set(string key, string value);

        string Get(string key);

        void Reset();
    }
}
using TMDbLib.Client;

namespace MovieBuffLib
{
    public class MovieManager
    {
        private static IMovieManager _instance = null;

        public static void Init(ILocalDataProvider provider, bool fake = false)
        {
            if (_instance == null)
            {
                //if (fake)
                //    _instance = new FakeMovieManager(provider);
                //else
                //{
                _instance = new RealMovieManager(provider);
                _instance.LoadInternalAsync();
                //}
            }
        }

        public static IMovieManager Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
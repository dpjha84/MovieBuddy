using Android.OS;

namespace MovieBuddy
{
    public class BaseFragment : Android.Support.V4.App.Fragment
    {
        protected AdapterBase adapter;
        protected string movieName;

        protected int MovieId { get { return Arguments.GetInt("movieId"); } }
        protected int CastId { get { return Arguments.GetInt("castId"); } }

        public BaseFragment()
        {
            
        }

        protected virtual void OnItemClick(object sender, int position)
        {
        }

        public override void OnDestroy()
        {
            if (adapter != null)
                adapter.ItemClick -= OnItemClick;
            base.OnDestroy();
        }

        
    }
}
using Android.Content;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using static Android.Support.V7.Widget.SearchView;

namespace MovieBuddy
{
    public class QueryTextListener : Java.Lang.Object, IOnQueryTextListener
    {
        ViewPager mViewPager;
        TabLayout mTabLayout;
        Android.Support.V7.Widget.Toolbar toolbar;
        Context context;
        Android.Support.V4.App.FragmentManager mgr;
        public QueryTextListener(Context c, ViewPager pager, TabLayout layout, Android.Support.V7.Widget.Toolbar t, Android.Support.V4.App.FragmentManager m)
        {
            mViewPager = pager;
            mTabLayout = layout;
            toolbar = t;
            context = c;
            mgr = m;
        }
        public bool OnQueryTextChange(string newText)
        {
            var tabPagerAdapter = new SearchPagerAdapter(context, mgr, newText);

            mViewPager.Adapter = tabPagerAdapter;
            mTabLayout.SetupWithViewPager(mViewPager);

            for (int i = 0; i < mTabLayout.TabCount; i++)
            {
                TabLayout.Tab tab = mTabLayout.GetTabAt(i);
                tab.SetCustomView(tabPagerAdapter.GetTabView(toolbar, i));
            }
            return true;
        }

        public bool OnQueryTextSubmit(string newText)
        {
            return false;
        }
    }
}
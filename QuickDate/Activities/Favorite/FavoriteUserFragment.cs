using System;
using System.Collections.ObjectModel;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Util;
using QuickDate.Activities.Favorite.Adapters;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Global;
using Fragment = Android.Support.V4.App.Fragment;

namespace QuickDate.Activities.Favorite
{
    public class FavoriteUserFragment : Fragment
    {
        #region Variables Basic

        public FavoriteUsersAdapter MAdapter;
        private SwipeRefreshLayout SwipeRefreshLayout;
        private RecyclerView MRecycler;
        private GridLayoutManager LayoutManager;
        private ViewStub EmptyStateLayout;
        private View Inflated;
        private HomeActivity MainContext;
        private AdView MAdView;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            MainContext = (HomeActivity)Activity;
            HasOptionsMenu = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // Use this to return your custom view for this Fragment
                View view = inflater.Inflate(Resource.Layout.RecyclerDefaultLayout, container, false);

                //Get Value And Set Toolbar
                InitComponent(view);
                InitToolbar(view); 
                SetRecyclerViewAdapters();

                SwipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;
                MAdapter.OnItemClick += FavoritesAdapterOnItemClick;

                //Get Data  
                GetFavorite();

                AdsGoogle.Ad_Interstitial(Context);

                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public override void OnResume()
        {
            try
            {
                MAdView?.Resume();
                base.OnResume();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnPause()
        {
            try
            {
                MAdView?.Pause();
                base.OnPause();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override void OnDestroy()
        {
            try
            { 
                MAdView?.Destroy();

                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Menu
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    try
                    {
                        MainContext.FragmentBottomNavigator.BackStackClickFragment();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }

                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                MRecycler = (RecyclerView)view.FindViewById(Resource.Id.recyler);
                EmptyStateLayout = view.FindViewById<ViewStub>(Resource.Id.viewStub);

                SwipeRefreshLayout = (SwipeRefreshLayout)view.FindViewById(Resource.Id.swipeRefreshLayout);
                SwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
                SwipeRefreshLayout.Refreshing = true;
                SwipeRefreshLayout.Enabled = true;

                MAdView = view.FindViewById<AdView>(Resource.Id.adView);
                AdsGoogle.InitAdView(MAdView, MRecycler);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetRecyclerViewAdapters()
        {
            try
            {
                MAdapter = new FavoriteUsersAdapter(Activity,MainContext)
                {
                    FavoritesList = new ObservableCollection<UserInfoObject>()
                };
                LayoutManager = new GridLayoutManager(Activity, 2);
                MRecycler.SetLayoutManager(LayoutManager);
                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(10);
                MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<UserInfoObject>(Activity, MAdapter, sizeProvider, 10);
                MRecycler.AddOnScrollListener(preLoader);
                MRecycler.SetAdapter(MAdapter); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitToolbar(View view)
        {
            try
            {
                var toolbar = view.FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    string title = Context.GetString(Resource.String.Lbl_Favorite);
                    MainContext.SetToolBar(toolbar, title);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
 
        #endregion

        #region Events

        private void FavoritesAdapterOnItemClick(object sender, FavoriteUsersAdapterClickEventArgs e)
        {
            try
            {
                var item = MAdapter.FavoritesList[e.Position];
                if (item != null)
                {
                    QuickDateTools.OpenProfile(Activity, "Close", item, e.Image); 
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                //Get Data Api
                MAdapter.FavoritesList.Clear();
                MAdapter.NotifyDataSetChanged();

                GetFavorite();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
         
        #region Load Data Api 

        private void GetFavorite()
        {
            try
            {
                if (ListUtils.FavoriteUserList.Count == 0)
                {
                    var sqlEntity = new SqLiteDatabase();
                    ListUtils.FavoriteUserList = sqlEntity.GetDataFavorite();
                    sqlEntity.Dispose();
                }
                else if (ListUtils.FavoriteUserList.Count > 0)
                {
                    MAdapter.FavoritesList = ListUtils.FavoriteUserList;
                    MAdapter.NotifyDataSetChanged();
                }

                ShowEmptyPage();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void ShowEmptyPage()
        {
            try
            {
                SwipeRefreshLayout.Refreshing = false;

                if (MAdapter.FavoritesList.Count > 0)
                {
                    MRecycler.Visibility = ViewStates.Visible;
                    EmptyStateLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    MRecycler.Visibility = ViewStates.Gone;

                    if (Inflated == null)
                        Inflated = EmptyStateLayout.Inflate();

                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(Inflated, EmptyStateInflater.Type.NoUsers);
                    if (!x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click += null;
                    }
                    EmptyStateLayout.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                SwipeRefreshLayout.Refreshing = false;
                Console.WriteLine(e);
            }
        }
         
        #endregion
    }
} 
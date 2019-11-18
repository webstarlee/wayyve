using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Util;
using Java.Lang;
using QuickDate.Activities.HotOrNot.Adapters;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Global;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using Exception = System.Exception;
using Fragment = Android.Support.V4.App.Fragment;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.HotOrNot
{
    public class HotOrNotFragment : Fragment, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private HotOrNotUserAdapter MAdapter;
        private SwipeRefreshLayout SwipeRefreshLayout;
        private RecyclerView MRecycler;
        private LinearLayoutManager LayoutManager;
        private ViewStub EmptyStateLayout;
        private View Inflated;
        private HomeActivity MainContext;
        private AdView MAdView;
        private RecyclerViewOnScrollListener MainScrollEvent;
        private TextView ActionButton;
        private string IdGender = "4525";

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
                 
                ActionButton.Click += ActionButtonOnClick;
                 
                //Get Data  
                GetData();

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
                Methods.SetMargin(SwipeRefreshLayout, 0, 0, 0, 0);

                ActionButton = (TextView)view.FindViewById(Resource.Id.toolbar_title);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ActionButton, IonIconsFonts.AndroidOptions);
                ActionButton.SetTextColor(AppSettings.TitleTextColor);
                ActionButton.SetTextSize(ComplexUnitType.Sp, 20f);
                ActionButton.Visibility = ViewStates.Visible;
                 
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
                MAdapter = new HotOrNotUserAdapter(Activity)
                {
                    UsersDateList = new ObservableCollection<UserInfoObject>()
                };
                MAdapter.HotItemClick += MAdapterOnHotItemClick;
                MAdapter.NotItemClick += MAdapterOnNotItemClick;
                LayoutManager = new LinearLayoutManager(Context);
                //LayoutManager.SetSpanSizeLookup(new MySpanSizeLookup(4, 1, 1)); //5, 1, 2 
                MRecycler.SetLayoutManager(LayoutManager); 
                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(10);
                MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<UserInfoObject>(Activity, MAdapter, sizeProvider, 10);
                MRecycler.AddOnScrollListener(preLoader);
                MRecycler.SetAdapter(MAdapter);
                 
                RecyclerViewOnScrollListener xamarinRecyclerViewOnScrollListener = new RecyclerViewOnScrollListener(LayoutManager);
                MainScrollEvent = xamarinRecyclerViewOnScrollListener;
                MainScrollEvent.LoadMoreEvent += MainScrollEventOnLoadMoreEvent;
                MRecycler.AddOnScrollListener(xamarinRecyclerViewOnScrollListener);
                MainScrollEvent.IsLoading = false; 
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
                    string title = Context.GetString(Resource.String.Lbl_HotORNot);
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

        private void MAdapterOnHotItemClick(object sender, HotOrNotUserAdapterClickEventArgs e)
        {
            try
            {
                var item = MAdapter.UsersDateList[e.Position];
                if (item != null)
                {
                    if (Methods.CheckConnectivity())
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.AddHotAsync(item.Id.ToString()) });

                        var index = MAdapter.UsersDateList.IndexOf(item);
                        if (index != -1)
                        {
                            MAdapter.UsersDateList.RemoveAt(index);
                            MAdapter.NotifyItemRemoved(index);
                        } 
                    }
                    else
                    {
                        Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        
        private void MAdapterOnNotItemClick(object sender, HotOrNotUserAdapterClickEventArgs e)
        {
            try
            {
                var item = MAdapter.UsersDateList[e.Position];
                if (item != null)
                {
                    if (Methods.CheckConnectivity())
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.SetNotAsync(item.Id.ToString()) });

                        var index = MAdapter.UsersDateList.IndexOf(item);
                        if (index != -1)
                        {
                            MAdapter.UsersDateList.RemoveAt(index);
                            MAdapter.NotifyItemRemoved(index);
                        } 
                    }
                    else
                    {
                        Toast.MakeText(Activity, Activity.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                    }
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
                MAdapter.UsersDateList.Clear();
                MAdapter.NotifyDataSetChanged();

                StartApiService();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void MainScrollEventOnLoadMoreEvent(object sender, EventArgs e)
        {
            try
            {
                //Event Scroll #LastChat
                var item = MAdapter.UsersDateList.LastOrDefault();
                if (item != null && MAdapter.UsersDateList.Count > 10)
                    StartApiService(item.Id.ToString());
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get User By >> gender
        private void ActionButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var genderArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Gender;

                if (genderArray?.Count > 0)
                {
                    var dialogList = new MaterialDialog.Builder(Activity);

                    var arrayAdapter = genderArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())).ToList();
                    arrayAdapter.Insert(0, GetString(Resource.String.Lbl_Default));

                    dialogList.Title(GetText(Resource.String.Lbl_Gender));
                    dialogList.Items(arrayAdapter);
                    dialogList.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(this);
                    dialogList.AlwaysCallSingleChoiceCallback();
                    dialogList.ItemsCallback(this).Build().Show();
                }
                else
                {
                    Methods.DisplayReportResult(Activity, "List Gender Not Found, Please check api option ");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        #endregion

        #region Load Data Api 

        private void GetData()
        {
            try
            {
                if (MainContext.TrendingFragment.HotOrNotUserAdapter.UsersDateList.Count > 0)
                {
                    MAdapter.UsersDateList = MainContext.TrendingFragment.HotOrNotUserAdapter.UsersDateList;
                    MAdapter.NotifyDataSetChanged();

                    SwipeRefreshLayout.Refreshing = false; 
                }

                var lastId = MAdapter.UsersDateList.LastOrDefault()?.Id ?? 0; 
                StartApiService(lastId.ToString());
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void StartApiService(string offset = "")
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> {() => LoadHotOrNotAsync(offset) });
        }

        private async Task LoadHotOrNotAsync(string offset = "")
        {
            if (Methods.CheckConnectivity())
            {
                int countList = MAdapter.UsersDateList.Count;
                var (apiStatus, respond) = await RequestsAsync.Users.HotOrNotAsync(IdGender, "30", offset);
                if (apiStatus != 200 || !(respond is RandomUsersObject result) || result.Data == null)
                {
                    Methods.DisplayReportResult(Activity, respond);
                }
                else
                {
                    var respondList = result.Data.Count;
                    if (respondList > 0)
                    {
                        foreach (var item in from item in result.Data let check = MAdapter.UsersDateList.FirstOrDefault(a => a.Id == item.Id) where check == null select item)
                        {
                            MAdapter.UsersDateList.Add(item);
                        }

                        if (countList > 0)
                            Activity.RunOnUiThread(() => MAdapter.NotifyItemRangeInserted(countList - 1, MAdapter.UsersDateList.Count - countList));
                        else
                            Activity.RunOnUiThread(() => MAdapter.NotifyDataSetChanged());
                    }
                    else
                    {
                        if (MAdapter.UsersDateList.Count > 10 && !MRecycler.CanScrollVertically(1))
                            Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_NoMoreUsers), ToastLength.Short).Show();
                    }
                }

                MainScrollEvent.IsLoading = false;
                Activity.RunOnUiThread(ShowEmptyPage);
            }
            else
            {
                Inflated = EmptyStateLayout.Inflate();
                EmptyStateInflater x = new EmptyStateInflater();
                x.InflateLayout(Inflated, EmptyStateInflater.Type.NoConnection);
                if (!x.EmptyStateButton.HasOnClickListeners)
                {
                    x.EmptyStateButton.Click += null;
                    x.EmptyStateButton.Click += EmptyStateButtonOnClick;
                }

                Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            }
        }

        private void EmptyStateButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartApiService();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void ShowEmptyPage()
        {
            try
            {
                SwipeRefreshLayout.Refreshing = false;

                if (MAdapter.UsersDateList.Count > 0)
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

        #region MaterialDialog

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                var genderArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Gender?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                if (itemString.ToString() == GetString(Resource.String.Lbl_Default))
                {
                    var enumerable = ListUtils.SettingsSiteList.FirstOrDefault()?.Gender;
                    if (enumerable != null)
                    {
                        IdGender = "";
                        foreach (var id in enumerable.Select(upper => upper.Keys.FirstOrDefault()))
                        {
                            IdGender += id + ",";
                        }

                        IdGender = !string.IsNullOrEmpty(IdGender) ? IdGender.Remove(IdGender.Length - 1, 1) : "4525";
                    }
                }
                else
                {
                    IdGender = genderArray ?? "4525";
                }

                MAdapter.UsersDateList.Clear();    
                MAdapter.NotifyDataSetChanged();
                  
                EmptyStateLayout.Visibility = ViewStates.Gone;

                StartApiService();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

    }
}
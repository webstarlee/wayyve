using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Util;
using Liaoinstan.SpringViewLib.Widgets;
using Plugin.Geolocator;
using QuickDate.Activities.HotOrNot;
using QuickDate.Activities.HotOrNot.Adapters;
using QuickDate.Activities.SearchFilter;
using QuickDate.Activities.Tabbes.Adapters;
using QuickDate.ButtomSheets;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Global;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using Exception = System.Exception;

namespace QuickDate.Activities.Tabbes.Fragment
{
    public class TrendingFragment : Android.Support.V4.App.Fragment, SpringView.IOnFreshListener
    {
        #region Variables Basic

        private RecyclerView ProRecyclerView, NearByRecyclerView, RecylerHotOrNot;
        public NearByAdapter NearByAdapter;
        public CardAdapter2 CardDateAdapter2;
        private LinearLayoutManager LayoutManager;
        public HotOrNotUserAdapter HotOrNotUserAdapter;
        private ProgressBar ProgressBarLoader;
        private ViewStub EmptyStateLayout;
        private View Inflated;
        private HomeActivity GlobalContext;
        private TextView ToolbarTitle;
        private RecyclerViewOnScrollListener MainScrollEvent, MainScrollEventHotOrNot;
        private ImageView FilterButton;
        private LocationManager LocationManager;
        private bool ShowAlertDialogGps = true;
        private string LocationProvider;
        private int CountOffset;
        private AppBarLayout AppBarLayout;
        private ProUserAdapter ProUserAdapter;
        private SpringView SwipeRefreshLayout;
        private LinearLayout HotORNotLinear;
        private TextView IconHotORNot;
        private HotOrNotFragment HotOrNotFragment;
        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            GlobalContext = (HomeActivity)Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.TTrendingLayout, container, false);

                InitComponent(view);
                SetRecyclerViewAdapters();

                FilterButton.Click += FilterButtonOnClick;
                HotORNotLinear.Click += HotOrNotLinearOnClick;
                InitializeLocationManager();
                 
                StartApiService();

                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Functions
         
        private void InitComponent(View view)
        {
            try
            {
                FilterButton = view.FindViewById<ImageView>(Resource.Id.Filterbutton);
                NearByRecyclerView = (RecyclerView)view.FindViewById(Resource.Id.Recylerusers);
                ProgressBarLoader = (ProgressBar)view.FindViewById(Resource.Id.sectionProgress);
                EmptyStateLayout = view.FindViewById<ViewStub>(Resource.Id.viewStub);
                ToolbarTitle = view.FindViewById<TextView>(Resource.Id.toolbartitle);
                ProRecyclerView = (RecyclerView)view.FindViewById(Resource.Id.proRecyler);
                 
                HotORNotLinear = (LinearLayout)view.FindViewById(Resource.Id.HotORNotLinear);
                IconHotORNot = (TextView)view.FindViewById(Resource.Id.iconHotORNot);
                RecylerHotOrNot = (RecyclerView)view.FindViewById(Resource.Id.recylerHotOrNot);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconHotORNot, IonIconsFonts.IosFlameOutline);

                ToolbarTitle.Text = AppSettings.ApplicationName;

                SwipeRefreshLayout = (SpringView)view.FindViewById(Resource.Id.material_style_ptr_frame);
                SwipeRefreshLayout.SetType(SpringView.Type.Overlap);
                SwipeRefreshLayout.Header = new Helpers.PullSwipeStyles.DefaultHeader(Activity);
                SwipeRefreshLayout.Footer = new Helpers.PullSwipeStyles.DefaultFooter(Activity);
                SwipeRefreshLayout.Enable = true;
                SwipeRefreshLayout.SetListener(this);
                  
                AppBarLayout = (AppBarLayout)view.FindViewById(Resource.Id.appBarLayout);
                AppBarLayout.SetExpanded(false);

                ProgressBarLoader.Visibility = ViewStates.Visible;
                NearByRecyclerView.Visibility = ViewStates.Gone;
                HotORNotLinear.Visibility = ViewStates.Gone;
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
                if (AppSettings.ShowUsersAsCards)
                {
                    GridLayoutManager mLayoutManager = new GridLayoutManager(Activity, 2);
                    NearByRecyclerView.SetLayoutManager(mLayoutManager);
                    CardDateAdapter2 = new CardAdapter2(Activity);
                    NearByRecyclerView.SetAdapter(CardDateAdapter2);
                    CardDateAdapter2.OnItemClick += CardDateAdapter2_OnItemClick;

                    if (MainScrollEvent != null) return;
                    MainScrollEvent = new RecyclerViewOnScrollListener(mLayoutManager);
                    NearByRecyclerView.AddOnScrollListener(MainScrollEvent);
                    MainScrollEvent.LoadMoreEvent += SeconderScrollEventOnLoadMoreEvent;
                }
                else
                {
                    StaggeredGridLayoutManager nearByLayoutManager = new StaggeredGridLayoutManager(3, LinearLayoutManager.Vertical);
                    NearByRecyclerView.SetLayoutManager(nearByLayoutManager);
                    NearByAdapter = new NearByAdapter(Activity);
                    NearByAdapter.OnItemClick += NearByAdapterOnItemClick;
                    NearByRecyclerView.SetAdapter(NearByAdapter);

                    RecyclerViewOnScrollListener xamarinRecyclerViewOnScrollListener = new RecyclerViewOnScrollListener(nearByLayoutManager , 3);
                    MainScrollEvent = xamarinRecyclerViewOnScrollListener;
                    MainScrollEvent.LoadMoreEvent += SeconderScrollEventOnLoadMoreEvent;
                    NearByRecyclerView.AddOnScrollListener(xamarinRecyclerViewOnScrollListener);
                    MainScrollEvent.IsLoading = false;
                }        
                
                //=======================================================
                
                //Pro Recycler View 
                ProUserAdapter = new ProUserAdapter(Activity);
                ProUserAdapter.OnItemClick += ProUserAdapterOnItemClick;

                ProRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false));
                ProRecyclerView.SetItemViewCacheSize(20);
                ProRecyclerView.HasFixedSize = true;
                ProRecyclerView.NestedScrollingEnabled = false;
                ProRecyclerView.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProviderPro = new FixedPreloadSizeProvider(10, 10);
                var preLoaderPro = new RecyclerViewPreloader<UserInfoObject>(Activity, ProUserAdapter, sizeProviderPro, 10);
                ProRecyclerView.AddOnScrollListener(preLoaderPro);
                ProRecyclerView.SetAdapter(ProUserAdapter);
                
                //=======================================================

                //Hot Or Not Recycler View 
                HotOrNotUserAdapter = new HotOrNotUserAdapter(Activity)
                {
                    UsersDateList = new ObservableCollection<UserInfoObject>()
                };
                HotOrNotUserAdapter.HotItemClick += MAdapterOnHotItemClick;
                HotOrNotUserAdapter.NotItemClick += MAdapterOnNotItemClick;

                LayoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false);
                RecylerHotOrNot.SetLayoutManager(LayoutManager);
                RecylerHotOrNot.SetItemViewCacheSize(20);
                RecylerHotOrNot.HasFixedSize = true;
                RecylerHotOrNot.GetLayoutManager().ItemPrefetchEnabled = true;
                RecylerHotOrNot.NestedScrollingEnabled = false;
                //var sizeProviderPro2 = new FixedPreloadSizeProvider(10, 10);
                //var preLoaderPro2 = new RecyclerViewPreloader<UserInfoObject>(Activity, HotOrNotUserAdapter, sizeProviderPro2, 10);
                //RecylerHotOrNot.AddOnScrollListener(preLoaderPro2);
                RecylerHotOrNot.SetAdapter(HotOrNotUserAdapter);

                RecyclerViewOnScrollListener xamarinRecyclerViewOnScrollListenerHotOrNot = new RecyclerViewOnScrollListener(LayoutManager);
                MainScrollEventHotOrNot = xamarinRecyclerViewOnScrollListenerHotOrNot;
                MainScrollEventHotOrNot.LoadMoreEvent += MainScrollEventHotOrNotOnLoadMoreEvent;
                RecylerHotOrNot.AddOnScrollListener(xamarinRecyclerViewOnScrollListenerHotOrNot);
                MainScrollEventHotOrNot.IsLoading = false;
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
                var item = HotOrNotUserAdapter.UsersDateList[e.Position];
                if (item != null)
                {
                    if (Methods.CheckConnectivity())
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.AddHotAsync(item.Id.ToString()) });

                        var index = HotOrNotUserAdapter.UsersDateList.IndexOf(item);
                        if (index != -1)
                        {
                            HotOrNotUserAdapter.UsersDateList.RemoveAt(index);
                            HotOrNotUserAdapter.NotifyItemRemoved(index);
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
                var item = HotOrNotUserAdapter.UsersDateList[e.Position];
                if (item != null)
                {
                    if (Methods.CheckConnectivity())
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.SetNotAsync(item.Id.ToString()) });

                        var index = HotOrNotUserAdapter.UsersDateList.IndexOf(item);
                        if (index != -1)
                        {
                            HotOrNotUserAdapter.UsersDateList.RemoveAt(index);
                            HotOrNotUserAdapter.NotifyItemRemoved(index);
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


        //Open search filter
        private void FilterButtonOnClick(object sender, EventArgs e)
        {
            try
            { 
                if (AppSettings.ShowFilterBasic && !AppSettings.ShowFilterLooks && !AppSettings.ShowFilterBackground && !AppSettings.ShowFilterLifestyle && !AppSettings.ShowFilterMore)
                {
                    var searchFilter = new SearchFilterBottomDialogFragment();
                    searchFilter.Show(Activity.SupportFragmentManager, "searchFilter");
                }
                else
                {
                    Context.StartActivity(new Intent(Context,typeof(SearchFilterTabbedActivity)));
                }  
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open profile user >> Near By
        private void NearByAdapterOnItemClick(object sender, SuggestionsAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position > -1)
                {
                    var item = NearByAdapter.GetItem(e.Position);
                    if (item != null)
                    {
                        QuickDateTools.OpenProfile(Activity, "LikeAndMoveTrending", item, e.Image); 
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        private void CardDateAdapter2_OnItemClick(object sender, CardAdapter2ClickEventArgs e)
        {
            try
            {
                if (e.Position <= -1) return;
                var item = CardDateAdapter2.GetItem(e.Position);
                if (item != null) QuickDateTools.OpenProfile(Activity, "LikeAndMoveTrending", item, e.Image);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        //Open profile user  >> Pro User
        private void ProUserAdapterOnItemClick(object sender, ProUserAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position > -1)
                {
                    var item = ProUserAdapter.GetItem(e.Position);
                    if (item != null)
                    {
                        if (item.Type == "Your")
                        {
                            var window = new PopupController(Activity);
                            window.DisplayPremiumWindow();
                        }
                        else
                        {
                            QuickDateTools.OpenProfile(Activity, "Close", item, e.Image);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open profile user  >> Hot Or NotL
        private void HotOrNotUserAdapterOnOnItemClick(object sender, HotOrNotUserAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position > -1)
                {
                    var item = ProUserAdapter.GetItem(e.Position);
                    if (item != null)
                    {
                        QuickDateTools.OpenProfile(Activity, "Close", item, e.Image);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        // show all user Hot Or NotL
        private void HotOrNotLinearOnClick(object sender, EventArgs e)
        {
            try
            {
                HotOrNotFragment = new HotOrNotFragment();
                GlobalContext.FragmentBottomNavigator.DisplayFragment(HotOrNotFragment);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Scroll

        private void MainScrollEventHotOrNotOnLoadMoreEvent(object sender, EventArgs e)
        {
            try
            {
                //Event Scroll #LastChat
                var item = HotOrNotUserAdapter.UsersDateList.LastOrDefault();
                if (item != null && HotOrNotUserAdapter.UsersDateList.Count > 10)
                    LoadHotOrNotAsync(item.Id.ToString()).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SeconderScrollEventOnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            { 
                CountOffset = CountOffset + 1;
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadUsersAsync(CountOffset.ToString()) });  
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion

        #region Location

        private void InitializeLocationManager()
        {
            try
            {
                LocationManager = (LocationManager)Activity.GetSystemService(Context.LocationService);
                var criteriaForLocationService = new Criteria
                {
                    Accuracy = Accuracy.Fine
                };
                var acceptableLocationProviders = LocationManager.GetProviders(criteriaForLocationService, true);
                LocationProvider = acceptableLocationProviders.Any() ? acceptableLocationProviders.First() : string.Empty;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Get Position GPS Current Location
        private async Task GetPosition()
        {
            try
            {
                if (CrossGeolocator.Current.IsGeolocationAvailable)
                {
                    // Check if we're running on Android 5.0 or higher
                    if ((int)Build.VERSION.SdkInt < 23)
                    {
                        CheckAndGetLocation();
                    }
                    else
                    {
                        if (Context.CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted && Context.CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted)
                        {
                            CheckAndGetLocation();
                        }
                        else
                        {
                            new PermissionsController(Activity).RequestPermission(105);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void CheckAndGetLocation()
        {
            try
            {
                Activity.RunOnUiThread(async () =>
                {
                    if (!LocationManager.IsProviderEnabled(LocationManager.GpsProvider))
                    {
                        if (ShowAlertDialogGps)
                        {
                            ShowAlertDialogGps = false;

                            Activity.RunOnUiThread(() =>
                            {
                                try
                                {
                                    // Call your Alert message
                                    AlertDialog.Builder alert = new AlertDialog.Builder(Context);
                                    alert.SetTitle(GetString(Resource.String.Lbl_Use_Location) + "?");
                                    alert.SetMessage(GetString(Resource.String.Lbl_GPS_is_disabled) + "?");

                                    alert.SetPositiveButton(GetString(Resource.String.Lbl_Ok), (senderAlert, args) =>
                                    {
                                        //Open intent Gps
                                        new IntentController(Activity).OpenIntentGps(LocationManager);
                                    });

                                    alert.SetNegativeButton(GetString(Resource.String.Lbl_Cancel), (senderAlert, args) => { });

                                    Dialog gpsDialog = alert.Create();
                                    gpsDialog.Show();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }
                            });
                        }
                    }
                    else
                    {
                        var locator = CrossGeolocator.Current;
                        locator.DesiredAccuracy = 50;
                        var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(10000));
                        Console.WriteLine("Position Status: {0}", position.Timestamp);
                        Console.WriteLine("Position Latitude: {0}", position.Latitude);
                        Console.WriteLine("Position Longitude: {0}", position.Longitude);

                        UserDetails.Lat = position.Latitude.ToString(CultureInfo.InvariantCulture);
                        UserDetails.Lng = position.Longitude.ToString(CultureInfo.InvariantCulture);

                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadUsersAsync() });
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Load Data Api 
         
        private void StartApiService()
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { LoadProUser,() =>  LoadHotOrNotAsync(), LoadUser  });
        }
         
        private async Task LoadProUser()
        {
            var data = ListUtils.MyUserInfo?.FirstOrDefault(a => a.Id == UserDetails.UserId);
            if (data?.IsPro != "1")
            {
                if (!AppSettings.EnableAppFree)
                {
                    var dataOwner = ProUserAdapter.ProUserList.FirstOrDefault(a => a.Type == "Your");
                    if (dataOwner == null && data != null)
                    {
                        data.Type = "Your";
                        data.Username = Context.GetText(Resource.String.Lbl_AddMe);
                        data.IsOwner = true;
                        ProUserAdapter.ProUserList.Insert(0, data);
                            
                        Activity.RunOnUiThread(() => { ProUserAdapter.NotifyItemInserted(0); });
                    } 
                }
            }
             
            if (Methods.CheckConnectivity())
            {
                int countList = ProUserAdapter.ProUserList.Count;
                (int apiStatus, var respond) = await RequestsAsync.Users.GetProAsync("25");
                if (apiStatus != 200 || !(respond is SearchObject result) || result.Data == null)
                {
                    Methods.DisplayReportResult(Activity, respond);
                }
                else
                {
                    var respondList = result.Data.Count;
                    if (respondList > 0)
                    {
                        if (countList > 0)
                        {
                            foreach (var item in from item in result.Data let check = ProUserAdapter.ProUserList.FirstOrDefault(a => a.Id == item.Id) where check == null select item)
                            {
                                ProUserAdapter.ProUserList.Add(item);
                            }

                            Activity.RunOnUiThread(() => { ProUserAdapter.NotifyItemRangeInserted(countList - 1, ProUserAdapter.ProUserList.Count - countList); });
                        }
                        else
                        {
                            ProUserAdapter.ProUserList = new ObservableCollection<UserInfoObject>(result.Data);
                            Activity.RunOnUiThread(() =>{ProUserAdapter.NotifyDataSetChanged(); });
                        }
                    }
                    else
                    {
                        if (ProUserAdapter.ProUserList.Count > 10 && !ProRecyclerView.CanScrollVertically(1))
                            Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_NoMoreUsers), ToastLength.Short).Show();
                    }
                }

                MainScrollEvent.IsLoading = false;

            }
            else
            {
                Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            }
        }

        public async Task LoadUser()
        {
            // Check if we're running on Android 5.0 or higher
            if ((int)Build.VERSION.SdkInt < 23)
                await GetPosition();
            else
            {
                if (Context.CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted && Context.CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted)
                    await GetPosition();
                else
                    new PermissionsController(Activity).RequestPermission(105);
            }
        }

        private async Task LoadUsersAsync(string offset = "0")
        {
            if (Methods.CheckConnectivity())
            { 
                if (UserDetails.Lat == "" && UserDetails.Lng == "")
                {
                    var locator = CrossGeolocator.Current;
                    locator.DesiredAccuracy = 50;
                    var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(10000));
                    Console.WriteLine("Position Status: {0}", position.Timestamp);
                    Console.WriteLine("Position Latitude: {0}", position.Latitude);
                    Console.WriteLine("Position Longitude: {0}", position.Longitude);

                    UserDetails.Lat = position.Latitude.ToString(CultureInfo.InvariantCulture);
                    UserDetails.Lng = position.Longitude.ToString(CultureInfo.InvariantCulture);
                }
                 
                UserDetails.Location = ListUtils.MyUserInfo.FirstOrDefault()?.Location;

                var dictionary = new Dictionary<string, string>
                {
                    {"limit", "35"},
                    {"offset", offset},
                    {"_gender", UserDetails.Gender},
                    {"_located", UserDetails.Located},
                    {"_location", UserDetails.Location},
                    {"_age_from", UserDetails.AgeMin.ToString()},
                    {"_age_to",  UserDetails.AgeMax.ToString()},
                    {"_lat", UserDetails.Lat},
                    {"_lng", UserDetails.Lng},

                    {"_body", UserDetails.Body ?? ""},
                    {"_ethnicity", UserDetails.Ethnicity ?? ""},
                    {"_religion", UserDetails.Religion ?? ""},
                    {"_drink", UserDetails.Drink ?? ""},
                    {"_smoke", UserDetails.Smoke ?? ""},
                    {"_education", UserDetails.Education ?? ""},
                    {"_pets", UserDetails.Pets ?? ""},
                    {"_relationship", UserDetails.RelationShip ?? ""},
                    {"_language", UserDetails.Language ?? "english"},
                    {"_interest", UserDetails.Interest ?? ""},
                    {"_height_from", UserDetails.FromHeight ?? "139"},
                    {"_height_to", UserDetails.ToHeight ?? "220"},
                };
                 
                int countList = AppSettings.ShowUsersAsCards ? CardDateAdapter2.UsersDateList.Count : NearByAdapter.NearByList.Count;  
                var (apiStatus, respond) = await RequestsAsync.Users.SearchAsync(dictionary);
                if (apiStatus != 200 || !(respond is SearchObject result) || result.Data == null)
                {
                    Methods.DisplayReportResult(Activity, respond);
                }
                else
                {
                    var respondList = result.Data.Count;
                    if (respondList > 0)
                    {
                        foreach (var item in from item in result.Data let check = AppSettings.ShowUsersAsCards ? CardDateAdapter2.UsersDateList.FirstOrDefault(a => a.Id == item.Id) : NearByAdapter.NearByList.FirstOrDefault(a => a.Id == item.Id) where check == null select item)
                        {
                            if (UserDetails.SwitchState)
                            {
                                var online = QuickDateTools.GetStatusOnline(item.Lastseen, item.Online);
                                if (online)
                                {
                                    if (AppSettings.ShowUsersAsCards)
                                        CardDateAdapter2.UsersDateList.Add(item);
                                    else
                                        NearByAdapter.NearByList.Add(item); 
                                }
                            }
                            else
                            {
                                if (AppSettings.ShowUsersAsCards)
                                    CardDateAdapter2.UsersDateList.Add(item);
                                else
                                    NearByAdapter.NearByList.Add(item);
                            }
                        }

                        if (AppSettings.ShowUsersAsCards)
                        {
                            if (countList > 0)
                                Activity.RunOnUiThread(() => CardDateAdapter2.NotifyItemRangeInserted(countList - 1, CardDateAdapter2.UsersDateList.Count - countList)); 
                            else
                                Activity.RunOnUiThread(() => CardDateAdapter2.NotifyDataSetChanged());
                        }
                        else
                        {
                            if (countList > 0)
                                Activity.RunOnUiThread(() => NearByAdapter.NotifyItemRangeInserted(countList - 1, NearByAdapter.NearByList.Count - countList)); 
                            else
                                Activity.RunOnUiThread(() => NearByAdapter.NotifyDataSetChanged());
                        }
                    }
                    else
                    {
                        if (AppSettings.ShowUsersAsCards)
                        {
                            if (CardDateAdapter2.UsersDateList.Count > 10 && !NearByRecyclerView.CanScrollVertically(1))
                                Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_NoMoreUsers), ToastLength.Short).Show();
                        }
                        else
                        {
                            if (NearByAdapter.NearByList.Count > 10 && !NearByRecyclerView.CanScrollVertically(1))
                                Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_NoMoreUsers), ToastLength.Short).Show();
                        } 
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
         
        private async Task LoadHotOrNotAsync(string offset = "0")
        {
            if (Methods.CheckConnectivity())
            {
                int countList = HotOrNotUserAdapter.UsersDateList.Count;
                var (apiStatus, respond) = await RequestsAsync.Users.HotOrNotAsync("","30", offset);
                if (apiStatus != 200 || !(respond is RandomUsersObject result) || result.Data == null)
                {
                    Methods.DisplayReportResult(Activity, respond);
                }
                else
                {
                    var respondList = result.Data.Count;
                    if (respondList > 0)
                    {
                        foreach (var item in from item in result.Data let check = HotOrNotUserAdapter.UsersDateList.FirstOrDefault(a => a.Id == item.Id) where check == null select item)
                        {
                            HotOrNotUserAdapter.UsersDateList.Add(item);
                        }

                        if (countList > 0)
                            Activity.RunOnUiThread(() => HotOrNotUserAdapter.NotifyItemRangeInserted(countList - 1, HotOrNotUserAdapter.UsersDateList.Count - countList));
                        else
                            Activity.RunOnUiThread(() => HotOrNotUserAdapter.NotifyDataSetChanged());
                    }
                    else
                    {
                        if (HotOrNotUserAdapter.UsersDateList.Count > 10 && !RecylerHotOrNot.CanScrollVertically(1))
                            Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_NoMoreUsers), ToastLength.Short).Show();
                    }
                }

                Activity.RunOnUiThread(() =>
                {
                    if (HotOrNotUserAdapter?.UsersDateList?.Count > 0)
                    {
                        RecylerHotOrNot.Visibility = ViewStates.Visible;
                        HotORNotLinear.Visibility = ViewStates.Visible;
                        EmptyStateLayout.Visibility = ViewStates.Gone;
                    }
                });
            }
            else
            {
                Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            }
        }
          
        private void ShowEmptyPage()
        {
            try
            {
                SwipeRefreshLayout.OnFinishFreshAndLoad();

                if (ProgressBarLoader.Visibility == ViewStates.Visible)
                {
                    ProgressBarLoader.Visibility = ViewStates.Gone;
                    AppBarLayout.SetExpanded(true);
                }
                 
                if (CardDateAdapter2?.UsersDateList?.Count > 0 || NearByAdapter?.NearByList?.Count > 0)
                {
                    NearByRecyclerView.Visibility = ViewStates.Visible;
                    EmptyStateLayout.Visibility = ViewStates.Gone;
                }  

                if (HotOrNotUserAdapter?.UsersDateList?.Count > 0 )
                {
                    RecylerHotOrNot.Visibility = ViewStates.Visible;
                    HotORNotLinear.Visibility = ViewStates.Visible;
                    EmptyStateLayout.Visibility = ViewStates.Gone;
                }  
                 
                if (CardDateAdapter2?.UsersDateList?.Count == 0 || NearByAdapter?.NearByList?.Count == 0 && HotOrNotUserAdapter?.UsersDateList?.Count == 0)
                {
                    NearByRecyclerView.Visibility = ViewStates.Gone;
                    RecylerHotOrNot.Visibility = ViewStates.Gone;
                    HotORNotLinear.Visibility = ViewStates.Gone;

                    if (Inflated == null)
                        Inflated = EmptyStateLayout.Inflate();

                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(Inflated, EmptyStateInflater.Type.NoSearchResult);
                    if (!x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click += null;
                    }

                    EmptyStateLayout.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                ProgressBarLoader.Visibility = ViewStates.Gone;
                SwipeRefreshLayout.OnFinishFreshAndLoad();
                Console.WriteLine(e);
            }
        }

        //No Internet Connection 
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

        #endregion

        #region Refresh

        public void OnRefresh()
        {
            try
            {
                ProUserAdapter.ProUserList.Clear();
                ProUserAdapter.NotifyDataSetChanged();

                if (AppSettings.ShowUsersAsCards)
                {
                    CardDateAdapter2.UsersDateList.Clear();
                    CardDateAdapter2.NotifyDataSetChanged();
                }
                else
                {
                    NearByAdapter.NearByList.Clear();
                    NearByAdapter.NotifyDataSetChanged();
                }

                ProgressBarLoader.Visibility = ViewStates.Gone;
                EmptyStateLayout.Visibility = ViewStates.Gone;
                NearByRecyclerView.Visibility = ViewStates.Visible;

                StartApiService(); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnLoadMore()
        {
            try
            {
                //Code get last id where LoadMore >>
                if (MainScrollEvent.IsLoading == false)
                {
                    MainScrollEvent.IsLoading = true;
                    CountOffset = CountOffset + 1;
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadUsersAsync(CountOffset.ToString()) }); 
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
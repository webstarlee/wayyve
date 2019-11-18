using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Util;
using QuickDate.Activities.Tabbes.Adapters;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Common;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.Tabbes.Fragment
{
    public class NotificationsFragment : Android.Support.V4.App.Fragment
    {
        #region Variables Basic

       private RecyclerView MRecycler;
       private ViewStub EmptyStateLayout;
       private View Inflated;
       private LinearLayoutManager NotifyLayoutManager;
       private Toolbar ToolbarView;
       private NotificationsAdapter MAdapter;
       private SwipeRefreshLayout SwipeRefreshLayout;
       private RelativeLayout MatchesButton, LikesButton, VisitsButton;
       private LinearLayout MatchesLayout, LikesLayout, VisitsLayout;
       private ImageView MatchesImage, LikesImage, VisitsImage;
       private TextView MatchesTextView, LikesTextView, VisitsTextView;
       private LinearLayout TabButtons;
        private HomeActivity GlobalContext;
        private RecyclerViewOnScrollListener MainScrollEvent;

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
                View view = inflater.Inflate(Resource.Layout.TNotificationsLayout, container, false);

                InitComponent(view);
                InitToolbar(view);
                SetRecyclerViewAdapters();

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    Activity.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                    Activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                    Activity.Window.SetStatusBarColor(Color.ParseColor("#DB2251"));
                }

                MatchesButton.Click += MatchesButtonOnClick;
                LikesButton.Click += LikesButtonOnClick;
                VisitsButton.Click += VisitsButtonOnClick;
                SwipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;

                StartApiService();

                if (Methods.CheckConnectivity())
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> {() => LoadMatchAsync()});
                 
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
                TabButtons = view.FindViewById<LinearLayout>(Resource.Id.TabButtons);
                MRecycler = (RecyclerView)view.FindViewById(Resource.Id.NotifcationRecyler);
                EmptyStateLayout = view.FindViewById<ViewStub>(Resource.Id.viewStub);
                MatchesButton = view.FindViewById<RelativeLayout>(Resource.Id.MatchesButton);
                LikesButton = view.FindViewById<RelativeLayout>(Resource.Id.LikesButton);
                VisitsButton = view.FindViewById<RelativeLayout>(Resource.Id.VisitsButton);
                MatchesLayout = view.FindViewById<LinearLayout>(Resource.Id.bt1);
                VisitsLayout = view.FindViewById<LinearLayout>(Resource.Id.bt2);
                LikesLayout = view.FindViewById<LinearLayout>(Resource.Id.bt3);

                MatchesImage = view.FindViewById<ImageView>(Resource.Id.ImageView1);
                VisitsImage = view.FindViewById<ImageView>(Resource.Id.ImageView2);
                LikesImage = view.FindViewById<ImageView>(Resource.Id.ImageView3);

                MatchesTextView = view.FindViewById<TextView>(Resource.Id.text1);
                LikesTextView = view.FindViewById<TextView>(Resource.Id.text3);
                VisitsTextView = view.FindViewById<TextView>(Resource.Id.text2);

                SwipeRefreshLayout = (SwipeRefreshLayout)view.FindViewById(Resource.Id.swipeRefreshLayout);
                SwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
                SwipeRefreshLayout.Refreshing = true;
                SwipeRefreshLayout.Enabled = true;

                MatchesButton.Tag = "Clicked";

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
                ToolbarView = view.FindViewById<Toolbar>(Resource.Id.toolbar);
                ToolbarView.Title = GetString(Resource.String.Lbl_Notifications);
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
                MAdapter = new NotificationsAdapter(Activity);
                MAdapter.NotificationsList = ListUtils.MatchList;
                  
                NotifyLayoutManager = new LinearLayoutManager(Activity);
                MRecycler.SetLayoutManager(NotifyLayoutManager);
                MRecycler.SetItemViewCacheSize(20);
                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(10);
                MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<GetNotificationsObject.Datum>(Activity, MAdapter, sizeProvider, 10);
                MRecycler.AddOnScrollListener(preLoader);
                MRecycler.SetAdapter(MAdapter);
                 
                MAdapter.OnItemClick += MAdapterOnItemClick;

                TranslateAnimation animation1 = new TranslateAnimation(1500.0f, 0.0f, 0.0f, 0.0f); // new TranslateAnimation(xFrom,xTo, yFrom,yTo)
                animation1.Duration = 500; // animation duration
                animation1.FillAfter = true;
                TabButtons.StartAnimation(animation1);
                animation1 = new TranslateAnimation(0.0f, 0.0f, 1500.0f, 0.0f);
                animation1.Duration = 700; // animation duration
                MRecycler.StartAnimation(animation1);


                RecyclerViewOnScrollListener xamarinRecyclerViewOnScrollListener = new RecyclerViewOnScrollListener(NotifyLayoutManager);
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

        #endregion

        #region Event

        //Scroll
        private void MainScrollEventOnLoadMoreEvent(object sender, EventArgs e)
        {
            try
            {
                //Code get last id where LoadMore >>
                var item = MAdapter.NotificationsList.LastOrDefault();
                if (item != null && !string.IsNullOrEmpty(item.Id.ToString()))
                {
                    if (MatchesButton.Tag.ToString() == "Clicked")
                    {
                        if (Methods.CheckConnectivity())
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadMatchAsync(item.Id.ToString()) }); 
                    }
                    else if (VisitsButton.Tag.ToString() == "Clicked")
                    {
                        if (Methods.CheckConnectivity())
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadVisitsAsync(item.Id.ToString()) });
                    } 
                    else  
                    {
                        StartApiService(item.Id.ToString());
                    } 
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open user profile
        private void MAdapterOnItemClick(object sender, NotificationsAdapterClickEventArgs e)
        {
            try
            {
                if (e.Position > -1)
                {
                    var item = MAdapter.GetItem(e.Position);
                    if (item != null)
                    {
                        string eventPage; 
                        switch (item.Type)
                        {
                            case "got_new_match":
                                eventPage = "HideButton";
                                break;
                            case "like":
                                eventPage = "likeAndClose";
                                break;
                            default:
                                eventPage = "Close";
                                break;
                        } 
                        QuickDateTools.OpenProfile(Activity, eventPage, item.Notifier, e.Image);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        private void VisitsButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                MAdapter.NotificationsList = ListUtils.VisitsList;
                MAdapter.NotifyDataSetChanged();

                ToolbarView.SetBackgroundResource(Resource.Drawable.Shape_Gradient_Normal);
                VisitsImage.SetColorFilter(Color.ParseColor("#ffffff"));
                VisitsTextView.SetTextColor(Color.ParseColor(AppSettings.MainColor));
                VisitsLayout.SetBackgroundResource(Resource.Drawable.Shape_Radius_Gradient_Btn);
                VisitsButton.Tag = "Clicked";

                ResetTabsButtonOnVistsClick();
                 
                ShowEmptyPage();

                if (Methods.CheckConnectivity())
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadVisitsAsync() });


                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                    return;

                Activity.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Activity.Window.SetStatusBarColor(Color.ParseColor(AppSettings.MainColor)); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void LikesButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                MAdapter.NotificationsList = ListUtils.LikesList;
                MAdapter.NotifyDataSetChanged();

                ToolbarView.SetBackgroundResource(Resource.Drawable.Shape_Gradient_Normal2);
                LikesImage.SetColorFilter(Color.ParseColor("#ffffff"));
                LikesTextView.SetTextColor(Color.ParseColor(AppSettings.MainColor));
                LikesLayout.SetBackgroundResource(Resource.Drawable.Shape_Radius_Gradient_Btn2);
                LikesButton.Tag = "Clicked";

                ResetTabsButtonOnLikesClick();

                ShowEmptyPage();

                if (Methods.CheckConnectivity())
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { LoadLikesAsync });

                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                    return;

                Activity.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Activity.Window.SetStatusBarColor(Color.ParseColor(AppSettings.MainColor)); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void MatchesButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                MAdapter.NotificationsList = ListUtils.MatchList;
                MAdapter.NotifyDataSetChanged();

                ToolbarView.SetBackgroundResource(Resource.Drawable.Shape_Gradient_Normal3);
                MatchesImage.SetColorFilter(Color.ParseColor("#ffffff"));
                MatchesTextView.SetTextColor(Color.ParseColor("#DB2251"));
                MatchesLayout.SetBackgroundResource(Resource.Drawable.Shape_Radius_Gradient_Btn3);
                MatchesButton.Tag = "Clicked";

                ResetTabsButtonOnMatchClick();
                  
                ShowEmptyPage();

                if (Methods.CheckConnectivity())
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadMatchAsync() });
                 
                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                    return;

                Activity.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Activity.Window.SetStatusBarColor(Color.ParseColor("#DB2251")); 
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
                ListUtils.MatchList.Clear();
                ListUtils.LikesList.Clear();
                ListUtils.VisitsList.Clear();

                MAdapter.NotificationsList.Clear();
                MAdapter.NotifyDataSetChanged();

                if (MatchesButton.Tag.ToString() == "Clicked")
                {
                    if (Methods.CheckConnectivity())
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadMatchAsync() });
                }
                else if (VisitsButton.Tag.ToString() == "Clicked")
                {
                    if (Methods.CheckConnectivity())
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadVisitsAsync() });
                }
                else
                {
                    StartApiService();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception); 
            } 
        }

        #endregion

        #region Set Tab

        private void ResetTabsButtonOnMatchClick()
        {
            try
            {
                LikesImage.SetColorFilter(Color.ParseColor("#A1A1A1"));
                LikesTextView.SetTextColor(Color.ParseColor("#A1A1A1"));
                LikesLayout.SetBackgroundResource(Resource.Drawable.Shape_Radius_Line_Grey);
                LikesButton.Tag = "UnClicked";
                VisitsImage.SetColorFilter(Color.ParseColor("#A1A1A1"));
                VisitsTextView.SetTextColor(Color.ParseColor("#A1A1A1"));
                VisitsLayout.SetBackgroundResource(Resource.Drawable.Shape_Radius_Line_Grey);
                VisitsButton.Tag = "UnClicked";
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            } 
        }

        private void ResetTabsButtonOnVistsClick()
        {
            try
            {
                MatchesImage.SetColorFilter(Color.ParseColor("#A1A1A1"));
                MatchesTextView.SetTextColor(Color.ParseColor("#A1A1A1"));
                MatchesLayout.SetBackgroundResource(Resource.Drawable.Shape_Radius_Line_Grey);
                MatchesButton.Tag = "UnClicked";

                LikesImage.SetColorFilter(Color.ParseColor("#A1A1A1"));
                LikesTextView.SetTextColor(Color.ParseColor("#A1A1A1"));
                LikesLayout.SetBackgroundResource(Resource.Drawable.Shape_Radius_Line_Grey);
                LikesButton.Tag = "UnClicked";
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            } 
        }

        private void ResetTabsButtonOnLikesClick()
        {
            try
            {
                MatchesImage.SetColorFilter(Color.ParseColor("#A1A1A1"));
                MatchesTextView.SetTextColor(Color.ParseColor("#A1A1A1"));
                MatchesLayout.SetBackgroundResource(Resource.Drawable.Shape_Radius_Line_Grey);
                MatchesButton.Tag = "UnClicked";

                VisitsImage.SetColorFilter(Color.ParseColor("#A1A1A1"));
                VisitsTextView.SetTextColor(Color.ParseColor("#A1A1A1"));
                VisitsLayout.SetBackgroundResource(Resource.Drawable.Shape_Radius_Line_Grey);
                VisitsButton.Tag = "UnClicked";
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            }  
        }

        #endregion
         
        #region Load Data 
         
        private void StartApiService(string offset = "")
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => LoadDataAsync(offset) });
        }

        private async Task LoadDataAsync(string offset = "")
        {
            if (Methods.CheckConnectivity())
            {
                int countList = MAdapter.NotificationsList.Count;
                (int apiStatus, var respond) = await RequestsAsync.Common.GetNotificationsAsync("25", offset, UserDetails.DeviceId);
                if (apiStatus != 200 || !(respond is GetNotificationsObject result) || result.Data == null)
                {
                    Methods.DisplayReportResult(Activity, respond);
                }
                else
                {
                    var respondList = result.Data.Count;
                    if (respondList > 0)
                    { 
                        foreach (var item in result.Data)
                        {
                            item.Text = QuickDateTools.GetNotification(item);
                        }

                        if (countList > 0)
                        { 
                            Activity.RunOnUiThread(() => { MAdapter.NotifyItemRangeInserted(countList - 1, MAdapter.NotificationsList.Count - countList); });
                        }
                        else
                        { 
                            Activity.RunOnUiThread(() => { MAdapter.NotifyDataSetChanged(); });
                        }
                    }
                    else
                    {
                        if (MAdapter.NotificationsList.Count > 10 && !MRecycler.CanScrollVertically(1))
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

        private async Task LoadMatchAsync(string offset = "")
        {
            if (Methods.CheckConnectivity())
            {
                int countList = MAdapter.NotificationsList.Count;
                (int apiStatus, var respond) = await RequestsAsync.Users.MatchesAsync(offset);
                if (apiStatus != 200 || !(respond is MatchesObject result) || result.Data == null)
                {
                    Methods.DisplayReportResult(Activity, respond);
                }
                else
                {
                    var respondList = result.Data.Count;
                    if (respondList > 0)
                    {
                        foreach (var item in result.Data)
                        { 
                           var check = ListUtils.MatchList.FirstOrDefault(a => a.Notifier.Id == item.Id);
                           if (check == null)
                           {
                               ListUtils.MatchList.Add(new GetNotificationsObject.Datum()
                               {
                                   Text = Context.GetText(Resource.String.Lbl_YouGotMatch),
                                   Notifier = item,
                                   NotifierId = item.Id,
                                   Type = "got_new_match"
                               }); 
                           }
                        }

                        MAdapter.NotificationsList = ListUtils.MatchList;
                        Activity.RunOnUiThread(() => { MAdapter.NotifyDataSetChanged(); });
                    }
                    else
                    {
                        if (MAdapter.NotificationsList.Count > 10 && !MRecycler.CanScrollVertically(1))
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
        
        private async Task LoadVisitsAsync(string offset = "")
        {
            if (Methods.CheckConnectivity())
            {
                int countList = MAdapter.NotificationsList.Count;
                (int apiStatus, var respond) = await RequestsAsync.Users.ListVisitsAsync("25", offset);
                if (apiStatus != 200 || !(respond is ListVisitsObject result) || result.Data == null)
                {
                    Methods.DisplayReportResult(Activity, respond);
                }
                else
                {
                    var respondList = result.Data.Count;
                    if (respondList > 0)
                    {
                        foreach (var item in result.Data)
                        { 
                           var check = ListUtils.VisitsList.FirstOrDefault(a => a.Notifier.Id == item.Id);
                           if (check == null)
                           {
                               ListUtils.VisitsList.Add(new GetNotificationsObject.Datum()
                               {
                                   Text = Context.GetText(Resource.String.Lbl_VisitYou),
                                   Notifier = item,
                                   NotifierId = item.Id,
                                   Type = "visit"
                               }); 
                           }
                        }

                        MAdapter.NotificationsList = ListUtils.VisitsList;
                        Activity.RunOnUiThread(() => { MAdapter.NotifyDataSetChanged(); }); 
                    }
                    else
                    {
                        if (MAdapter.NotificationsList.Count > 10 && !MRecycler.CanScrollVertically(1))
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
         
        private async Task LoadLikesAsync()
        {
            if (Methods.CheckConnectivity())
            {
                int countList = MAdapter.NotificationsList.Count;
                (int apiStatus, var respond) = await RequestsAsync.Users.ProfileAsync(UserDetails.UserId.ToString(), "likes");
                if (apiStatus != 200 || !(respond is ProfileObject result) || result.Data == null)
                {
                    Methods.DisplayReportResult(Activity, respond);
                }
                else
                {
                    var respondList = result.Data.Likes.Count;
                    if (respondList > 0)
                    {
                        foreach (var item in result.Data.Likes)
                        {
                            var check = ListUtils.LikesList.FirstOrDefault(a => a.Notifier.Id == item.Id);
                            if (check == null)
                            {
                                ListUtils.MatchList.Add(new GetNotificationsObject.Datum()
                                {
                                    Text = Context.GetText(Resource.String.Lbl_LikeYou),
                                    Notifier = item.Data,
                                    NotifierId = item.Data.Id,
                                    Type = "like"
                                });
                            }
                        }

                        MAdapter.NotificationsList = ListUtils.LikesList;
                        Activity.RunOnUiThread(() => { MAdapter.NotifyDataSetChanged(); });
                    }
                    else
                    {
                        if (MAdapter.NotificationsList.Count > 10 && !MRecycler.CanScrollVertically(1))
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
         
        private void ShowEmptyPage()
        {
            try
            {
                SwipeRefreshLayout.Refreshing = false;

                if (MAdapter.NotificationsList.Count > 0)
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
                    x.InflateLayout(Inflated, EmptyStateInflater.Type.NoNotifications);
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
    }
}
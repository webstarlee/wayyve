using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Gigamole.Navigationtabbar.Ntb;
using Com.Theartofdev.Edmodo.Cropper;
using Java.IO;
 using Newtonsoft.Json;
using QuickDate.Activities.Chat;
using QuickDate.Activities.Chat.Service;
using QuickDate.Activities.Tabbes.Fragment;
using QuickDate.Activities.UserProfile;
using QuickDate.ButtomSheets;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.OneSignal;
using QuickDate.SQLite;
using QuickDateClient;
using QuickDateClient.Classes.Authorization;
using QuickDateClient.Classes.Chat;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using Xamarin.PayPal.Android;
using Console = System.Console;
using Uri = Android.Net.Uri;
using BigDecimal = Java.Math.BigDecimal;
using Exception = System.Exception;
using Permission = Android.Content.PM.Permission;

namespace QuickDate.Activities.Tabbes
{
    [Activity(Icon = "@drawable/icon",Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Keyboard | ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenLayout | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode)]
    public class HomeActivity : AppCompatActivity, ServiceResultReceiver.IReceiver 
    {
        #region Variables Basic

        public CardMachFragment CardFragment;
        public TrendingFragment TrendingFragment;
        public NotificationsFragment NotificationsFragment;
        public ProfileFragment ProfileFragment;
        public NavigationTabBar NavigationTabBar;
        public FragmentBottomNavigationView FragmentBottomNavigator;
        
        private string  TypeAvatar = "";
        public static int CountNotificationsStatic, CountMessagesStatic;
        private static HomeActivity Instance;
        public TracksCounter TracksCounter;
        private PowerManager.WakeLock Wl;
        private ServiceResultReceiver Receiver;
        private readonly Handler ExitHandler = new Handler();
        private bool RecentlyBackPressed;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                AddFlagsWakeLock();

                // Create your application here
                SetContentView(Resource.Layout.TabbedMainLayout);

                Instance = this;

                TracksCounter = new TracksCounter(this);

                CardFragment = new CardMachFragment();
                TrendingFragment = new TrendingFragment();
                NotificationsFragment = new NotificationsFragment();
                ProfileFragment = new ProfileFragment();

                if (AppSettings.EnableAppFree)
                {
                    AppSettings.PremiumSystemEnabled = false;
                }
                 
                //Get Value
                SetupBottomNavigationView();
                 
                GetMyInfoData();
                 
                string type = Intent.GetStringExtra("TypeNotification") ?? "Don't have type";
                if (!string.IsNullOrEmpty(type) && type != "Don't have type")
                {
                    //var result = DbDatabase.Get_data_Login_Credentials();

                    var intent = new Intent(this, typeof(UserProfileActivity));

                    if (type == "got_new_match")
                    {
                        intent.PutExtra("EventPage", "HideButton");
                    }
                    else if (type == "like")
                    {
                        intent.PutExtra("EventPage", "likeAndClose");
                    }
                    else
                    {
                        intent.PutExtra("EventPage", "Close");
                    }

                    intent.PutExtra("DataType", "OneSignal");
                    intent.PutExtra("ItemUser", JsonConvert.SerializeObject(OneSignalNotification.UserData));
                    StartActivity(intent);
                }

                SetService();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                
                SetWakeLock();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                
                OffWakeLock();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            { 
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
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
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static HomeActivity GetInstance()
        {
            try
            {
                return Instance;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                if (AppSettings.ShowPaypal)
                    StopService(new Intent(this, typeof(PayPalService)));

                ProfileFragment?.Time?.SetStopTimer();

                OffWakeLock();
                 
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        #endregion

        #region Functions

        public void SetToolBar(Android.Support.V7.Widget.Toolbar toolbar, string title, bool showIconBack = true)
        {
            try
            {
                if (toolbar != null)
                {
                    if (!string.IsNullOrEmpty(title))
                        toolbar.Title = title;

                    toolbar.SetTitleTextColor(AppSettings.TitleTextColor);
                    SetSupportActionBar(toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(showIconBack);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetupBottomNavigationView()
        {
            try
            {
                NavigationTabBar = FindViewById<NavigationTabBar>(Resource.Id.ntb_horizontal);
                FragmentBottomNavigator = new FragmentBottomNavigationView(this);

                CardFragment = new CardMachFragment();
                
                TrendingFragment = new TrendingFragment();
                NotificationsFragment = new NotificationsFragment();
                ProfileFragment = new ProfileFragment();

                FragmentBottomNavigator.FragmentListTab0.Add(CardFragment);

                if (AppSettings.ShowTrending)
                    FragmentBottomNavigator.FragmentListTab1.Add(TrendingFragment);

                FragmentBottomNavigator.FragmentListTab2.Add(NotificationsFragment);
                FragmentBottomNavigator.FragmentListTab4.Add(ProfileFragment);

                FragmentBottomNavigator.SetupNavigation(NavigationTabBar);
                NavigationTabBar.SetModelIndex(0, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion

        #region Events

        //Open Chat 
        public void ShowChat()
        {
            try
            {
                //Convert to fragment
                Intent Int = new Intent(this, typeof(LastChatActivity));
                StartActivity(Int);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void ShowMessagesBox(UserInfoObject dataUser)
        {
            try
            {
                Intent Int = new Intent(this, typeof(MessagesBoxActivity));
                Int.PutExtra("UserId", dataUser.Id.ToString());
                Int.PutExtra("TypeChat", "LastChat");
                Int.PutExtra("UserItem", JsonConvert.SerializeObject(dataUser));

                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    StartActivity(Int);
                }
                else
                {
                    //Check to see if any permission in our group is available, if one, then all are
                    if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                    {
                        StartActivity(Int);

                    }
                    else
                        new PermissionsController(this).RequestPermission(100);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        public void OpenDialogGallery(string typeAvatar = "")
        {
            try
            {
                TypeAvatar = typeAvatar;
                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    Methods.Path.Chack_MyFolder();

                    //Open Image 
                    var myUri = Uri.FromFile(new File(Methods.Path.FolderDcimImage, Methods.GetTimestamp(DateTime.Now) + ".jpeg"));
                    CropImage.Builder()
                        .SetInitialCropWindowPaddingRatio(0)
                        .SetAutoZoomEnabled(true)
                        .SetMaxZoom(4)
                        .SetGuidelines(CropImageView.Guidelines.On)
                        .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                        .SetOutputUri(myUri).Start(this);
                }
                else
                {
                    if (!CropImage.IsExplicitCameraPermissionRequired(this) && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted && CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted)
                    {
                        Methods.Path.Chack_MyFolder();

                        //Open Image 
                        var myUri = Uri.FromFile(new File(Methods.Path.FolderDcimImage, Methods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder()
                            .SetInitialCropWindowPaddingRatio(0)
                            .SetAutoZoomEnabled(true)
                            .SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On)
                            .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                            .SetOutputUri(myUri).Start(this);
                    }
                    else
                    {
                        //request Code 108
                        new PermissionsController(this).RequestPermission(108);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OpenAddPhotoFragment()
        {
            try
            {
                var addPhoto = new AddPhotoBottomDialogFragment();
                addPhoto.Show(SupportFragmentManager, "addPhoto");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Permissions && Result

        //Result
        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == 108 || requestCode == CropImage.CropImageActivityRequestCode)
                {
                    if (Methods.CheckConnectivity())
                    {
                        var result = CropImage.GetActivityResult(data);
                        if (result.IsSuccessful)
                        {
                            var resultPathImage = result.Uri.Path;
                            if (!string.IsNullOrEmpty(resultPathImage))
                            {
                                if (TypeAvatar != "Avatar")
                                {
                                    GlideImageLoader.LoadImage(this,resultPathImage, ProfileFragment?.ProfileImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.UpdateAvatarApi(this, resultPathImage) });
                                }
                                else
                                {
                                    //sent api  
                                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Users.UploadImageUserAsync(resultPathImage) });
                                }
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Long).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_something_went_wrong), ToastLength.Long).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 1050) //1050
                {
                    //Get Location And Get Data Api
                    TrendingFragment?.CheckAndGetLocation();
                }
                else if (requestCode == PayPalDataRequestCode)
                {
                    switch (resultCode)
                    {
                        case Result.Ok:
                            var confirmObj = data.GetParcelableExtra(PaymentActivity.ExtraResultConfirmation);
                            PaymentConfirmation configuration = Android.Runtime.Extensions.JavaCast<PaymentConfirmation>(confirmObj);
                            if (configuration != null)
                            {
                                string createTime = configuration.ProofOfPayment.CreateTime;
                                string intent = configuration.ProofOfPayment.Intent;
                                string paymentId = configuration.ProofOfPayment.PaymentId;
                                string state = configuration.ProofOfPayment.State;
                                string transactionId = configuration.ProofOfPayment.TransactionId;

                                if (PayType != "membership")
                                {
                                    if (Methods.CheckConnectivity())
                                    {
                                        (int apiStatus, var respond) = await RequestsAsync.Auth.SetCreditAsync(Credits, Price, "PayPal").ConfigureAwait(false);
                                        if (apiStatus == 200)
                                        {
                                            if (respond is SetCreditObject result)
                                            {
                                                RunOnUiThread(() =>
                                                {
                                                    var dataUser = ListUtils.MyUserInfo.FirstOrDefault(a => a.Id == UserDetails.UserId);
                                                    if (dataUser != null)
                                                    {
                                                        dataUser.Balance = result.Balance;

                                                        var sqlEntity = new SqLiteDatabase();
                                                        sqlEntity.InsertOrUpdate_DataMyInfo(dataUser);
                                                        sqlEntity.Dispose();
                                                    }

                                                    if (ProfileFragment.WalletNumber != null)
                                                        ProfileFragment.WalletNumber.Text = result.Balance;

                                                    Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Long).Show();
                                                });
                                            }
                                        }
                                        else Methods.DisplayReportResult(this, respond);

                                    }
                                    else
                                    {
                                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                                    }
                                }
                                else
                                {
                                    if (Methods.CheckConnectivity())
                                    {
                                        (int apiStatus, var respond) = await RequestsAsync.Auth.SetProAsync(Id, Price, "PayPal").ConfigureAwait(false);
                                        if (apiStatus == 200)
                                        {
                                            RunOnUiThread(() =>
                                            {
                                                var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                                                if (dataUser != null)
                                                {
                                                    dataUser.VerifiedFinal = true;
                                                    dataUser.IsPro = "1";

                                                    var sqlEntity = new SqLiteDatabase();
                                                    sqlEntity.InsertOrUpdate_DataMyInfo(dataUser);
                                                    sqlEntity.Dispose();
                                                }

                                                Toast.MakeText(this, GetText(Resource.String.Lbl_Done),
                                                    ToastLength.Long).Show();
                                            });
                                        }
                                        else Methods.DisplayReportResult(this, respond);
                                    }
                                    else
                                    {
                                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                                    }
                                }
                            }

                            break;
                        case Result.Canceled:
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Canceled), ToastLength.Long).Show();
                            break;
                    } 
                }
                else if (requestCode == PaymentActivity.ResultExtrasInvalid)
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Invalid), ToastLength.Long).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 105)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        //Get Location And Get Data Api
                        TrendingFragment?.CheckAndGetLocation();
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 108)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        OpenDialogGallery(TypeAvatar);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
                else if(requestCode == 110)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
                else if(requestCode == 100)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        ShowMessagesBox(DialogController.DataUser);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Back Pressed 

        public override void OnBackPressed()
        {
            try
            {
                if (FragmentBottomNavigator.GetCountFragment() > 0)
                {
                    FragmentBottomNavigator.BackStackClickFragment();
                }
                else
                {
                    if (RecentlyBackPressed)
                    {
                        ExitHandler.RemoveCallbacks(() => { RecentlyBackPressed = false; });
                        RecentlyBackPressed = false;
                        MoveTaskToBack(true);
                    }
                    else
                    {
                        RecentlyBackPressed = true;
                        Toast.MakeText(this, GetString(Resource.String.press_again_exit), ToastLength.Long).Show();
                        ExitHandler.PostDelayed(() => { RecentlyBackPressed = false; }, 2000L);
                    }
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Timer
         
        public async void GetNotifications()
        {
            try
            {
                if (FragmentBottomNavigator.Models != null)
                {
                    var (countNotifications, countMessages) = await ApiRequest.GetCountNotifications(this).ConfigureAwait(false);
                    NavigationTabBar.Model tabNotifications = FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_Notifications));
                    NavigationTabBar.Model tabMessages = FragmentBottomNavigator.Models.First(a => a.Title == GetText(Resource.String.Lbl_messages));
                    if (tabNotifications != null && countNotifications != 0 && countNotifications != CountNotificationsStatic)
                    {
                        RunOnUiThread(() =>
                        {
                            try
                            {
                                CountNotificationsStatic = countNotifications;
                                tabNotifications.BadgeTitle = countNotifications.ToString();
                                tabNotifications.UpdateBadgeTitle(countNotifications.ToString());
                                tabNotifications.ShowBadge();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        });
                    }

                    if (tabMessages != null && countMessages != 0 && countMessages != CountMessagesStatic)
                    {
                        RunOnUiThread(() =>
                        {
                            try
                            {
                                CountMessagesStatic = countMessages;
                                tabMessages.BadgeTitle = countMessages.ToString();
                                tabMessages.UpdateBadgeTitle(countMessages.ToString());
                                tabMessages.ShowBadge();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        });
                    } 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            }
        }
         
        #endregion

        private void GetMyInfoData()
        {
            try
            {
                var dbDatabase = new SqLiteDatabase();
                 
                var settingsData = dbDatabase.GetSettings();
                if (settingsData == null) 
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetSettings_Api(this) });

                var myInfoData = dbDatabase.GetDataMyInfo();
                if (myInfoData == null)
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetInfoData(this, UserDetails.UserId.ToString()) });
                 
                var listFavorite = dbDatabase.GetDataFavorite();

                if (ListUtils.FavoriteUserList.Count == 0 && listFavorite?.Count > 0)
                    ListUtils.FavoriteUserList = listFavorite;
                 
                dbDatabase.Dispose(); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        #region WakeLock System

        public void AddFlagsWakeLock()
        {
            try
            {
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.WakeLock) == Permission.Granted)
                    {
                        Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                    }
                    else
                    {
                        //request Code 110
                        new PermissionsController(this).RequestPermission(110);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetWakeLock()
        {
            try
            {
                if (Wl == null)
                {
                    PowerManager pm = (PowerManager)GetSystemService(PowerService);
                    Wl = pm.NewWakeLock(WakeLockFlags.ScreenBright, "My Tag");
                    Wl.Acquire();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetOnWakeLock()
        {
            try
            {
                PowerManager pm = (PowerManager)GetSystemService(PowerService);
                Wl = pm.NewWakeLock(WakeLockFlags.ScreenBright, "My Tag");
                Wl.Acquire();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetOffWakeLock()
        {
            try
            {
                PowerManager pm = (PowerManager)GetSystemService(PowerService);
                Wl = pm.NewWakeLock(WakeLockFlags.ProximityScreenOff, "My Tag");
                Wl.Acquire();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OffWakeLock()
        {
            try
            {
                // ..screen will stay on during this section..
                Wl?.Release();
                Wl = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Service Chat

        public void SetService(bool run = true)
        {
            try
            {
                if (run)
                {
                    try
                    {
                        Receiver = new ServiceResultReceiver(new Handler());
                        Receiver.SetReceiver(this);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    var intent = new Intent(this, typeof(ScheduledApiService));
                    intent.PutExtra("receiverTag", Receiver);
                    StartService(intent);
                }
                else
                {
                    var intentService = new Intent(this, typeof(ScheduledApiService));
                    StopService(intentService);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnReceiveResult(int resultCode, Bundle resultData)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<GetConversationListObject>(resultData.GetString("Json"));
                if (result != null)
                {
                    LastChatActivity.GetInstance()?.LoadDataJsonLastChat(result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                // Toast.MakeText(Application.Context, "Exception  " + e, ToastLength.Short).Show();
            }
        }

        #endregion

        #region PayPal

        private static PayPalConfiguration PayPalConfig;
        private PayPalPayment PayPalPayment;
        private Intent IntentService;
        private readonly int PayPalDataRequestCode = 7171;
        private string Price, PayType, Credits, Id;

        //Paypal
        public void BtnPaypalOnClick(string price, string payType, string credits, string id)
        {
            try
            {
                InitPayPal(price, payType, credits, id);
                 
                Intent intent = new Intent(this, typeof(PaymentActivity));
                intent.PutExtra(PayPalService.ExtraPaypalConfiguration, PayPalConfig);
                intent.PutExtra(PaymentActivity.ExtraPayment, PayPalPayment);
                StartActivityForResult(intent, PayPalDataRequestCode);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void InitPayPal(string price, string payType, string credits, string id)
        {
            try
            {
                Price = price;
                PayType = payType;
                Credits = credits;
                Id = id;
                 
                //PayerID
                string currency = "USD";
                string paypalClintId = "";
                var option = ListUtils.SettingsSiteList.FirstOrDefault();
                if (option != null)
                {
                    currency = option?.Currency ?? "USD";
                    paypalClintId = option?.PaypalId;
                }

                PayPalConfig = new PayPalConfiguration()
                    .Environment(PayPalConfiguration.EnvironmentProduction)
                    .ClientId(paypalClintId)
                    .LanguageOrLocale(AppSettings.Lang)
                    .MerchantName(AppSettings.ApplicationName)
                    .MerchantPrivacyPolicyUri(Uri.Parse(Client.WebsiteUrl + "/privacy"));

                PayPalPayment = new PayPalPayment(new BigDecimal(price), currency, "Pay the card",PayPalPayment.PaymentIntentSale);

                IntentService = new Intent(this, typeof(PayPalService));

                IntentService.PutExtra(PayPalService.ExtraPaypalConfiguration, PayPalConfig);
                StartService(IntentService);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
         
    }
}
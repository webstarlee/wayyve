using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Org.Json;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.SocialLogins;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient;
using QuickDateClient.Classes.Authorization;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;
using Object = Java.Lang.Object;
using Profile = Xamarin.Facebook.Profile;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.Default
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/ProfileTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class LoginActivity : AppCompatActivity, IFacebookCallback, GraphRequest.IGraphJSONObjectCallback
    {
        #region Variables Basic

        private EditText EmailEditText, PasswordEditText;
        private ProgressBar ProgressBar;
        private Button BtnSignIn;
        private TextView ForgotPassTextView, TermsTextView;
        private LinearLayout LinearRegister;

        private LoginButton FbLoginButton;

        private ICallbackManager MFbCallManager;
        private FbMyProfileTracker MprofileTracker; 
        public Toolbar Toolbar;
        #endregion

        #region General
 
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                //Set Full screen 
                View mContentView = Window.DecorView;
                var uiOptions = (int)mContentView.SystemUiVisibility;
                var newUiOptions = uiOptions;

                newUiOptions |= (int)SystemUiFlags.Fullscreen;
                newUiOptions |= (int)SystemUiFlags.HideNavigation;
                mContentView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;

                Window.AddFlags(WindowManagerFlags.Fullscreen);

                // Create your application here
                SetContentView(Resource.Layout.LoginLayout);
                 
                var client = new Client(AppSettings.TripleDesAppServiceProvider);

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
                InitSocialLogins();
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
                AddOrRemoveEvent(true);
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
                AddOrRemoveEvent(false);
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
         
        #endregion 
         
        #region Functions

        private void InitComponent()
        {
            try
            {
                EmailEditText = FindViewById<EditText>(Resource.Id.edt_email);
                PasswordEditText = FindViewById<EditText>(Resource.Id.edt_password);
                ProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
                BtnSignIn = FindViewById<Button>(Resource.Id.SignInButton);
                ForgotPassTextView = FindViewById<TextView>(Resource.Id.txt_forgot_pass);
                TermsTextView = FindViewById<TextView>(Resource.Id.terms);
                LinearRegister = FindViewById<LinearLayout>(Resource.Id.tvRegister);

                ProgressBar.Visibility = ViewStates.Invisible;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitToolbar()
        {
            try
            {
                Toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (Toolbar != null)
                {
                    //Toolbar.Title = GetString(Resource.String.Lbl_SignIn);
                    Toolbar.Title = " ";
                    Toolbar.SetTitleTextColor(AppSettings.TitleTextColor);
                    SetSupportActionBar(Toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(false);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitSocialLogins()
        {
            try
            {
                //#Facebook
                if (AppSettings.ShowFacebookLogin)
                { 
                    MprofileTracker = new FbMyProfileTracker();
                    MprofileTracker.MOnProfileChanged += MProfileTrackerOnMOnProfileChanged;
                    MprofileTracker.StartTracking();

                    FbLoginButton = FindViewById<LoginButton>(Resource.Id.fblogin_button);
                    FbLoginButton.Visibility = ViewStates.Visible;
                    //FbLoginButton.SetReadPermissions(new List<string>
                    //{
                    //    "email",
                    //    "public_profile"
                    //});

                    MFbCallManager = CallbackManagerFactory.Create();
                    FbLoginButton.RegisterCallback(MFbCallManager, this);

                    //FB accessToken
                    var accessToken = AccessToken.CurrentAccessToken;
                    var isLoggedIn = accessToken != null && !accessToken.IsExpired;
                    if (isLoggedIn && Profile.CurrentProfile != null)
                    {
                        LoginManager.Instance.LogOut();
                    }

                    string hash = Methods.App.GetKeyHashesConfigured(this);
                    Console.WriteLine(hash);
                }
                else
                {
                    FbLoginButton = FindViewById<LoginButton>(Resource.Id.fblogin_button);
                    FbLoginButton.Visibility = ViewStates.Gone;
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    BtnSignIn.Click += BtnSignInOnClick;
                    ForgotPassTextView.Click += ForgotPassTextViewOnClick;
                    TermsTextView.Click += TermsTextViewOnClick;
                    LinearRegister.Click += LinearRegisterOnClick;
                }
                else
                {
                    BtnSignIn.Click -= BtnSignInOnClick;
                    ForgotPassTextView.Click -= ForgotPassTextViewOnClick;
                    TermsTextView.Click -= TermsTextViewOnClick;
                    LinearRegister.Click -= LinearRegisterOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        //Login QuickDate
        private async void BtnSignInOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (!string.IsNullOrEmpty(EmailEditText.Text.Replace(" ", "")) || !string.IsNullOrEmpty(PasswordEditText.Text))
                    {
                        ProgressBar.Visibility = ViewStates.Visible;
                        BtnSignIn.Visibility = ViewStates.Gone;
                        ForgotPassTextView.Visibility = ViewStates.Gone;

                        (int apiStatus, var respond) = await RequestsAsync.Auth.LoginAsync(EmailEditText.Text.Replace(" ", ""), PasswordEditText.Text,UserDetails.DeviceId);
                        if (apiStatus == 200)
                        {
                            if (respond is LoginObject auth)
                            {
                                SetDataLogin(auth);
                                StartActivity(AppSettings.ShowWalkTroutPage ? new Intent(this, typeof(BoardingActivity)) : new Intent(this, typeof(HomeActivity)));
                                FinishAffinity();
                            }
                        }
                        else if (apiStatus == 400)
                        {
                            if (respond is ErrorObject error)
                            {
                                string errorText = error.Message;
                                int errorId = error.Code;
                                switch (errorId)
                                {
                                    case 1:
                                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_1), GetText(Resource.String.Lbl_Ok));
                                        break;
                                    case 2:
                                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_2), GetText(Resource.String.Lbl_Ok));
                                        break;
                                    case 3:
                                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_3), GetText(Resource.String.Lbl_Ok));
                                        break;
                                    case 4:
                                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_4), GetText(Resource.String.Lbl_Ok));
                                        break;
                                    case 5:
                                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_5), GetText(Resource.String.Lbl_Ok));
                                        break;
                                    default:
                                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), errorText, GetText(Resource.String.Lbl_Ok));
                                        break;
                                } 
                            }

                            ProgressBar.Visibility = ViewStates.Gone;
                            BtnSignIn.Visibility = ViewStates.Visible;
                            ForgotPassTextView.Visibility = ViewStates.Visible;
                        }
                        else if (apiStatus == 404)
                        {
                            ProgressBar.Visibility = ViewStates.Gone;
                            BtnSignIn.Visibility = ViewStates.Visible;
                            ForgotPassTextView.Visibility = ViewStates.Visible;
                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), respond.ToString(), GetText(Resource.String.Lbl_Ok));
                        }
                    }
                    else
                    {
                        ProgressBar.Visibility = ViewStates.Gone;
                        BtnSignIn.Visibility = ViewStates.Visible;
                        ForgotPassTextView.Visibility = ViewStates.Visible;
                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_Please_enter_your_data), GetText(Resource.String.Lbl_Ok));
                    }
                }
                else
                {
                    ProgressBar.Visibility = ViewStates.Gone;
                    BtnSignIn.Visibility = ViewStates.Visible;
                    ForgotPassTextView.Visibility = ViewStates.Visible;
                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_CheckYourInternetConnection), GetText(Resource.String.Lbl_Ok));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ProgressBar.Visibility = ViewStates.Gone;
                BtnSignIn.Visibility = ViewStates.Visible;
                ForgotPassTextView.Visibility = ViewStates.Visible;
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), ex.Message, GetText(Resource.String.Lbl_Ok));

            }
        }

        //Open Register
        private void LinearRegisterOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(this, typeof(RegisterActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Terms of use 
        private void TermsTextViewOnClick(object sender, EventArgs e)
        {
            try
            {
                var url = Client.WebsiteUrl + "/terms-of-use";
                Methods.App.OpenbrowserUrl(this, url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //Open Forgot Password
        private void ForgotPassTextViewOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(this, typeof(ForgotPasswordActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        //Login With Facebook
        private void MProfileTrackerOnMOnProfileChanged(object sender, OnProfileChangedEventArgs e)
        {
            try
            {
                if (e.MProfile != null)
                {
                    //FbFirstName = e.MProfile.FirstName;
                    //FbLastName = e.MProfile.LastName;
                    //FbName = e.MProfile.Name;
                    //FbProfileId = e.MProfile.Id;

                    var request = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, this);
                    var parameters = new Bundle();
                    parameters.PutString("fields", "id,name,age_range,email");
                    request.Parameters = parameters;
                    request.ExecuteAsync();
                } 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void SetDataLogin(LoginObject auth)
        {
            try
            {
                UserDetails.Username = EmailEditText.Text;
                UserDetails.FullName = EmailEditText.Text;
                UserDetails.Password = PasswordEditText.Text;
                UserDetails.AccessToken = auth.Data.AccessToken;
                UserDetails.UserId = auth.Data.UserId;
                UserDetails.Status = "Pending";
                UserDetails.Cookie = auth.Data.AccessToken;
                UserDetails.Email = EmailEditText.Text;

                Current.AccessToken = auth.Data.AccessToken;

                //Insert user data to database
                var user = new DataTables.LoginTb
                {
                    UserId = UserDetails.UserId.ToString(),
                    AccessToken = UserDetails.AccessToken,
                    Cookie = UserDetails.Cookie,
                    Username = EmailEditText.Text,
                    Password = PasswordEditText.Text,
                    Status = "Pending",
                    Lang = "",
                    DeviceId = UserDetails.DeviceId,
                };
                ListUtils.DataUserLoginList.Add(user);

                var dbDatabase = new SqLiteDatabase();
                dbDatabase.InsertOrUpdateLogin_Credentials(user);

                if (auth.Data.UserInfo != null)
                {
                    dbDatabase.InsertOrUpdate_DataMyInfo(auth.Data.UserInfo);

                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetInfoData(this,UserDetails.UserId.ToString()) });
                }
                 
                dbDatabase.Dispose(); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion 

        #region Social Logins

        private string FbAccessToken;

        #region Facebook

        public void OnCancel()
        {
            try
            {
                ProgressBar.Visibility = ViewStates.Gone;
                BtnSignIn.Visibility = ViewStates.Visible;
                ForgotPassTextView.Visibility = ViewStates.Visible;

                SetResult(Result.Canceled);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnError(FacebookException error)
        {
            try
            {

                ProgressBar.Visibility = ViewStates.Gone;
                BtnSignIn.Visibility = ViewStates.Visible;
                ForgotPassTextView.Visibility = ViewStates.Visible;

                // Handle e
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), error.Message, GetText(Resource.String.Lbl_Ok));

                SetResult(Result.Canceled);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public void OnSuccess(Object result)
        {
            try
            {
                var loginResult = result as LoginResult;
                var id = AccessToken.CurrentAccessToken.UserId;

                ProgressBar.Visibility = ViewStates.Visible;
                BtnSignIn.Visibility = ViewStates.Gone;
                ForgotPassTextView.Visibility = ViewStates.Gone;

                SetResult(Result.Ok);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async void OnCompleted(JSONObject json, GraphResponse response)
        {
            try
            {
                //var data = json.ToString();
                //var result = JsonConvert.DeserializeObject<FacebookResult>(data);
                //FbEmail = result.Email;

                ProgressBar.Visibility = ViewStates.Visible;
                BtnSignIn.Visibility = ViewStates.Gone;
                ForgotPassTextView.Visibility = ViewStates.Gone;

                var accessToken = AccessToken.CurrentAccessToken;
                if (accessToken != null)
                {
                    FbAccessToken = accessToken.Token;

                    //Login Api 
                    (int apiStatus, var respond) = await RequestsAsync.Auth.SocialLoginAsync(FbAccessToken, "facebook",UserDetails.DeviceId);
                    if (apiStatus == 200)
                    {
                        if (respond is LoginObject auth)
                        {
                            SetDataLogin(auth);

                            StartActivity(AppSettings.ShowWalkTroutPage ? new Intent(this, typeof(BoardingActivity)) : new Intent(this, typeof(HomeActivity)));
                            FinishAffinity();  
                        }
                    }
                    else if (apiStatus == 400)
                    {
                        if (respond is ErrorObject error)
                        {
                            string errorText = error.Message;
                            int errorId = error.Code;
                            switch (errorId)
                            {
                                case 1:
                                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_1), GetText(Resource.String.Lbl_Ok));
                                    break;
                                case 2:
                                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_2), GetText(Resource.String.Lbl_Ok));
                                    break;
                                case 3:
                                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_3), GetText(Resource.String.Lbl_Ok));
                                    break;
                                case 4:
                                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_4), GetText(Resource.String.Lbl_Ok));
                                    break;
                                case 5:
                                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_5), GetText(Resource.String.Lbl_Ok));
                                    break;
                                default:
                                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), errorText, GetText(Resource.String.Lbl_Ok));
                                    break;
                            }
                        }

                        ProgressBar.Visibility = ViewStates.Gone;
                        BtnSignIn.Visibility = ViewStates.Visible;
                        ForgotPassTextView.Visibility = ViewStates.Visible;
                    }
                    else if (apiStatus == 404)
                    {
                        ProgressBar.Visibility = ViewStates.Gone;
                        BtnSignIn.Visibility = ViewStates.Visible;
                        ForgotPassTextView.Visibility = ViewStates.Visible;
                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), respond.ToString(), GetText(Resource.String.Lbl_Ok));
                    }
                }
            }
            catch (Exception e)
            {
                ProgressBar.Visibility = ViewStates.Gone;
                BtnSignIn.Visibility = ViewStates.Visible;
                ForgotPassTextView.Visibility = ViewStates.Visible;
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), e.Message, GetText(Resource.String.Lbl_Ok));
                Console.WriteLine(e);
            }
        }
         
        #endregion

        //======================================================
        
        #endregion

        #region Permissions && Result

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                // Logins Facebook
                MFbCallManager.OnActivityResult(requestCode, (int)resultCode, data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
         
    }
}
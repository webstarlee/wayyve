using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Lang;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient;
using QuickDateClient.Classes.Authorization;
using QuickDateClient.Classes.Global;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using Exception = System.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.Default
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/ProfileTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class RegisterActivity : AppCompatActivity, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private Toolbar Toolbar;
        private EditText FirstNameEditText, LastNameEditText, EmailEditText, UsernameEditText, PasswordEditText, ConfirmPasswordEditText, GenderEditText;
        private Button RegisterButton;
        private LinearLayout TermsLayout, SignLayout;
        private ProgressBar ProgressBar;
        private int IdGender = 4525;
        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                View mContentView = Window.DecorView;
                var uiOptions = (int)mContentView.SystemUiVisibility;
                var newUiOptions = uiOptions;

                newUiOptions |= (int)SystemUiFlags.Fullscreen;
                newUiOptions |= (int)SystemUiFlags.HideNavigation;
                mContentView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;

                Window.AddFlags(WindowManagerFlags.Fullscreen);

                base.OnCreate(savedInstanceState);

                // Create your application here
                SetContentView(Resource.Layout.RegisterLayout);

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();

                var client = new Client(AppSettings.TripleDesAppServiceProvider);
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
                FirstNameEditText = FindViewById<EditText>(Resource.Id.edt_firstname);
                LastNameEditText = FindViewById<EditText>(Resource.Id.edt_lastname);
                EmailEditText = FindViewById<EditText>(Resource.Id.edt_email);
                UsernameEditText = FindViewById<EditText>(Resource.Id.edt_username);
                PasswordEditText = FindViewById<EditText>(Resource.Id.edt_password);
                ConfirmPasswordEditText = FindViewById<EditText>(Resource.Id.edt_Confirmpassword);
                GenderEditText = FindViewById<EditText>(Resource.Id.edt_gender);

                ProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
                RegisterButton = FindViewById<Button>(Resource.Id.SignInButton);
                TermsLayout = FindViewById<LinearLayout>(Resource.Id.termsLayout);
                SignLayout = FindViewById<LinearLayout>(Resource.Id.SignLayout); 

                Methods.SetFocusable(GenderEditText);
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
                    //Toolbar.Title = GetString(Resource.String.Lbl_Register);
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
         
        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    RegisterButton.Click += RegisterButtonOnClick;
                    TermsLayout.Click += TermsLayoutOnClick;
                    SignLayout.Click += SignLayoutOnClick;
                    GenderEditText.Touch += GenderEditTextOnTouch;
                }
                else
                {
                    RegisterButton.Click -= RegisterButtonOnClick;
                    TermsLayout.Click -= TermsLayoutOnClick;
                    SignLayout.Click -= SignLayoutOnClick;
                    GenderEditText.Touch -= GenderEditTextOnTouch;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion

        #region Events

        //Open Login Activity
        private void SignLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(this, typeof(LoginActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Terms of Service
        private void TermsLayoutOnClick(object sender, EventArgs e)
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

        //Register QuickDate
        private async void RegisterButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (IdGender == -1)
                    {
                        ProgressBar.Visibility = ViewStates.Gone;
                        RegisterButton.Visibility = ViewStates.Visible;
                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_Please_enter_your_data), GetText(Resource.String.Lbl_Ok));
                        return;
                    }

                    if (!string.IsNullOrEmpty(EmailEditText.Text.Replace(" ", "")) || !string.IsNullOrEmpty(UsernameEditText.Text.Replace(" ", "")) ||
                        !string.IsNullOrEmpty(FirstNameEditText.Text.Replace(" ", "")) || !string.IsNullOrEmpty(LastNameEditText.Text.Replace(" ", "")) ||
                        !string.IsNullOrEmpty(PasswordEditText.Text) || !string.IsNullOrEmpty(ConfirmPasswordEditText.Text))
                    { 
                        var check = Methods.FunString.IsEmailValid(EmailEditText.Text);
                        if (!check)
                        {
                            Methods.DialogPopup.InvokeAndShowDialog(this,GetText(Resource.String.Lbl_VerificationFailed),GetText(Resource.String.Lbl_IsEmailValid), GetText(Resource.String.Lbl_Ok));
                        }
                        else
                        {
                            if (PasswordEditText.Text != ConfirmPasswordEditText.Text)
                            {
                                ProgressBar.Visibility = ViewStates.Gone;
                                RegisterButton.Visibility = ViewStates.Visible;
                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),GetText(Resource.String.Lbl_Error_Register_password),GetText(Resource.String.Lbl_Ok));
                            }
                            else
                            {
                                ProgressBar.Visibility = ViewStates.Visible;
                                RegisterButton.Visibility = ViewStates.Gone;

                                (int apiStatus, var respond) = await RequestsAsync.Auth.RegisterAsync(FirstNameEditText.Text.Replace(" ", ""), LastNameEditText.Text.Replace(" ", ""), UsernameEditText.Text.Replace(" ", ""), EmailEditText.Text.Replace(" ", ""), PasswordEditText.Text, UserDetails.DeviceId);
                                if (apiStatus == 200)
                                {
                                    if (respond is RegisterObject auth)
                                    { 
                                        if (auth.SuccessType == "registered")
                                        {
                                            SetDataRegister(auth);

                                            StartActivity(AppSettings.ShowWalkTroutPage ? new Intent(this, typeof(BoardingActivity)) : new Intent(this, typeof(HomeActivity))); 
                                            FinishAffinity();  
                                        }
                                        else if (auth.SuccessType == "confirm_account")
                                        { 
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
                                            dbDatabase.Dispose();
                                             
                                            Intent Int = new Intent(this, typeof(VerificationAccountActivity)); 
                                            Int.PutExtra("Email", EmailEditText.Text);
                                            StartActivity(Int); 
                                            Finish();
                                        }
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
                                            case 6:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_6), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 7:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_7), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 8:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_8), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 9:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_9), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 10:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_10), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 11:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_11), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 12:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_12), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 13:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_13), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 14:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_14), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 15:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_15), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 16:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_16), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 17:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_17), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            case 18:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_18), GetText(Resource.String.Lbl_Ok));
                                                break;
                                            default:
                                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), errorText, GetText(Resource.String.Lbl_Ok));
                                                break;
                                        }
                                    }

                                    ProgressBar.Visibility = ViewStates.Gone;
                                    RegisterButton.Visibility = ViewStates.Visible; 
                                }
                                else if (apiStatus == 404)
                                {
                                    ProgressBar.Visibility = ViewStates.Gone;
                                    RegisterButton.Visibility = ViewStates.Visible;
                                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), respond.ToString(), GetText(Resource.String.Lbl_Ok));
                                }
                            }
                        }
                    }
                    else
                    {
                        ProgressBar.Visibility = ViewStates.Gone;
                        RegisterButton.Visibility = ViewStates.Visible;
                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_Please_enter_your_data), GetText(Resource.String.Lbl_Ok));
                    }
                }
                else
                {
                    ProgressBar.Visibility = ViewStates.Gone;
                    RegisterButton.Visibility = ViewStates.Visible;
                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_CheckYourInternetConnection), GetText(Resource.String.Lbl_Ok));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ProgressBar.Visibility = ViewStates.Gone;
                RegisterButton.Visibility = ViewStates.Visible;
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), ex.Message, GetText(Resource.String.Lbl_Ok));

            }
        }

        private async void SetDataRegister(RegisterObject auth)
        {
            try
            {
                if (auth.Data != null)
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
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetInfoData(this, UserDetails.UserId.ToString()) });
                    }

                    if (IdGender > -1)
                    {
                        if (Methods.CheckConnectivity())
                        {
                            var dictionary = new Dictionary<string, string>
                            {
                                {"gender", IdGender.ToString()},
                            };

                            (int apiStatus, var respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                            if (apiStatus == 200)
                            {
                                if (respond is UpdateProfileObject result)
                                {
                                    Console.WriteLine(result.Message);
                                    var local = ListUtils.MyUserInfo.FirstOrDefault();
                                    if (local != null)
                                    { 
                                        local.Gender = IdGender.ToString();
                                         
                                        dbDatabase.InsertOrUpdate_DataMyInfo(local); 
                                    }
                                }
                            }
                            else Methods.DisplayReportResult(this, respond); 
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                        }
                    }

                    dbDatabase.Dispose();
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        private void GenderEditTextOnTouch(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;

                var genderArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Gender;

                if (genderArray?.Count > 0)
                {
                    var dialogList = new MaterialDialog.Builder(this);

                    var arrayAdapter = genderArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())).ToList();

                    dialogList.Title(GetText(Resource.String.Lbl_Gender));
                    dialogList.Items(arrayAdapter);
                    dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                    dialogList.AlwaysCallSingleChoiceCallback();
                    dialogList.ItemsCallback(this).Build().Show();
                }
                else
                {
                    Methods.DisplayReportResult(this, "List Gender Not Found, Please check api option ");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region MaterialDialog

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                var genderArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Gender?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                IdGender = int.Parse(genderArray ?? "4525");
                GenderEditText.Text = itemString.ToString();
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
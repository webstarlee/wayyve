using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Authorization;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using Exception = System.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.Default
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class ReplacePasswordActivity : AppCompatActivity
    {
        #region Variables Basic

        private EditText TxtNumber1, TxtPassword;
        private Button BtnVerify, BtnReSent;
        private string EmailCode, Email;
        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.ReplacePasswordUserLayout);

                EmailCode = Intent.GetStringExtra("EmailCode") ?? "";
                Email = Intent.GetStringExtra("Email") ?? "";

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
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

        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                TxtNumber1 = FindViewById<EditText>(Resource.Id.TextNumber1);
                TxtPassword = FindViewById<EditText>(Resource.Id.edt_password);
                BtnVerify = FindViewById<Button>(Resource.Id.verifyButton);
                BtnReSent = FindViewById<Button>(Resource.Id.reSentButton);
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
                var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = " ";
                    toolbar.SetTitleTextColor(AppSettings.TitleTextColor);
                    SetSupportActionBar(toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
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
                    BtnVerify.Click += BtnVerifyOnClick;
                    BtnReSent.Click += BtnReSentOnClick;
                }
                else
                {
                    BtnVerify.Click -= BtnVerifyOnClick;
                    BtnReSent.Click -= BtnReSentOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        private async void BtnVerifyOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(TxtNumber1.Text) && !string.IsNullOrWhiteSpace(TxtNumber1.Text) && !string.IsNullOrEmpty(TxtPassword.Text) && !string.IsNullOrWhiteSpace(TxtPassword.Text))
                {
                    if (EmailCode == TxtNumber1.Text)
                    {
                        if (Methods.CheckConnectivity())
                        {
                            //Show a progress
                            AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                            (int apiStatus, var respond) = await RequestsAsync.Auth.ReplacePasswordAsync(Email, TxtNumber1.Text, TxtPassword.Text);
                            if (apiStatus == 200)
                            {
                                if (respond is ReplacePasswordObject auth)
                                {  
                                    StartActivity(new Intent(this, typeof(LoginActivity)));
                                    Finish();
                                }
                            }
                            else
                            {
                                if (respond is ErrorObject errorMessage)
                                {
                                    var errorId = errorMessage.ErrorData.ErrorId;
                                    if (errorId == "3")
                                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_CodeNotCorrect), GetText(Resource.String.Lbl_Ok));
                                }
                                Methods.DisplayReportResult(this, respond);
                            }

                            AndHUD.Shared.Dismiss(this);
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                        }
                    }
                    else
                    {
                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_CodeNotCorrect), GetText(Resource.String.Lbl_Ok));
                    }
                }
                else
                {
                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_Please_enter_your_data), GetText(Resource.String.Lbl_Ok));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                AndHUD.Shared.Dismiss(this);
            }
        }

        private async void BtnReSentOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    (int apiStatus, var respond) = await RequestsAsync.Auth.ResendEmailAsync(Email);
                    if (apiStatus != 200)
                        Methods.DisplayReportResult(this, respond);

                    AndHUD.Shared.Dismiss(this);
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                AndHUD.Shared.Dismiss(this);
            }
        }


        #endregion 
    }
}
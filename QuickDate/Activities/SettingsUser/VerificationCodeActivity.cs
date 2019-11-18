using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Global;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.SettingsUser
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class VerificationCodeActivity : AppCompatActivity
    {
        #region Variables Basic

        private EditText TxtNumber1, TxtNumber2, TxtNumber3, TxtNumber4;
        private Button BtnVerify; 
        private string Number = "";

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                Number = Intent.GetStringExtra("Number")/*.TrimStart(new[] { '0', '+' }) ?? ""*/;

                // Create your application here
                SetContentView(Resource.Layout.VerificationCodeLayout);

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
                InitTwilio(); 
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
                TxtNumber2 = FindViewById<EditText>(Resource.Id.TextNumber2);
                TxtNumber3 = FindViewById<EditText>(Resource.Id.TextNumber3);
                TxtNumber4 = FindViewById<EditText>(Resource.Id.TextNumber4);
                BtnVerify = FindViewById<Button>(Resource.Id.verifyButton); 
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
                }
                else
                {
                    BtnVerify.Click -= BtnVerifyOnClick;
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
                var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                if (dataUser != null)
                {
                    if (!string.IsNullOrEmpty(TxtNumber1.Text) && !string.IsNullOrEmpty(TxtNumber2.Text) && !string.IsNullOrEmpty(TxtNumber3.Text) && !string.IsNullOrEmpty(TxtNumber4.Text))
                    {
                        string code = TxtNumber1.Text + TxtNumber2.Text + TxtNumber3.Text + TxtNumber4.Text;
                        if (dataUser.SmsCode == code)
                        { 
                            if (Methods.CheckConnectivity())
                            { 
                                //Show a progress
                                AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                                var dictionary = new Dictionary<string, string>
                                {
                                    {"phone_number", Number},
                                    {"verified", "1"},
                                    {"phone_verified", "1"},
                                };

                                (int apiStatus, var respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                                if (apiStatus == 200)
                                {
                                    if (respond is UpdateProfileObject result)
                                    {
                                        dataUser.PhoneNumber = Number;
                                        dataUser.PhoneVerified = 1;
                                        dataUser.Verified = "1";

                                        SqLiteDatabase database = new SqLiteDatabase();
                                        database.InsertOrUpdate_DataMyInfo(dataUser);
                                        database.Dispose();

                                        Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Short).Show();
                                        AndHUD.Shared.Dismiss(this);

                                        Intent intent = new Intent();
                                        intent.PutExtra("status", "1");
                                        SetResult(Result.Ok, intent);
                                        Finish();
                                    }
                                }
                                else  
                                {
                                     Methods.DisplayReportResult(this, respond);
                                    if (respond is ErrorObject error)
                                    {
                                        var errorText = error.ErrorData.ErrorText;
                                        AndHUD.Shared.ShowError(this, errorText, MaskType.Clear, TimeSpan.FromSeconds(2));
 
                                    }
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
                            Toast.MakeText(this, GetText(Resource.String.Lbl_CodeNotCorrect), ToastLength.Short).Show();
                        }
                    }
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                AndHUD.Shared.Dismiss(this);
            }
        }

        #endregion

        private async void InitTwilio()
        {
            try
            {
                string accountSid = "";
                string authToken = "";
                string numberFrom = "";

                var dataTwilio = ListUtils.SettingsSiteList.FirstOrDefault();
                if (dataTwilio != null)
                {
                    accountSid = dataTwilio.SmsTwilioUsername;
                    authToken = dataTwilio.SmsTwilioPassword;
                    numberFrom = dataTwilio.SmsTPhoneNumber;
                }

                TwilioClient.Init(accountSid, authToken);
                 
                var smsCode = ListUtils.MyUserInfo.FirstOrDefault()?.SmsCode;
                 
                var message = await MessageResource.CreateAsync(
                    body: "Mobile Activation code." + smsCode,
                    from: new Twilio.Types.PhoneNumber(numberFrom),
                    to: new Twilio.Types.PhoneNumber(Number)
                );

                if (message.ErrorCode != null)
                {
                    //Twilio sent an error back 
                    Toast.MakeText(this, $"ErrorCode: {message.ErrorCode}, ErrorMessage: {message.ErrorMessage}",ToastLength.Long).Show();
                } 

                Console.WriteLine(message.Sid); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Toast.MakeText(this, GetText(Resource.String.Lbl_TheNumber) + " " + Number + " " + GetText(Resource.String.Lbl_ISNotValidNumber), ToastLength.Long).Show();
            }
        } 
    }
}
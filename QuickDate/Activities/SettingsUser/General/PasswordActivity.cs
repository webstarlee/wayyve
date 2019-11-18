using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using QuickDate.Activities.Default;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.SettingsUser.General
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class PasswordActivity : AppCompatActivity
    {
        #region Variables Basic

        public Toolbar Toolbar;
        public TextView SaveTextView, LinkTextView;
        public EditText CurrentPasswordEditText, NewPasswordEditText, RepeatPasswordEditText;

        #endregion

        #region General
         
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                // Create your application here
                SetContentView(Resource.Layout.PasswordLayout);

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
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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
                SaveTextView = FindViewById<TextView>(Resource.Id.toolbar_title);
                LinkTextView = FindViewById<TextView>(Resource.Id.linkText);
                CurrentPasswordEditText = FindViewById<EditText>(Resource.Id.currentPasswordText);
                NewPasswordEditText = FindViewById<EditText>(Resource.Id.newPsswordText);
                RepeatPasswordEditText = FindViewById<EditText>(Resource.Id.repeatPasswordText);
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
                    Toolbar.Title = GetText(Resource.String.Lbl_Change_Password);
                    Toolbar.SetTitleTextColor(AppSettings.TitleTextColor);
                    SetSupportActionBar(Toolbar);
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
                    SaveTextView.Click += SaveTextViewOnClick;
                    LinkTextView.Click += LinkTextViewOnClick;
                }
                else
                {
                    SaveTextView.Click -= SaveTextViewOnClick;
                    LinkTextView.Click -= LinkTextViewOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        #endregion

        #region Events

        private void LinkTextViewOnClick(object sender, EventArgs e)
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

        // Save New Password
        private async void SaveTextViewOnClick(object sender, EventArgs e)
        {
            try
            {
                if (CurrentPasswordEditText.Text == "" || NewPasswordEditText.Text == "" || RepeatPasswordEditText.Text == "")
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Please_check_your_details), ToastLength.Long).Show();
                    return;
                }

                if (NewPasswordEditText.Text != RepeatPasswordEditText.Text || CurrentPasswordEditText.Text != UserDetails.Password)
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Your_password_dont_match), ToastLength.Long).Show();
                }
                else
                {
                    if (Methods.CheckConnectivity())
                    {
                        Dictionary<string, string> dictionary = new Dictionary<string, string>
                        {
                            {"c_pass", CurrentPasswordEditText.Text},
                            {"n_pass", NewPasswordEditText.Text},
                            {"cn_pass", RepeatPasswordEditText.Text}
                        };

                        if (dictionary.Count > 0)
                        {
                            //Show a progress
                            AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                            //Send Api
                            (int apiStatus, var respond) = await RequestsAsync.Users.ChangePasswordAsync(dictionary);
                            if (apiStatus == 200)
                            {
                                UserDetails.Password = NewPasswordEditText.Text;
                                AndHUD.Shared.ShowSuccess(this, GetText(Resource.String.Lbl_Done), MaskType.Clear, TimeSpan.FromSeconds(2));
                                Finish();
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
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
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

    }
}
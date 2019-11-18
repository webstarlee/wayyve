using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.SettingsUser.Support
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class DeleteAccountActivity : AppCompatActivity
    {
        #region Variables Basic

        public Toolbar Toolbar;
        public EditText PasswordEditText;
        public CheckBox DeleteCheckBox;
        public Button DeleteButton;
        public AdView MAdView;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                // Create your application here
                SetContentView(Resource.Layout.DeleteAccountLayout);

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
                MAdView?.Resume();
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
                MAdView?.Pause();
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

         protected override void OnDestroy()
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
                PasswordEditText = FindViewById<EditText>(Resource.Id.passwordEdit);
                DeleteCheckBox = FindViewById<CheckBox>(Resource.Id.DeleteCheckBox);
                DeleteButton = FindViewById<Button>(Resource.Id.DeleteButton);

                DeleteCheckBox.Text = GetText(Resource.String.Lbl_IWantToDelete1) + " " + UserDetails.Username + " " +
                                      GetText(Resource.String.Lbl_IWantToDelete2) + " " + AppSettings.ApplicationName +
                                      " " + GetText(Resource.String.Lbl_IWantToDelete3);
            
                MAdView = FindViewById<AdView>(Resource.Id.adView);
                AdsGoogle.InitAdView(MAdView, null);
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
                Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = GetString(Resource.String.Lbl_DeleteAccount);
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
                    DeleteButton.Click += DeleteButtonOnClick;
                }
                else
                {
                    DeleteButton.Click -= DeleteButtonOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }



        #endregion

        #region Events

        private void DeleteButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (DeleteCheckBox.Checked)
                {
                    if (!Methods.CheckConnectivity())
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                    }
                    else
                    {
                        var localData = ListUtils.DataUserLoginList.FirstOrDefault();
                        if (localData != null)
                        {
                            if (PasswordEditText.Text == localData.Password)
                            {
                                ApiRequest.RunLogout = false;
                                ApiRequest.Delete(this);
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Your_account_was_successfully_deleted), ToastLength.Long).Show();
                            }
                            else
                            {
                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Warning), GetText(Resource.String.Lbl_Please_confirm_your_password), GetText(Resource.String.Lbl_Ok));
                            }
                        }
                    }
                }
                else
                {
                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Warning),GetText(Resource.String.Lbl_Error_Terms), GetText(Resource.String.Lbl_Ok));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        #endregion

    }
}
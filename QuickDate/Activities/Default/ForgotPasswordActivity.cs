using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.Default
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class ForgotPasswordActivity : AppCompatActivity
    {
        #region Variables Basic
         
        private EditText EmailEditText;
        private Button BtnSend;
        private ProgressBar ProgressBar1;
     
        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                // Create your application here
                SetContentView(Resource.Layout.ForgotPasswordLayout);

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
                EmailEditText = FindViewById<EditText>(Resource.Id.edt_email);
                BtnSend = FindViewById<Button>(Resource.Id.SignInButton);
                
                ProgressBar1 = FindViewById<ProgressBar>(Resource.Id.progressBar1); 
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
                    toolbar.Title = GetString(Resource.String.Lbl_Forget_password);
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
                    BtnSend.Click += BtnSendOnClick;  
                }
                else
                {
                    BtnSend.Click -= BtnSendOnClick; 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion

        #region Events

        //Send email
        private async void BtnSendOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(EmailEditText.Text))
                {
                    if (Methods.CheckConnectivity())
                    {
                        var check = Methods.FunString.IsEmailValid(EmailEditText.Text);
                        if (!check)
                        {
                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_VerificationFailed), GetText(Resource.String.Lbl_IsEmailValid), GetText(Resource.String.Lbl_Ok));
                        }
                        else
                        {
                            ProgressBar1.Visibility = ViewStates.Visible;
                            BtnSend.Visibility = ViewStates.Gone;
                            var (apiStatus, respond) = await RequestsAsync.Auth.ResetPasswordAsync(EmailEditText.Text);
                            if (apiStatus == 200)
                            {
                                if (respond is InfoObject result)
                                {
                                    Intent Int = new Intent(this, typeof(ReplacePasswordActivity));
                                    Int.PutExtra("EmailCode", result.EmailCode);
                                    Int.PutExtra("Email", EmailEditText.Text);
                                    StartActivity(Int);
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
                                        case 21:
                                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_21), GetText(Resource.String.Lbl_Ok));
                                            break;
                                        case 22:
                                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetString(Resource.String.Lbl_Error_22), GetText(Resource.String.Lbl_Ok));
                                            break;
                                        default:
                                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), errorText, GetText(Resource.String.Lbl_Ok));
                                            break;
                                    }
                                }

                            }
                            else if (apiStatus == 404)
                            {
                                ProgressBar1.Visibility = ViewStates.Gone;
                                BtnSend.Visibility = ViewStates.Visible;
                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), respond.ToString(), GetText(Resource.String.Lbl_Ok));
                            }

                            ProgressBar1.Visibility = ViewStates.Gone;
                            BtnSend.Visibility = ViewStates.Visible;

                        }
                    }
                    else
                    {
                        ProgressBar1.Visibility = ViewStates.Gone;
                        BtnSend.Visibility = ViewStates.Visible;
                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_VerificationFailed), GetText(Resource.String.Lbl_CheckYourInternetConnection), GetText(Resource.String.Lbl_Ok));
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                ProgressBar1.Visibility = ViewStates.Gone;
                BtnSend.Visibility = ViewStates.Visible;
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_VerificationFailed), exception.ToString(), GetText(Resource.String.Lbl_Ok));
            }
        }
         
        #endregion
    }
}
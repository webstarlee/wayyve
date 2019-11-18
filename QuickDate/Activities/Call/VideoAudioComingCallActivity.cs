using System;
using System.Collections.Generic;
using AFollestad.MaterialDialogs;
using Android;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.Hardware;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using Java.Lang;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Call;
using QuickDateClient.Classes.Chat;
using QuickDateClient.Requests;
using Exception = System.Exception;

namespace QuickDate.Activities.Call
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class VideoAudioComingCallActivity : AppCompatActivity,ValueAnimator.IAnimatorUpdateListener, ISensorEventListener, MaterialDialog.ISingleButtonCallback, MaterialDialog.IListCallback, MaterialDialog.IInputCallback
    {

        private string TwilioAccessToken = "YOUR_TOKEN";
        private string TwilioAccessTokenUser2 = "YOUR_TOKEN";
        private string RoomName = "TestRoom";
        private string CallId = "0";
        private string CallType = "0";
        private string UserId = "";
        private string Avatar = "0";
        private string Name = "0";
        private string FromId = "0";
        private string Active = "0";
        private string Time = "0";
        private string Status = "0";

        private ImageView UserImageView;
        private TextView UserNameTextView, TypeCallTextView;
        private View GradientPreView;
        public static VideoAudioComingCallActivity CallActivity;
        private GradientDrawable GradientDrawableView;
        private int start, mid, end;

        public CircleButton AcceptCallButton, RejectCallButton, MessageCallButton;

        public static bool IsActive;

        private SensorManager SensorManager;
        private Sensor Proximity;
        private readonly int SENSOR_SENSITIVITY = 4;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                 
                SetContentView(Resource.Layout.TwilioCommingVideoCallLayout);
                Window.AddFlags(WindowManagerFlags.KeepScreenOn);

                SensorManager = (SensorManager)GetSystemService(SensorService);
                Proximity = SensorManager.GetDefaultSensor(SensorType.Proximity); 

                CallActivity = this;
                 
                var dataCallId = Intent.GetStringExtra("CallID") ?? "";
                if (!string.IsNullOrEmpty(dataCallId))
                {
                    CallType = Intent.GetStringExtra("type");

                    if (CallType == "Twilio_video_call" || CallType == "Twilio_audio_call")
                    {
                        CallId = dataCallId;
                        UserId = Intent.GetStringExtra("UserID");
                        Avatar = Intent.GetStringExtra("avatar");
                        Name = Intent.GetStringExtra("name");
                        FromId = Intent.GetStringExtra("from_id");
                        Active = Intent.GetStringExtra("active");
                        Time = Intent.GetStringExtra("time");
                        Status = Intent.GetStringExtra("status");
                        RoomName = Intent.GetStringExtra("room_name");
                        TwilioAccessToken = Intent.GetStringExtra("access_token");
                        TwilioAccessTokenUser2 = Intent.GetStringExtra("access_token_2");
                    } 
                }
                UserNameTextView = FindViewById<TextView>(Resource.Id.UsernameTextView);
                TypeCallTextView = FindViewById<TextView>(Resource.Id.TypecallTextView);
                UserImageView = FindViewById<ImageView>(Resource.Id.UserImageView);
                GradientPreView = FindViewById<View>(Resource.Id.gradientPreloaderView);
                AcceptCallButton = FindViewById<CircleButton>(Resource.Id.accept_call_button);
                RejectCallButton = FindViewById<CircleButton>(Resource.Id.end_call_button);
                MessageCallButton = FindViewById<CircleButton>(Resource.Id.message_call_button);

                StartAnimatedBackground();

                AcceptCallButton.Click += AcceptCallButton_Click;
                RejectCallButton.Click += RejectCallButton_Click;
                MessageCallButton.Click += MessageCallButton_Click;

                if (!string.IsNullOrEmpty(Name))
                    UserNameTextView.Text = Name;

                if (!string.IsNullOrEmpty(Avatar))
                    GlideImageLoader.LoadImage(this, Avatar, UserImageView, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                if (CallType == "Twilio_video_call" || CallType == "Agora_video_call_recieve")
                    TypeCallTextView.Text = GetText(Resource.String.Lbl_Video_call);
                else
                    TypeCallTextView.Text = GetText(Resource.String.Lbl_Voice_call);

                Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("mystic_call.mp3"); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            } 
        }

        protected override void OnStart()
        {
            base.OnStart();
            IsActive = true;
        }

        protected override void OnStop()
        {
            base.OnStop();
            IsActive = false;
        }


        protected override void OnResume()
        {
            try
            {
                base.OnResume();  
                SensorManager.RegisterListener(this, Proximity,SensorDelay.Normal ); 
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
                SensorManager.UnregisterListener(this);
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

        private void MessageCallButton_Click(object sender, EventArgs e)
        {
            try
            {

                if (Methods.CheckConnectivity())
                {
                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialDialog.Builder(this);

                    arrayAdapter.Add(GetString(Resource.String.Lbl_MessageCall1));
                    arrayAdapter.Add(GetString(Resource.String.Lbl_MessageCall2));
                    arrayAdapter.Add(GetString(Resource.String.Lbl_MessageCall3));
                    arrayAdapter.Add(GetString(Resource.String.Lbl_MessageCall4));
                    arrayAdapter.Add(GetString(Resource.String.Lbl_MessageCall5));
                     
                    dialogList.Items(arrayAdapter);
                    dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnNegative(this);
                    dialogList.AlwaysCallSingleChoiceCallback();
                    dialogList.ItemsCallback(this).Build().Show();
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception); 
            }
        }

        private void RejectCallButton_Click(object sender, EventArgs e)
        {
            try
            { 
                if (CallType == "Twilio_video_call")
                {
                    RequestsAsync.Call.DeclineCallAsync(CallId, TypeCall.Video).ConfigureAwait(false);
                }
                else if (CallType == "Twilio_audio_call")
                {
                    RequestsAsync.Call.DeclineCallAsync(CallId, TypeCall.Audio).ConfigureAwait(false);
                }

                FinishVideoAudio();
            }
            catch (Exception exception)
            {
                FinishVideoAudio();
                Console.WriteLine(exception);
            }
        }

        private void AcceptCallButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (CallType == "Twilio_video_call")
                {
                    MenuVideoCallIcon_Click(); 
                }
                if (CallType == "Twilio_audio_call")
                {
                    MenuPhoneCallIcon_Click();
                }

                FinishVideoAudio();
            }
            catch (Exception exception)
            {
                FinishVideoAudio();
                Console.WriteLine(exception);
            }
        }
         
        private void MenuPhoneCallIcon_Click()
        {
            try
            {
                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    StartAudioCall();
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted && CheckSelfPermission(Manifest.Permission.RecordAudio) == Permission.Granted && CheckSelfPermission(Manifest.Permission.ModifyAudioSettings) == Permission.Granted)
                    {
                        StartAudioCall();
                    }
                    else
                        new PermissionsController(this).RequestPermission(111);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        private void MenuVideoCallIcon_Click()
        {
            try
            {
                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    StartVideoCall();
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted && CheckSelfPermission(Manifest.Permission.RecordAudio) == Permission.Granted && CheckSelfPermission(Manifest.Permission.ModifyAudioSettings) == Permission.Granted)
                    {
                        StartVideoCall();
                    }
                    else
                        new PermissionsController(this).RequestPermission(111);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void StartAudioCall()
        {
            try
            {
                Intent intent = new Intent(this, typeof(TwilioAudioCallActivity));
                intent.SetFlags(ActivityFlags.TaskOnHome | ActivityFlags.BroughtToFront | ActivityFlags.NewTask);
                intent.PutExtra("UserID", UserId);
                intent.PutExtra("avatar", Avatar);
                intent.PutExtra("name", Name);
                intent.PutExtra("access_token", TwilioAccessToken);
                intent.PutExtra("access_token_2", TwilioAccessTokenUser2);
                intent.PutExtra("from_id", FromId);
                intent.PutExtra("active", Active);
                intent.PutExtra("time", Time);
                intent.PutExtra("CallID", CallId);
                intent.PutExtra("room_name", RoomName);
                intent.PutExtra("type", CallType);
                StartActivity(intent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void StartVideoCall()
        {
            try
            {  
                Intent intent = new Intent(this, typeof(TwilioVideoCallActivity));
                intent.SetFlags(ActivityFlags.TaskOnHome | ActivityFlags.BroughtToFront);
                intent.PutExtra("UserID", UserId);
                intent.PutExtra("avatar", Avatar);
                intent.PutExtra("name", Name);
                intent.PutExtra("access_token", TwilioAccessToken);
                intent.PutExtra("access_token_2", TwilioAccessTokenUser2);
                intent.PutExtra("from_id", FromId);
                intent.PutExtra("active", Active);
                intent.PutExtra("time", Time);
                intent.PutExtra("CallID", CallId);
                intent.PutExtra("room_name", RoomName);
                intent.PutExtra("type", CallType);

                StartActivity(intent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void StartAnimatedBackground()
        {
            GradientDrawableView = (GradientDrawable)GradientPreView.Background;
             start = ContextCompat.GetColor(this, Resource.Color.accent);
             mid = ContextCompat.GetColor(this, Resource.Color.primaryDark);
             end = ContextCompat.GetColor(this, Resource.Color.extraDark);
            var animator = ValueAnimator.OfFloat(0.0f, 1.0f);
            animator.SetDuration(1500);
            animator.RepeatCount = 1000;
            animator.RepeatMode = ValueAnimatorRepeatMode.Reverse;
            animator.AddUpdateListener(this); 
            animator.Start();
        }

        public void OnAnimationUpdate(ValueAnimator animation)
        {
            try
            {
                var evaluator = new ArgbEvaluator();
                var newStart = (int)evaluator.Evaluate(animation.AnimatedFraction, start, end);
                var newMid = (int)evaluator.Evaluate(animation.AnimatedFraction, mid, start);
                var newEnd = (int)evaluator.Evaluate(animation.AnimatedFraction, end, mid);
                int[] newArray = { newStart, newMid, newEnd };
                GradientDrawableView.SetColors(newArray);
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            }

        }

        #region Sensor System

        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
            try
            {
                // Do something here if sensor accuracy changes.
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void OnSensorChanged(SensorEvent e)
        {
            try
            { 
                if (e.Sensor.Type == SensorType.Proximity) 
                {
                    if (e.Values[0] >= -SENSOR_SENSITIVITY && e.Values[0] <= SENSOR_SENSITIVITY)
                    {
                        //near 
                        HomeActivity.GetInstance()?.SetOffWakeLock(); 
                    }
                    else
                    {
                        //far 
                        HomeActivity.GetInstance()?.SetOnWakeLock();  
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
         
        #region MaterialDialog

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

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                string text = itemString.ToString();

                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
                else
                {
                    if (text == GetString(Resource.String.Lbl_MessageCall5))
                    {
                        var dialog = new MaterialDialog.Builder(this); 
                        dialog.Input(Resource.String.Lbl_Write_your_message, 0, false, this);
                        dialog.InputType(InputTypes.TextFlagImeMultiLine);
                        dialog.PositiveText(GetText(Resource.String.Lbl_Send)).OnPositive(this);
                        dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                        dialog.Build().Show();
                        dialog.AlwaysCallSingleChoiceCallback();
                    }
                    else
                    {
                        SendMess(text);
                    } 
                }  
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnInput(MaterialDialog p0, ICharSequence p1)
        {
            try
            {
                if (p1.Length() > 0)
                {
                    var text = p1.ToString();
                    SendMess(text);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
         
        private async void SendMess(string text)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
                else
                {
                    var unixTimestamp = (int) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    var hashId = unixTimestamp.ToString();

                    //Here on This function will send Selected audio file to the user 
                    var (apiStatus, respond) = await RequestsAsync.Chat.SendMessageAsync(UserId, text, "", "", hashId);
                    if (apiStatus == 200)
                    {
                        if (respond is SendMessageObject result)
                        {
                            Console.WriteLine(result.Message);
                            if (!string.IsNullOrEmpty(CallId))
                            {
                                if (CallType == "Twilio_video_call")
                                {
                                   await RequestsAsync.Call.DeclineCallAsync(CallId, TypeCall.Video).ConfigureAwait(false);
                                }
                                else if (CallType == "Twilio_audio_call")
                                {
                                    await RequestsAsync.Call.DeclineCallAsync(CallId, TypeCall.Audio).ConfigureAwait(false);
                                }
                            }

                            FinishVideoAudio();
                        }
                    }
                    else Methods.DisplayReportResult(this, respond);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void FinishVideoAudio()
        {
            try
            {
                Methods.AudioRecorderAndPlayer.StopAudioFromAsset();

                Finish();
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
                 
                 if (requestCode == 111)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        if (CallType == "Twilio_video_call")
                        {
                            StartVideoCall();
                        }
                        else if (CallType == "Twilio_audio_call")
                        {
                            StartAudioCall();
                        } 
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
    }
}
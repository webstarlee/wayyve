using System;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Call;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using TwilioVideo;

namespace QuickDate.Activities.Call
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation, ResizeableActivity = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class TwilioAudioCallActivity : AppCompatActivity, TwilioVideoHelper.IListener, ISensorEventListener
    { 
        #region Variables Basic

        private TwilioVideoHelper TwilioVideo { get; set; }
        private string TwilioAccessToken = "YOUR_TOKEN", TwilioAccessTokenUser2 = "YOUR_TOKEN", RoomName = "TestRoom";
        private string  CallId = "0",CallType = "0", UserId = "", Avatar = "0",Name = "0",FromId = "0", Active = "0", Time = "0", Status = "0";
        private CircleButton EndCallButton, SpeakerAudioButton, MuteAudioButton;
        private ImageView UserImageView;
        private TextView UserNameTextView, DurationTextView;
        private Timer TimerRequestWaiter = new Timer();
        private LocalVideoTrack LocalvideoTrack;
        private VideoTrack UserVideoTrack;
        private bool DataUpdated;
        private int CountSecoundsOfOutgoingCall;
        private string LocalVideoTrackId, RemoteVideoTrackId;

        private SensorManager SensorManager;
        private Sensor Proximity;
        private readonly int SENSOR_SENSITIVITY = 4;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                Window.AddFlags(WindowManagerFlags.KeepScreenOn);

                // Create your application here
                SetContentView(Resource.Layout.TwilioAudioCallActivityLayout);

                SensorManager = (SensorManager)GetSystemService(SensorService);
                Proximity = SensorManager.GetDefaultSensor(SensorType.Proximity);
                  
                //Get Value And Set Toolbar
                InitComponent();

                InitTwilioCall();
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
                SensorManager.RegisterListener(this, Proximity, SensorDelay.Normal);
                AddOrRemoveEvent(true);
                UpdateState();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnStart()
        {
            try
            {
                base.OnStart();
                UpdateState();
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
                DataUpdated = false;
                base.OnPause();
                AddOrRemoveEvent(false);
                SensorManager.UnregisterListener(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnRestart()
        {
            try
            {
                base.OnRestart();
                TwilioVideo = TwilioVideoHelper.GetOrCreate(this);
                UpdateState();
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
                SpeakerAudioButton = FindViewById<CircleButton>(Resource.Id.speaker_audio_button);
                EndCallButton = FindViewById<CircleButton>(Resource.Id.end_audio_call_button);
                MuteAudioButton = FindViewById<CircleButton>(Resource.Id.mute_audio_call_button);

                UserImageView = FindViewById<ImageView>(Resource.Id.audiouserImageView);
                UserNameTextView = FindViewById<TextView>(Resource.Id.audiouserNameTextView);
                DurationTextView = FindViewById<TextView>(Resource.Id.audiodurationTextView);
                 
                SpeakerAudioButton.SetImageResource(Resource.Drawable.ic_speaker_close);
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
                    SpeakerAudioButton.Click += Speaker_audio_button_Click;
                    EndCallButton.Click += End_call_button_Click;
                    MuteAudioButton.Click += Mute_audio_button_Click;
                }
                else
                {
                    SpeakerAudioButton.Click -= Speaker_audio_button_Click;
                    EndCallButton.Click -= End_call_button_Click;
                    MuteAudioButton.Click -= Mute_audio_button_Click;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion

        #region Events

        private void Speaker_audio_button_Click(object sender, EventArgs e)
        {
            try
            {
                //Speaker
                if (SpeakerAudioButton.Selected)
                {
                    SpeakerAudioButton.Selected = false;
                    SpeakerAudioButton.SetImageResource(Resource.Drawable.ic_speaker_close);

                }
                else
                {
                    SpeakerAudioButton.Selected = true;

                    SpeakerAudioButton.SetImageResource(Resource.Drawable.ic_speaker_up);

                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Mute_audio_button_Click(object sender, EventArgs e)
        {

            try
            {
                if (MuteAudioButton.Selected)
                {
                    MuteAudioButton.Selected = false;
                    MuteAudioButton.SetImageResource(Resource.Drawable.ic_camera_mic_open);
                }
                else
                {
                    MuteAudioButton.Selected = true;
                    MuteAudioButton.SetImageResource(Resource.Drawable.ic_camera_mic_mute);
                }

                TwilioVideo.Mute(MuteAudioButton.Selected);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void End_call_button_Click(object sender, EventArgs e)
        {
            try
            {
                RequestsAsync.Call.DeleteCallAsync(CallId, TypeCall.Audio).ConfigureAwait(false);
                FinishCall(true);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        #endregion
         
        private async void InitTwilioCall()
        {
            try
            { 
                UserId = Intent.GetStringExtra("UserID");
                Avatar = Intent.GetStringExtra("avatar");
                Name = Intent.GetStringExtra("name");

                var dataCallId = Intent.GetStringExtra("CallID") ?? "Data not available";
                if (dataCallId != "Data not available" && !string.IsNullOrEmpty(dataCallId))
                {
                    CallId = dataCallId;

                    TwilioAccessToken = Intent.GetStringExtra("access_token");
                    TwilioAccessTokenUser2 = Intent.GetStringExtra("access_token_2");
                    FromId = Intent.GetStringExtra("from_id");
                    Active = Intent.GetStringExtra("active");
                    Time = Intent.GetStringExtra("time");
                    Status = Intent.GetStringExtra("status");
                    RoomName = Intent.GetStringExtra("room_name");
                    CallType = Intent.GetStringExtra("type");
                }
                 
                if (CallType == "Twilio_audio_call")
                {
                    if (!string.IsNullOrEmpty(TwilioAccessToken))
                    {
                        if (!string.IsNullOrEmpty(UserId))
                            Load_userWhenCall();

                        TwilioVideo = TwilioVideoHelper.GetOrCreate(this);
                        UpdateState();
                        DurationTextView.Text = GetText(Resource.String.Lbl_Waiting_for_answer);

                        var (apiStatus, respond) = await RequestsAsync.Call.SendAnswerCallAsync(CallId, TypeCall.Audio);
                        if (apiStatus == 200)
                        {
                            if (respond is AnswerCallObject result)
                            {
                                if (result.Data != null)
                                {
                                    ConnectToRoom(); 
                                }
                            }
                        }
                        else Methods.DisplayReportResult(this, respond); 
                    }
                }
                else if (CallType == "Twilio_audio_calling_start")
                {
                    LoadProfileFromUserId();

                    DurationTextView.Text = GetText(Resource.String.Lbl_Calling);
                    TwilioVideo = TwilioVideoHelper.GetOrCreate(this);

                    Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("mystic_call.mp3");

                    UpdateState();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Load_userWhenCall()
        {
            try
            {
                UserNameTextView.Text = Name;

                //profile_picture
                GlideImageLoader.LoadImage(this, Avatar, UserImageView, ImageStyle.CircleCrop, ImagePlaceholders.Drawable); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
          
        private async void LoadProfileFromUserId()
        {
            try
            {
                Load_userWhenCall();
                var (apiStatus, respond) = await RequestsAsync.Call.CreateNewCallAsync(UserId, TypeCall.Audio);
                if (apiStatus == 200)
                {
                    if (respond is CreateNewCallObject result)
                    {
                        CallId = result.Data.Id.ToString();
                        TwilioAccessToken = result.Data.AccessToken;
                        TwilioAccessTokenUser2 = result.Data.AccessToken2;
                        RoomName = result.Data.RoomName;

                        TimerRequestWaiter = new Timer();
                        TimerRequestWaiter.Interval = 5000;
                        TimerRequestWaiter.Elapsed += TimerCallRequestAnswer_Waiter_Elapsed;
                        TimerRequestWaiter.Start();
                    }
                }
                else
                {
                    Methods.DisplayReportResult(this, respond);
                    FinishCall(true); 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async void TimerCallRequestAnswer_Waiter_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var (apiStatus, respond) = await RequestsAsync.Call.CheckForAnswerAsync(CallId, TypeCall.Audio);
                if (apiStatus == 200)
                {
                    if (respond is AnswerCallObject result)
                    {
                        Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                        if (result.Data != null && result.Data.Value.CallUserClass != null)
                        {
                            TwilioAccessToken = result.Data.Value.CallUserClass.AccessToken;
                            TwilioAccessTokenUser2 = result.Data.Value.CallUserClass.AccessToken2;
                            RoomName = result.Data.Value.CallUserClass.RoomName;
                        }

                        if (!string.IsNullOrEmpty(TwilioAccessToken))
                        {
                            TimerRequestWaiter.Enabled = false;
                            TimerRequestWaiter.Stop();
                            TimerRequestWaiter.Close();

                            RunOnUiThread(async () =>
                            {
                                await Task.Delay(1000);

                                TwilioVideo?.UpdateToken(TwilioAccessTokenUser2);
                                TwilioVideo?.JoinRoom(this, RoomName); 
                            });
                        }
                    } 
                } 
                else if (apiStatus == 300)
                {
                    if (respond is InfoObject result)
                    {
                        if (result.Message == "calling")
                        {
                            if (CountSecoundsOfOutgoingCall < 70)
                            {
                                CountSecoundsOfOutgoingCall += 10;
                            }
                            else
                            {
                                //Call Is inactive 
                                TimerRequestWaiter.Enabled = false;
                                TimerRequestWaiter.Stop();
                                TimerRequestWaiter.Close(); 
                                FinishCall(true);
                            }
                        }
                        else if (result.Message == "declined")
                        {
                            //Call Is inactive 
                            TimerRequestWaiter.Enabled = false;
                            TimerRequestWaiter.Stop();
                            TimerRequestWaiter.Close(); 
                            FinishCall(true);
                        }
                    } 
                } 
                else Methods.DisplayReportResult(this, respond); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void ConnectToRoom()
        {
            TwilioVideo?.UpdateToken(TwilioAccessToken);
            TwilioVideo?.JoinRoom(this, RoomName);
        }

        private void UpdateState()
        {
            try
            {
                if (DataUpdated)
                    return;
                DataUpdated = true;
                TwilioVideo?.Bind(this);
                UpdatingState();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        protected virtual void UpdatingState()
        {
        }
         
        protected void CloseScreen()
        {
            Finish();
        }

        public override void OnBackPressed()
        {
            FinishCall(true); 
        }

        protected virtual void FinishCall(bool hangup)
        {
            try
            {
                if (TwilioVideo != null && TwilioVideo.ClientIsReady)
                {
                    TwilioVideo.Unbind(this);
                    TwilioVideo.FinishCall();
                }

                Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                Finish();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region TwilioVideo.IListener

        public void SetLocalVideoTrack(LocalVideoTrack track)
        {
            try
            {
                if (LocalvideoTrack == null)
                {
                    LocalvideoTrack = track;
                    var trackId = track?.TrackId;
                    if (LocalVideoTrackId == trackId)
                    {
                        LocalvideoTrack.Enable(false);
                    }
                    else
                    {
                        LocalVideoTrackId = trackId;
                        LocalvideoTrack.Enable(false);


                    }
                }
                else
                {
                    if (LocalvideoTrack.IsEnabled)
                    {
                        LocalvideoTrack.Enable(false);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetRemoteVideoTrack(VideoTrack track)
        {
            try
            {
                var trackId = track?.TrackId;

                if (RemoteVideoTrackId == trackId)
                    return;

                RemoteVideoTrackId = trackId;
                if (UserVideoTrack == null)
                {
                    UserVideoTrack = track;

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void RemoveLocalVideoTrack(LocalVideoTrack track)
        {
            SetLocalVideoTrack(null);
        }

        public void RemoveRemoteVideoTrack(VideoTrack track)
        {

        }

        public void OnRoomConnected(string roomId)
        {

        }

        public void OnRoomDisconnected(TwilioVideoHelper.StopReason reason)
        {
            Toast.MakeText(this, GetText(Resource.String.Lbl_Room_Disconnected), ToastLength.Short).Show();
        }

        public void OnParticipantConnected(string participantId)
        {
            try
            {
                DurationTextView.Text = GetText(Resource.String.Lbl_Connected);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnParticipantDisconnected(string participantId)
        {
            RunOnUiThread(async () =>
            {
                try
                {
                    DurationTextView.Text = GetText(Resource.String.Lbl_User_Lost_Connection);
                    await Task.Delay(2000);
                    FinishCall(true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        public void SetCallTime(int seconds)
        {
            try
            {
                DurationTextView.Text = seconds.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

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

    }
}
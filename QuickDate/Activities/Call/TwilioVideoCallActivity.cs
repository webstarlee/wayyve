using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Hardware;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
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
using VideoView = TwilioVideo.VideoView;

namespace QuickDate.Activities.Call
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation, ResizeableActivity = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class TwilioVideoCallActivity : AppCompatActivity, TwilioVideoHelper.IListener, ISensorEventListener
    {
        #region Variables Basic

        private TwilioVideoHelper TwilioVideo { get; set; }
        private string TwilioAccessToken = "YOUR_TOKEN", TwilioAccessTokenUser2 = "YOUR_TOKEN", RoomName = "TestRoom";
        private string CallId = "0", CallType = "0", UserId = "", Avatar = "0", Name = "0", FromId = "0", Active = "0", Time = "0", Status = "0";

        private RelativeLayout MainUserViewProfile;
        private Button SwitchCamButton; 
        private CircleButton EndCallButton, MuteAudioButton, MuteVideoButton;
        private ImageView UserImageView, PictureInToPictureButton;
        private TextView UserNameTextView, NoteTextView;
        private Timer TimerRequestWaiter = new Timer();
        private VideoView UserprimaryVideo, ThumbnailVideo;
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
                SetContentView(Resource.Layout.TwilioVideoCallActivityLayout);

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
                MainUserViewProfile = FindViewById<RelativeLayout>(Resource.Id.userInfoview_container);
                UserprimaryVideo = FindViewById<VideoView>(Resource.Id.userthumbnailVideo); // userthumbnailVideo
                ThumbnailVideo = FindViewById<VideoView>(Resource.Id.local_video_view_container); //local_video_view_container
                SwitchCamButton = FindViewById<Button>(Resource.Id.switch_cam_button);
                MuteVideoButton = FindViewById<CircleButton>(Resource.Id.mute_video_button);
                EndCallButton = FindViewById<CircleButton>(Resource.Id.end_call_button);
                MuteAudioButton = FindViewById<CircleButton>(Resource.Id.mute_audio_button);
                UserImageView = FindViewById<ImageView>(Resource.Id.userImageView);
                UserNameTextView = FindViewById<TextView>(Resource.Id.userNameTextView);
                NoteTextView = FindViewById<TextView>(Resource.Id.noteTextView);

                PictureInToPictureButton = FindViewById<ImageView>(Resource.Id.pictureintopictureButton); 
                if (!PackageManager.HasSystemFeature(PackageManager.FeaturePictureInPicture))
                    PictureInToPictureButton.Visibility = ViewStates.Gone;

                MuteVideoButton.Selected = true;
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
                    SwitchCamButton.Click += Switch_cam_button_Click;
                    MuteVideoButton.Click += Mute_video_button_Click;
                    EndCallButton.Click += End_call_button_Click;
                    MuteAudioButton.Click += Mute_audio_button_Click;
                    PictureInToPictureButton.Click += PictureInToPictureButton_Click;
                }
                else
                {
                    SwitchCamButton.Click -= Switch_cam_button_Click;
                    MuteVideoButton.Click -= Mute_video_button_Click;
                    EndCallButton.Click -= End_call_button_Click;
                    MuteAudioButton.Click -= Mute_audio_button_Click;
                    PictureInToPictureButton.Click -= PictureInToPictureButton_Click;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        private void PictureInToPictureButton_Click(object sender, EventArgs e)
        {
            try
            {
                var actions = new List<RemoteAction>();
                //.SetActions(new List<RemoteAction>().Add(new RemoteAction().Title = "")
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var param = new PictureInPictureParams.Builder().SetAspectRatio(new Rational(9, 16)).Build();
                    EnterPictureInPictureMode(param);
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            } 
        }

        private void Switch_cam_button_Click(object sender, EventArgs e)
        {
            try
            {
                TwilioVideo.FlipCamera();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Mute_video_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (MuteVideoButton.Selected)
                {
                    MuteVideoButton.SetImageResource(Resource.Drawable.ic_camera_video_mute);

                    MuteVideoButton.Selected = false;
                }
                else
                {
                    MuteVideoButton.SetImageResource(Resource.Drawable.ic_camera_video_open);
                    MuteVideoButton.Selected = true;
                }

                var isVideoEnabled = MuteVideoButton.Selected;
                FindViewById(Resource.Id.local_video_container).Visibility =isVideoEnabled ? ViewStates.Visible : ViewStates.Gone;
                LocalvideoTrack.Enable(isVideoEnabled);
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
                RequestsAsync.Call.DeleteCallAsync(CallId, TypeCall.Video).ConfigureAwait(false);
                FinishCall(true);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
         
        public override void OnPictureInPictureModeChanged(bool isInPictureInPictureMode, Configuration newConfig)
        {
            try
            {
                if (isInPictureInPictureMode)
                {
                    EndCallButton.Visibility = ViewStates.Gone;
                    MuteAudioButton.Visibility = ViewStates.Gone;
                    MuteVideoButton.Visibility = ViewStates.Gone;
                    UserNameTextView.Visibility = ViewStates.Gone;
                    NoteTextView.Visibility = ViewStates.Gone;
                    ThumbnailVideo.Visibility = ViewStates.Gone;
                    PictureInToPictureButton.Visibility = ViewStates.Gone;
                    MainUserViewProfile.Visibility = ViewStates.Gone;
                    FindViewById(Resource.Id.local_video_container).Visibility = ViewStates.Gone;
                }
                else
                {
                    EndCallButton.Visibility = ViewStates.Visible;
                    MuteAudioButton.Visibility = ViewStates.Visible;
                    UserNameTextView.Visibility = ViewStates.Visible;
                    NoteTextView.Visibility = ViewStates.Visible;
                    ThumbnailVideo.Visibility = ViewStates.Visible;
                    MuteVideoButton.Visibility = ViewStates.Visible;
                    PictureInToPictureButton.Visibility = ViewStates.Visible;
                    FindViewById(Resource.Id.local_video_container).Visibility = ViewStates.Visible;
                }

                base.OnPictureInPictureModeChanged(isInPictureInPictureMode, newConfig);

            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            } 
        }

        protected override void OnUserLeaveHint()
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var param = new PictureInPictureParams.Builder().SetAspectRatio(new Rational(9, 16)).Build();
                    EnterPictureInPictureMode(param);
                }
                base.OnUserLeaveHint();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

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

                if (CallType == "Twilio_video_call")
                {
                    if (!string.IsNullOrEmpty(TwilioAccessToken))
                    {
                        if (!string.IsNullOrEmpty(UserId))
                            Load_userWhenCall();

                        TwilioVideo = TwilioVideoHelper.GetOrCreate(this);
                        UpdateState();
                        NoteTextView.Text = GetText(Resource.String.Lbl_Waiting_for_answer);
                         
                        var (apiStatus, respond) = await RequestsAsync.Call.SendAnswerCallAsync(CallId, TypeCall.Video);
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
                else if (CallType == "Twilio_video_calling_start")
                {
                    LoadProfileFromUserId();

                    NoteTextView.Text = GetText(Resource.String.Lbl_Calling_video);
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
                MainUserViewProfile.Visibility = ViewStates.Visible;

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
                var (apiStatus, respond) = await RequestsAsync.Call.CreateNewCallAsync(UserId, TypeCall.Video);
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
                                TwilioVideo?.JoinRoom(ApplicationContext, RoomName); 
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

        public override bool OnSupportNavigateUp()
        {
            TryCancelCall();
            return true;
        }

        protected virtual void UpdatingState()
        {
        }

        protected void TryCancelCall()
        {
            CloseScreen();
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
                    }
                    else
                    {
                        LocalVideoTrackId = trackId;
                        LocalvideoTrack.AddRenderer(ThumbnailVideo);
                        ThumbnailVideo.Visibility =LocalvideoTrack == null ? ViewStates.Invisible : ViewStates.Visible;
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
                    UserVideoTrack?.AddRenderer(UserprimaryVideo);
                    ThumbnailVideo.Visibility = LocalvideoTrack == null ? ViewStates.Invisible : ViewStates.Visible;
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
            try
            {
                MainUserViewProfile.Visibility = ViewStates.Visible;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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
                MainUserViewProfile.Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnParticipantDisconnected(string participantId)
        {
            RunOnUiThread(() =>
            {
                try
                { 
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
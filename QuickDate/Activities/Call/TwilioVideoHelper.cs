using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Android.Content;
using Android.Hardware;
using Android.Media;
using Android.Runtime;
using TwilioVideo;
using AudioTrack = TwilioVideo.AudioTrack;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.Call
{
    public class TwilioVideoHelper : Object, Room.IListener, Participant.IListener
    {
        public class Cameras
        {
            public static int Count()
            {
                return Camera.NumberOfCameras;
            }

            public static bool HasFrontCamera()
            {
                int numCameras = Camera.NumberOfCameras;
                var cameraInfo = new Camera.CameraInfo();
                for (int i = 0; i < numCameras; i++)
                {
                    Camera.GetCameraInfo(i, cameraInfo);
                    if (cameraInfo.Facing == CameraFacing.Front)
                        return true;
                }

                return false;
            }

            public static int GetCameraRotation()
            {
                int numCameras = Camera.NumberOfCameras;
                var cameraInfo = new Camera.CameraInfo();
                for (int i = numCameras - 1; i >= 0; i--)
                {
                    Camera.GetCameraInfo(i, cameraInfo);
                    if (cameraInfo.Facing == CameraFacing.Front)
                        break;
                }

                return cameraInfo.Orientation;
            }
        }

        public interface IListener
        {
            void SetLocalVideoTrack(LocalVideoTrack track);
            void SetRemoteVideoTrack(VideoTrack track);
            void RemoveLocalVideoTrack(LocalVideoTrack track);
            void RemoveRemoteVideoTrack(VideoTrack track);
            void OnRoomConnected(string roomId);
            void OnRoomDisconnected(StopReason reason);
            void OnParticipantConnected(string participantId);
            void OnParticipantDisconnected(string participantId);
            void SetCallTime(int seconds);
        }

        public enum StopReason
        {
            Error,
            VideoTrackRemoved,
            ParticipantDisconnected,
            RoomDisconnected
        }

        public static TwilioVideoHelper Instance { get; private set; }

        static volatile bool CallProgress;

        public static bool CallInProgress
        {
            get { return CallProgress; }
            set { CallProgress = value; }
        }

        protected LocalVideoTrack CurrentVideoTrack { get; private set; }
        protected VideoTrack RemoteVideoTrack { get; private set; }

        protected LocalAudioTrack CurrentAudioTrack { get; private set; }

        protected AudioTrack RemoteAudioTrack { get; private set; }
        protected CameraCapturer VideoCapturer { get; private set; }
        protected Participant Participant { get; private set; }
        protected Room CurrentRoom { get; private set; }

        protected Stopwatch Timer { get; private set; } = new Stopwatch();

        public bool ClientIsReady
        {
            get { return AccessToken != null; }
        }

        string AccessToken;

        IListener Listener;

        AudioManager AudioManager;
        Mode PreviousAudioMode;
        bool PreviousSpeakerphoneOn;

        public TwilioVideoHelper()
        {
        }

        public TwilioVideoHelper(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public static TwilioVideoHelper GetOrCreate(Context context)
        {
            try
            {
                if (Instance == null)
                {
                    Instance = new TwilioVideoHelper();
                }

                if (Instance.CurrentVideoTrack == null)
                    Instance.CreateLocalMedia(context);
                return Instance;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public void CreateLocalMedia(Context context)
        {
            AudioManager = (AudioManager) context.GetSystemService(Context.AudioService);
            var cameraSource = Cameras.HasFrontCamera()
                ? CameraCapturer.CameraSource.FrontCamera
                : CameraCapturer.CameraSource.BackCamera;
             
            AudioManager.SpeakerphoneOn = false;

            VideoCapturer = new CameraCapturer(context, cameraSource);

            CurrentVideoTrack = LocalVideoTrack.Create(context, true, VideoCapturer);
            CurrentAudioTrack = LocalAudioTrack.Create(context, true);
        }

        public void Bind(IListener listener)
        {
            try
            {
                Listener = listener;
                if (CurrentRoom != null)
                    Listener.OnRoomConnected(CurrentRoom.Sid);
                if (Participant != null)
                    Listener.OnParticipantConnected(Participant.Identity);
                if (CurrentVideoTrack != null)
                    Listener.SetLocalVideoTrack(CurrentVideoTrack);
                if (RemoteVideoTrack != null)
                    Listener.SetRemoteVideoTrack(RemoteVideoTrack);
                if (Timer.IsRunning)
                    Listener.SetCallTime((int) Timer.Elapsed.TotalSeconds);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        void DropRenderers(VideoTrack track)
        {
            if (track?.Renderers?.Any() == true)
                foreach (var r in track.Renderers.ToArray())
                    track.RemoveRenderer(r);
        }

        public void Unbind(IListener listener)
        {
            try
            {
                RemoveTracksRenderers();
             

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void UpdateToken(string capabilityToken)
        {
           
            if (AccessToken == capabilityToken)
                return;
            AccessToken = capabilityToken;
            if (CurrentRoom == null)
                return;
            CurrentRoom.Disconnect();
            CurrentRoom = null;
        }

        public void FlipCamera()
        {
            VideoCapturer?.SwitchCamera();
        }

        public void Mute(bool muted)
        {
            CurrentAudioTrack?.Enable(!muted);
        }

        public void JoinRoom(Context context, string roomname)
        {
            try
            {
                if (CurrentRoom != null)
                    return;
           
                IList<LocalVideoTrack> videoTracks = new List<LocalVideoTrack> {CurrentVideoTrack};
                IList<LocalAudioTrack> audioTracks = new List<LocalAudioTrack> {CurrentAudioTrack};
                var options = new ConnectOptions.Builder(AccessToken)
                    .VideoTracks(videoTracks)
                    .AudioTracks(audioTracks)
                    .RoomName(roomname)
                    .Build();

                CurrentRoom = Video.Connect(context, options, this);
                CallInProgress = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        void RemoveTracksRenderers()
        {
            DropRenderers(RemoteVideoTrack);
            DropRenderers(CurrentVideoTrack);
        }

        void ReleaseRoom()
        {
            try
            {
                try
                {
                    CurrentRoom?.Disconnect();
                }
                finally
                {
                    CurrentRoom = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ReleaseMedia()
        {
            try
            {
                if (VideoCapturer != null)
                {
                    VideoCapturer.StopCapture();
                    VideoCapturer = null;
                }

                if (CurrentVideoTrack != null)
                {
                    var videoTrack = CurrentVideoTrack;
                    CurrentVideoTrack = null;
                    CurrentRoom?.LocalParticipant.RemoveVideoTrack(videoTrack);
                    DropRenderers(videoTrack);
                    videoTrack.Release();
                }

                if (CurrentAudioTrack != null)
                {
                    var audioTrack = CurrentAudioTrack;
                    CurrentAudioTrack = null;
                    CurrentRoom?.LocalParticipant.RemoveAudioTrack(audioTrack);
                    audioTrack.Enable(false);
                    audioTrack.Release();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        void SetAudioFocus(bool focused)
        {
            try
            {
                if (focused)
                {
                    PreviousAudioMode = AudioManager.Mode;
                    PreviousSpeakerphoneOn = AudioManager.SpeakerphoneOn;
                    AudioManager.RequestAudioFocus(null, Stream.VoiceCall, AudioFocus.GainTransient);
                    AudioManager.SpeakerphoneOn = false;
                    AudioManager.Mode = Mode.InCommunication;
                }
                else
                {
                    AudioManager.Mode = PreviousAudioMode;
                    AudioManager.SpeakerphoneOn = PreviousSpeakerphoneOn;
                    AudioManager.AbandonAudioFocus(null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region Media.Listener

        public void OnAudioTrackAdded(Participant participant, AudioTrack audioTrack)
        {
            RemoteAudioTrack = audioTrack;
        }

        public void OnAudioTrackDisabled(Participant participant, AudioTrack audioTrack)
        {
        }

        public void OnAudioTrackEnabled(Participant participant, AudioTrack audioTrack)
        {

        }

        public void OnAudioTrackRemoved(Participant participant, AudioTrack audioTrack)
        {
            if (RemoteAudioTrack.TrackId == audioTrack.TrackId)
                RemoteAudioTrack = null;
        }

        public void OnVideoTrackAdded(Participant participant, VideoTrack videoTrack)
        {
            try
            {
                RemoteVideoTrack = videoTrack;
                Listener?.SetRemoteVideoTrack(RemoteVideoTrack);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnVideoTrackDisabled(Participant participant, VideoTrack videoTrack)
        {

        }

        public void OnVideoTrackEnabled(Participant participant, VideoTrack videoTrack)
        {

        }

        public void OnVideoTrackRemoved(Participant participant, VideoTrack videoTrack)
        {
            if (RemoteVideoTrack.TrackId != videoTrack.TrackId)
                return;

            Listener?.RemoveRemoteVideoTrack(RemoteVideoTrack);
            RemoteVideoTrack = null;
        }

        #endregion

        #region Room.Listener

        public void OnConnectFailure(Room room, TwilioException e)
        {
            OnFinishConversation(StopReason.Error);
        }

        public void OnConnected(Room room)
        {
            try
            {
                CurrentRoom = room;
                Listener?.OnRoomConnected(room.Name);
                var participant = room.Participants.FirstOrDefault(p => p.Identity != room.LocalParticipant.Identity);
                if (participant != null)
                    OnParticipantConnected(room, participant);
                if (AudioManager != null)
                    SetAudioFocus(true);

                if (!Timer.IsRunning)
                {
                    Timer.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnDisconnected(Room room, TwilioException e)
        {
            room.Dispose();
            Timer.Stop();
            if (e != null)
                OnFinishConversation(StopReason.Error);
            else
                OnFinishConversation(StopReason.RoomDisconnected);
        }

        public void OnParticipantConnected(Room room, Participant participant)
        {
            try
            {
                Participant = participant;
                Participant.SetListener(this);
                Timer.Restart();
                Listener?.OnParticipantConnected(participant.Identity);
                var videoTrack = Participant.VideoTracks.FirstOrDefault();
                if (videoTrack != null)
                    OnVideoTrackAdded(Participant, videoTrack);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnParticipantDisconnected(Room room, Participant participant)
        {
            try
            {
                if (Participant?.Identity != participant.Identity)
                    return;

                Listener?.OnParticipantDisconnected(participant.Identity);
                OnFinishConversation(StopReason.ParticipantDisconnected);
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnRecordingStarted(Room room = null)
        {

        }

        public void OnRecordingStopped(Room room = null)
        {

        }

        #endregion

        void OnFinishConversation(StopReason reason)
        {
            try
            {
                if (!CallInProgress)
                    return;

                CallInProgress = false;
                RemoveTracksRenderers();
                ReleaseRoom();
                Listener?.OnRoomDisconnected(reason);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void FinishCall()
        {
         
            try
            {
                Listener = null;

                ReleaseRoom();

                Participant = null;
                RemoteVideoTrack = null;
                RemoteAudioTrack = null;

                ReleaseMedia();

                if (AudioManager != null)
                {
                    SetAudioFocus(false);
                    AudioManager = null;
                }

                Timer.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                FinishCall();
                base.Dispose(disposing);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
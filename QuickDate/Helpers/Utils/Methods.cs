using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Support.CustomTabs;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Java.Security;
using Java.Util;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDateClient.Classes.Global;
using Calendar = Android.Icu.Util.Calendar;
using Console = System.Console;
using Environment = System.Environment;
using Exception = System.Exception;
using File = Java.IO.File;
using Process = Android.OS.Process;
using Random = System.Random;
using Stream = System.IO.Stream;
using String = System.String;
using Thread = System.Threading.Thread;
using TimeZone = System.TimeZone;
using Uri = Android.Net.Uri;
using MimeTypeMap = QuickDateClient.MimeTypeMap;

namespace QuickDate.Helpers.Utils
{
    public class Methods
    {

        #region Methods

        //Checks for Internet connection 
        public static bool CheckConnectivity()
        {
            try
            {
                ConnectivityManager connectivityManager = (ConnectivityManager)Application.Context.GetSystemService(Context.ConnectivityService);
                NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;
                if (networkInfo != null)
                {
                    bool isOnline = networkInfo.IsConnectedOrConnecting;
                    if (isOnline)
                    {
                        // Now that we know it's connected, determine if we're on WiFi or something else.
                        return true;
                    }

                    //NetworkState.Disconnected;
                    return false;
                }

                //NetworkState.Disconnected;
                return false;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return false;
            }
        }

        public static void SetFocusable(View v)
        {
            try
            {
                if (v == null) return; 
                v.Focusable = true;
                v.FocusableInTouchMode = true;
                v.ClearFocus();
                v.SetFocusable(ViewFocusability.NotFocusable);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        public static void SetMargin(View v, int left, int top, int right, int bottom)
        {
            try
            {
                if (v == null) return;
                var parameter = (RelativeLayout.LayoutParams)v.LayoutParameters;
                parameter.SetMargins(left, top, right, bottom); // left, top, right, bottom
                v.LayoutParameters = parameter;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        // Add Short Cut Icon Applications
        public static void AddShortcut()
        {
            try
            {
                Intent shortcutIntent = new Intent(Application.Context, typeof(HomeActivity));
                shortcutIntent.SetAction(Intent.ActionView);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                {
                    var shortCutManager = Application.Context.GetSystemService(Context.ActivityService) as ShortcutManager;

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.NMr1)
                    {
                        ShortcutInfo shortcut1 = new ShortcutInfo.Builder(Application.Context, "shortcut1")
                            .SetIntent(shortcutIntent)
                            .SetShortLabel("shortcut1")
                            .SetLongLabel("Shortcut 1")
                            .SetShortLabel("This is the shortcut 1")
                            .SetDisabledMessage("Login to open this")
                            .SetIcon(Icon.CreateWithResource(Application.Context, Resource.Drawable.icon))
                            .Build();

                        shortCutManager?.SetDynamicShortcuts((IList<ShortcutInfo>)Arrays.AsList(shortcut1));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void Set_SoundPlay(string typeUri)
        {
            try
            {
                //Type_uri >>  mystic_call - Popup_GetMesseges - Popup_SendMesseges 
                var uri = Uri.Parse("android.resource://" + Application.Context.PackageName + "/raw/" +
                                                typeUri);

                RingtoneManager.GetRingtone(Application.Context, uri).Play();
                //RingtoneManager.GetRingtone(Application.Context, uri).Play();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public static void DisplayReportResult(Activity activityContext, dynamic respond)
        {
            try
            {
                string errorText;
                if (respond is ErrorObject errorMessage)
                {
                    errorText = errorMessage.ErrorData.ErrorText;

                    if (errorText.Contains("Permission Denied"))
                        ApiRequest.Logout(activityContext);
                }
                else if (respond is ErrorData error)
                {
                    errorText = error.ErrorText;

                    if (errorText.Contains("Permission Denied"))
                        ApiRequest.Logout(activityContext);
                }
                else if (respond is InfoObject errorX)
                {
                    errorText = errorX.Errors.ErrorText;

                    if (errorText.Contains("Permission Denied"))
                        ApiRequest.Logout(activityContext);
                }
                else
                {
                    errorText = respond.ToString();
                }

                if (AppSettings.SetApisReportMode)
                {
                    DialogPopup.InvokeAndShowDialog(activityContext, "ReportMode", errorText, "Close");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("Error Report");
            }
        }


        public static ClipboardManager ClipBoardManager;

        public static void CopyToClipboard(string text)
        {
            try
            {
                ClipData clipData = ClipData.NewPlainText("text", text);
                ClipBoardManager.PrimaryClip = clipData;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public static void Load_Image_From_Url(Activity context, ImageView image, string imageUrl)
        {
            try
            {
                if (!String.IsNullOrEmpty(imageUrl))
                {
                    GlideImageLoader.LoadImage(context, imageUrl, image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                }
                else
                {
                    GlideImageLoader.LoadImage(context, "no_profile_image.png", image, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public static void Load_Image_From_Url_Normally(Activity context, ImageView image, string imageUrl)
        {
            try
            {
                if (!String.IsNullOrEmpty(imageUrl))
                {
                    GlideImageLoader.LoadImage(context, imageUrl, image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                }
                else
                {
                    GlideImageLoader.LoadImage(context, "ImagePlacholder.jpg", image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public static void Load_Image_From_Url_WithoutDownSample(Activity context, ImageView image, string imageUrl)
        {
            try
            {
                if (!String.IsNullOrEmpty(imageUrl))
                {
                    GlideImageLoader.LoadImage(context, imageUrl, image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                }
                else
                {
                    GlideImageLoader.LoadImage(context, "no_profile_image.png", image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        public static byte[] ConvertFileToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }

        #endregion

        #region Audio Record & Play

        public class AudioRecorderAndPlayer
        {
            public Android.Media.MediaPlayer Player { get; set; }
            public static Android.Media.MediaPlayer PlayerStatic { get; set; }

            public static string AudioFileNameReleased;
            public static string AudioFileFullPathReleased;
            public File Filedir;
            private readonly string SoundFile;
            private readonly File SoundFileFullPath;
            public MediaRecorder Recorder;
            public int RecorderDuration;

            public AudioRecorderAndPlayer(string applicationName)
            {
                try
                {
                    Player = new Android.Media.MediaPlayer();
                    Filedir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim);
                    SoundFile = GetTimestamp(DateTime.Now) + ".mp3";
                    AudioFileNameReleased = SoundFile;
                    SoundFileFullPath = new File(Filedir + "/" + applicationName + "/Sounds/" + SoundFile);

                    var dir = Path.AndroidDcimFolder + "/" + AppSettings.ApplicationName + "/Sounds/";
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    if (!Directory.Exists(Filedir + "/" + applicationName))
                        Directory.CreateDirectory(Filedir + "/" + applicationName);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }


            public static long Get_MediaFileDuration(string path)
            {
                try
                {
                    Android.Media.MediaPlayer mp = new Android.Media.MediaPlayer();
                    FileInputStream stream = new FileInputStream(path);
                    mp.SetDataSource(stream.FD);
                    stream.Close();
                    mp.Prepare();
                    long duration = mp.Duration;
                    mp.Release();
                    return duration;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return 0;
                }
            }

            public void StartRecourding()
            {
                try
                {
                    Recorder = new MediaRecorder();
                    Recorder.Reset();
                    Recorder.SetAudioSource(AudioSource.Mic);
                    Recorder.SetOutputFormat(OutputFormat.Default);
                    Recorder.SetAudioEncoder(AudioEncoder.AmrNb);
                    Recorder.SetOutputFile(SoundFileFullPath.AbsolutePath);
                    Recorder.Prepare();
                    Recorder.Start();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public void StopRecourding()
            {
                try
                {
                    Recorder.Stop();
                    Recorder.Release();
                    AudioFileFullPathReleased = SoundFileFullPath.AbsolutePath;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }

            }


            public string GetRecorded_Sound_Path()
            {
                if (System.IO.File.Exists(SoundFileFullPath.AbsolutePath))
                {
                    return SoundFileFullPath.AbsolutePath;
                }

                return null;
            }

            public static string Check_Sound_File_if_Exits(string folderName, string soundFile)
            {
                var soundFileFullPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim) + "/" +
                                        folderName + "/" + soundFile;
                if (System.IO.File.Exists(soundFileFullPath))
                {
                    return soundFileFullPath;
                }

                return "File Dont Exists";

            }

            public Stream GetSound_as_Stream(string path)
            {

                if (System.IO.File.Exists(path))
                {
                    byte[] databyte = System.IO.File.ReadAllBytes(path);
                    Stream stream = System.IO.File.OpenRead(path);

                    return stream;
                }

                return null;

            }

            public string Delete_Sound_Path(string path)
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);

                    return "Deleted";
                }

                return "Not exits";
            }

            public static void PlayAudioFromAsset(string fileName)
            {
                try
                {
                    PlayerStatic = new Android.Media.MediaPlayer();
                    var fd = Application.Context.Assets.OpenFd(fileName);
                    PlayerStatic.Prepared += (s, e) =>
                    {
                        PlayerStatic.Start();
                    };
                    PlayerStatic.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
                    PlayerStatic.Prepare();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void StopAudioFromAsset()
            {
                try
                {
                    if (PlayerStatic.IsPlaying)
                    {
                        PlayerStatic.Stop();
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);

                }

            }

            public void PlayAudioFromPath(string audioPath)
            {
                try
                {
                    Player.Completion += (sender, e) =>
                    {
                        Player.Reset();
                    };

                    Player.SetDataSource(audioPath);
                    Player.Prepare();
                    Player.Prepared += (s, e) =>
                    {
                        Player.Start();
                    };
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }


            public void StopAudioPlay()
            {
                if (Player.IsPlaying)
                {
                    Player.Stop();
                }
            }

            public void PauseAudioPlay()
            {
                if (Player.IsPlaying)
                {
                    Player.Pause();
                }

            }

            public static string GetTimeString(long millis)
            {
                String finalTimerString = "";
                string secondsString, minutsString;

                int hours = (int)(millis / (1000 * 60 * 60));
                int minutes = (int)(millis % (1000 * 60 * 60) / (1000 * 60));
                int seconds = (int)(millis % (1000 * 60 * 60) % (1000 * 60) / 1000);

                // Add hours if there
                if (hours > 0)
                {
                    finalTimerString = hours + ":";
                }

                // Prepending 0 to seconds if it is one digit
                if (seconds < 10)
                {
                    secondsString = "0" + seconds;
                }
                else
                {
                    secondsString = "" + seconds;
                }

                if (minutes < 10)
                {
                    minutsString = "0" + minutes;
                }
                else
                {
                    minutsString = "" + minutes;
                }

                finalTimerString = finalTimerString + minutsString + ":" + secondsString;

                return finalTimerString;
            }

        }

        #endregion

        #region Images And video

        public class MultiMedia
        {
            public static void Save_Images_CostomName(string savedfoldername, string fileUrl, string typeimage, string imageid)
            {
                try
                {
                    string unix = fileUrl.Split('/').Last();

                    string filename = imageid + "_" + typeimage + ".jpg";
                    string filePath = System.IO.Path.Combine(savedfoldername);
                    string mediaFile = filePath + "/" + filename;

                    if (!System.IO.File.Exists(mediaFile))
                    {
                        if (!Directory.Exists(filePath))
                            Directory.CreateDirectory(filePath);

                        using (WebClient web = new WebClient())
                        {
                            web.DownloadDataAsync(new System.Uri(fileUrl), mediaFile);

                            web.DownloadDataCompleted += (s, e) =>
                            {
                                try
                                {
                                    System.IO.File.WriteAllBytes(mediaFile, e.Result);
                                }
                                catch (Exception exception)
                                {
                                    Console.WriteLine(exception);
                                }
                            };
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public static string Get_Images_CostomName(string savedfoldername, string typeimage, string imageid)
            {
                try
                {
                    string filename = imageid + "_" + typeimage + ".jpg";

                    string fileUrl = GetMediaFrom_Disk(savedfoldername, filename);
                    return fileUrl;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return "File Dont Exists";
                }
            }

            public static string GetMediaFrom_Disk(string foldername, string filename)
            {
                try
                {
                    string file = foldername + "/" + filename;
                    if (System.IO.File.Exists(file))
                    {
                        FileInfo fi = new FileInfo(file);
                        var size = fi.Length;

                        FileInfo fileVol = new FileInfo(file);
                        string fileLength = fileVol.Length.ToString();

                        return file;
                    }

                    return "File Dont Exists";
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return "File Dont Exists";
                }
            }

            public static string GetMediaFrom_Gallery(string foldername, string filename)
            {
                try
                {
                    string filePath = System.IO.Path.Combine(foldername);
                    string mediaFile = filePath + "/" + filename;

                    if (System.IO.File.Exists(mediaFile))
                    {
                        return mediaFile;
                    }

                    return "File Dont Exists";
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return "File Dont Exists";
                }
            }

            public static void DeleteMediaFrom_Disk(string path)
            {
                try
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }

            }

            public static string CheckFileIfExits(string filepath)
            {
                try
                {
                    if (System.IO.File.Exists(filepath))
                    {
                        return filepath;
                    }

                    return "File Dont Exists";

                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return "File Dont Exists";
                }
            }

            public static string CopyMediaFileTo(string pathOfFile, string toFolderName, bool saveOnPersonalFolder = true, bool saveOnGallaryFolder = false)
            {
                //Change the file name to new unique name
                string fileName = pathOfFile.Contains("/")
                    ? pathOfFile.Split('/').Last()
                    : pathOfFile.Split('\\').Last();
                string extension = fileName.Split('.').Last();
                fileName = fileName.Split('.').First();
                fileName = fileName.Replace(fileName, GetTimestamp(DateTime.Now)) + "." + extension;

                string newFolderPath = System.IO.Path.Combine(toFolderName);
                string copyFileFullPath = newFolderPath + "/" + fileName;

                if (saveOnPersonalFolder)
                {
                    if (!Directory.Exists(newFolderPath))
                        Directory.CreateDirectory(newFolderPath);

                    if (System.IO.File.Exists(pathOfFile))
                    {
                        System.IO.File.Copy(pathOfFile, copyFileFullPath);
                        return copyFileFullPath;
                    }

                    return "Path File Dont exits";
                }

                if (saveOnGallaryFolder)
                {
                    newFolderPath = System.IO.Path.Combine(toFolderName);
                    copyFileFullPath = newFolderPath + "/" + fileName;

                    if (!Directory.Exists(newFolderPath))
                        Directory.CreateDirectory(newFolderPath);

                    if (System.IO.File.Exists(pathOfFile))
                    {
                        System.IO.File.Copy(pathOfFile, copyFileFullPath);
                        var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                        mediaScanIntent.SetData(Uri.FromFile(new File(copyFileFullPath)));
                        Application.Context.SendBroadcast(mediaScanIntent);

                        return copyFileFullPath;
                    }

                    //File.Copy(pathOfFile, CopyFileFullPath);
                    return "Path File Dont exits";
                }
                return "Done";
            }

            public static void DownloadMediaTo_DiskAsync(string savedfoldername, string url)
            {
                try
                {
                    if (url.Contains("http"))
                    {
                        string filename = url.Split('/').Last();
                        string filePath = System.IO.Path.Combine(savedfoldername);
                        string mediaFile = filePath + "/" + filename;

                        if (!Directory.Exists(filePath))
                            Directory.CreateDirectory(filePath);

                        if (!System.IO.File.Exists(mediaFile))
                        {
                            WebClient webClient = new WebClient();

                            webClient.DownloadDataAsync(new System.Uri(url), mediaFile);

                            webClient.DownloadDataCompleted += (s, e) =>
                            {
                                try
                                {
                                    System.IO.File.WriteAllBytes(mediaFile, e.Result);
                                }
                                catch (Exception exception)
                                {
                                    Console.WriteLine(exception);
                                }
                            };
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void DownloadMediaTo_GalleryAsync(string savedfoldername, string url)
            {
                try
                {
                    string filename = url.Split('/').Last();
                    string filePath = System.IO.Path.Combine(savedfoldername);
                    string mediaFile = filePath + "/" + filename;

                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);

                    if (!System.IO.File.Exists(mediaFile))
                    {
                        WebClient webClient = new WebClient();

                        webClient.DownloadDataAsync(new System.Uri(url));
                        webClient.DownloadDataCompleted += (s, e) =>
                        {
                            try
                            {
                                System.IO.File.WriteAllBytes(mediaFile, e.Result);
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine(exception);
                            }

                            var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                            mediaScanIntent.SetData(Uri.FromFile(new File(mediaFile)));
                            Application.Context.SendBroadcast(mediaScanIntent);
                        };
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static string GetRealVideoPathFromUri(Uri contentUri)
            {
                ICursor cursor = null;
                try
                {
                    String[] proj = { MediaStore.Video.Media.InterfaceConsts.Data };
                    cursor = Application.Context.ContentResolver.Query(contentUri, proj, null, null, null);
                    int columnIndex = cursor.GetColumnIndexOrThrow(MediaStore.Video.Media.InterfaceConsts.Data);
                    cursor.MoveToFirst();
                    return cursor.GetString(columnIndex);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return null;
                }
                finally
                {
                    cursor?.Close();
                }
            }

            public static string GetRealImagePathFromUri(Uri contentUri)
            {
                ICursor cursor = null;
                try
                {
                    String[] proj = { MediaStore.Images.Media.InterfaceConsts.Data };
                    cursor = Application.Context.ContentResolver.Query(contentUri, proj, null, null, null);
                    int columnIndex = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.Data);
                    cursor.MoveToFirst();
                    return cursor.GetString(columnIndex);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return null;
                }
                finally
                {
                    cursor?.Close();
                }
            }

            public static bool IsCameraAvailable()
            {
                PackageManager pm = Application.Context.PackageManager;
                if (pm.HasSystemFeature(PackageManager.FeatureCamera))
                    return true;

                return false;
            }

            public static Bitmap Retrieve_VideoFrame_AsBitmap(string mediaFile, ThumbnailKind thumbnailKind = ThumbnailKind.MiniKind)
            {
                try
                {
                    Bitmap bitmap = ThumbnailUtils.CreateVideoThumbnail(mediaFile, thumbnailKind);

                    return bitmap;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return null;
                }
            }

            public static void Export_Bitmap_As_Image(Bitmap bitmap, string filename, string pathTofolder)
            {
                try
                {
                    if (!Directory.Exists(pathTofolder))
                        Directory.CreateDirectory(pathTofolder);

                    string filePath = System.IO.Path.Combine(pathTofolder);
                    string mediaFile = filePath + "/" + filename + ".png";
                    var stream = new FileStream(mediaFile, FileMode.Create);
                    bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
                    stream.Close();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static Stream GetMedia_as_Stream(string path)
            {
                try
                {
                    byte[] datass = System.IO.File.ReadAllBytes(path);
                    Stream dsd = System.IO.File.OpenRead(path);
                    return dsd;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return null;
                }
            }

            public void image_compression(string path)
            {
                try
                {
                    string anyString = System.IO.File.ReadAllText(path);
                    CompressStringToFile("new.gz", anyString);
                }
                catch (Exception exception) // Couldn't compress.
                {
                    Console.WriteLine(exception);
                }
            }

            public static void CompressStringToFile(string fileName, string value)
            {
                try
                {
                    string temp = System.IO.Path.GetTempFileName();
                    System.IO.File.WriteAllText(temp, value);
                    byte[] b;
                    using (FileStream f = new FileStream(temp, FileMode.Open))
                    {
                        b = new byte[f.Length];
                        f.Read(b, 0, (int)f.Length);
                    }
                    using (FileStream f2 = new FileStream(fileName, FileMode.Create))
                    using (GZipStream gz = new GZipStream(f2, CompressionMode.Compress, false))
                    {
                        gz.Write(b, 0, b.Length);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        #endregion

        #region Contacts

        public class PhoneContactManager
        {
            public class UserContact
            {
                public string PhoneNumber { get; set; }
                public string UserDisplayName { get; set; }
                public string FirstName { get; set; }
                public string LastName { get; set; }
            }

            public static IEnumerable<UserContact> GetAllContacts()
            {
                var phoneContactsList = new ObservableCollection<UserContact>();
                using (var phones = Application.Context.ContentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, null, null, null, null))
                {
                    if (phones != null)
                    {
                        while (phones.MoveToNext())
                        {
                            try
                            {
                                string name = phones.GetString(phones.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName));
                                string phoneNumber = phones.GetString(phones.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number));

                                string[] words = name.Split(' ');
                                var contact = new UserContact();

                                contact.FirstName = words[0];

                                if (words.Length > 1)
                                    contact.LastName = words[1];
                                else
                                    contact.LastName = ""; //no last name

                                contact.UserDisplayName = name;
                                contact.PhoneNumber = phoneNumber.Replace("+", "00").Replace("-", "").Replace(" ", "");

                                var check = phoneContactsList.FirstOrDefault(a => a.PhoneNumber == contact.PhoneNumber);
                                if (check == null)
                                {
                                    phoneContactsList.Add(contact);
                                }
                            }
                            catch (Exception exception)
                            {
                                //something wrong with one contact, may be display name is completely empty, decide what to do
                                Console.WriteLine(exception);
                            }
                        }
                        phones.Close();
                    }
                    // if we get here, we can't access the contacts. Consider throwing an exception to display to the user
                }

                return phoneContactsList;
            }

            public static UserContact Get_ContactInfoBy_Id(string fromUriId)
            {
                ICursor cursor = null;
                try
                {
                    //var uri = ContactsContract.Contacts.ContentUri;
                    var contacts = Application.Context.ContentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, null, "_id = ?", new[] { fromUriId }, null);
                    if (contacts != null)
                    {
                        UserContact userContact = new UserContact();
                        contacts.MoveToFirst();
                        string displayName = contacts.GetString(contacts.GetColumnIndex("display_name"));
                        int indexNumber = contacts.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number);


                        string mobileNumber = contacts.GetString(indexNumber);

                        userContact.PhoneNumber = mobileNumber;
                        userContact.UserDisplayName = displayName;

                        //var columnNames = contacts.GetColumnNames();
                        //foreach (var columnName in columnNames)
                        //{
                        //    int index = contacts.GetColumnIndex(columnName);
                        //    var value = contacts.GetString(index);
                        //    Console.WriteLine("Allen >> index = {0}, value = {1}", index, value);
                        //}

                        if (!string.IsNullOrEmpty(mobileNumber))
                        {
                            return userContact;
                        }


                        return null;
                    }
                    else
                    {
                        return null;
                    }

                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return null;
                }
                finally
                {
                    cursor?.Close();
                }
            }

            public void InsertContact(string fisrtName, string lastName, string number, string email, string company)
            {

                try
                {
                    List<ContentProviderOperation> ops = new List<ContentProviderOperation>();
                    ContentProviderOperation.Builder builder =
                        ContentProviderOperation.NewInsert(ContactsContract.RawContacts.ContentUri);
                    builder.WithValue(ContactsContract.RawContacts.InterfaceConsts.AccountType, null);
                    builder.WithValue(ContactsContract.RawContacts.InterfaceConsts.AccountName, null);
                    ops.Add(builder.Build());

                    //Name
                    builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
                    builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
                    builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
                        ContactsContract.CommonDataKinds.StructuredName.ContentItemType);
                    builder.WithValue(ContactsContract.CommonDataKinds.StructuredName.FamilyName, lastName);
                    builder.WithValue(ContactsContract.CommonDataKinds.StructuredName.GivenName, fisrtName);
                    ops.Add(builder.Build());

                    //Number
                    builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
                    builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
                    builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
                        ContactsContract.CommonDataKinds.Phone.ContentItemType);
                    builder.WithValue(ContactsContract.CommonDataKinds.Phone.Number, number);
                    builder.WithValue(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Type,
                        ContactsContract.CommonDataKinds.Phone.InterfaceConsts.TypeCustom);
                    builder.WithValue(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Label, "Work");
                    ops.Add(builder.Build());

                    //Email
                    builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
                    builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
                    builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
                        ContactsContract.CommonDataKinds.Email.ContentItemType);
                    builder.WithValue(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Data, email);
                    builder.WithValue(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Type,
                        ContactsContract.CommonDataKinds.Email.InterfaceConsts.TypeCustom);
                    builder.WithValue(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Label, "Work");
                    ops.Add(builder.Build());

                    //Company
                    builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
                    builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
                    builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype,
                        ContactsContract.CommonDataKinds.Organization.ContentItemType);
                    builder.WithValue(ContactsContract.CommonDataKinds.Organization.InterfaceConsts.Data, company);
                    builder.WithValue(ContactsContract.CommonDataKinds.Organization.InterfaceConsts.Type,
                        ContactsContract.CommonDataKinds.Organization.InterfaceConsts.TypeCustom);
                    builder.WithValue(ContactsContract.CommonDataKinds.Organization.InterfaceConsts.Label, "Work");
                    ops.Add(builder.Build());


                    try
                    {
                        ContentProviderResult[] res = Application.Context.ContentResolver.ApplyBatch(ContactsContract.Authority,
                            ops);

                        Toast.MakeText(Application.Context, "Done contacted added", ToastLength.Short)
                            .Show();
                    }
                    catch (Exception exception)
                    {
                        Toast.MakeText(Application.Context, "Error ", ToastLength.Long).Show();
                        Console.WriteLine(exception);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

        }

        #endregion

        #region String 

        public class FunString
        {
            //========================= Variables =========================
            public static Random Random = new Random();

            //========================= Functions =========================

            //creat new Random String Session 
            public static string RandomString(int length)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXdsdaawerthklmnbvcxer46gfdsYZ0123456789";
                return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
            }

            //creat new Random Color
            public static string RandomColor()
            {
                string color = "";
                int b;
                b = Random.Next(1, 11);
                switch (b)
                {
                    case 1:
                        color = "#AD1457"; //pink
                        break;
                    case 2:
                        color = "#6A1B9A"; // purple
                        break;
                    case 3:
                        color = "#1565C0"; //blue
                        break;
                    case 4:
                        color = "#00695C"; //teal
                        break;
                    case 5:
                        color = "#00838F"; //cyan
                        break;
                    case 6:
                        color = "#558B2F"; //green
                        break;
                    case 7:
                        color = "#FF8F00"; //amber
                        break;
                    case 8:
                        color = "#D84315"; //orange
                        break;
                    case 9:
                        color = "#4E342E"; //brown
                        break;
                    case 10:
                        color = "#2196F3"; //Light blue
                        break;
                    case 11:
                        color = "#c62828"; //red
                        break;
                }
                return color;
            }

            public static string GetoLettersfromString(string key)
            {
                try
                {
                    var string1 = key.Split(' ').First();
                    var string2 = key.Split(' ').Last();

                    if (string1 != string2)
                    {
                        String substring1 = string1.Substring(0, 1);
                        String substring2 = string2.Substring(0, 1);
                        var result = substring1 + substring2;
                        return result.ToUpper();
                    }
                    else
                    {
                        String substring1 = string1.Substring(0, 2);

                        var result = substring1;
                        return result.ToUpper();
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return "";
                }
            }

            public static string Format_byte_size(string filepath)
            {
                try
                {
                    /*
                    * var size = new FileInfo(filepath).Length;
                    * double totalSize = size / 1024.0F / 1024.0F;
                    * string sizeFile = totalSize.ToString("0.### KB"); 
                    */

                    string[] sizes = { "B", "KB", "MB", "GB", "TB" };
                    double len = new FileInfo(filepath).Length;
                    int order = 0;
                    while (len >= 1024 && order < sizes.Length - 1)
                    {
                        order++;
                        len = len / 1024;
                    }

                    // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
                    // show a single decimal place, and no space.
                    string result = $"{len:0.##} {sizes[order]}";
                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return "0B";
                }
            }

            public static string UppercaseFirst(string s)
            {
                // Check for empty string.
                if (string.IsNullOrEmpty(s))
                {
                    return string.Empty;
                }
                // Return char and concat substring.
                return char.ToUpper(s[0]) + s.Substring(1);
            }

            public static string TrimTo(string str, int maxLength)
            {
                try
                {
                    if (str.Length <= maxLength)
                    {
                        return str;
                    }

                    if (str.Length > 35)
                    {
                        str.Remove(0, 10);
                        return str;
                    }

                    if (str.Length > 65)
                    {
                        str.Remove(0, 30);
                        return str;
                    }

                    if (str.Length > 85)
                    {
                        str.Remove(0, 50);
                        return str;
                    }

                    if (str.Length > 105)
                    {
                        str.Remove(0, 70);
                        return str;
                    }

                    return str.Substring(maxLength - 17, maxLength);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return str.Substring(maxLength - 17, maxLength);
                }
            }

            //SubString Cut Of
            public static string SubStringCutOf(string s, int x)
            {
                try
                {
                    if (s.Length > x)
                    {
                        String substring = s.Substring(0, x);
                        return substring + "...";
                    }

                    return s;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return s;
                }
            }

            //Null Remover >> return Empty
            public static string StringNullRemover(string s)
            {
                try
                {
                    if (string.IsNullOrEmpty(s))
                    {
                        s = "-----";
                    }
                    return s;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return s;
                }
            }

            //De code
            public static string DecodeString(string content)
            {
                try
                {
                    const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";
                    const string stripFormatting = @"<[^>]*(>|$)";
                    const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";
                    var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
                    var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
                    var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

                    var text = content;
                      
                    //Decode html specific characters
                    text = WebUtility.HtmlDecode(text);

                    //Remove tag whitespace/line breaks
                    text = tagWhiteSpaceRegex.Replace(text, "><");

                    //Replace <br /> with line breaks
                    text = lineBreakRegex.Replace(text, Environment.NewLine);

                    //Strip formatting
                    text = stripFormattingRegex.Replace(text, string.Empty);

                    var stringFormater = text.Replace(":)", "\ud83d\ude0a")
                       .Replace(";)", "\ud83d\ude09")
                       .Replace("0)", "\ud83d\ude07")
                       .Replace("(<", "\ud83d\ude02")
                       .Replace(":D", "\ud83d\ude01")
                       .Replace("*_*", "\ud83d\ude0d")
                       .Replace("(<", "\ud83d\ude02")
                       .Replace("<3", "\ud83d\u2764")
                       .Replace("/_)", "\ud83d\ude0f")
                       .Replace("-_-", "\ud83d\ude11")
                       .Replace(":-/", "\ud83d\ude15")
                       .Replace(":*", "\ud83d\ude18")
                       .Replace(":_p", "\ud83d\ude1b")
                       .Replace(":p", "\ud83d\ude1c")
                       .Replace("x(", "\ud83d\ude20")
                       .Replace("X(", "\ud83d\ude21")
                       .Replace(":_(", "\ud83d\ude22")
                       .Replace("<5", "\ud83d\u2B50")
                       .Replace(":0", "\ud83d\ude31")
                       .Replace("B)", "\ud83d\ude0e")
                       .Replace("o(", "\ud83d\ude27")
                       .Replace("</3", "\uD83D\uDC94")
                       .Replace(":o", "\ud83d\ude26")
                       .Replace("o(", "\ud83d\ude27")
                       .Replace(":__(", "\ud83d\ude2d")
                       .Replace("!_", "\uD83D\u2757")
                       .Replace("<br> <br>", "\n")
                       .Replace("<br />", "\n")
                       .Replace("[/a]", "/")
                       .Replace("[a]", "")
                       .Replace("%3A", ":")
                       .Replace("%2F", "/")
                       .Replace("%3F", "?")
                       .Replace("%3D", "=")
                       .Replace("<a href=", "")
                       .Replace("target=", "")
                       .Replace("_blank", "")
                       .Replace(@"""", "")
                       .Replace("</a>", "")
                       .Replace("class=hash", "")
                       .Replace("rel=nofollow>", "")
                       .Replace("<p>", "")
                       .Replace("</p>", "")
                       .Replace("</body>", "")
                       .Replace("<body>", "")
                       .Replace("<div>", "")
                       .Replace("<div ", "")
                       .Replace("</div>", "")
                       .Replace("&#039;", "'")
                       .Replace("&amp;", "&")
                       .Replace("&lt;", "<")
                       .Replace("&gt;", ">")
                       .Replace("&le;", "≤")
                       .Replace("&ge;", "≥")
                       .Replace("<iframe", "")
                       .Replace("</iframe>", "")
                       .Replace("<table", "")
                       .Replace("<ul>", "")
                       .Replace("<li>", "")
                       .Replace("&nbsp;", "")
                       .Replace("&amp;nbsp;&lt;/p&gt;&lt;p&gt;", "\r\n")
                       .Replace("&amp;", "&")
                       .Replace("&quot;", "")
                       .Replace("&apos;", "")
                       .Replace("&cent;", "¢")
                       .Replace("&pound;", "£")
                       .Replace("&yen;", "¥")
                       .Replace("&euro;", "€")
                       .Replace("&copy;", "©")
                       .Replace("&reg;", "®")
                       .Replace("<b>", "")
                       .Replace("<u>", "")
                       .Replace("<i>", "")
                       .Replace("</i>", "")
                       .Replace("</u>", "")
                       .Replace("</b>", "")
                       .Replace("<br>", "\n")
                       .Replace("</li>", "")
                       .Replace("</ul>", "")
                       .Replace("</table>", " ")
                       .Replace("a&#768;", "")
                       .Replace("a&#769;", "")
                       .Replace("a&#770;", "")
                       .Replace("a&#771;", "")
                       .Replace("O&#768;", "")
                       .Replace("O&#769;", "")
                       .Replace("O&#770;", "")
                       .Replace("O&#771;", "")
                       .Replace("</table>", "")
                       .Replace("&bull;", "•")
                       .Replace("&hellip;", "…")
                       .Replace("&prime;", "′")
                       .Replace("&Prime;", "″")
                       .Replace("&oline;", "‾")
                       .Replace("&frasl;", "⁄")
                       .Replace("&weierp;", "℘")
                       .Replace("&image;", "ℑ")
                       .Replace("&real;", "ℜ")
                       .Replace("&trade;", "™")
                       .Replace("&alefsym;", "ℵ")
                       .Replace("&larr;", "←")
                       .Replace("&uarr;", "↑")
                       .Replace("&rarr;", "→")
                       .Replace("&darr;", "↓")
                       .Replace("&barr;", "↔")
                       .Replace("&crarr;", "↵")
                       .Replace("&lArr;", "⇐")
                       .Replace("&uArr;", "⇑")
                       .Replace("&rArr;", "⇒")
                       .Replace("&dArr;", "⇓")
                       .Replace("&hArr;", "⇔")
                       .Replace("&forall;", "∀")
                       .Replace("&part;", "∂")
                       .Replace("&exist;", "∃")
                       .Replace("&empty;", "∅")
                       .Replace("&nabla;", "∇")
                       .Replace("&isin;", "∈")
                       .Replace("&notin;", "∉")
                       .Replace("&ni;", "∋")
                       .Replace("&prod;", "∏")
                       .Replace("&sum;", "∑")
                       .Replace("&minus;", "−")
                       .Replace("&lowast", "∗")
                       .Replace("&radic;", "√")
                       .Replace("&prop;", "∝")
                       .Replace("&infin;", "∞")
                       .Replace("&OEig;", "Œ")
                       .Replace("&oelig;", "œ")
                       .Replace("&Yuml;", "Ÿ")
                       .Replace("&spades;", "♠")
                       .Replace("&clubs;", "♣")
                       .Replace("&hearts;", "♥")
                       .Replace("&diams;", "♦")
                       .Replace("&thetasym;", "ϑ")
                       .Replace("&upsih;", "ϒ")
                       .Replace("&piv;", "ϖ")
                       .Replace("&Scaron;", "Š")
                       .Replace("&scaron;", "š")
                       .Replace("&ang;", "∠")
                       .Replace("&and;", "∧")
                       .Replace("&or;", "∨")
                       .Replace("&cap;", "∩")
                       .Replace("&cup;", "∪")
                       .Replace("&int;", "∫")
                       .Replace("&there4;", "∴")
                       .Replace("&sim;", "∼")
                       .Replace("&cong;", "≅")
                       .Replace("&asymp;", "≈")
                       .Replace("&ne;", "≠")
                       .Replace("&equiv;", "≡")
                       .Replace("&le;", "≤")
                       .Replace("&ge;", "≥")
                       .Replace("&sub;", "⊂")
                       .Replace("&sup;", "⊃")
                       .Replace("&nsub;", "⊄")
                       .Replace("&sube;", "⊆")
                       .Replace("&supe;", "⊇")
                       .Replace("&oplus;", "⊕")
                       .Replace("&otimes;", "⊗")
                       .Replace("&perp;", "⊥")
                       .Replace("&sdot;", "⋅")
                       .Replace("&lcell;", "⌈")
                       .Replace("&rcell;", "⌉")
                       .Replace("&lfloor;", "⌊")
                       .Replace("&rfloor;", "⌋")
                       .Replace("&lang;", "⟨")
                       .Replace("&rang;", "⟩")
                       .Replace("&loz;", "◊")
                       .Replace("</table>", " ");

                    return stringFormater;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return "";
                }
            }

            //String format numbers to millions, thousands with rounding
            public static string FormatPriceValue(int num)
            {
                try
                {
                    if (num >= 100000000)
                    {
                        return ((num >= 10050000 ? num - 500000 : num) / 1000000D).ToString("#M");
                    }
                    if (num >= 10000000)
                    {
                        return ((num >= 10500000 ? num - 50000 : num) / 1000000D).ToString("0.#M");
                    }
                    if (num >= 1000000)
                    {
                        return ((num >= 1005000 ? num - 5000 : num) / 1000000D).ToString("0.##M");
                    }
                    if (num >= 100000)
                    {
                        return ((num >= 100500 ? num - 500 : num) / 1000D).ToString("0.k");
                    }
                    if (num >= 10000)
                    {
                        return ((num >= 10550 ? num - 50 : num) / 1000D).ToString("0.#k");
                    }

                    return num >= 1000 ? ((num >= 1005 ? num - 5 : num) / 1000D).ToString("0.##k") : num.ToString("#,0");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return num.ToString();
                }
            }

            public static bool IsEmailValid(string emailaddress)
            {
                try
                {
                    MailAddress m = new MailAddress(emailaddress);

                    return true;
                }
                catch (FormatException)
                {
                    return false;
                }
            }

            public static bool IsUrlValid(string url)
            {
                try
                {
                    string pattern =
                        @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
                    Regex reg = new Regex(pattern, RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
                    Match m = reg.Match(url);
                    while (m.Success)
                    {
                        //do things with your matching text 
                        m = m.NextMatch();
                        break;
                    }
                    if (reg.IsMatch(url))
                    {
                        var ss = "http://" + m.Value;
                        return true;
                    }

                    return false;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return false;
                }
            }

            public static bool IsPhoneNumber(string number)
            {
                return Regex.Match(number, @"^(\+?)([0-9]{9,20}?)$").Success;
            }


            // Functions convert color RGB to HEX
            public static string ConvertColorRgBtoHex(string color)
            {
                //to rgba => string.Format("rgba({0}, {1}, {2}, {3});", color_red, color_green, color_blue, color_alpha);
                try
                {
                    if (color.Contains("rgb"))
                    {
                        var regex = new Regex(@"([0-9]+)");
                        string colorData = color;

                        var matches = regex.Matches(colorData);

                        var colorRed = Convert.ToInt32(matches[0].ToString());
                        var colorGreen = Convert.ToInt32(matches[1].ToString());
                        var colorBlue = Convert.ToInt32(matches[2].ToString());
                        var colorAlpha = Convert.ToInt16(matches[3].ToString());
                        var hex = $"#{colorRed:X2}{colorGreen:X2}{colorBlue:X2}";

                        return hex;
                    }

                    return AppSettings.MainColor;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return AppSettings.MainColor;
                }
            }

            public static bool OnlyHexInString(string color)
            {
                try
                {
                    if (color.Contains("rgba"))
                    {
                        var regex = new Regex(@"([0-9]+)");
                        string colorData = color;

                        var matches = regex.Matches(colorData);

                        var colorRed = Convert.ToInt32(matches[0].ToString());
                        var colorGreen = Convert.ToInt32(matches[1].ToString());
                        var colorBlue = Convert.ToInt32(matches[2].ToString());
                        var colorAlpha = Convert.ToInt16(matches[3].ToString());
                        var hex = $"#{colorAlpha:X2}{colorRed:X2}{colorGreen:X2}{colorBlue:X2}";

                        return true;
                    }

                    if (color.Contains("rgb"))
                    {
                        var regex = new Regex(@"([0-9]+)");
                        string colorData = color;

                        var matches = regex.Matches(colorData);
                        var colorRed = Convert.ToInt32(matches[0].ToString());
                        var colorGreen = Convert.ToInt32(matches[1].ToString());
                        var colorBlue = Convert.ToInt32(matches[2].ToString());
                        var colorAlpha = Convert.ToInt16(00);
                        var hex = $"#{colorAlpha:X2}{colorRed:X2}{colorGreen:X2}{colorBlue:X2}";

                        return true;
                    }

                    var rxColor = new Regex("^#(?:[0-9a-fA-F]{3}){1,2}$");
                    var rxColor2 = new Regex("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3}|[0-9]{3}|[0-9]{6})$");
                    var rxColor3 = new Regex(@"\A\b[0-9a-fA-F]+\b\Z");
                    var rxColor4 =
                        new Regex(
                            @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"); // For C-style hex notation (0xFF) you can use @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"

                    if (rxColor.IsMatch(color) || rxColor2.IsMatch(color) || rxColor3.IsMatch(color) ||
                        rxColor4.IsMatch(color))
                    {
                        return true;
                    }

                    return false;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return false;
                }
            }

            public static string Check_Regex(string text)
            {
                try
                {
                    var rxEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    var rxWebsite = new Regex(@"^(http|https|ftp|www)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*$");
                    var rxHashtag = new Regex(@"(?<=#)\w+");
                    var rxMention = new Regex("@(?<name>[^\\s]+)");
                    var rxNumber1 = new Regex(@"^\d$");
                    var rxNumber2 = new Regex("[0-9]");
                    var resultEmail = IsEmailValid(text);
                    var resultWeb = IsUrlValid(text);

                    if (rxEmail.IsMatch(text) || resultEmail)
                    {
                        return "Email";
                    }

                    if (rxWebsite.IsMatch(text) || resultWeb)
                    {
                        return "Website";
                    }

                    if (rxHashtag.IsMatch(text))
                    {
                        return "Hashtag";
                    }

                    if (rxMention.IsMatch(text))
                    {
                        //var results = Rx_Mention.Matches(text).Cast<Match>().Select(m => m.Groups["name"].Value).ToArray();

                        return "Mention";
                    }

                    if (rxNumber1.IsMatch(text) || rxNumber2.IsMatch(text))
                    {
                        return "Number";
                    }

                    return text;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return text;
                }
            }
        }

        #endregion

        #region Time

        public class Time
        {
            public static string LblJustNow = Application.Context.GetText(Resource.String.Lbl_justNow);
            public static string LblHours = Application.Context.GetText(Resource.String.Lbl_hours);
            public static string LblDays = Application.Context.GetText(Resource.String.Lbl_days);
            public static string LblMonth = Application.Context.GetText(Resource.String.Lbl_month);
            public static string LblMinutes = Application.Context.GetText(Resource.String.Lbl_minutes);
            public static string LblSeconds = Application.Context.GetText(Resource.String.Lbl_seconds);
            public static string LblYear = Application.Context.GetText(Resource.String.Lbl_year);
            public static string LblCutHours = Application.Context.GetText(Resource.String.Lbl_CutHours);
            public static string LblCutDays = Application.Context.GetText(Resource.String.Lbl_CutDays);
            public static string LblCutMonth = Application.Context.GetText(Resource.String.Lbl_CutMonth);
            public static string LblCutMinutes = Application.Context.GetText(Resource.String.Lbl_CutMinutes);
            public static string LblCutSeconds = Application.Context.GetText(Resource.String.Lbl_CutSeconds);
            public static string LblCutYear = Application.Context.GetText(Resource.String.Lbl_CutYear);
            public static string LblAboutMinute = Application.Context.GetText(Resource.String.Lbl_about_minute);
            public static string LblAboutHour = Application.Context.GetText(Resource.String.Lbl_about_hour);
            public static string LblYesterday = Application.Context.GetText(Resource.String.Lbl_yesterday);
            public static string LblAboutMonth = Application.Context.GetText(Resource.String.Lbl_about_month);
            public static string LblAboutYear = Application.Context.GetText(Resource.String.Lbl_about_year);

            //Split String Duration (00:00:00)
            public static string SplitStringDuration(string duration)
            {
                try
                {
                    string[] durationsplit = duration.Split(':');
                    if (durationsplit.Length == 3)
                    {

                        if (durationsplit[0] == "00")
                        {
                            string newDuration = durationsplit[1] + ":" + durationsplit[2];
                            return newDuration;
                        }

                        return duration;
                    }

                    return duration;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return duration;
                }
            }

            //Get TimeZone
            public static string GetTimeZone()
            {
                try
                {
                    const string dataFmt = "{0,-30}{1}";
                    const string timeFmt = "{0,-30}{1:MM-dd-yyyy HH:mm}";
                    TimeZone curTimeZone = TimeZone.CurrentTimeZone;
                    // What is TimeZone name?
                    Console.WriteLine(dataFmt, "TimeZone Name:", curTimeZone.StandardName);
                    // Is TimeZone DayLight Saving?|
                    Console.WriteLine(dataFmt, "Daylight saving time?", curTimeZone.IsDaylightSavingTime(DateTime.Now));
                    // What is GMT (also called Coordinated Universal Time (UTC)
                    DateTime curUtc = curTimeZone.ToUniversalTime(DateTime.Now);
                    Console.WriteLine(timeFmt, "Coordinated Universal Time:", curUtc);
                    // What is GMT/UTC offset ?
                    TimeSpan currentOffset = curTimeZone.GetUtcOffset(DateTime.Now);
                    Console.WriteLine(dataFmt, "UTC offset:", currentOffset);
                    // Get DaylightTime object 
                    DaylightTime dl = curTimeZone.GetDaylightChanges(DateTime.Now.Year);
                    // DateTime when the daylight-saving period begins.
                    Console.WriteLine("Start: {0:MM-dd-yyyy HH:mm} ", dl.Start);
                    // DateTime when the daylight-saving period ends.
                    Console.WriteLine("End: {0:MM-dd-yyyy HH:mm} ", dl.End);
                    // Difference between standard time and the daylight-saving time.
                    Console.WriteLine("delta: {0}", dl.Delta);
                    Console.Read();

                    Calendar cal = Calendar.Instance;
                    var tz = cal.TimeZone;
                    Log.Debug("Time zone", "=" + tz.DisplayName);

                    var time = Java.Util.TimeZone.Default.DisplayName;
                    return !string.IsNullOrEmpty(time) ? time : "UTC";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return "UTC";
                }
            }

            public static string TimeAgo(int time)
            {
                try
                {
                    DateTime dateTime = UnixTimeStampToDateTime(time);
                    string result;
                    var timeSpan = DateTime.Now.Subtract(dateTime);

                    if (timeSpan <= TimeSpan.FromSeconds(60))
                    {
                        //result = $"{timeSpan.Seconds} " + Lbl_seconds;
                        result = LblJustNow;
                    }
                    else if (timeSpan <= TimeSpan.FromMinutes(60))
                    {
                        result = timeSpan.Minutes > 1 ? $"{timeSpan.Minutes} " + LblMinutes : LblAboutMinute;
                    }
                    else if (timeSpan <= TimeSpan.FromHours(24))
                    {
                        result = timeSpan.Hours > 1 ? $"{timeSpan.Hours} " + LblHours : LblAboutHour;
                    }
                    else if (timeSpan <= TimeSpan.FromDays(30))
                    {
                        result = timeSpan.Days > 1 ? $"{timeSpan.Days} " + LblDays : LblYesterday;
                    }
                    else if (timeSpan <= TimeSpan.FromDays(365))
                    {
                        result = timeSpan.Days > 30 ? $"{timeSpan.Days / 30} " + LblMonth : LblAboutMonth;
                    }
                    else
                    {
                        result = timeSpan.Days > 365 ? $"{timeSpan.Days / 365} " + LblYear : LblAboutYear;
                    }

                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return time.ToString();
                }
            }

            public static string TimeAgo(DateTime dateTime)
            {
                try
                {
                    string result;
                    var timeSpan = DateTime.Now.Subtract(dateTime);

                    if (timeSpan <= TimeSpan.FromSeconds(60))
                    {
                        //result = $"{timeSpan.Seconds} " + Lbl_seconds;
                        result = LblJustNow;
                    }
                    else if (timeSpan <= TimeSpan.FromMinutes(60))
                    {
                        result = timeSpan.Minutes > 1 ? $"{timeSpan.Minutes} " + LblMinutes : LblAboutMinute;
                    }
                    else if (timeSpan <= TimeSpan.FromHours(24))
                    {
                        result = timeSpan.Hours > 1 ? $"{timeSpan.Hours} " + LblHours : LblAboutHour;
                    }
                    else if (timeSpan <= TimeSpan.FromDays(30))
                    {
                        result = timeSpan.Days > 1 ? $"{timeSpan.Days} " + LblDays : LblYesterday;
                    }
                    else if (timeSpan <= TimeSpan.FromDays(365))
                    {
                        result = timeSpan.Days > 30 ? $"{timeSpan.Days / 30} " + LblMonth : LblAboutMonth;
                    }
                    else
                    {
                        result = timeSpan.Days > 365 ? $"{timeSpan.Days / 365} " + LblYear : LblAboutYear;
                    }

                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return dateTime.ToShortTimeString();
                }
            }

            //Functions Replace Time
            public static string ReplaceTime(string time)
            {
                if (time.Contains("hours ago") || time.Contains("hour ago"))
                {
                    time = time.Replace("hours ago", LblCutHours);
                    time = time.Replace("hour ago", LblCutHours);
                }
                else if (time.Contains("days ago") || time.Contains("day ago"))
                {
                    time = time.Replace("days ago", LblCutDays);
                    time = time.Replace("day ago", LblCutDays);
                }
                else if (time.Contains("month ago"))
                {
                    time = time.Replace("month ago", LblCutMonth);
                }
                else if (time.Contains("minutes ago") || time.Contains("minutes ago"))
                {
                    time = time.Replace("minutes ago", LblCutMinutes);
                }
                else if (time.Contains("seconds ago") || time.Contains("second ago"))
                {
                    time = time.Replace("seconds ago", LblCutSeconds);
                    time = time.Replace("second ago", LblCutSeconds);
                }
                else if (time.Contains("year ago") || time.Contains("years ago"))
                {
                    time = time.Replace("year ago", LblCutYear);
                    time = time.Replace("years ago", LblCutYear);
                }
                else if (time.Contains("yesterday"))
                {
                    time = time.Replace("yesterday", LblYesterday);
                }

                return time;
            }

            //convert a Unix timestamp to DateTime 
            public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
            {
                // Unix timestamp is seconds past epoch
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
                return dtDateTime;
            }

            #region To days

            public static double ConvertMillisecondsToDays(double milliseconds)
            {
                return TimeSpan.FromMilliseconds(milliseconds).TotalDays;
            }

            public static double ConvertSecondsToDays(double seconds)
            {
                return TimeSpan.FromSeconds(seconds).TotalDays;
            }

            public static double ConvertMinutesToDays(double minutes)
            {
                return TimeSpan.FromMinutes(minutes).TotalDays;
            }

            public static double ConvertHoursToDays(double hours)
            {
                return TimeSpan.FromHours(hours).TotalDays;
            }

            #endregion

            #region To hours

            public static double ConvertMillisecondsToHours(double milliseconds)
            {
                return TimeSpan.FromMilliseconds(milliseconds).TotalHours;
            }

            public static double ConvertSecondsToHours(double seconds)
            {
                return TimeSpan.FromSeconds(seconds).TotalHours;
            }

            public static double ConvertMinutesToHours(double minutes)
            {
                return TimeSpan.FromMinutes(minutes).TotalHours;
            }

            public static double ConvertDaysToHours(double days)
            {
                return TimeSpan.FromHours(days).TotalHours;
            }

            #endregion

            #region To minutes

            public static double ConvertMillisecondsToMinutes(double milliseconds)
            {
                return TimeSpan.FromMilliseconds(milliseconds).Minutes;
            }

            public static double ConvertSecondsToMinutes(double seconds)
            {
                return TimeSpan.FromSeconds(seconds).TotalMinutes;
            }

            public static double ConvertHoursToMinutes(double hours)
            {
                return TimeSpan.FromHours(hours).TotalMinutes;
            }

            public static double ConvertDaysToMinutes(double days)
            {
                return TimeSpan.FromDays(days).TotalMinutes;
            }

            #endregion

            #region To seconds

            public static double ConvertMillisecondsToSeconds(double milliseconds)
            {
                return TimeSpan.FromMilliseconds(milliseconds).Seconds;
            }

            public static double ConvertMinutesToSeconds(double minutes)
            {
                return TimeSpan.FromMinutes(minutes).TotalSeconds;
            }

            public static double ConvertHoursToSeconds(double hours)
            {
                return TimeSpan.FromHours(hours).TotalSeconds;
            }

            public static double ConvertDaysToSeconds(double days)
            {
                return TimeSpan.FromDays(days).TotalSeconds;
            }

            #endregion

            #region To milliseconds

            public static double ConvertSecondsToMilliseconds(double seconds)
            {
                return TimeSpan.FromSeconds(seconds).TotalMilliseconds;
            }

            public static double ConvertMinutesToMilliseconds(double minutes)
            {
                return TimeSpan.FromMinutes(minutes).TotalMilliseconds;
            }

            public static double ConvertHoursToMilliseconds(double hours)
            {
                return TimeSpan.FromHours(hours).TotalMilliseconds;
            }

            public static double ConvertDaysToMilliseconds(double days)
            {
                return TimeSpan.FromDays(days).TotalMilliseconds;
            }

            #endregion

        }

        #endregion

        #region Dialog Popup

        public class DialogPopup
        {
            public enum MessageResult
            {
                None = 0,
                Ok = 1,
                Cancel = 2,
                Abort = 3,
                Retry = 4,
                Ignore = 5,
                Yes = 6,
                No = 7
            }

            readonly Activity Mcontext;

            public DialogPopup(Activity activity)
            {
                Mcontext = activity;
            }

            public Task<MessageResult> ShowDialog(string title, string message, bool setCancelable = false,
                bool setInverseBackgroundForced = false, MessageResult positiveButton = MessageResult.Ok,
                MessageResult negativeButton = MessageResult.None, MessageResult neutralButton = MessageResult.None,
                int iconAttribute = Android.Resource.Attribute.AlertDialogIcon)
            {
                var tcs = new TaskCompletionSource<MessageResult>();

                var builder = new AlertDialog.Builder(Mcontext, Resource.Style.AlertDialogCustom);
                //builder.SetIconAttribute(IconAttribute);
                builder.SetTitle(title);
                builder.SetMessage(message);
                builder.SetInverseBackgroundForced(setInverseBackgroundForced);
                builder.SetCancelable(setCancelable);

                builder.SetPositiveButton(
                    positiveButton != MessageResult.None ? positiveButton.ToString() : string.Empty,
                    (senderAlert, args) =>
                    {
                        tcs.SetResult(positiveButton);
                    });
                builder.SetNegativeButton(
                    negativeButton != MessageResult.None ? negativeButton.ToString() : string.Empty,
                    delegate
                    {
                        tcs.SetResult(negativeButton);
                    });
                builder.SetNeutralButton(
                    neutralButton != MessageResult.None ? neutralButton.ToString() : string.Empty,
                    delegate
                    {
                        tcs.SetResult(neutralButton);
                    });

                builder.Show();

                return tcs.Task;
            }

            public static void InvokeAndShowDialog(Activity activity, string title, string message, string positiveButtonstring)
            {
                try
                {
                    activity.RunOnUiThread(() =>
                    {
                        try
                        {
                            Android.Support.V7.App.AlertDialog.Builder builder;
                            builder = new Android.Support.V7.App.AlertDialog.Builder(activity, Resource.Style.AlertDialogCustom);
                            builder.SetTitle(title);
                            builder.SetMessage(message);
                            builder.SetCancelable(false);
                            builder.SetPositiveButton(positiveButtonstring, delegate { builder.Dispose(); });
                            builder.Show();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }


        #endregion

        #region IApp

        public class App
        {
            public static void FullScreenApp(Activity context)
            {
                try
                {
                    if (AppSettings.EnableFullScreenApp)
                    {
                        View mContentView = context.Window.DecorView;
                        var uiOptions = (int)mContentView.SystemUiVisibility;
                        var newUiOptions = uiOptions;

                        newUiOptions |= (int)SystemUiFlags.Fullscreen;
                        newUiOptions |= (int)SystemUiFlags.HideNavigation;
                        mContentView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;

                        context.Window.AddFlags(WindowManagerFlags.Fullscreen);
                        //context.Window.RequestFeature(WindowFeatures.NoTitle);

                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }


            public static void OpenWebsiteUrl(Context context, string website)
            {
                try
                {
                    var uri = Uri.Parse(website);
                    var intent = new Intent(Intent.ActionView, uri);
                    intent.AddFlags(ActivityFlags.NewTask);
                    context.StartActivity(intent);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void OpenbrowserUrl(Context context, String url)
            {
                try
                {
                    CustomTabsIntent.Builder builder = new CustomTabsIntent.Builder();
                    CustomTabsIntent customTabsIntent = builder.Build();
                    customTabsIntent.Intent.AddFlags(ActivityFlags.NewTask);
                    customTabsIntent.LaunchUrl(context, Uri.Parse(url));
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void OpenAppByPackageName(Context context, string packageName, string userId = "")
            {
                try
                {
                    Intent intent;
                    Intent chkintent = context.PackageManager.GetLaunchIntentForPackage(packageName);
                    if (chkintent != null)
                    {
                        if (userId != "")
                        {
                            Intent launchIntent = context.PackageManager.GetLaunchIntentForPackage(packageName);
                            if (launchIntent != null)
                            {
                                launchIntent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                                launchIntent.PutExtra("UserID", userId);

                                launchIntent.AddFlags(ActivityFlags.SingleTop);
                                context.StartActivity(launchIntent);
                            }
                        }
                        else
                        {
                            intent = context.PackageManager.GetLaunchIntentForPackage(packageName);
                            intent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                            intent.AddFlags(ActivityFlags.SingleTop);
                            context.StartActivity(intent);
                        }
                    }
                    else
                    {
                        intent = new Intent(Intent.ActionView, Uri.Parse("market://details?id=" + packageName));
                        intent.AddFlags(ActivityFlags.NewTask);
                        context.StartActivity(intent);
                    }
                }
                catch (ActivityNotFoundException es)
                {
                    Console.WriteLine(es);
                    var intent = new Intent(Intent.ActionView, Uri.Parse("http://play.google.com/store/apps/details?id=" + packageName));
                    intent.AddFlags(ActivityFlags.NewTask);
                    context.StartActivity(intent);
                }
            }

            public static void ClearWebViewCache(Activity context)
            {
                try
                {
                    WebView wv = new WebView(context);
                    // wv.ClearCache(true);

                    if (AppSettings.RenderPriorityFastPostLoad)
                    {
                        wv.Settings.SetRenderPriority(WebSettings.RenderPriority.High);
                        wv.Settings.SetAppCacheEnabled(true);
                        wv.Settings.EnableSmoothTransition();
                        wv.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.NarrowColumns);

                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
                            wv.SetLayerType(LayerType.Hardware, null);
                        else
                            wv.SetLayerType(LayerType.Software, null);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void SendSms(Context context, string phoneNumber, string textmessges)
            {
                try
                {
                    var smsUri = Uri.Parse("smsto:" + phoneNumber);
                    var smsIntent = new Intent(Intent.ActionSendto, smsUri);
                    smsIntent.PutExtra("sms_body", textmessges);
                    smsIntent.AddFlags(ActivityFlags.NewTask);
                    context.StartActivity(smsIntent);

                    //Or use this code
                    // Android.Telephony.SmsManager.Default.SendTextMessage(item.PhoneNumber, null, "Hello Xamarin This is My Test SMS", null, null);

                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void SaveContacts(Context context, string phonenumber, string name, string type)
            {
                try
                {
                    if (type == "1")
                    {
                        Intent intent = new Intent(ContactsContract.Intents.Insert.Action);
                        intent.SetType(ContactsContract.RawContacts.ContentType);
                        intent.PutExtra(ContactsContract.Intents.Insert.Phone, phonenumber);
                        intent.PutExtra(ContactsContract.Intents.Insert.Name, name);
                        intent.PutExtra(ContactsContract.Intents.Insert.Email, "wael@test.com");
                        context.StartActivity(intent);
                    }
                    else
                    {
                        var contactUri = Uri.Parse("tel:" + phonenumber);
                        Intent intent = new Intent(ContactsContract.Intents.ShowOrCreateContact, contactUri);
                        intent.PutExtra(ContactsContract.Intents.ExtraRecipientContactName, true);
                        context.StartActivity(intent);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void SendEmail(Context context, string email)
            {
                try
                {
                    var emailIntent = new Intent(Intent.ActionSend);
                    emailIntent.PutExtra(Intent.ExtraEmail, email);
                    emailIntent.PutExtra(Intent.ExtraCc, email);
                    emailIntent.PutExtra(Intent.ExtraSubject, "Hello Email");
                    emailIntent.PutExtra(Intent.ExtraText, " ");
                    emailIntent.SetType("message/rfc822");
                    context.StartActivity(emailIntent);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void SendPhoneCall(Context context, string phoneNumber)
            {
                try
                {
                    var urlNumber = Uri.Parse("tel:" + phoneNumber);
                    var callIntent = new Intent(Intent.ActionCall);
                    callIntent.SetData(urlNumber);
                    callIntent.AddFlags(ActivityFlags.NewTask);
                    context.StartActivity(callIntent);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void Restart_App(Context context, string packageName)
            {
                try
                {
                    Intent intent =
                        Application.Context.PackageManager.GetLaunchIntentForPackage(packageName);
                    // If not NULL run the app, if not, take the user to the app store
                    if (intent != null)
                    {
                        intent.AddFlags(ActivityFlags.NewTask);
                        context.StartActivity(intent);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void Close_App()
            {
                try
                {
                    Process.KillProcess(Process.MyPid());
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    Process.KillProcess(Process.MyPid());
                }
            }


            public static string GetKeyHashesConfigured(Context applicationContext)
            {
                try
                {
                    PackageInfo info = applicationContext.PackageManager.GetPackageInfo(applicationContext.PackageName, PackageInfoFlags.Signatures);
                    foreach (var signature in info.Signatures)
                    {
                        MessageDigest md = MessageDigest.GetInstance("SHA");
                        md.Update(signature.ToByteArray());
                        var dd = signature.ToString();
                        var a = md.Algorithm;
                        var ass = md.DigestLength;
                        var asss = md.ToString();
                        string returnValue = Convert.ToBase64String(md.Digest());

                        Log.Debug("KeyHash:", returnValue);
                        return returnValue;
                    }
                }
                catch (PackageManager.NameNotFoundException e)
                {
                    Console.WriteLine(e);
                    return "";
                }
                catch (NoSuchAlgorithmException e)
                {
                    Console.WriteLine(e);
                    return "";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return "";
                }
                return "";
            }

            public static string GetValueFromManifest(Context applicationContext, string nameValue)
            {
                try
                {
                    ApplicationInfo ai = applicationContext.PackageManager.GetApplicationInfo(applicationContext.PackageName, PackageInfoFlags.MetaData);
                    Bundle bundle = ai.MetaData;
                    string myApiKey = bundle.GetString(nameValue);
                    return myApiKey;
                }
                catch (PackageManager.NameNotFoundException e)
                {
                    string error = "Failed to load meta-data, NameNotFound: " + e.Message;
                    Console.WriteLine(error);
                    Console.WriteLine(e.InnerException);
                }
                catch (NullPointerException e)
                {
                    Console.WriteLine(e.InnerException);
                    Console.WriteLine("Failed to load meta-data, NullPointer: " + e.Message);
                }

                return "";
            }
        }

        #endregion
         
        #region LocalNotification

        public class LocalNotification
        {
            public static string NotificationId = "NOTIFICATION_ID";
            public static string ChannelId = "Channel_2018";
            public static WebClient WebClient = new WebClient();

            public static void CreateLocalNotification(string notificationId, string title, string contentText)
            {
                try
                {
                    if (AppSettings.ShowNotification)
                    {
                        // Instantiate the builder and set notification elements:
                        Notification.Builder builder = new Notification.Builder(Application.Context)
                            .SetContentTitle(title) //Sample Notification
                            .SetContentText(contentText) //Hello World! This is my first notification!
                            .SetStyle(new Notification.BigTextStyle().BigText(contentText))
                            .SetSmallIcon(Resource.Drawable.icon);

                        builder.SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate);

                        // Build the notification:
                        Notification notification = builder.Build();

                        // Get the notification manager:
                        NotificationManager notificationManager =
                            Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

                        // Publish the notification:
                        var id = Convert.ToInt32(notificationId);

                        if (notificationManager != null) notificationManager.Notify(id, notification);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public static void Create_Progress_Notification(string notificationId, string notificationTitle)
            {
                try
                {
                    if (AppSettings.ShowNotification)
                    {
                        NotificationManagerCompat notificationManager = NotificationManagerCompat.From(Application.Context);
                        var id = Convert.ToInt32(notificationId);

                        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                        {
                            // Create the NotificationChannel, but only on API 26+ because
                            // the NotificationChannel class is new and not in the support library
                            var channel = new NotificationChannel(ChannelId, "Video_Notifciation_Channel_1", NotificationImportance.High);
                            channel.Description = "";
                            channel.EnableVibration(true);
                            channel.LockscreenVisibility = NotificationVisibility.Public;
                            if (notificationManager != null)
                            {
                                //notificationManager.CreateNotificationChannel(channel);
                            }
                        }

                        var notificationBroadcasterAction = new Intent(Application.Context, typeof(NotificationBroadcasterCloser));
                        notificationBroadcasterAction.PutExtra(NotificationId, notificationId);
                        notificationBroadcasterAction.PutExtra("type", "dismiss");
                        PendingIntent cancelIntent = PendingIntent.GetBroadcast(Application.Context, id, notificationBroadcasterAction, 0);

                        NotificationCompat.Builder builder = new NotificationCompat.Builder(Application.Context)
                            .SetContentTitle(notificationTitle).SetOngoing(true).SetProgress(100, 0, false)
                            .SetSmallIcon(Resource.Drawable.icon);
                        builder.SetPriority(NotificationCompat.PriorityMax);
                        //.AddAction(Resource.Drawable.icon, "Dismiss", cancelIntent)

                        Notification notification = builder.Build();

                        try
                        {
                            string url = "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4";
                            string filename = url.Split('/').Last();
                            string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "mmSavedVideos");
                            string mediaFile = filePath + "/" + filename;

                            if (!System.IO.File.Exists(mediaFile))
                            {
                                WebClient = new WebClient();

                                if (!Directory.Exists(filePath))
                                    Directory.CreateDirectory(filePath);

                                WebClient.DownloadFileAsync(new System.Uri(url), mediaFile);

                                WebClient.DownloadProgressChanged += (sender, ep) => {

                                    double bytesIn = double.Parse(ep.BytesReceived.ToString());
                                    double totalBytes = double.Parse(ep.TotalBytesToReceive.ToString());
                                    double percentage = bytesIn / totalBytes * 100;
                                    var presint = Convert.ToInt32(percentage);

                                    new Thread(() =>
                                    {
                                        builder.SetProgress(100, presint, false);
                                        if (notificationManager != null)
                                            notificationManager.Notify(Convert.ToInt32(id), builder.Build());
                                    }).Start();
                                };
                                WebClient.DownloadDataCompleted += (s, e) =>
                                {
                                    try
                                    {
                                        builder.SetContentText("Download complete")
                                            .SetProgress(0, 0, false);
                                        if (notificationManager != null)
                                        {
                                            notificationManager.Notify(id, builder.Build());
                                            notificationManager.Cancel(id);
                                        }

                                        System.IO.File.WriteAllBytes(mediaFile, e.Result);
                                    }
                                    catch (Exception exception)
                                    {
                                        Console.WriteLine(exception);
                                    }
                                };
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }

                        if (notificationManager != null) notificationManager.Notify(id, notification);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            [BroadcastReceiver]
            [IntentFilter(new[] { "select.notif.id" })]
            public class NotificationBroadcasterCloser : BroadcastReceiver
            {
                public override void OnReceive(Context context, Intent intent)
                {
                    try
                    {
                        int notificationId = intent.GetIntExtra("NOTIFICATION_ID", 0);

                        NotificationManager notifyMgr = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);
                        notifyMgr.Cancel(notificationId);

                        if (intent.GetStringExtra("action") == "dismiss")
                        {
                            WebClient.CancelAsync();
                            notifyMgr.CancelAll();
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
            }

            [Service]
            public class BackgroundRunner : IntentService
            {
                protected override void OnHandleIntent(Intent intent)
                {

                }
            }
        }

        #endregion

        #region AttachmentFiles

        public class AttachmentFiles
        {
            public static string GetActualPathFromFile(Context context, Uri uri)
            {
                bool isKitKat = Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat;

                if (isKitKat && DocumentsContract.IsDocumentUri(context, uri))
                {
                    // ExternalStorageProvider
                    if (IsExternalStorageDocument(uri))
                    {
                        string docId = DocumentsContract.GetDocumentId(uri);

                        char[] chars = {':'};
                        string[] split = docId.Split(chars);
                        string type = split[0];

                        if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                        {
                            return Android.OS.Environment.ExternalStorageDirectory + "/" + split[1];
                        }
                    }
                    // DownloadsProvider
                    else if (IsDownloadsDocument(uri))
                    {
                        string id = DocumentsContract.GetDocumentId(uri);

                        Uri contentUri = ContentUris.WithAppendedId(Uri.Parse("content://downloads/public_downloads"),
                            long.Parse(id));

                        //System.Diagnostics.Debug.WriteLine(contentUri.ToString());

                        return GetDataColumn(context, contentUri, null, null);
                    }
                    // MediaProvider
                    else if (IsMediaDocument(uri))
                    {
                        String docId = DocumentsContract.GetDocumentId(uri);

                        char[] chars = {':'};
                        String[] split = docId.Split(chars);

                        String type = split[0];

                        Uri contentUri = null;
                        if ("image".Equals(type))
                        {
                            contentUri = MediaStore.Images.Media.ExternalContentUri;
                        }
                        else if ("video".Equals(type))
                        {
                            contentUri = MediaStore.Video.Media.ExternalContentUri;
                        }
                        else if ("audio".Equals(type))
                        {
                            contentUri = MediaStore.Audio.Media.ExternalContentUri;
                        }

                        String selection = "_id=?";
                        String[] selectionArgs =
                        {
                            split[1]
                        };

                        return GetDataColumn(context, contentUri, selection, selectionArgs);
                    }
                }
                // MediaStore (and general)
                else if ("content".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
                {

                    // Return the remote address
                    if (IsGooglePhotosUri(uri))
                        return uri.LastPathSegment;

                    return GetDataColumn(context, uri, null, null);
                }
                // File
                else if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    return uri.Path;
                }

                return null;
            }

            private static String GetDataColumn(Context context, Uri uri, String selection, String[] selectionArgs)
            {
                ICursor cursor = null;
                String column = "_data";
                String[] projection =
                {
                    column
                };

                try
                {
                    cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs, null);
                    if (cursor != null && cursor.MoveToFirst())
                    {
                        int index = cursor.GetColumnIndexOrThrow(column);
                        return cursor.GetString(index);
                    }
                }
                finally
                {
                    cursor?.Close();
                }

                return null;
            }

            //Whether the Uri authority is ExternalStorageProvider.
            private static bool IsExternalStorageDocument(Uri uri)
            {
                return "com.android.externalstorage.documents".Equals(uri.Authority);
            }

            //Whether the Uri authority is DownloadsProvider.
            private static bool IsDownloadsDocument(Uri uri)
            {
                return "com.android.providers.downloads.documents".Equals(uri.Authority);
            }

            //Whether the Uri authority is MediaProvider.
            private static bool IsMediaDocument(Uri uri)
            {
                return "com.android.providers.media.documents".Equals(uri.Authority);
            }

            //Whether the Uri authority is Google Photos.
            private static bool IsGooglePhotosUri(Uri uri)
            {
                return "com.google.android.apps.photos.content".Equals(uri.Authority);
            }

            // Functions Check File Extension */Audio, Image, Video\*
            public static string Check_FileExtension(string filename)
            {
                var mime = MimeTypeMap.GetMimeType(filename.Split('.').LastOrDefault());
                if (string.IsNullOrEmpty(mime)) return "Forbidden";
                if (mime.Contains("audio"))
                {
                    return "Audio";
                }

                if (mime.Contains("video"))
                {
                    return "Video";
                }

                if (mime.Contains("image") || mime.Contains("drawing"))
                {
                    return "Image";
                }

                if (mime.Contains("application") || mime.Contains("text") || mime.Contains("x-world") ||
                    mime.Contains("message"))
                {
                    return "File";
                }

                return "Forbidden";
            }
        }


        #endregion

        #region IPath URL

        public class Path
        {
            public static string PersonalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            public static string AndroidDcimFolder = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim).AbsolutePath;

            //DcimFolder 
            public static string FolderDcimMyApp = AndroidDcimFolder + "/" + AppSettings.ApplicationName + "/";
            public static string FolderDcimImage = FolderDcimMyApp + "/Images/";
            public static string FolderDcimVideo = FolderDcimMyApp + "/Video/";
            public static string FolderDcimStory = FolderDcimMyApp + "/Story/";
            public static string FolderDcimSound = FolderDcimMyApp + "/Sound/";
            public static string FolderDcimFile = FolderDcimMyApp + "/File/";
            public static string FolderDcimGif = FolderDcimImage + "/Gif/";
            public static string FolderDcimSticker = FolderDcimImage + "/Sticker/";
            public static string FolderDcimPage = FolderDcimImage + "/Page/";
            public static string FolderDcimGroup = FolderDcimImage + "/Group/";
            public static string FolderDcimEvent = FolderDcimImage + "/Event/";
            public static string FolderDcimMovie = FolderDcimVideo + "/Movie/";
            public static string FolderDcimArticles = FolderDcimImage + "/Articles/";
            public static string FolderDcimMarket = FolderDcimImage + "/Market/";
            public static string FolderDcimPost = FolderDcimMyApp + "/Post/";
            public static string FolderDcimNiceArt = FolderDcimMyApp + "/Editor/";

            //Disk
            public static string FolderDiskMyApp = PersonalFolder + "/" + AppSettings.ApplicationName + "/";
            public static string FolderDiskImage = FolderDiskMyApp + "/Images/";
            public static string FolderDiskVideo = FolderDiskMyApp + "/Video/";
            public static string FolderDiskStory = FolderDiskMyApp + "/Story/";
            public static string FolderDiskSound = FolderDiskMyApp + "/Sound/";
            public static string FolderDiskFile = FolderDiskMyApp + "/File/";
            public static string FolderDiskGif = FolderDiskMyApp + "/Gif/";
            public static string FolderDiskSticker = FolderDiskMyApp + "/Sticker/";
            public static string FolderDiskPage = FolderDiskImage + "/Page/";
            public static string FolderDiskGroup = FolderDiskImage + "/Group/";
            public static string FolderDiskEvent = FolderDiskImage + "/Event/";
            public static string FolderDiskMovie = FolderDiskVideo + "/Movie/";
            public static string FolderDiskArticles = FolderDiskImage + "/Articles/";
            public static string FolderDiskMarket = FolderDiskImage + "/Market/";
            public static string FolderDiskPost = FolderDiskMyApp + "/Post/";
            public static string FolderDiskNiceArt = FolderDiskMyApp + "/Editor/";

            public static void Chack_MyFolder()
            {
                try
                {
                    if (!Directory.Exists(FolderDcimMyApp))
                        Directory.CreateDirectory(FolderDcimMyApp);

                    if (!Directory.Exists(FolderDiskMyApp))
                        Directory.CreateDirectory(FolderDiskMyApp);

                    if (!Directory.Exists(FolderDcimImage))
                        Directory.CreateDirectory(FolderDcimImage);

                    if (!Directory.Exists(FolderDcimVideo))
                        Directory.CreateDirectory(FolderDcimVideo);

                    if (!Directory.Exists(FolderDcimStory))
                        Directory.CreateDirectory(FolderDcimStory);

                    if (!Directory.Exists(FolderDcimSound))
                        Directory.CreateDirectory(FolderDcimSound);

                    if (!Directory.Exists(FolderDcimFile))
                        Directory.CreateDirectory(FolderDcimFile);

                    if (!Directory.Exists(FolderDcimGif))
                        Directory.CreateDirectory(FolderDcimGif);

                    if (!Directory.Exists(FolderDcimSticker))
                        Directory.CreateDirectory(FolderDcimSticker);

                    if (!Directory.Exists(FolderDcimPage))
                        Directory.CreateDirectory(FolderDcimPage);

                    if (!Directory.Exists(FolderDcimGroup))
                        Directory.CreateDirectory(FolderDcimGroup);

                    if (!Directory.Exists(FolderDcimEvent))
                        Directory.CreateDirectory(FolderDcimEvent);

                    if (!Directory.Exists(FolderDcimMovie))
                        Directory.CreateDirectory(FolderDcimMovie);

                    if (!Directory.Exists(FolderDcimArticles))
                        Directory.CreateDirectory(FolderDcimArticles);

                    if (!Directory.Exists(FolderDcimMarket))
                        Directory.CreateDirectory(FolderDcimMarket);

                    if (!Directory.Exists(FolderDcimPost))
                        Directory.CreateDirectory(FolderDcimPost);

                    //if (!Directory.Exists(FolderDcimNiceArt))
                    //    Directory.CreateDirectory(FolderDcimNiceArt);

                    //================================================

                    if (!Directory.Exists(FolderDiskImage))
                        Directory.CreateDirectory(FolderDiskImage);

                    if (!Directory.Exists(FolderDiskVideo))
                        Directory.CreateDirectory(FolderDiskVideo);

                    if (!Directory.Exists(FolderDiskStory))
                        Directory.CreateDirectory(FolderDiskStory);

                    if (!Directory.Exists(FolderDiskFile))
                        Directory.CreateDirectory(FolderDiskFile);

                    if (!Directory.Exists(FolderDiskSound))
                        Directory.CreateDirectory(FolderDiskSound);

                    if (!Directory.Exists(FolderDiskGif))
                        Directory.CreateDirectory(FolderDiskGif);

                    if (!Directory.Exists(FolderDiskSticker))
                        Directory.CreateDirectory(FolderDiskSticker);

                    if (!Directory.Exists(FolderDiskPage))
                        Directory.CreateDirectory(FolderDiskPage);

                    if (!Directory.Exists(FolderDiskGroup))
                        Directory.CreateDirectory(FolderDiskGroup);

                    if (!Directory.Exists(FolderDiskEvent))
                        Directory.CreateDirectory(FolderDiskEvent);

                    if (!Directory.Exists(FolderDiskMovie))
                        Directory.CreateDirectory(FolderDiskMovie);

                    if (!Directory.Exists(FolderDiskArticles))
                        Directory.CreateDirectory(FolderDiskArticles);

                    if (!Directory.Exists(FolderDiskMarket))
                        Directory.CreateDirectory(FolderDiskMarket);

                    if (!Directory.Exists(FolderDiskPost))
                        Directory.CreateDirectory(FolderDiskPost);

                    //if (!Directory.Exists(FolderDiskNiceArt))
                    //    Directory.CreateDirectory(FolderDiskNiceArt);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public static void DeleteAll_MyFolder()
            {
                try
                {

                    //if (Directory.Exists(FolderDiskMyApp))
                    //    Directory.Delete(FolderDiskMyApp,true);

                    if (Directory.Exists(FolderDcimImage))
                        Directory.Delete(FolderDcimImage, true);

                    if (Directory.Exists(FolderDcimVideo))
                        Directory.Delete(FolderDcimVideo, true);

                    if (Directory.Exists(FolderDcimStory))
                        Directory.Delete(FolderDcimStory, true);

                    if (Directory.Exists(FolderDcimSound))
                        Directory.Delete(FolderDcimSound, true);

                    if (Directory.Exists(FolderDcimGif))
                        Directory.Delete(FolderDcimGif, true);

                    if (Directory.Exists(FolderDcimSticker))
                        Directory.Delete(FolderDcimSticker, true);

                    if (Directory.Exists(FolderDcimPage))
                        Directory.Delete(FolderDcimPage, true);

                    if (Directory.Exists(FolderDcimGroup))
                        Directory.Delete(FolderDcimGroup, true);

                    if (Directory.Exists(FolderDcimEvent))
                        Directory.Delete(FolderDcimEvent, true);

                    if (Directory.Exists(FolderDcimMovie))
                        Directory.Delete(FolderDcimMovie, true);

                    if (Directory.Exists(FolderDcimArticles))
                        Directory.Delete(FolderDcimArticles, true);

                    if (Directory.Exists(FolderDcimMarket))
                        Directory.Delete(FolderDcimMarket, true);

                    if (!Directory.Exists(FolderDcimNiceArt))
                        Directory.CreateDirectory(FolderDcimNiceArt);

                    //================================================

                    //if (Directory.Exists(FolderDcimMyApp))
                    //    Directory.Delete(FolderDcimMyApp,true);

                    if (Directory.Exists(FolderDiskImage))
                        Directory.Delete(FolderDiskImage, true);

                    if (Directory.Exists(FolderDiskVideo))
                        Directory.Delete(FolderDiskVideo, true);

                    if (Directory.Exists(FolderDiskStory))
                        Directory.Delete(FolderDiskStory, true);

                    if (Directory.Exists(FolderDiskSound))
                        Directory.Delete(FolderDiskSound, true);

                    if (Directory.Exists(FolderDiskGif))
                        Directory.Delete(FolderDiskGif, true);

                    if (Directory.Exists(FolderDiskSticker))
                        Directory.Delete(FolderDiskSticker, true);

                    if (Directory.Exists(FolderDiskPage))
                        Directory.Delete(FolderDiskPage, true);

                    if (Directory.Exists(FolderDiskGroup))
                        Directory.Delete(FolderDiskGroup, true);

                    if (Directory.Exists(FolderDiskEvent))
                        Directory.Delete(FolderDiskEvent, true);

                    if (Directory.Exists(FolderDiskMovie))
                        Directory.Delete(FolderDiskMovie, true);

                    if (Directory.Exists(FolderDiskArticles))
                        Directory.Delete(FolderDiskArticles, true);

                    if (Directory.Exists(FolderDiskMarket))
                        Directory.Delete(FolderDiskMarket, true);

                    if (!Directory.Exists(FolderDiskNiceArt))
                        Directory.Delete(FolderDiskNiceArt, true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public static void DeleteAll_MyFolderDisk()
            {
                try
                {

                    if (Directory.Exists(FolderDiskImage))
                        Directory.Delete(FolderDiskImage, true);

                    if (Directory.Exists(FolderDiskVideo))
                        Directory.Delete(FolderDiskVideo, true);

                    if (Directory.Exists(FolderDiskStory))
                        Directory.Delete(FolderDiskStory, true);

                    if (Directory.Exists(FolderDiskSound))
                        Directory.Delete(FolderDiskSound, true);

                    if (Directory.Exists(FolderDiskGif))
                        Directory.Delete(FolderDiskGif, true);

                    if (Directory.Exists(FolderDiskSticker))
                        Directory.Delete(FolderDiskSticker, true);

                    if (Directory.Exists(FolderDiskPage))
                        Directory.Delete(FolderDiskPage, true);

                    if (Directory.Exists(FolderDiskGroup))
                        Directory.Delete(FolderDiskGroup, true);

                    if (Directory.Exists(FolderDiskEvent))
                        Directory.Delete(FolderDiskEvent, true);

                    if (Directory.Exists(FolderDiskMovie))
                        Directory.Delete(FolderDiskMovie, true);

                    if (Directory.Exists(FolderDiskArticles))
                        Directory.Delete(FolderDiskArticles, true);

                    if (Directory.Exists(FolderDiskMarket))
                        Directory.Delete(FolderDiskMarket, true);

                    if (!Directory.Exists(FolderDiskNiceArt))
                        Directory.Delete(FolderDiskNiceArt, true);

                    if (Directory.Exists(FolderDiskMyApp))
                        Directory.Delete(FolderDiskMyApp, true);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }


        #endregion

        //########################## End Class IMethods Application Version 3.0 ##########################
    }
}
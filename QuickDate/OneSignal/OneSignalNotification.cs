using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Android.Widget;
using Com.OneSignal.Abstractions;
using Com.OneSignal.Android;
using Newtonsoft.Json;
using Org.Json;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Model;
using QuickDateClient.Classes.Global;
using OSNotification = Com.OneSignal.Abstractions.OSNotification;
using OSNotificationPayload = Com.OneSignal.Abstractions.OSNotificationPayload;

namespace QuickDate.OneSignal
{
    public static class OneSignalNotification
    {
        //Force your app to Register Notification directly without loading it from server (For Best Result)

        public static string Type;
        public static UserInfoObject UserData;

        public static void RegisterNotificationDevice()
        {
            try
            {
                if (UserDetails.NotificationPopup)
                {
                    if (AppSettings.OneSignalAppId != "")
                    {
                        Com.OneSignal.OneSignal.Current.StartInit(AppSettings.OneSignalAppId)
                            .InFocusDisplaying(OSInFocusDisplayOption.Notification)
                            .HandleNotificationReceived(HandleNotificationReceived)
                            .HandleNotificationOpened(HandleNotificationOpened)
                            .EndInit();
                        Com.OneSignal.OneSignal.Current.IdsAvailable(IdsAvailable);
                        Com.OneSignal.OneSignal.Current.RegisterForPushNotifications();

                        AppSettings.ShowNotification = true;
                    }
                }
                else
                {
                    UnRegisterNotificationDevice();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void UnRegisterNotificationDevice()
        {
            try
            {
                Com.OneSignal.OneSignal.Current.SetSubscription(false);
                AppSettings.ShowNotification = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void IdsAvailable(string userId, string pushToken)
        {
            try
            {
                UserDetails.DeviceId = userId;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void HandleNotificationReceived(OSNotification notification)
        {
            try
            {

                OSNotificationPayload payload = notification.payload;
                Dictionary<string, object> additionalData = payload.additionalData;

                string message = payload.body;

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.ToString(), ToastLength.Long).Show(); //Allen
                Console.WriteLine(ex);
            }
        }

        private static void HandleNotificationOpened(OSNotificationOpenedResult result)
        {
            try
            {
                OSNotificationPayload payload = result.notification.payload;
                Dictionary<string, object> additionalData = payload.additionalData;
                string message = payload.body;
                string actionId = result.action.actionID;

                if (additionalData != null)
                {
                    foreach (var item in additionalData)
                    {
                        switch (item.Key)
                        {
                            case "type":
                                Type = item.Value.ToString();
                                break;
                            case "userdata":
                                UserData = JsonConvert.DeserializeObject<UserInfoObject>(item.Value.ToString());
                                break;
                        }
                    }

                    //to : do
                    //go to activity or fragment depending on data

                    Intent intent = new Intent(Application.Context, typeof(HomeActivity));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    intent.AddFlags(ActivityFlags.SingleTop);
                    intent.SetAction(Intent.ActionView);
                    intent.PutExtra("TypeNotification", Type);
                    Application.Context.StartActivity(intent);
                      
                    if (additionalData.ContainsKey("discount"))
                    {
                        // Take user to your store..

                    }
                }

                if (actionId != null)
                {
                    // actionSelected equals the id on the button the user pressed.
                    // actionSelected will equal "__DEFAULT__" when the notification itself was tapped when buttons were present.  
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public class NotificationExtenderServiceHandeler : NotificationExtenderService, NotificationCompat.IExtender
        {
            protected override void OnHandleIntent(Intent intent)
            {

            }

            protected override bool OnNotificationProcessing(OSNotificationReceivedResult p0)
            {
                OverrideSettings overrideSettings = new OverrideSettings();
                overrideSettings.Extender = new NotificationCompat.CarExtender();

                Com.OneSignal.Android.OSNotificationPayload payload = p0.Payload;
                JSONObject additionalData = payload.AdditionalData;

                if (additionalData.Has("room_name"))
                {
                    string roomName = additionalData.Get("room_name").ToString();
                    string callType = additionalData.Get("call_type").ToString();
                    string callId = additionalData.Get("call_id").ToString();
                    string fromId = additionalData.Get("from_id").ToString();
                    string toId = additionalData.Get("to_id").ToString();

                    return false;
                }

                return true;
            }

            public NotificationCompat.Builder Extend(NotificationCompat.Builder builder)
            {
                return builder;
            }
        }
    }
}
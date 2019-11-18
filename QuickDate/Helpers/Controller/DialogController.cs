using System;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Widget;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Common;
using QuickDateClient.Classes.Global;

namespace QuickDate.Helpers.Controller
{
   public class DialogController
   {
       private Dialog AlertDialogMatchFound, AlertDialogSkipTutorial,  AlertDialogFinishTutorial;
       private readonly Activity Activity;
       public static UserInfoObject DataUser;

       public DialogController(Activity activity)
       {
           Activity = activity;
       }

        public void OpenDialogMatchFound(UserInfoObject dataUser)
        {
            try
            {
                DataUser = dataUser;

                AlertDialogMatchFound = new Dialog(Activity);
                AlertDialogMatchFound.SetContentView(Resource.Layout.DialogMatchFound);
                AlertDialogMatchFound.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));

                var image1 = AlertDialogMatchFound.FindViewById<ImageView>(Resource.Id.Iconimage);
                var image2 = AlertDialogMatchFound.FindViewById<ImageView>(Resource.Id.Iconimage2);

                GlideImageLoader.LoadImage(Activity,dataUser.Avater, image1, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                GlideImageLoader.LoadImage(Activity,UserDetails.Avatar, image2, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                var subTitle = AlertDialogMatchFound.FindViewById<TextView>(Resource.Id.secondarytextview);
                subTitle.Text = Activity.GetText(Resource.String.Lbl_YouAnd) + " " + QuickDateTools.GetNameFinal(dataUser) + " " + Activity.GetText(Resource.String.Lbl_SubTitle_Match);

                Button btnSkipMatch = AlertDialogMatchFound.FindViewById<Button>(Resource.Id.skippButton);
                Button btnNextMatch = AlertDialogMatchFound.FindViewById<Button>(Resource.Id.NextButton);

                btnSkipMatch.Click += BtnSkipMatchOnClick;
                btnNextMatch.Click += BtnNextMatchOnClick;

                AlertDialogMatchFound.Show();

                //got_new_match
               var data = ListUtils.MatchList.FirstOrDefault(a => a.Notifier.Id == dataUser.Id);
               if (data == null)
               {
                   ListUtils.MatchList.Add(new GetNotificationsObject.Datum()
                   { 
                       Notifier = dataUser,
                       Type = "got_new_match"
                   });
               }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void BtnNextMatchOnClick(object sender, EventArgs e)
        {
            try
            { 
                HomeActivity.GetInstance().ShowMessagesBox(DataUser);

                AlertDialogMatchFound.Hide();
                AlertDialogMatchFound.Dismiss(); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void BtnSkipMatchOnClick(object sender, EventArgs e)
        {
            try
            {
                AlertDialogMatchFound.Hide();
                AlertDialogMatchFound.Dismiss(); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception); 
            } 
        }


        ///////////////////////////////////////////////////////////

        public void OpenDialogSkipTutorial()
        {
            try
            {
                AlertDialogSkipTutorial = new Dialog(Activity);
                AlertDialogSkipTutorial.SetContentView(Resource.Layout.DialogSkipTutorial);
                AlertDialogSkipTutorial.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                Button btnSkipTutorial = AlertDialogSkipTutorial.FindViewById<Button>(Resource.Id.skippButton);
                Button btnNextTutorial = AlertDialogSkipTutorial.FindViewById<Button>(Resource.Id.NextButton);

                btnSkipTutorial.Click += BtnSkipTutorialOnClick;
                btnNextTutorial.Click += BtnNextTutorialOnClick;

                AlertDialogSkipTutorial.Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void BtnSkipTutorialOnClick(object sender, EventArgs e)
        {
            try
            {
                AlertDialogSkipTutorial.Hide();
                AlertDialogSkipTutorial.Dismiss();

                SetActive();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void BtnNextTutorialOnClick(object sender, EventArgs e)
        {
            try
            {
                SetActive();

                AlertDialogSkipTutorial.Hide();
                AlertDialogSkipTutorial.Dismiss();

                //Open Dialog Finish Tutorial
                AlertDialogFinishTutorial = new Dialog(Activity);
                AlertDialogFinishTutorial.SetContentView(Resource.Layout.DialogFinishTutorial);
                AlertDialogFinishTutorial.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));

                Button btnFinishTutorial = AlertDialogFinishTutorial.FindViewById<Button>(Resource.Id.finishButton);

                btnFinishTutorial.Click += BtnFinishTutorialOnClick;

                AlertDialogFinishTutorial.Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        private void BtnFinishTutorialOnClick(object sender, EventArgs e)
        {
            try
            {
                AlertDialogFinishTutorial.Dismiss();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        /////////////////////////////////////////////////
        
        private void SetActive()
        {
            try
            {
                var data = ListUtils.DataUserLoginList.FirstOrDefault();
                if (data != null)
                {
                    if (data.Status != "Active")
                    {
                        data.Status = "Active";
                        UserDetails.Status = "Active";

                        SqLiteDatabase dbDatabase = new SqLiteDatabase();
                        dbDatabase.InsertOrUpdateLogin_Credentials(data);
                        dbDatabase.Dispose();
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
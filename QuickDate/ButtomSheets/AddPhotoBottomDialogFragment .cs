using System;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Fonts;

namespace QuickDate.ButtomSheets
{
    public class AddPhotoBottomDialogFragment : BottomSheetDialogFragment
    {
        #region Variables Basic

        public ImageView UserAvatar;
        public TextView Headline ,SkipTextView, Seconderytext ,Icon,Icon2;
        public Button AddPhoto;  
        public HomeActivity GlobalContext;

        #endregion
         
        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            GlobalContext = (HomeActivity)Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.ButtomSheetAddPhoto, container, false);

                InitComponent(view);

                AddPhoto.Click += AddPhotoOnClick;
                SkipTextView.Click += SkipTextViewOnClick;
                  
                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
                return null;
            } 
        }

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                UserAvatar = view.FindViewById<ImageView>(Resource.Id.useravatar);
                Headline = view.FindViewById<TextView>(Resource.Id.headline);
                Seconderytext = view.FindViewById<TextView>(Resource.Id.seconderytext);
                Icon = view.FindViewById<TextView>(Resource.Id.Icon);
                Icon2 = view.FindViewById<TextView>(Resource.Id.Icon2);
                AddPhoto = view.FindViewById<Button>(Resource.Id.addButton);
                SkipTextView = view.FindViewById<TextView>(Resource.Id.skipbutton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, Icon, IonIconsFonts.Camera);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, Icon2, IonIconsFonts.Camera);
                
                FontUtils.SetFont(Headline, Fonts.SfRegular);
                FontUtils.SetFont(Seconderytext, Fonts.SfRegular);
                FontUtils.SetFont(AddPhoto, Fonts.SfRegular);
                FontUtils.SetFont(SkipTextView, Fonts.SfRegular);

                GlideImageLoader.LoadImage(Activity,"no_profile_image", UserAvatar, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion

        #region Event

        private void AddPhotoOnClick(object sender, EventArgs e)
        {
            try
            {
                GlobalContext.OpenDialogGallery();
                Dismiss();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SkipTextViewOnClick(object sender, EventArgs e)
        {
            try
            {
                Dismiss();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        #endregion

    }
}
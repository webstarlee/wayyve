using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Java.Lang;
using QuickDate.Helpers.CacheLoaders;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.UserProfile.Adapters
{
    public class MultiImagePagerAdapter : PagerAdapter
    {
        public List<string> Images;
        public LayoutInflater Inflater;
        private readonly Activity ActivityContext;

        public MultiImagePagerAdapter(Activity context, List<string> images)
        {
            ActivityContext = context;
            Images = images;
            Inflater = LayoutInflater.From(context);
        }

        public override bool IsViewFromObject(View view, Object @object)
        {
            return view.Equals(@object);
        }

        public override int Count
        {
            get
            {
                if (Images != null)
                {
                    return Images.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public override Object InstantiateItem(ViewGroup view, int position)
        {
            try
            {
                View imageLayout = Inflater.Inflate(Resource.Layout.Style_ImageNormalView, view, false);
                ImageView imageView = imageLayout.FindViewById<ImageView>(Resource.Id.image);

                GlideImageLoader.LoadImage(ActivityContext,Images[position], imageView, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                view.AddView(imageLayout, 0);
                return imageLayout;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

        }

        public override void DestroyItem(ViewGroup container, int position, Object @object)
        {
            container.RemoveView((View)@object);
        }

        public override void RestoreState(IParcelable state, ClassLoader loader)
        {

        }

    }
}
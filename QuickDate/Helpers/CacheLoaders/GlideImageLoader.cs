using System;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Load.Resource.Bitmap;
using Bumptech.Glide.Request;
using Java.IO;
using QuickDate.Helpers.Utils;
using Console = System.Console;
using Uri = Android.Net.Uri;

namespace QuickDate.Helpers.CacheLoaders
{
    public enum ImageStyle
    {
        CenterCrop, CircleCrop, RoundedCrop, FitCenter
    }

    public enum ImagePlaceholders
    {
        Color, Drawable
    }

    public static class GlideImageLoader
    {
        public static void LoadImage(Activity activity, string imageUri, ImageView image, ImageStyle style, ImagePlaceholders imagePlaceholders, RequestOptions options = null)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUri) || string.IsNullOrWhiteSpace(imageUri) || image == null)
                    return;

                imageUri = imageUri.Replace(" ", "");

                var newImage = Glide.With(activity);

                if (options == null)
                    options = GetOptions(style, imagePlaceholders);

                if (AppSettings.CompressImage && style != ImageStyle.RoundedCrop)
                {
                    if (imageUri.Contains("avatar") || imageUri.Contains("Avatar"))
                        options.Override(AppSettings.AvatarSize);
                    else if (imageUri.Contains("gif"))
                        options.Override(AppSettings.ImageSize);
                    else
                        options.Override(AppSettings.ImageSize);
                }

                if (AppSettings.CompressImage)
                    options.Override(AppSettings.ImageSize);

                if (imageUri.Contains("FirstImageOne") || imageUri.Contains("FirstImageTwo") || imageUri.Contains("no_profile_image") || imageUri.Contains("no_profile_image_circle")
                    || imageUri.Contains("ImagePlacholder") || imageUri.Contains("ImagePlacholder_circle") || imageUri.Contains("d-avatar")||  imageUri.Contains("Grey_Offline") || imageUri.Contains("addImage"))
                {
                    if (imageUri.Contains("no_profile_image_circle") || imageUri.Contains("d-avatar.jpg"))
                        newImage.Load(Resource.Drawable.no_profile_image_circle).Apply(options).Into(image);
                    else if (imageUri.Contains("no_profile_image") || imageUri.Contains("d-avatar.jpg"))
                        newImage.Load(Resource.Drawable.no_profile_image).Apply(options).Into(image);
                    else if (imageUri.Contains("ImagePlacholder"))
                        newImage.Load(Resource.Drawable.ImagePlacholder).Apply(options).Into(image);
                    else if (imageUri.Contains("ImagePlacholder_circle"))
                        newImage.Load(Resource.Drawable.ImagePlacholder_circle).Apply(options).Into(image);
                    else if (imageUri.Contains("FirstImageOne"))
                        newImage.Load(Resource.Drawable.FirstImageOne).Apply(options).Into(image);
                    else if (imageUri.Contains("FirstImageTwo"))
                        newImage.Load(Resource.Drawable.FirstImageTwo).Apply(options).Into(image);
                    else if (imageUri.Contains("addImage.jpg"))
                        newImage.Load(Resource.Drawable.addImage).Apply(options).Into(image);
                    else if (imageUri.Contains("Grey_Offline"))
                        newImage.Load(Resource.Drawable.Grey_Offline).Apply(options).Into(image);
                }
                else if (!string.IsNullOrEmpty(imageUri) && imageUri.Contains("http"))
                {
                    newImage.Load(imageUri).Apply(options).Into(image);
                }
                else if (!string.IsNullOrEmpty(imageUri) && (imageUri.Contains("file://") || imageUri.Contains("content://") || imageUri.Contains("storage")))
                {
                    var file = Uri.FromFile(new File(imageUri));
                    newImage.Load(file.Path).Apply(options).Into(image);
                }
                else
                {
                    newImage.Load(Resource.Drawable.no_profile_image).Apply(options).Into(image);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static RequestOptions GetOptions(ImageStyle style, ImagePlaceholders imagePlaceholders)
        {
            try
            {
                RequestOptions options = new RequestOptions();

                switch (style)
                {
                    case ImageStyle.CenterCrop:
                        options = new RequestOptions().Apply(RequestOptions.CenterCropTransform()
                            .CenterCrop()
                            .SetPriority(Priority.High)
                            .SetUseAnimationPool(false).SetDiskCacheStrategy(DiskCacheStrategy.All)
                            .Error(Resource.Drawable.ImagePlacholder)
                            .Placeholder(Resource.Drawable.ImagePlacholder));
                        break;
                    case ImageStyle.FitCenter:
                        options = new RequestOptions().Apply(RequestOptions.CenterCropTransform()
                            .CenterCrop().FitCenter()
                            .SetPriority(Priority.High)
                            .SetUseAnimationPool(false).SetDiskCacheStrategy(DiskCacheStrategy.All)
                            .Error(Resource.Drawable.ImagePlacholder)
                            .Placeholder(Resource.Drawable.ImagePlacholder));
                        break;
                    case ImageStyle.CircleCrop:
                        options = new RequestOptions().Apply(RequestOptions.CircleCropTransform()
                            .CenterCrop().CircleCrop()
                            .SetPriority(Priority.High)
                            .SetUseAnimationPool(false).SetDiskCacheStrategy(DiskCacheStrategy.All)
                            .Error(Resource.Drawable.ImagePlacholder_circle)
                            .Placeholder(Resource.Drawable.ImagePlacholder_circle));
                        break;
                    case ImageStyle.RoundedCrop:
                        options = new RequestOptions().Apply(RequestOptions.CircleCropTransform()
                            .CenterCrop()
                            .Transform(new MultiTransformation(new CenterCrop(), new RoundedCorners(20)))
                            .SetPriority(Priority.High)
                            .SetUseAnimationPool(false).SetDiskCacheStrategy(DiskCacheStrategy.All)
                            .Error(Resource.Drawable.ImagePlacholder_circle)
                            .Placeholder(Resource.Drawable.ImagePlacholder_circle));
                        break;

                    default:
                        options.CenterCrop();
                        break;
                }

                switch (imagePlaceholders)
                {
                    case ImagePlaceholders.Color:
                        var color = Methods.FunString.RandomColor();
                        options.Placeholder(new ColorDrawable(Color.ParseColor(color))).Fallback(new ColorDrawable(Color.ParseColor(color)));
                        break;
                    case ImagePlaceholders.Drawable:
                        switch (style)
                        {
                            case ImageStyle.CircleCrop:
                                options.Placeholder(Resource.Drawable.ImagePlacholder_circle).Fallback(Resource.Drawable.ImagePlacholder_circle);
                                break;
                            default:
                                options.Placeholder(Resource.Drawable.ImagePlacholder).Fallback(Resource.Drawable.ImagePlacholder);
                                break;
                        }
                        break;
                }

                return options;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new RequestOptions().CenterCrop();
            }
        }

        public static RequestBuilder GetPreLoadRequestBuilder(Activity activityContext, string url, ImageStyle style)
        {
            try
            {
                if (url == null || string.IsNullOrEmpty(url))
                    return null;

                var options = GetOptions(style, ImagePlaceholders.Drawable);

                if (url.Contains("avatar"))
                    options.CircleCrop();

                if (url.Contains("avatar"))
                {
                    options.Override(AppSettings.AvatarSize);
                }
                else if (url.Contains("gif"))
                {
                    options.Override(AppSettings.ImageSize);
                }
                else
                {
                    options.Override(AppSettings.ImageSize);
                }

                options.SetDiskCacheStrategy(DiskCacheStrategy.All);

                return Glide.With(activityContext)
                    .Load(url)
                    .Apply(options);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

        }

        public static RequestOptions GetRequestOptions(ImageStyle style, ImagePlaceholders imagePlaceholders)
        {
            try
            {
                var options = new RequestOptions();


                switch (style)
                {
                    case ImageStyle.CenterCrop:
                        options.CenterCrop();
                        break;
                    case ImageStyle.FitCenter:
                        options.FitCenter();
                        break;
                    case ImageStyle.CircleCrop:
                        options.CircleCrop();
                        break;
                    case ImageStyle.RoundedCrop:
                        options.Transform(new MultiTransformation(new CenterCrop(), new RoundedCorners(20)));
                        break;

                    default:
                        options.CenterCrop();
                        break;
                }
                 
                switch (imagePlaceholders)
                {
                    case ImagePlaceholders.Color:
                        var color = Methods.FunString.RandomColor();
                        options.Placeholder(new ColorDrawable(Color.ParseColor(color))).Fallback(new ColorDrawable(Color.ParseColor(color)));
                        break;
                    case ImagePlaceholders.Drawable:
                        options.Placeholder(Resource.Drawable.ImagePlacholder).Fallback(Resource.Drawable.ImagePlacholder);
                        break;
                }

                return options.SetDiskCacheStrategy(DiskCacheStrategy.All);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new RequestOptions();

            }

        }
    }
}
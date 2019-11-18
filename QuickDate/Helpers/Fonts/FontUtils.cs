using System;
using Android.App;
using Android.Graphics;
using Android.Widget;

namespace QuickDate.Helpers.Fonts
{
    public class FontUtils 
    {
        public TextView TextviewResizable;
        public int MaxLine;
        public string ExpandText;
        public bool ViewMore;

        public static void SetFont(TextView textView , Fonts type)
        {
            Typeface regularTxt = Typeface.CreateFromAsset(textView.Context.Assets, "fonts/SF-UI-Display-Regular.ttf");
            Typeface semiboldTxt = Typeface.CreateFromAsset(textView.Context.Assets, "fonts/SF-UI-Display-Semibold.ttf");
            Typeface semiMediumTxt = Typeface.CreateFromAsset(textView.Context.Assets, "fonts/SF-UI-Display-Medium.ttf");

            if (type == Fonts.SfRegular)
            {
                textView.SetTypeface(regularTxt, TypefaceStyle.Normal);
            }
            else if(type == Fonts.SfSemibold)
            {
                textView.SetTypeface(semiboldTxt, TypefaceStyle.Bold);
            }
            else 
            {
                textView.SetTypeface(semiMediumTxt, TypefaceStyle.Normal);
            }
        }


        //Changes the TextView To IconFrameWork Fonts

        public static Typeface SetTextViewIcon(FontsIconFrameWork type, TextView textViewUi, string iconUnicode)
        {
            try
            {
                if (type == FontsIconFrameWork.IonIcons)
                {
                    var font = Typeface.CreateFromAsset(Application.Context.Resources.Assets, "ionicons.ttf");
                    textViewUi.SetTypeface(font, TypefaceStyle.Normal);
                    if (!string.IsNullOrEmpty(iconUnicode)) textViewUi.Text = iconUnicode;
                    return font;
                }
                else if (type == FontsIconFrameWork.FontAwesomeSolid)
                {
                    var font = Typeface.CreateFromAsset(Application.Context.Resources.Assets, "fa-solid-900.ttf");
                    textViewUi.SetTypeface(font, TypefaceStyle.Normal);
                    if (!string.IsNullOrEmpty(iconUnicode)) textViewUi.Text = iconUnicode;
                    return font;
                }
                else if (type == FontsIconFrameWork.FontAwesomeRegular)
                {
                    var font = Typeface.CreateFromAsset(Application.Context.Resources.Assets, "fa-regular-400.ttf");
                    textViewUi.SetTypeface(font, TypefaceStyle.Normal);
                    if (!string.IsNullOrEmpty(iconUnicode)) textViewUi.Text = iconUnicode;
                    return font;
                }
                else if (type == FontsIconFrameWork.FontAwesomeBrands)
                {
                    var font = Typeface.CreateFromAsset(Application.Context.Resources.Assets, "fa-brands-400.ttf");
                    textViewUi.SetTypeface(font, TypefaceStyle.Normal);
                    if (!string.IsNullOrEmpty(iconUnicode)) textViewUi.Text = iconUnicode;
                    return font;
                }
                else if (type == FontsIconFrameWork.FontAwesomeLight)
                {
                    var font = Typeface.CreateFromAsset(Application.Context.Resources.Assets, "fa-light-300.ttf");
                    textViewUi.SetTypeface(font, TypefaceStyle.Normal);
                    if (!string.IsNullOrEmpty(iconUnicode)) textViewUi.Text = iconUnicode;
                    return font;
                }
                else if (type == FontsIconFrameWork.FontAwesomeV3)
                {
                    var font = Typeface.CreateFromAsset(Application.Context.Resources.Assets, "fontawesome-v3.1.ttf");
                    textViewUi.SetTypeface(font, TypefaceStyle.Normal);
                    if (!string.IsNullOrEmpty(iconUnicode)) textViewUi.Text = iconUnicode;
                    return font;
                }
                else
                {
                    var font = Typeface.CreateFromAsset(Application.Context.Resources.Assets, "fontawesome-webfont.ttf");
                    textViewUi.SetTypeface(font, TypefaceStyle.Normal);
                    if (!string.IsNullOrEmpty(iconUnicode)) textViewUi.Text = iconUnicode;
                    return font;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Set_TextViewIcon Function ERROR " + e);
                Console.WriteLine(e);
                return null;
            }
        } 
    }
}
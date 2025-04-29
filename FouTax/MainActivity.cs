using Android.App;
using Android.Content.Res;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using Android.Widget;
using System.Text.RegularExpressions;
using System;

namespace FoTax
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private AdView adView;
        private AdView adView1;
        private EditText amountEditText1;
        private EditText amountEditText2;
        private TextView profitTextView;
        private TextView taxTextView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                WindowCompat.SetDecorFitsSystemWindows(Window, false);
                SetContentView(Resource.Layout.activity_main);
                // لا تقم بإعادة الإنشاء فورًا عند الإنشاء. سيتم عرض اللغة بناءً على إعدادات الجهاز.
                // SetLocale(false);

                MobileAds.Initialize(this);

                adView = FindViewById<AdView>(Resource.Id.adView);
                adView1 = FindViewById<AdView>(Resource.Id.adView1);

                var adRequest = new AdRequest.Builder().Build();
                adView.LoadAd(adRequest);
                adView1.LoadAd(adRequest);

                amountEditText1 = FindViewById<EditText>(Resource.Id.amountEditText1);
                amountEditText2 = FindViewById<EditText>(Resource.Id.amountEditText2);
                profitTextView = FindViewById<TextView>(Resource.Id.profitTextView);
                taxTextView = FindViewById<TextView>(Resource.Id.taxTextView);

                amountEditText1.TextChanged += AmountEditText_TextChanged;
                amountEditText2.TextChanged += AmountEditText_TextChanged;
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error("MainActivity", ex.Message);
            }
        }

        private void AmountEditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CalculateProfit(amountEditText1, amountEditText2, profitTextView, taxTextView);
        }

        private void CalculateProfit(EditText amountEditText1, EditText amountEditText2, TextView profitTextView, TextView taxTextView)
        {
            if (decimal.TryParse(amountEditText1.Text, out decimal buyInPrice) &&
                decimal.TryParse(amountEditText2.Text, out decimal sellingPrice))
            {
                decimal tax = sellingPrice * 0.05m;
                decimal profit = (sellingPrice - tax) - buyInPrice;

                // عرض الربح والضريبة بدون تنسيق العملة
                profitTextView.Text = $"{GetString(Resource.String.profit)}: {profit}";
                taxTextView.Text = $"{GetString(Resource.String.tax)}: {tax}";

                profitTextView.SetTextColor(profit > 0 ? Android.Graphics.Color.Green : profit < 0 ? Android.Graphics.Color.Red : Android.Graphics.Color.White);

                taxTextView.Visibility = Android.Views.ViewStates.Visible;
                profitTextView.Visibility = Android.Views.ViewStates.Visible;
            }
            else
            {
                profitTextView.Visibility = Android.Views.ViewStates.Gone;
                taxTextView.Visibility = Android.Views.ViewStates.Gone;
            }
        }

        private Regex NonNumericOrSeparatorRegex()
        {
            return new Regex(@"[^\d.,-]");
        }

        public (decimal? Tax, decimal? Profit) GetTaxAndProfit()
        {
            decimal? taxValue = null;
            decimal? profitValue = null;

            // استخراج قيمة الضريبة
            if (!string.IsNullOrEmpty(taxTextView.Text))
            {
                string taxString = taxTextView.Text.Replace($"{GetString(Resource.String.tax)}: ", "");
                string cleanedTaxString = NonNumericOrSeparatorRegex().Replace(taxString, "").Trim();

                if (decimal.TryParse(cleanedTaxString, out decimal parsedTax))
                {
                    taxValue = parsedTax;
                }
                else
                {
                    Android.Util.Log.Warn("GetTaxAndProfit", $"خطأ في تحليل قيمة الضريبة: {taxString}");
                }
            }

            // استخراج قيمة الربح
            if (!string.IsNullOrEmpty(profitTextView.Text))
            {
                string profitString = profitTextView.Text.Replace($"{GetString(Resource.String.profit)}: ", "");
                string cleanedProfitString = NonNumericOrSeparatorRegex().Replace(profitString, "").Trim();

                if (decimal.TryParse(cleanedProfitString, out decimal parsedProfit))
                {
                    profitValue = parsedProfit;
                }
                else
                {
                    Android.Util.Log.Warn("GetTaxAndProfit", $"خطأ في تحليل قيمة الربح: {profitString}");
                }
            }

            return (taxValue, profitValue);
        }

        // لم تعد هناك حاجة لإعادة الإنشاء القسري للغة العربية هنا.
        // سيقوم Android تلقائيًا بتحميل الموارد بناءً على لغة الجهاز.
        // public void SetLocale(bool shouldRecreate = true)
        // {
        //    var locale = new Java.Util.Locale("ar");
        //    var config = new Android.Content.Res.Configuration(Resources.Configuration);
        //    config.SetLocale(locale);
        //
        //    Resources.Configuration.SetLocale(locale);
        //
        //    if (shouldRecreate)
        //    {
        //        Recreate();
        //    }
        //    else
        //    {
        //        // يمكنك هنا تحديث النصوص المعروضة يدويًا إذا لزم الأمر
        //        // FindViewById<TextView>(Resource.Id.someTextView)?.Text = GetString(Resource.String.some_text_in_arabic);
        //    }
        // }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            adView?.Destroy();
            adView1?.Destroy();
        }
    }
}
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using Java.Util;

namespace FoTax
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            WindowCompat.SetDecorFitsSystemWindows(Window, false);

            SetContentView(Resource.Layout.activity_main);
            SetLocale();

            var amountEditText1 = FindViewById<EditText>(Resource.Id.amountEditText1);
            var amountEditText2 = FindViewById<EditText>(Resource.Id.amountEditText2);
            var profitTextView = FindViewById<TextView>(Resource.Id.profitTextView);
            var taxTextView = FindViewById<TextView>(Resource.Id.taxTextView);

            amountEditText1.TextChanged += (sender, e) => CalculateProfit(amountEditText1, amountEditText2, profitTextView, taxTextView);
            amountEditText2.TextChanged += (sender, e) => CalculateProfit(amountEditText1, amountEditText2, profitTextView, taxTextView);
        }

        private void CalculateProfit(EditText amountEditText1, EditText amountEditText2, TextView profitTextView, TextView taxTextView)
        {
            if (decimal.TryParse(amountEditText1.Text, out decimal buyInPrice) &&
                decimal.TryParse(amountEditText2.Text, out decimal sellingPrice))
            {
                decimal tax = sellingPrice * 0.05m;
                decimal profit = (sellingPrice - tax) - buyInPrice;

                profitTextView.Text = $"Profit: {profit:C}";

                if (profit > 0)
                {
                    profitTextView.SetTextColor(Android.Graphics.Color.Green);
                }
                else if (profit < 0)
                {
                    profitTextView.SetTextColor(Android.Graphics.Color.Red);
                }
                else
                {
                    profitTextView.SetTextColor(Android.Graphics.Color.White);
                }

                taxTextView.Text = $"Tax: {tax:C}";
                taxTextView.Visibility = Android.Views.ViewStates.Visible;
                profitTextView.Visibility = Android.Views.ViewStates.Visible;
            }
            else
            {
                profitTextView.Visibility = Android.Views.ViewStates.Gone;
                taxTextView.Visibility = Android.Views.ViewStates.Gone;
            }
        }

        public void SetLocale()
        {
            var currentLocale = Resources.Configuration.Locale;
            var locale = new Locale(currentLocale.Language);
            Locale.Default = locale;

            var config = new Android.Content.Res.Configuration();
            config.SetLocale(locale);
            Resources.UpdateConfiguration(config, Resources.DisplayMetrics);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
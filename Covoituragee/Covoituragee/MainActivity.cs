using Android.App;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using Android.Graphics;
using System.Data.SqlClient;
using System.Data;
using System;
using Android.Content;
using Android.Content.PM;
using Android.Preferences;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Dalvik.Annotation;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;



namespace Covoituragee
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const string CLIENT_ID = "";
        private const string CLIENT_SECRET = "";
        private const string SANDBOX_URL = "https://api-m.sandbox.paypal.com";
        static public decimal montant;
        public static readonly string connectionString = @"data source=yourIphere;initial catalog=covoiturage;user id=yourid;password=yourPassword;";
        public static int userId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);


           // ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            //int userId = prefs.GetInt("id_utilisateur", 0); // the second parameter is the default value


            Button button = FindViewById<Button>(Resource.Id.btn_sign_in);
            button.Click += delegate { StartActivity(typeof(LoginActivity)); };

            Button button2 = FindViewById<Button>(Resource.Id.btn_sign_up);
            button2.Click += delegate { StartActivity(typeof(SignUpActivity)); };

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }






       static public async Task<JObject> GetOrderDetails(string orderId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(CLIENT_ID + ":" + CLIENT_SECRET)));

                var response = await client.GetAsync(SANDBOX_URL + "/v2/checkout/orders/" + orderId);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(json);
                }
                else
                {
                    throw new Exception("Error getting PayPal order details: " + await response.Content.ReadAsStringAsync());
                }
            }
        }

      static public  async Task<JObject> CreateOrder()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(CLIENT_ID + ":" + CLIENT_SECRET)));

                /*var content = new StringContent(@"{""intent"": ""CAPTURE"",""purchase_units"": [{""amount"": {""currency_code"": ""EUR"",""value"": ""{ttc.toString()}""},""payer"": {""email_address"": ""sb-d8ecw25506199@business.example.com""}}],""application_context"": {""return_url"": ""https://www.sandbox.paypal.com/checkoutnow/error"",""cancel_url"": ""https://www.sandbox.paypal.com/checkoutnow/error""}}", Encoding.UTF8, "application/json"); 
*/
                string contenuRequete = string.Format(@"{{""intent"": ""CAPTURE"",""purchase_units"": [{{""amount"": {{""currency_code"": ""EUR"",""value"": ""{0}""}},""payer"": {{""email_address"": ""sb-d8ecw25506199@business.example.com""}}}}],""application_context"": {{""return_url"": ""https://www.sandbox.paypal.com/checkoutnow/error"",""cancel_url"": ""https://www.sandbox.paypal.com/checkoutnow/error""}}}}", montant);

                var content = new StringContent(contenuRequete, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(SANDBOX_URL + "/v2/checkout/orders", content);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(json);
                }
                else
                {
                    throw new Exception("Error creating PayPal order: " + await response.Content.ReadAsStringAsync());
                }
            }
        }


       static public async Task<JObject> CapturePayment(string orderId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(CLIENT_ID + ":" + CLIENT_SECRET)));

                var content = new StringContent(@"{""status"": ""COMPLETED""}", Encoding.UTF8, "application/json");
                var response = await client.PostAsync(SANDBOX_URL + "/v2/checkout/orders/" + orderId + "/capture", content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(json);
                }
                else
                {
                    throw new Exception("Error capturing PayPal payment: " + await response.Content.ReadAsStringAsync());
                }
            }
        }













    }
}
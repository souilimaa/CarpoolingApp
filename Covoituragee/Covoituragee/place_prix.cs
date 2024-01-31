using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;

namespace Covoituragee
{
    [Activity(Label = "place_prix")]
    public class place_prix : Activity
    {
        EditText editTextprix, editTextplace;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.place_prix);


            ImageView im1 = FindViewById<ImageView>(Resource.Id.imageView1);
            im1.Click += delegate
            {
                StartActivity(typeof(ajoute_trajet));
            };

            
            editTextprix = FindViewById<EditText>(Resource.Id.editTextprix);
            editTextplace = FindViewById<EditText>(Resource.Id.editTextplace);
            string txtdate = Intent.GetStringExtra("date");
            string txttime = Intent.GetStringExtra("time");
            string txtdepart = Intent.GetStringExtra("depart");
            string txtarriver = Intent.GetStringExtra("arriver");

            Button button = FindViewById<Button>(Resource.Id.button1);

            button.Click += delegate
            {

                if (string.IsNullOrEmpty(editTextprix.Text) ||
               string.IsNullOrEmpty(editTextplace.Text)


               )
                {
                    // display error message if any fields are missing
                    Toast.MakeText(this, "verifier votre information", ToastLength.Short).Show();

                }
                else
                {
                    var intent = new Intent(this, typeof(ref_bag));
                    intent.PutExtra("date", txtdate);
                    intent.PutExtra("time", txttime);
                    intent.PutExtra("depart", txtdepart);
                    intent.PutExtra("arriver", txtarriver);
                    intent.PutExtra("prix", editTextprix.Text);
                    intent.PutExtra("place", editTextplace.Text);
                    

                    StartActivity(intent);


                }
            };
        }
    }
}
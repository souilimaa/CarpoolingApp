using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Refractored.Controls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Covoituragee
{
    [Activity(Label = "RatePassager")]
    public class RatePassager : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.RatePassager);
            ImageButton backButton = FindViewById<ImageButton>(Resource.Id.backButton);
            backButton.Click += (sender, args) => { Finish(); };

            int DriverID = Intent.GetIntExtra("Driver", 0);
            int PassagerID = Intent.GetIntExtra("passager", 0);
            int RatingValue = 5;
            string RatingDes = "Excellent";
            getPassagerInfo(PassagerID);
            TextView RatingDescription = FindViewById<TextView>(Resource.Id.ratingDesc);
            ImageButton R1 = FindViewById<ImageButton>(Resource.Id.R1);
            ImageButton R2 = FindViewById<ImageButton>(Resource.Id.R2);
            ImageButton R3 = FindViewById<ImageButton>(Resource.Id.R3);
            ImageButton R4 = FindViewById<ImageButton>(Resource.Id.R4);
            ImageButton R5 = FindViewById<ImageButton>(Resource.Id.R5);

            R1.Click += (s, e) =>
            {
                RatingValue = 1;
                RatingDes = "Mauvais";
                RatingDescription.Text = "Mauvais";
                R1.SetImageResource(Resource.Drawable.starForRating);
                R2.SetImageResource(Resource.Drawable.starForRatingGrey);
                R3.SetImageResource(Resource.Drawable.starForRatingGrey);
                R4.SetImageResource(Resource.Drawable.starForRatingGrey);
                R5.SetImageResource(Resource.Drawable.starForRatingGrey);
            };
           

            R2.Click += (s, e) =>
            {
                RatingValue = 2;
                RatingDes = "Inférieur à la moyenne";
                RatingDescription.Text = "Inférieur à la moyenne";
                R1.SetImageResource(Resource.Drawable.starForRating);
                R2.SetImageResource(Resource.Drawable.starForRating);
                R3.SetImageResource(Resource.Drawable.starForRatingGrey);
                R4.SetImageResource(Resource.Drawable.starForRatingGrey);
                R5.SetImageResource(Resource.Drawable.starForRatingGrey);

            };
            R3.Click += (s, e) =>
            {
                RatingValue = 3;
                RatingDes = "Moyenne";
                RatingDescription.Text = "Moyenne";
                R1.SetImageResource(Resource.Drawable.starForRating);
                R2.SetImageResource(Resource.Drawable.starForRating);
                R3.SetImageResource(Resource.Drawable.starForRating);
                R4.SetImageResource(Resource.Drawable.starForRatingGrey);
                R5.SetImageResource(Resource.Drawable.starForRatingGrey);

            };
            R4.Click += (s, e) =>
            {
                RatingValue = 4;
                RatingDes = "Bon";
                RatingDescription.Text = "Bon";
                R1.SetImageResource(Resource.Drawable.starForRating);
                R2.SetImageResource(Resource.Drawable.starForRating);
                R3.SetImageResource(Resource.Drawable.starForRating);
                R4.SetImageResource(Resource.Drawable.starForRating);
                R5.SetImageResource(Resource.Drawable.starForRatingGrey);

            };
            R5.Click += (s, e) =>
            {
                RatingValue = 5;
                RatingDes = "Excellent";
                RatingDescription.Text = "Excellent";
                R1.SetImageResource(Resource.Drawable.starForRating);
                R2.SetImageResource(Resource.Drawable.starForRating);
                R3.SetImageResource(Resource.Drawable.starForRating);
                R4.SetImageResource(Resource.Drawable.starForRating);
                R5.SetImageResource(Resource.Drawable.starForRating);

            };

            AppCompatButton rateDriver = FindViewById<AppCompatButton>(Resource.Id.RateDriverBtn);
            rateDriver.Click += (sender, e) =>
            {
                //insertRating(DriverID, UtilisateurId, RatingValue, RatingDes);
                insertRating(DriverID, PassagerID, RatingValue, RatingDes);
                Toast.MakeText(this, "Merci pour votre évaluation", ToastLength.Short).Show();
                StartActivity(typeof(TrajetsOfferts));
            };

        }



        public void getPassagerInfo(int PassagerID)
        {
            CircleImageView userPic = FindViewById<CircleImageView>(Resource.Id.userImage);
            TextView fullName = FindViewById<TextView>(Resource.Id.fullnameDriver);
            string Sql = "select id_utilisateur,nom,prenom,imageUtilisateur from Utilisateur where id_utilisateur=@PassagerID";
            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(Sql, con))
                {
                    command.Parameters.AddWithValue("@PassagerID", PassagerID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            fullName.Text = reader.GetString(2) + " " + reader.GetString(1);
                            byte[] imageData = (byte[])reader["imageUtilisateur"];

                            // Convert the image data to a bitmap 
                            Bitmap userpic = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
                            userPic.SetImageBitmap(userpic);
                        }
                    }
                    con.Close();
                }



            }
        }


        public void insertRating(int DriverID, int PassagerID, int ratingValue, string RatingDesc)
        {
            string Sql = "insert into Rating(UtilisateurId,RatedByUtilisateurId,Rating,Review)values(@utilisateurID,@RatedByID,@ratingValue,@ratingDesc)";
            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(Sql, con))
                {
                    command.Parameters.AddWithValue("@utilisateurID", PassagerID);
                    command.Parameters.AddWithValue("@RatedByID",DriverID);
                    command.Parameters.AddWithValue("@ratingValue", ratingValue);
                    command.Parameters.AddWithValue("@RatingDesc", RatingDesc);







                    command.ExecuteNonQuery();
                    con.Close();
                }
            }


        }




    }
}
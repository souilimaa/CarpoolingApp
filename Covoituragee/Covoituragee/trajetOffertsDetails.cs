using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Org.Xmlpull.V1.Sax2;
using Refractored.Controls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace Covoituragee
{
    [Activity(Label = "trajetOffertsDetailscs")]
    public class trajetOffertsDetails : Activity
    {
        List<passager> passagers= new List<passager>();
        static int trajetID;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            trajetID =Intent.GetIntExtra("trajetId", 0);
            int Utilisateur_id= Intent.GetIntExtra("UtilisateurId", 0);
            // Create your application here
            SetContentView(Resource.Layout.trajetOffertsDetails);
            GetPassagers(trajetID);
            ListView PassagerLV = FindViewById<ListView>(Resource.Id.listViewPassagers);
            PassagerLV.Adapter = new HomeScreenAdapter(this,passagers);
            ImageButton backButton = FindViewById<ImageButton>(Resource.Id.backButton);
            backButton.Click += (sender, args) => { Finish(); };
            getTrajetInfo(trajetID);
            getDriverInfo(Utilisateur_id);
            if (passagers.Count == 0)
            {
                FindViewById<TextView>(Resource.Id.PassagerDetailsTV).Visibility=ViewStates.Gone;
            }
            else
            {
                FindViewById<TextView>(Resource.Id.PassagerDetailsTV).Visibility = ViewStates.Visible;

            }
        }
        public void getTrajetInfo(int trajetID)
        {
            TextView carInfo = FindViewById<TextView>(Resource.Id.CarMarkModeleTextV);
            TextView TrajetPrice = FindViewById<TextView>(Resource.Id.TrajetPriceTextV);
            TextView source = FindViewById<TextView>(Resource.Id.sourceTextV);
            TextView destination = FindViewById<TextView>(Resource.Id.destinationTextV);
            TextView trajetDate = FindViewById<TextView>(Resource.Id.dateTrajetTextV);
            TextView lieuRencontre = FindViewById<TextView>(Resource.Id.LieuRencontreTV);
            TextView tempsTrajet = FindViewById<TextView>(Resource.Id.TempsTrajetTextV);
            TextView nbrPlaceRestant = FindViewById<TextView>(Resource.Id.nbrPlaceRestantsTextV);
            TextView nbrTotalPlace = FindViewById<TextView>(Resource.Id.nbrPlaceTotalTextV);
            string Sql = "SELECT  source, destination, dateTrajet, tempsTrajet, prix, LieuRencontre, nbrPlaces, nbrPlaces - COALESCE(SUM(nombrePlace), 0) AS placeRestant, marque, Model\r\nFROM trajet T\r\nJOIN UtlisateurVoiture UV ON UV.utilisateur_voiture_id = T.utilisateur_voiture_id\r\nJOIN voiture V ON V.voiture_id = UV.voiture_id\r\nLEFT JOIN demandeTrajet D ON D.trajet_id = T.trajet_id AND Etat_demande = 'ACCEPTED'\r\nWHERE T.trajet_id = @trajetID\r\nGROUP BY T.trajet_id, source, destination, dateTrajet, tempsTrajet, nbrPlaces, marque, Model, prix, LieuRencontre";
            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(Sql, con))
                {
                    command.Parameters.AddWithValue("@trajetID", trajetID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            carInfo.Text = reader.GetString(8) + " | "+reader.GetString(9);
                            TrajetPrice.Text=reader.GetDouble(4)+" DHS";
                            source.Text = reader.GetString(0);
                            destination.Text = reader.GetString(1);
                            trajetDate.Text = reader.GetDateTime(2).ToString("yyyy-MM-dd");
                            tempsTrajet.Text = reader.GetTimeSpan(3).ToString(@"hh\:mm");
                            lieuRencontre.Text = reader.GetString(5);
                            nbrTotalPlace.Text = reader.GetInt32(6)+" Total";
                            nbrPlaceRestant.Text = reader.GetInt32(7) + " restant(s)";
                        }
                    }
                    con.Close();
                }



            }

        }
        public void getDriverInfo(int driverID)
        {
            string Sql = "select nom,prenom,imageUtilisateur from Utilisateur where id_utilisateur=@userID";
            TextView fullnameDriver = FindViewById<TextView>(Resource.Id.DriverFullNameTextV);
            CircleImageView userImage = FindViewById<CircleImageView>(Resource.Id.userImage);

            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(Sql, con))
                {
                    command.Parameters.AddWithValue("@userID",driverID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            byte[] imageData = (byte[])reader["imageUtilisateur"];

                            // Convert the image data to a bitmap 
                            Bitmap userpic = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
                            fullnameDriver.Text =reader.GetString(1)+" "+reader.GetString(0);
                            userImage.SetImageBitmap(userpic);

                        }
                    }
                    con.Close();
                }

            }
            }

            public void GetPassagers(int trajetID)
        {
            string sql = "\r\nSELECT id_utilisateur,nom,prenom,imageUtilisateur,(SELECT COUNT(*)FROM Rating WHERE Rating.UtilisateurId = U.id_utilisateur) AS rating_count,\r\n(SELECT AVG(Rating) FROM Rating WHERE Rating.UtilisateurId = U.id_utilisateur) AS rating_value\r\nFROM Utilisateur U\r\nWHERE U.id_utilisateur IN (SELECT demandeur_id FROM demandeTrajet WHERE trajet_id =@trajetID and Etat_demande='ACCEPTED')";
            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(sql, con))
                {
                    command.Parameters.AddWithValue("@trajetID", trajetID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            byte[] imageData = (byte[])reader["imageUtilisateur"];

                            // Convert the image data to a bitmap 
                            Bitmap userpic = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
                            int ratingValue = 0;
                            int nbrRating = 0;
                            if (reader.IsDBNull(5))
                            {
                                ratingValue = 0;
                            }
                            else
                            {
                                ratingValue = reader.GetInt32(5);

                            }
                            if (reader.IsDBNull(4))
                            {
                                nbrRating = 0;
                            }
                            else
                            {
                                nbrRating = reader.GetInt32(4);
                            }
                            passagers.Add(new passager(reader.GetInt32(0), reader.GetString(2),
                            reader.GetString(1),
                            userpic,
                            nbrRating,
                            ratingValue
                            
                            
                            ));

                        }
                    }
                    con.Close();
                }



            }


        }

    
        public class HomeScreenAdapter : BaseAdapter<passager>
        {
            List<passager> items;
            Activity context;
            public HomeScreenAdapter(Activity context, List<passager> items)
                : base()
            {
                this.context = context;
                this.items = items;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override passager this[int position]
            {
                get { return items[position]; }
            }
            public override int Count
            {
                get { return items.Count; }
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                var item = items[position];
                View view = convertView;
                if (view == null) // no view to re-use, create new
                    view = context.LayoutInflater.Inflate(Resource.Layout.passagerInfo, null);
                view.FindViewById<CircleImageView>(Resource.Id.userImage).SetImageBitmap(item.passagerImage);
                view.FindViewById<TextView>(Resource.Id.PassagerFullNameTextV).Text = item.prenom + " " + item.nom;
                view.FindViewById<TextView>(Resource.Id.nbrAvisTV).Text = "(" + item.nbravis+" avis)" ;
                if (item.ratingValue == 0)
                {
                    ImageView R1 = view.FindViewById<ImageView>(Resource.Id.R1);
                    ImageView R2 = view.FindViewById<ImageView>(Resource.Id.R2);
                    ImageView R3 = view.FindViewById<ImageView>(Resource.Id.R3);
                    ImageView R4 = view.FindViewById<ImageView>(Resource.Id.R4);
                    ImageView R5 = view.FindViewById<ImageView>(Resource.Id.R5);

                    R1.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R2.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R3.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R4.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R5.SetImageResource(Resource.Drawable.starRatingIconeGray);
                }
                else if (item.ratingValue==1)
                {
                    ImageView R1 = view.FindViewById<ImageView>(Resource.Id.R1);
                    ImageView R2 = view.FindViewById<ImageView>(Resource.Id.R2);
                    ImageView R3 = view.FindViewById<ImageView>(Resource.Id.R3);
                    ImageView R4 = view.FindViewById<ImageView>(Resource.Id.R4);
                    ImageView R5 = view.FindViewById<ImageView>(Resource.Id.R5);

                    R1.SetImageResource(Resource.Drawable.starRatingIcone);
                    R2.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R3.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R4.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R5.SetImageResource(Resource.Drawable.starRatingIconeGray);
                }
                else if (item.ratingValue == 2)
                {
                    ImageView R1 = view.FindViewById<ImageView>(Resource.Id.R1);
                    ImageView R2 = view.FindViewById<ImageView>(Resource.Id.R2);
                    ImageView R3 = view.FindViewById<ImageView>(Resource.Id.R3);
                    ImageView R4 = view.FindViewById<ImageView>(Resource.Id.R4);
                    ImageView R5 = view.FindViewById<ImageView>(Resource.Id.R5);

                    R1.SetImageResource(Resource.Drawable.starRatingIcone);
                    R2.SetImageResource(Resource.Drawable.starRatingIcone);
                    R3.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R4.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R5.SetImageResource(Resource.Drawable.starRatingIconeGray);
                }

                else if (item.ratingValue==3)
                {
                    ImageView R1 = view.FindViewById<ImageView>(Resource.Id.R1);
                    ImageView R2 = view.FindViewById<ImageView>(Resource.Id.R2);
                    ImageView R3 = view.FindViewById<ImageView>(Resource.Id.R3);
                    ImageView R4 = view.FindViewById<ImageView>(Resource.Id.R4);
                    ImageView R5 = view.FindViewById<ImageView>(Resource.Id.R5);
                    R1.SetImageResource(Resource.Drawable.starRatingIcone);
                    R2.SetImageResource(Resource.Drawable.starRatingIcone);
                    R3.SetImageResource(Resource.Drawable.starRatingIcone);
                    R4.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R5.SetImageResource(Resource.Drawable.starRatingIconeGray);
                }

                else if (item.ratingValue == 4)
                {
                    ImageView R1 = view.FindViewById<ImageView>(Resource.Id.R1);
                    ImageView R2 = view.FindViewById<ImageView>(Resource.Id.R2);
                    ImageView R3 = view.FindViewById<ImageView>(Resource.Id.R3);
                    ImageView R4 = view.FindViewById<ImageView>(Resource.Id.R4);
                    ImageView R5 = view.FindViewById<ImageView>(Resource.Id.R5);

                    R1.SetImageResource(Resource.Drawable.starRatingIcone);
                    R2.SetImageResource(Resource.Drawable.starRatingIcone);
                    R3.SetImageResource(Resource.Drawable.starRatingIcone);
                    R4.SetImageResource(Resource.Drawable.starRatingIcone);
                    R5.SetImageResource(Resource.Drawable.starRatingIconeGray);
                }

                else if (item.ratingValue == 5)
                {
                    ImageView R1 = view.FindViewById<ImageView>(Resource.Id.R1);
                    ImageView R2 = view.FindViewById<ImageView>(Resource.Id.R2);
                    ImageView R3 = view.FindViewById<ImageView>(Resource.Id.R3);
                    ImageView R4 = view.FindViewById<ImageView>(Resource.Id.R4);
                    ImageView R5 = view.FindViewById<ImageView>(Resource.Id.R5);

                    R1.SetImageResource(Resource.Drawable.starRatingIcone);
                    R2.SetImageResource(Resource.Drawable.starRatingIcone);
                    R3.SetImageResource(Resource.Drawable.starRatingIcone);
                    R4.SetImageResource(Resource.Drawable.starRatingIcone);
                    R5.SetImageResource(Resource.Drawable.starRatingIcone);
                }

               int nbrReservePlace= getNumberPlaceForPassager(trajetID, item.passagerID);
                view.FindViewById<TextView>(Resource.Id.textViewnbrPlace).Text="("+nbrReservePlace+" Place(s))";


                return view;
            }
        }

       static  public int getNumberPlaceForPassager(int trajetID, int PassagerID)
        {
            int nbrPlaceReserve = 0;
            string sql = "select nombrePlace from demandeTrajet where trajet_id=@trajetID and demandeur_id=@PassagerID";
            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(sql, con))
                {
                    command.Parameters.AddWithValue("@trajetID", trajetID);
                    command.Parameters.AddWithValue("@PassagerID", PassagerID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            nbrPlaceReserve=reader.GetInt32(0);
                           

                        }
                        con.Close();
                    }



                }

            }
            return nbrPlaceReserve;
        }

        public class passager
        {
            public int passagerID;
            public string prenom;
            public string nom;
            public int ratingValue;
            public int nbravis;
            public Bitmap passagerImage;
            public passager(int passagerID,string prenom, string nom,Bitmap passagerImage, int nbravis, int ratingValue)
            {
                this.passagerID = passagerID;
                this.nom = nom;
                this.prenom = prenom;
                this.ratingValue = ratingValue;
                this.nbravis = nbravis;
                this.passagerImage = passagerImage;
            }
        }








    }
}
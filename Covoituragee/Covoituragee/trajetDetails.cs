using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Refractored.Controls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using static Android.Content.ClipData;
using static Android.Graphics.ImageDecoder;
using static Android.Provider.Telephony;

namespace Covoituragee
{
    [Activity(Label = "trajetDetails")]
    public class trajetDetails : Activity
    {
        static string connection = MainActivity.connectionString;
        private int trajetId;
        private int Utilisateur_id;
        int DriverID;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.trajetDetails);

            trajetId = Intent.GetIntExtra("trajetId",0);
            Utilisateur_id=Intent.GetIntExtra("UtilisateurId",0);
             DriverID= Intent.GetIntExtra("DriverID",0);

            getDetailsTrajet(trajetId);
            getNumberOfAvailableSeats();
            getPrixPlacesReserver();
            GetPreferences();
            getRatingOfDriver();
            ImageButton backButton = FindViewById<ImageButton>(Resource.Id.backButton);
            backButton.Click += (sender, args) => { Finish(); };
        }

        public void getDetailsTrajet(int trajetid)
        {

            TextView DriverFullName = FindViewById<TextView>(Resource.Id.DriverFullNameTextV);
            TextView source = FindViewById<TextView>(Resource.Id.sourceTextV);
            TextView destination= FindViewById<TextView>(Resource.Id.destinationTextV);
            TextView dateTrajet = FindViewById<TextView>(Resource.Id.dateTrajetTextV);
            TextView tempsTrajet = FindViewById<TextView>(Resource.Id.TempsTrajetTextV);
            TextView nbrTotalSeatsTrajets = FindViewById<TextView>(Resource.Id.nbrPlaceTotalTextV);
            TextView prixOfSeat = FindViewById<TextView>(Resource.Id.PrixOfSeat);
            TextView car = FindViewById<TextView>(Resource.Id.carMarkTextV);
            TextView carColor = FindViewById<TextView>(Resource.Id.carColorTextV);
            TextView SizeBaggage = FindViewById<TextView>(Resource.Id.BaggageSize);
            TextView lieuRecontre = FindViewById<TextView>(Resource.Id.LieuRencontreTV);
            CircleImageView userImage=FindViewById<CircleImageView>(Resource.Id.userImage);
           


            string sql = "select nom,prenom,imageUtilisateur,marque,Model,Couleur,source,destination,dateTrajet,tempsTrajet,prix,nbrPlaces,tailleBagageAutorisee,LieuRencontre\r\nfrom Utilisateur U,trajet T,UtlisateurVoiture UV,voiture V\r\nWHERE T.trajet_id=@trajetId\r\nand UV.utilisateur_voiture_id=T.utilisateur_voiture_id\r\nand U.id_utilisateur=UV.id_utilisateur\r\nand V.voiture_id=UV.voiture_id\r\n";
            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                using(SqlCommand command=new SqlCommand(sql, con))
                {
                    command.Parameters.Add("@trajetId", trajetid);
                    con.Open();
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DriverFullName.Text = reader.GetString(1)+" "+reader.GetString(0);
                            source.Text = reader.GetString(6);
                            destination.Text=reader.GetString(7);
                            DateTime dt = reader.GetDateTime(8);
                            dateTrajet.Text = dt.ToString("yyyy-MM-dd");
                            tempsTrajet.Text = reader.GetTimeSpan(9).ToString(@"hh\:mm");
                            prixOfSeat.Text = reader.GetDouble(10) + " DHS";
                            nbrTotalSeatsTrajets.Text = reader.GetInt32(11).ToString() + " Total";
                            car.Text=reader.GetString(3)+" | "+reader.GetString(4);
                            carColor.Text = reader.GetString(5);
                            SizeBaggage.Text = reader.GetString(12);
                            lieuRecontre.Text= reader.GetString(13);
                            byte[] imageData = (byte[])reader["imageUtilisateur"];

                            // Convert the image data to a bitmap 
                            Bitmap userpic = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
                            userImage.SetImageBitmap(userpic);
                        }


                    }
                    con.Close();

                }


            }

        }

        public void getNumberOfAvailableSeats()
        {
            TextView placeRest = FindViewById<TextView>(Resource.Id.nbrPlaceRestantsTextV);
            String Sql = "select T.nbrPlaces-sum(d.nombrePlace)\r\nfrom demandeTrajet d,Trajet T\r\nwhere d.trajet_id=@TrajetID and T.trajet_id=d.trajet_id\r\nand Etat_demande='ACCEPTED'\r\nGROUP BY T.nbrPlaces";
            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                using (SqlCommand command = new SqlCommand(Sql, con))
                {
                    command.Parameters.Add("@TrajetID", trajetId);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                           placeRest.Text = reader.GetInt32(0).ToString()+" restant(s)";
                        }


                    }
                    con.Close();

                }


            }
        }

        public void getPrixPlacesReserver()
        {
            string Sql = "select nombrePlace,prix*nombrePlace\r\nfrom trajet T,demandeTrajet D\r\nwhere T.trajet_id=@trajetID and D.demandeur_id=@utilisateurID\r\nand D.trajet_id=T.trajet_id";
            TextView prixSeatsReserve = FindViewById<TextView>(Resource.Id.TrajetPriceTextV);
            TextView seatsReserve = FindViewById<TextView>(Resource.Id.NbrSeatsTextV);
                 using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                using (SqlCommand command = new SqlCommand(Sql, con))
                {
                    command.Parameters.Add("@TrajetID", trajetId);
                    command.Parameters.Add("@utilisateurID", Utilisateur_id);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            prixSeatsReserve.Text=reader.GetDouble(1).ToString()+" DHS";
                            seatsReserve.Text ="("+ reader.GetInt32(0).ToString()+" Place(s))";
                        }


                    }
                    con.Close();

                }
            }


            }

        public void GetPreferences()
        {
            string bavardage="";
            string fumer="";
            string animaux="";
            string musique="";
            string autrePreference="";
            string Sql = "select bavardage,fumer,animaux,musique,autrePreferences from preference where trajet_id=@trajetID";

            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                using (SqlCommand command = new SqlCommand(Sql, con))
                {
                    command.Parameters.Add("@TrajetID", trajetId);
                    command.Parameters.Add("@utilisateurID", Utilisateur_id);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                           bavardage=reader.GetString(0);
                            fumer=reader.GetString(1);  
                            animaux=reader.GetString(2);    
                            musique=reader.GetString(3);
                            if (reader.IsDBNull(4))
                            {
                                autrePreference = "none";
                            }
                            else
                            {
                                autrePreference = reader.GetString(4);
                            }
                        }


                    }
                    con.Close();
                                             
                }
            }
            if (bavardage == "non")
            {
                ImageView bvr = FindViewById<ImageView>(Resource.Id.ChatImageV);
                ImageView nobvr= FindViewById<ImageView>(Resource.Id.NoChatImageV);
                bvr.Visibility = ViewStates.Gone;
                nobvr.Visibility = ViewStates.Visible;
            }
            else
            {
                ImageView bvr = FindViewById<ImageView>(Resource.Id.ChatImageV);
                ImageView nobvr = FindViewById<ImageView>(Resource.Id.NoChatImageV);
                bvr.Visibility = ViewStates.Visible;
                nobvr.Visibility = ViewStates.Gone;
            }
            if (fumer == "non")
            {
                ImageView fum = FindViewById<ImageView>(Resource.Id.AllowSmokingimageV);
                ImageView nofum = FindViewById<ImageView>(Resource.Id.NoSmokingimageV);
                fum.Visibility = ViewStates.Gone;
                nofum.Visibility = ViewStates.Visible;
            }
            else
            {
                ImageView fum = FindViewById<ImageView>(Resource.Id.AllowSmokingimageV);
                ImageView nofum = FindViewById<ImageView>(Resource.Id.NoSmokingimageV);
                fum.Visibility = ViewStates.Visible;
                nofum.Visibility = ViewStates.Gone;
            }
            if (animaux=="non")
            {
                ImageView anim = FindViewById<ImageView>(Resource.Id.petsimageV);
                ImageView noanim = FindViewById<ImageView>(Resource.Id.noPetsImageV);
                anim.Visibility = ViewStates.Gone;
                noanim.Visibility = ViewStates.Visible;
            }
            else
            {
                ImageView anim = FindViewById<ImageView>(Resource.Id.petsimageV);
                ImageView noanim = FindViewById<ImageView>(Resource.Id.noPetsImageV);
                anim.Visibility = ViewStates.Visible;
                noanim.Visibility = ViewStates.Gone;
            }

            if (musique == "non")
            {
                ImageView mus = FindViewById<ImageView>(Resource.Id.MusicImageV);
                ImageView nomus = FindViewById<ImageView>(Resource.Id.NoMusicImageV);
                mus.Visibility = ViewStates.Gone;
                nomus.Visibility = ViewStates.Visible;
            }
            else { 
                ImageView mus = FindViewById<ImageView>(Resource.Id.MusicImageV);
            ImageView nomus = FindViewById<ImageView>(Resource.Id.NoMusicImageV);
            mus.Visibility = ViewStates.Visible;
            nomus.Visibility = ViewStates.Gone;

        }


        }

        public void getRatingOfDriver()
        {
            int RatingAvg=0;
            int nombreAvis = 0;
            string sql = "select AVG(Rating),count(*)\r\nfrom Rating\r\nwhere UtilisateurID=@DriverID";
            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, con))
                {
                    command.Parameters.Add("@DriverID",DriverID);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                RatingAvg = reader.GetInt32(0);
                                nombreAvis=reader.GetInt32(1);

                            }
                                
                           
                        }


                    }
                    con.Close();

                }

            }
            if (RatingAvg == 0)
            {
                ImageView R1 = FindViewById<ImageView>(Resource.Id.R1);
                ImageView R2 = FindViewById<ImageView>(Resource.Id.R2);
                ImageView R3 = FindViewById<ImageView>(Resource.Id.R3);
                ImageView R4 = FindViewById<ImageView>(Resource.Id.R4);
                ImageView R5 = FindViewById<ImageView>(Resource.Id.R5);

                R1.SetImageResource(Resource.Drawable.starRatingIconeGray);
                R2.SetImageResource(Resource.Drawable.starRatingIconeGray);
                R3.SetImageResource(Resource.Drawable.starRatingIconeGray);
                R4.SetImageResource(Resource.Drawable.starRatingIconeGray);
                R5.SetImageResource(Resource.Drawable.starRatingIconeGray);
                TextView nbrAvis = FindViewById<TextView>(Resource.Id.nbrAvisTV);
                nbrAvis.Text = "(0 avis)";


            }
            else if (RatingAvg == 1)
            {
                ImageView R1 = FindViewById<ImageView>(Resource.Id.R1);
                ImageView R2 = FindViewById<ImageView>(Resource.Id.R2);
                ImageView R3 = FindViewById<ImageView>(Resource.Id.R3);
                ImageView R4 = FindViewById<ImageView>(Resource.Id.R4);
                ImageView R5 = FindViewById<ImageView>(Resource.Id.R5);

                R1.SetImageResource(Resource.Drawable.starRatingIcone);
                R2.SetImageResource(Resource.Drawable.starRatingIconeGray);
                R3.SetImageResource(Resource.Drawable.starRatingIconeGray);
                R4.SetImageResource(Resource.Drawable.starRatingIconeGray);
                R5.SetImageResource(Resource.Drawable.starRatingIconeGray);

                TextView nbrAvis = FindViewById<TextView>(Resource.Id.nbrAvisTV);
                nbrAvis.Text = "("+nombreAvis+" avis)";
               
            }
            else if (RatingAvg == 2)
            {
                ImageView R1 = FindViewById<ImageView>(Resource.Id.R1);
                ImageView R2 = FindViewById<ImageView>(Resource.Id.R2);
                ImageView R3 = FindViewById<ImageView>(Resource.Id.R3);
                ImageView R4 = FindViewById<ImageView>(Resource.Id.R4);
                ImageView R5 = FindViewById<ImageView>(Resource.Id.R5);

                R1.SetImageResource(Resource.Drawable.starRatingIcone);
                R2.SetImageResource(Resource.Drawable.starRatingIcone);
                R3.SetImageResource(Resource.Drawable.starRatingIconeGray);
                R4.SetImageResource(Resource.Drawable.starRatingIconeGray);
                R5.SetImageResource(Resource.Drawable.starRatingIconeGray);
                TextView nbrAvis = FindViewById<TextView>(Resource.Id.nbrAvisTV);
                nbrAvis.Text = "(" + nombreAvis + " avis)";


            }
            else if (RatingAvg == 3)
            {
                ImageView R1 = FindViewById<ImageView>(Resource.Id.R1);
                ImageView R2 = FindViewById<ImageView>(Resource.Id.R2);
                ImageView R3 = FindViewById<ImageView>(Resource.Id.R3);
                ImageView R4 = FindViewById<ImageView>(Resource.Id.R4);
                ImageView R5 = FindViewById<ImageView>(Resource.Id.R5);
                R1.SetImageResource(Resource.Drawable.starRatingIcone);
                R2.SetImageResource(Resource.Drawable.starRatingIcone);
                R3.SetImageResource(Resource.Drawable.starRatingIcone);
                R4.SetImageResource(Resource.Drawable.starRatingIconeGray);
                R5.SetImageResource(Resource.Drawable.starRatingIconeGray);
                TextView nbrAvis = FindViewById<TextView>(Resource.Id.nbrAvisTV);
                nbrAvis.Text = "(" + nombreAvis + " avis)";


            }
            else if (RatingAvg == 4)
            {
                ImageView R1 = FindViewById<ImageView>(Resource.Id.R1);
                ImageView R2 = FindViewById<ImageView>(Resource.Id.R2);
                ImageView R3 = FindViewById<ImageView>(Resource.Id.R3);
                ImageView R4 = FindViewById<ImageView>(Resource.Id.R4);
                ImageView R5 = FindViewById<ImageView>(Resource.Id.R5);

                R1.SetImageResource(Resource.Drawable.starRatingIcone);
                R2.SetImageResource(Resource.Drawable.starRatingIcone);
                R3.SetImageResource(Resource.Drawable.starRatingIcone);
                R4.SetImageResource(Resource.Drawable.starRatingIcone);
                R5.SetImageResource(Resource.Drawable.starRatingIconeGray);
                TextView nbrAvis = FindViewById<TextView>(Resource.Id.nbrAvisTV);
                nbrAvis.Text = "(" + nombreAvis + " avis)";

            }
            else if (RatingAvg == 5)
            {
                ImageView R1 = FindViewById<ImageView>(Resource.Id.R1);
                ImageView R2 = FindViewById<ImageView>(Resource.Id.R2);
                ImageView R3 = FindViewById<ImageView>(Resource.Id.R3);
                ImageView R4 = FindViewById<ImageView>(Resource.Id.R4);
                ImageView R5 = FindViewById<ImageView>(Resource.Id.R5);

                R1.SetImageResource(Resource.Drawable.starRatingIcone);
                R2.SetImageResource(Resource.Drawable.starRatingIcone);
                R3.SetImageResource(Resource.Drawable.starRatingIcone);
                R4.SetImageResource(Resource.Drawable.starRatingIcone);
                R5.SetImageResource(Resource.Drawable.starRatingIcone);
                TextView nbrAvis = FindViewById<TextView>(Resource.Id.nbrAvisTV);
                nbrAvis.Text = "(" + nombreAvis + " avis)";


            }






        }















    }

}
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.TextField;
using Refractored.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using static Android.Graphics.ImageDecoder;

namespace Covoituragee
{
    [Activity(Label = "trajetDetailsReserve")]
    public class trajetDetailsReserve : Activity
    {
        string connection=MainActivity.connectionString;
        int total;
        private int trajetId;
        int DriverID;
        //EditText placereserve;
        int myInt;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.trajetDetailsReserve);
            ImageView im1 = FindViewById<ImageView>(Resource.Id.imageView1);
            im1.Click += delegate
            {
                StartActivity(typeof(Home));
            };

            trajetId = Intent.GetIntExtra("trajetId", 0);
            DriverID = Intent.GetIntExtra("DriverID", 0);
            int Utilisateur_id = MainActivity.userId;

            total=getDetailsTrajet(trajetId);
            // getPrixPlacesReserver();
            GetPreferences();
            getRatingOfDriver();
            // placereserve = FindViewById<EditText>(Resource.Id.textInputEditText1);
            AutoCompleteTextView placereserve = FindViewById<AutoCompleteTextView>(Resource.Id.textInputEditText1);
            int val = placereserve.Text.Length;

            // placereserve = FindViewById<EditText>(Resource.Id.textInputEditText1);
            // int outputInt = Convert.ToInt32(placereserve.Text);
            int outputInt=0;
            placereserve.TextChanged += delegate
            {
                if (placereserve.Text != "")
                {
                    outputInt = Convert.ToInt32(placereserve.Text);

                   
                }
            };



            getDriverID(trajetId);









            /*using (SqlConnection com = new SqlConnection(connection))
            {
                com.Open();
                string query = "select T.nbrPlaces-sum(d.nombrePlace) from demandeTrajet d,Trajet T where d.trajet_id=@TrajetID and T.trajet_id=d.trajet_id and Etat_demande='ACCEPTED' GROUP BY T.nbrPlaces";
                using (SqlCommand command = new SqlCommand(query, com))
                {
                    command.Parameters.AddWithValue("@trajetId", trajetId);


                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            if (reader.HasRows)

                                myInt = reader.GetInt32(0);
                            else
                                myInt =Convert.ToInt32(FindViewById<TextView>(Resource.Id.nbrPlaceTotalTextV).Text) ;


                        }
                    }


                   
                };
                com.Close();
            }*/
            getReservedTrajet(trajetId);
            int restant = getNumberofReservedPlace();
            myInt =total-restant;


            TextView nbrplaceReste = FindViewById<TextView>(Resource.Id.nbrPlaceRestantsTextV);
            nbrplaceReste.Text = myInt.ToString()+ " restant(s)";
            AppCompatButton button2 = FindViewById<AppCompatButton>(Resource.Id.btnareserve);
            button2.Click += delegate
            {


                // --->>>

                if (outputInt == 0 || outputInt > myInt)
                {
                    Toast.MakeText(this, "Vérifier le nombre de place reserver ", ToastLength.Short).Show();
                }
                else
                {

                    using (SqlConnection con = new SqlConnection(connection))
                    {
                        con.Open();
                        string sql = "Insert Into demandeTrajet(trajet_id,demandeur_id,Etat_demande,nombrePlace) VALUES(@trajetId,@UtilisateurId,NULL,@placereserve)";
                        using (SqlCommand command = new SqlCommand(sql, con))
                        {

                            command.Parameters.AddWithValue("@trajetId", trajetId);
                            command.Parameters.AddWithValue("@UtilisateurId", Utilisateur_id);
                            command.Parameters.AddWithValue("@placereserve", placereserve.Text);

                            command.ExecuteNonQuery();
                        };
                        con.Close();

                    };
                    StartActivity(typeof(Home));
                    Toast.MakeText(this, "Attendre la réponse par rapport a votre demande", ToastLength.Short).Show();

                }

            };

       
        }

        public int getDetailsTrajet(int trajetid)
        {

            TextView DriverFullName = FindViewById<TextView>(Resource.Id.DriverFullNameTextV);
            TextView source = FindViewById<TextView>(Resource.Id.sourceTextV);
            TextView destination = FindViewById<TextView>(Resource.Id.destinationTextV);
            TextView dateTrajet = FindViewById<TextView>(Resource.Id.dateTrajetTextV);
            TextView tempsTrajet = FindViewById<TextView>(Resource.Id.TempsTrajetTextV);
            TextView nbrTotalSeatsTrajets = FindViewById<TextView>(Resource.Id.nbrPlaceTotalTextV);
            TextView prixOfSeat = FindViewById<TextView>(Resource.Id.PrixOfSeat);
            TextView car = FindViewById<TextView>(Resource.Id.carMarkTextV);
            TextView carColor = FindViewById<TextView>(Resource.Id.carColorTextV);
            TextView SizeBaggage = FindViewById<TextView>(Resource.Id.BaggageSize);
            TextView lieuRecontre = FindViewById<TextView>(Resource.Id.LieuRencontreTV);
            CircleImageView userImage = FindViewById<CircleImageView>(Resource.Id.userImage);




            string sql = "select nom,prenom,imageUtilisateur,marque,Model,Couleur,source," +
                   "destination,dateTrajet,tempsTrajet,prix,nbrPlaces,tailleBagageAutorisee,LieuRencontre " +
                   "from Utilisateur U, trajet T, UtlisateurVoiture UV, voiture V " +
                   "WHERE T.trajet_id=@trajetId and UV.utilisateur_voiture_id=T.utilisateur_voiture_id " +
                   "and U.id_utilisateur=UV.id_utilisateur and V.voiture_id=UV.voiture_id";
            using (SqlConnection con = new SqlConnection(connection))
            {
                using (SqlCommand command = new SqlCommand(sql, con))
                {
                    command.Parameters.Add("@trajetId", trajetid);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DriverFullName.Text = reader.GetString(1) + " " + reader.GetString(0);
                            source.Text = reader.GetString(6);
                            destination.Text = reader.GetString(7);
                            DateTime dt = reader.GetDateTime(8);
                            dateTrajet.Text = dt.ToString("yyyy-MM-dd ");
                            tempsTrajet.Text = reader.GetTimeSpan(9).ToString(@"hh\:mm");
                            prixOfSeat.Text = reader.GetDouble(10) + " DHS";
                            nbrTotalSeatsTrajets.Text = reader.GetInt32(11).ToString() + " Total";
                            car.Text = reader.GetString(3) + " | " + reader.GetString(4);
                            carColor.Text = reader.GetString(5);
                            total = reader.GetInt32(11);
                            SizeBaggage.Text = reader.GetString(12);
                            byte[] imageData = (byte[])reader["imageUtilisateur"];
                            lieuRecontre.Text = "Le point de départ est: " + reader.GetString(13);
                            // Convert the image data to a bitmap 
                            Bitmap userpic = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
                            userImage.SetImageBitmap(userpic);

                        }


                    }
                    con.Close();

                }


            }
            return total;
        }
        public int getNumberofReservedPlace()
        {
            int k=0;
            TextView placeRest = FindViewById<TextView>(Resource.Id.nbrPlaceRestantsTextV);
            string Sql = "select count(*) from demandeTrajet d,Trajet T where d.trajet_id=@TrajetID and T.trajet_id=d.trajet_id and Etat_demande='ACCEPTED' GROUP BY T.nbrPlaces";
            using (SqlConnection con = new SqlConnection(connection))
            {
                using (SqlCommand command = new SqlCommand(Sql, con))
                {
                    command.Parameters.Add("@TrajetID", trajetId);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            placeRest.Text = reader.GetInt32(0).ToString() + " restant(s)";
                            k=reader.GetInt32(0);
                        }


                    }
                    con.Close();

                }


            }
            return k;
            
        }


       

        public void GetPreferences()
        {
            string bavardage = "";
            string fumer = "";
            string animaux = "";
            string musique = "";
            string autrePreference = "";
            string Sql = "select bavardage,fumer,animaux,musique,autrePreferences from preference where trajet_id=@trajetID";

            using (SqlConnection con = new SqlConnection(connection))
            {
                using (SqlCommand command = new SqlCommand(Sql, con))
                {
                    command.Parameters.Add("@TrajetID", trajetId);
                    command.Parameters.Add("@utilisateurID", DriverID);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bavardage = reader.GetString(0);
                            fumer = reader.GetString(1);
                            animaux = reader.GetString(2);
                            musique = reader.GetString(3);
                           
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
                ImageView nobvr = FindViewById<ImageView>(Resource.Id.NoChatImageV);
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
            if (animaux == "non")
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
            else
            {
                ImageView mus = FindViewById<ImageView>(Resource.Id.MusicImageV);
                ImageView nomus = FindViewById<ImageView>(Resource.Id.NoMusicImageV);
                mus.Visibility = ViewStates.Visible;
                nomus.Visibility = ViewStates.Gone;

            }


        }


        public void getRatingOfDriver()
        {
            int RatingAvg = 0;
            int nombreAvis = 0;
            string sql = "select AVG(Rating),count(*) from Rating where UtilisateurID=@DriverID";
            using (SqlConnection con = new SqlConnection(connection))
            {
                using (SqlCommand command = new SqlCommand(sql, con))
                {
                    command.Parameters.Add("@DriverID", DriverID);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                RatingAvg = reader.GetInt32(0);
                                nombreAvis = reader.GetInt32(1);

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

                /* */
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

                /**/
                R1.SetImageResource(Resource.Drawable.starRatingIcone);
                R2.SetImageResource(Resource.Drawable.starRatingIconeGray);
                R3.SetImageResource(Resource.Drawable.starRatingIconeGray);
                R4.SetImageResource(Resource.Drawable.starRatingIconeGray);
                R5.SetImageResource(Resource.Drawable.starRatingIconeGray);

                TextView nbrAvis = FindViewById<TextView>(Resource.Id.nbrAvisTV);
                nbrAvis.Text = "(" + nombreAvis + " avis)";

            }
            else if (RatingAvg == 2)
            {
                ImageView R1 = FindViewById<ImageView>(Resource.Id.R1);
                ImageView R2 = FindViewById<ImageView>(Resource.Id.R2);
                ImageView R3 = FindViewById<ImageView>(Resource.Id.R3);
                ImageView R4 = FindViewById<ImageView>(Resource.Id.R4);
                ImageView R5 = FindViewById<ImageView>(Resource.Id.R5);

                /* */
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


                /**/
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

                /* */
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

                /**/
                R1.SetImageResource(Resource.Drawable.starRatingIcone);
                R2.SetImageResource(Resource.Drawable.starRatingIcone);
                R3.SetImageResource(Resource.Drawable.starRatingIcone);
                R4.SetImageResource(Resource.Drawable.starRatingIcone);
                R5.SetImageResource(Resource.Drawable.starRatingIcone);
                TextView nbrAvis = FindViewById<TextView>(Resource.Id.nbrAvisTV);
                nbrAvis.Text = "(" + nombreAvis + " avis)";


            }
        }

        void getDriverID(int TrajetID)
        {
            int Driver=0;
            string sql = "select id_utilisateur from trajet,UtlisateurVoiture where trajet.trajet_id=@trajetID";
            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, con))
                {
                    command.Parameters.Add("@trajetId",TrajetID);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Driver = reader.GetInt32(0);

                        }


                    }
                    con.Close();

                }


            }
            if (MainActivity.userId == DriverID)
            {
                AppCompatButton btnReserve = FindViewById<AppCompatButton>(Resource.Id.btnareserve);
                btnReserve.Enabled = false;
                btnReserve.Text = "Vous êtes le conducteur de ce trajet!";
                AutoCompleteTextView placereserve = FindViewById<AutoCompleteTextView>(Resource.Id.textInputEditText1);
                placereserve.Visibility = ViewStates.Gone;



            }



        }


        public void getReservedTrajet(int TrajetID)
        {
            int count=0;
            string sql = "select count(demande_id) from demandeTrajet where demandeur_id=@demandeurId and trajet_id=@trajetId\r\n";
            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, con))
                {
                    command.Parameters.Add("@demandeurId",MainActivity.userId);

                    command.Parameters.Add("@trajetId", TrajetID);
                    con.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                           count = reader.GetInt32(0);

                        }


                    }
                    con.Close();

                }


            }


            if (count != 0)
            {
                AppCompatButton btnReserve = FindViewById<AppCompatButton>(Resource.Id.btnareserve);
                btnReserve.Enabled = false;
                btnReserve.Text = "Déjà réservé !";
                AutoCompleteTextView placereserve = FindViewById<AutoCompleteTextView>(Resource.Id.textInputEditText1);
                placereserve.Visibility = ViewStates.Gone;
                var layoutParams = (ViewGroup.MarginLayoutParams)btnReserve.LayoutParameters;
                layoutParams.TopMargin = 35;
                btnReserve.LayoutParameters = layoutParams;
            }








            }







        }
}
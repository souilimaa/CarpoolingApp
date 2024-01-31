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


namespace Covoituragee { 
    [Activity(Label = "RatePassager")]
    public class RatePassagerList : Activity
    {
        List<passager> passagers = new List<passager>();
       static int Utilisateur_id;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.RatePassagerList);
            int trajetID = Intent.GetIntExtra("trajetId", 0);
            Utilisateur_id = Intent.GetIntExtra("UtilisateurId", 0);
            string sql = "SELECT id_utilisateur,nom,prenom,imageUtilisateur,\r\n(SELECT COUNT(*)FROM Rating WHERE Rating.UtilisateurId = U.id_utilisateur) \r\nAS rating_count,\r\n(SELECT AVG(Rating) FROM Rating WHERE Rating.UtilisateurId = U.id_utilisateur)\r\nAS rating_value\r\nFROM Utilisateur U\r\nWHERE U.id_utilisateur IN (SELECT demandeur_id FROM demandeTrajet WHERE trajet_id =@trajetID\r\nand Etat_demande='ACCEPTED') and U.id_utilisateur not in(\r\nselect UtilisateurId from Rating where RatedByUtilisateurId in (select U.id_utilisateur\r\nfrom Utilisateur U,UtlisateurVoiture UV,trajet T where trajet_id=2 \r\nand UV.utilisateur_voiture_id=T.utilisateur_voiture_id and U.id_utilisateur=UV.id_utilisateur)\r\n)";
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
                            if (!reader.IsDBNull(5))
                            {
                                ratingValue = reader.GetInt32(5);
                            }
                            if (!reader.IsDBNull(4))
                            {
                                nbrRating = reader.GetInt32(4);
                            }
                            passagers.Add(new passager(reader.GetInt32(0), reader.GetString(1),
                            reader.GetString(2),
                            userpic,
                            nbrRating,
                            ratingValue


                            ));

                        }
                    }
                    con.Close();
                }



            }
            ListView PassagerLV = FindViewById<ListView>(Resource.Id.listViewPassager);
            PassagerLV.Adapter = new HomeScreenAdapter(this, passagers);
            ImageButton backButton = FindViewById<ImageButton>(Resource.Id.backButton);
            backButton.Click += (sender, args) => { Finish(); };

            if (passagers.Count == 0)
            {
                LinearLayout EmptyPassagers = FindViewById<LinearLayout>(Resource.Id.EmptyPassagers);
                EmptyPassagers.Visibility = ViewStates.Visible;

            }
            else
            {
                LinearLayout EmptyPassagers = FindViewById<LinearLayout>(Resource.Id.EmptyPassagers);
                EmptyPassagers.Visibility = ViewStates.Gone;
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
                view.FindViewById<TextView>(Resource.Id.nbrAvisTV).Text = "(" + item.nbravis + " avis)";
                view.FindViewById<TextView>(Resource.Id.textViewnbrPlace).Visibility = ViewStates.Gone;
                
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
                else if (item.ratingValue == 1)
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

                else if (item.ratingValue == 3)
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
                view.Click += (sender, args) =>
                {
                    Intent intent = new Intent(view.Context, typeof(RatePassager));
                    intent.PutExtra("passager", item.passagerID);
                    intent.PutExtra("Driver", Utilisateur_id);
                    view.Context.StartActivity(intent);

                };



                return view;
            }
        }

        public void GetPassagers(int trajetID)
        {

            string sql = "SELECT id_utilisateur,nom,prenom,imageUtilisateur,\r\n(SELECT COUNT(*)FROM Rating WHERE Rating.UtilisateurId = U.id_utilisateur) \r\nAS rating_count,\r\n(SELECT AVG(Rating) FROM Rating WHERE Rating.UtilisateurId = U.id_utilisateur)\r\nAS rating_value\r\nFROM Utilisateur U\r\nWHERE U.id_utilisateur IN (SELECT demandeur_id FROM demandeTrajet WHERE trajet_id =2\r\nand Etat_demande='ACCEPTED') and U.id_utilisateur not in(\r\nselect UtilisateurId from Rating where RatedByUtilisateurId in (select U.id_utilisateur\r\nfrom Utilisateur U,UtlisateurVoiture UV,trajet T where trajet_id=2 \r\nand UV.utilisateur_voiture_id=T.utilisateur_voiture_id and U.id_utilisateur=UV.id_utilisateur)\r\n)";
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
                            if (!reader.IsDBNull(5))
                            {
                                ratingValue = reader.GetInt32(5);
                            }
                            if (!reader.IsDBNull(4))
                            {
                                nbrRating = reader.GetInt32(4);
                            }
                            passagers.Add(new passager(reader.GetInt32(0), reader.GetString(1),
                            reader.GetString(2),
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


        public class passager
        {
            public int passagerID;
            public string prenom;
            public string nom;
            public int ratingValue;
            public int nbravis;
            public Bitmap passagerImage;
            public passager(int passagerID, string prenom, string nom, Bitmap passagerImage, int nbravis, int ratingValue)
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
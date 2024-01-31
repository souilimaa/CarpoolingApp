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
    [Activity(Label = "TrajetsOfferts")]
    public class TrajetsOfferts : Activity
    {
        static ListView TrajetOffertsListView;
        List<CTrajetsOfferts> tabletrajetsOfferts = new List<CTrajetsOfferts>();
        static int Utilisateur_id;
        string status = "NoUpdate";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Utilisateur_id = MainActivity.userId;
            SetContentView(Resource.Layout.TrajetsOfferts);


            TrajetOffertsListView = FindViewById<ListView>(Resource.Id.TrajetsOffertsListView);
            GetTrajets(Utilisateur_id);
            // Create your application here
            AppCompatButton MestrajetsBtn = FindViewById<AppCompatButton>(Resource.Id.trajets);
            MestrajetsBtn.Click += (sender, args) =>
            {
                Intent intent = new Intent(this, typeof(MesTrajet));

                intent.PutExtra("UtilisateurId", Utilisateur_id);
                StartActivity(intent);

            };
            if (tabletrajetsOfferts.Count == 0)
            {
                TrajetOffertsListView.Visibility = ViewStates.Invisible;
                LinearLayout ly = FindViewById<LinearLayout>(Resource.Id.EmptyTrajets);
                ly.Visibility = ViewStates.Visible;

            }
            else
            {
                LinearLayout ly = FindViewById<LinearLayout>(Resource.Id.EmptyTrajets);
                ly.Visibility = ViewStates.Invisible;
                TrajetOffertsListView.Visibility = ViewStates.Visible;
                TrajetOffertsListView.Adapter = new TrajetOffertsHomeScreenAdapter(this, tabletrajetsOfferts);


            }
            LinearLayout homeLayout = FindViewById<LinearLayout>(Resource.Id.homeLayout);
            homeLayout.Click += (sender, args) =>
            {
                Intent intent = new Intent(this, typeof(Home));
                StartActivity(intent);

            };

            LinearLayout trajetLayout = FindViewById<LinearLayout>(Resource.Id.trajetLayout);
            trajetLayout.Click += (sender, args) =>
            {
                Intent intent = new Intent(this, typeof(MesTrajet));

                intent.PutExtra("UtilisateurId", MainActivity.userId);
                StartActivity(intent);
            };

            LinearLayout searchLayout = FindViewById<LinearLayout>(Resource.Id.searchLayout);
            searchLayout.Click += (sender, args) =>
            {
                Intent intent = new Intent(this, typeof(ComandeListe));
                StartActivity(intent);

            };



            LinearLayout profilLayout = FindViewById<LinearLayout>(Resource.Id.profilLayout);
            profilLayout.Click += (sender, args) =>
            {
                Intent intent = new Intent(this, typeof(ProfileActivity));
                intent.PutExtra("id_utilisateur", MainActivity.userId);
                StartActivity(intent);
            };


        }
        public void GetTrajets(int utilisateur_id)
        {
            string sql = "select nom,prenom,imageUtilisateur,trajet_id,source,destination,dateTrajet,tempsTrajet,prix,nbrPlaces,Status,marque,Model\r\nfrom trajet T,UtlisateurVoiture UV,Voiture V,Utilisateur U\r\nwhere UV.id_utilisateur=@UtilisateurID\r\nand T.utilisateur_voiture_id=UV.utilisateur_voiture_id\r\nand V.voiture_id=UV.voiture_id\r\nand U.id_utilisateur=UV.id_utilisateur";
            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(sql, con))
                {
                    command.Parameters.AddWithValue("@UtilisateurID", utilisateur_id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            byte[] imageData = (byte[])reader["imageUtilisateur"];

                            // Convert the image data to a bitmap 
                            Bitmap userpic = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
                            if (reader.IsDBNull(10))
                            {
                                status = "NoUpdate";
                            }
                            else
                            {
                                status = reader.GetString(10);
                            }
                            tabletrajetsOfferts.Add(new CTrajetsOfferts(reader.GetString(0),reader.GetString(1),userpic,reader.GetInt32(3),
                                reader.GetString(4),reader.GetString(5),reader.GetDateTime(6),reader.GetTimeSpan(7),
                                reader.GetDouble(8),reader.GetInt32(9),status,reader.GetString(11),reader.GetString(12)

                                ));


                        }
                    }
                    con.Close();
                }
            }
        }

        public static int GetRatingButton(int TrajetID)
        {
            int res=0;
            string sql = "\tSELECT \r\n    CASE \r\n        WHEN COUNT(DISTINCT demandeur_id) = SUM(CASE WHEN Rating IS NOT NULL THEN 1 ELSE 0 END) \r\n            THEN 1 \r\n        ELSE 0 \r\n    END AS driver_rated_all_passengers \r\nFROM \r\n    demandeTrajet \r\n    INNER JOIN trajet ON demandeTrajet.trajet_id = trajet.trajet_id\r\n    INNER JOIN UtlisateurVoiture ON trajet.utilisateur_voiture_id = UtlisateurVoiture.utilisateur_voiture_id\r\n    LEFT JOIN Rating ON demandeTrajet.demandeur_id = Rating.UtilisateurId \r\n           AND UtlisateurVoiture.id_utilisateur = Rating.RatedByUtilisateurId \r\nWHERE \r\n    demandeTrajet.trajet_id = @trajetID\r\n";
            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                con.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@trajetID",TrajetID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            res = (reader.GetInt32(0));
                        }
                    }
                }
            }
            return res;
        }
        public class TrajetOffertsHomeScreenAdapter : BaseAdapter<CTrajetsOfferts>
        {
            List<CTrajetsOfferts> items;
            Activity context;
            public TrajetOffertsHomeScreenAdapter(Activity context, List<CTrajetsOfferts> items)
                : base()
            {
                this.context = context;
                this.items = items;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override CTrajetsOfferts this[int position]
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
                    view = context.LayoutInflater.Inflate(Resource.Layout.userTrajetsOfferts, null);
                view.FindViewById<TextView>(Resource.Id.UserFullNameTextV).Text = item.prenom + " " + item.nom;
                view.FindViewById<TextView>(Resource.Id.CarMarkModeleTextV).Text = item.marque + " | " + item.model;
                view.FindViewById<TextView>(Resource.Id.dateTrajetTextV).Text =item.trajetDate.ToString("yyyy-MM-dd");
                view.FindViewById<TextView>(Resource.Id.TempsTrajetTextV).Text =item.tempsTrajet.ToString(@"hh\:mm");
                view.FindViewById<TextView>(Resource.Id.sourceTextV).Text =item.source;
                view.FindViewById<TextView>(Resource.Id.destinationTextV).Text = item.destination;
                view.FindViewById<TextView>(Resource.Id.nbrPlaceTrajetTV).Text =" "+item.nombrePlace;
                view.FindViewById<TextView>(Resource.Id.prixTV).Text =item.prix+" DHS";
                view.FindViewById<CircleImageView>(Resource.Id.userImage).SetImageBitmap(item.imageUtilisateur);
                if (item.status=="NoUpdate")
                {
                    view.FindViewById<AppCompatButton>(Resource.Id.pickUpBtn).Visibility = ViewStates.Visible;
                    view.FindViewById<AppCompatButton>(Resource.Id.btnStartTrajet).Visibility = ViewStates.Gone;
                    view.FindViewById<AppCompatButton>(Resource.Id.btnTerminerTrajet).Visibility = ViewStates.Gone;
                    view.FindViewById<AppCompatButton>(Resource.Id.btnEvaluerPassagers).Visibility = ViewStates.Gone;
                    view.FindViewById<AppCompatButton>(Resource.Id.btnTrajetCompleted).Visibility = ViewStates.Gone;

                }

                else if (item.status == "Picked Up")
                {
                    view.FindViewById<AppCompatButton>(Resource.Id.btnStartTrajet).Visibility = ViewStates.Visible;
                    view.FindViewById<AppCompatButton>(Resource.Id.pickUpBtn).Visibility = ViewStates.Gone;
                    view.FindViewById<AppCompatButton>(Resource.Id.btnTerminerTrajet).Visibility = ViewStates.Gone;
                    view.FindViewById<AppCompatButton>(Resource.Id.btnEvaluerPassagers).Visibility = ViewStates.Gone;
                    view.FindViewById<AppCompatButton>(Resource.Id.btnTrajetCompleted).Visibility = ViewStates.Gone;

                }
                else if (item.status == "Started")
                {
                    view.FindViewById<AppCompatButton>(Resource.Id.btnTerminerTrajet).Visibility = ViewStates.Visible;
                    view.FindViewById<AppCompatButton>(Resource.Id.btnStartTrajet).Visibility = ViewStates.Gone;
                    view.FindViewById<AppCompatButton>(Resource.Id.pickUpBtn).Visibility = ViewStates.Gone;
                    view.FindViewById<AppCompatButton>(Resource.Id.btnEvaluerPassagers).Visibility = ViewStates.Gone;
                    view.FindViewById<AppCompatButton>(Resource.Id.btnTrajetCompleted).Visibility = ViewStates.Gone;

                }
                else if (item.status == "Completed")
                {
                    view.FindViewById<AppCompatButton>(Resource.Id.btnTerminerTrajet).Visibility = ViewStates.Gone;
                    view.FindViewById<AppCompatButton>(Resource.Id.btnStartTrajet).Visibility = ViewStates.Gone;
                    view.FindViewById<AppCompatButton>(Resource.Id.pickUpBtn).Visibility = ViewStates.Gone;
                    int res = GetRatingButton(item.TrajetID);
                    if (res==0)
                    {
                        view.FindViewById<AppCompatButton>(Resource.Id.btnEvaluerPassagers).Visibility = ViewStates.Visible;
                        view.FindViewById<AppCompatButton>(Resource.Id.btnTrajetCompleted).Visibility = ViewStates.Gone;

                    }
                    else
                    {
                        view.FindViewById<AppCompatButton>(Resource.Id.btnEvaluerPassagers).Visibility = ViewStates.Gone;
                        view.FindViewById<AppCompatButton>(Resource.Id.btnTrajetCompleted).Visibility = ViewStates.Visible;

                    }

                }


                view.FindViewById<AppCompatButton>(Resource.Id.pickUpBtn).Click += (sender, args) =>
                {
                    string sql = "update trajet set status='Picked Up' where trajet_id=@trajet_id";
                    using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand(sql, con))
                        {
                            command.Parameters.AddWithValue("@trajet_id",item.TrajetID);
                            command.ExecuteNonQuery();


                        }
                    }
                    view.FindViewById<AppCompatButton>(Resource.Id.btnTerminerTrajet).Visibility = ViewStates.Gone;
                    view.FindViewById<AppCompatButton>(Resource.Id.btnStartTrajet).Visibility = ViewStates.Visible;
                    view.FindViewById<AppCompatButton>(Resource.Id.pickUpBtn).Visibility = ViewStates.Gone;
                    view.FindViewById<AppCompatButton>(Resource.Id.btnEvaluerPassagers).Visibility = ViewStates.Gone;
                    Toast.MakeText(context, "Vous avez pris les Passagers.", ToastLength.Short).Show();

                };

                view.FindViewById<AppCompatButton>(Resource.Id.btnStartTrajet).Click += (sender, args) =>
                {
                    string sql = "update trajet set status='Started' where trajet_id=@trajet_id";
                    using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand(sql, con))
                        {
                            command.Parameters.AddWithValue("@trajet_id", item.TrajetID);
                            command.ExecuteNonQuery();


                        }
                    }
                    view.FindViewById<AppCompatButton>(Resource.Id.btnTerminerTrajet).Visibility = ViewStates.Visible;
                    view.FindViewById<AppCompatButton>(Resource.Id.btnStartTrajet).Visibility = ViewStates.Gone;
                    view.FindViewById<AppCompatButton>(Resource.Id.pickUpBtn).Visibility = ViewStates.Gone;
                    view.FindViewById<AppCompatButton>(Resource.Id.btnEvaluerPassagers).Visibility = ViewStates.Gone;
                    Toast.MakeText(context, "Votre trajet est Commencé.", ToastLength.Short).Show();

                };

                view.FindViewById<AppCompatButton>(Resource.Id.btnEvaluerPassagers).Click += (sender, args) =>
                {
                    Intent intent = new Intent(view.Context, typeof(RatePassagerList));
                    intent.PutExtra("trajetId", item.TrajetID);
                    intent.PutExtra("UtilisateurId", MainActivity.userId);
                    view.Context.StartActivity(intent);

                };

                    view.FindViewById<AppCompatButton>(Resource.Id.btnTerminerTrajet).Click += (sender, args) =>
                 {
                     string sql = "update trajet set status='Completed' where trajet_id=@trajet_id";
                     using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
                     {
                         con.Open();
                         using (SqlCommand command = new SqlCommand(sql, con))
                         {
                             command.Parameters.AddWithValue("@trajet_id", item.TrajetID);
                             command.ExecuteNonQuery();


                         }
                     }
                     view.FindViewById<AppCompatButton>(Resource.Id.btnTerminerTrajet).Visibility = ViewStates.Gone;
                     view.FindViewById<AppCompatButton>(Resource.Id.btnStartTrajet).Visibility = ViewStates.Gone;
                     view.FindViewById<AppCompatButton>(Resource.Id.pickUpBtn).Visibility = ViewStates.Gone;


                     view.FindViewById<AppCompatButton>(Resource.Id.btnEvaluerPassagers).Visibility = ViewStates.Visible;
                     Toast.MakeText(context, "Votre trajet est Terminé.", ToastLength.Short).Show();

                 };


                view.Click += (sender, args) =>
                {
                    Intent intent = new Intent(view.Context, typeof(trajetOffertsDetails));
                    intent.PutExtra("trajetId", item.TrajetID);
                    intent.PutExtra("UtilisateurId", Utilisateur_id);
                    view.Context.StartActivity(intent);

                };

                return view;
            }
        }

       












        public class CTrajetsOfferts
        {
            public string nom;
            public string prenom;
            public Bitmap imageUtilisateur;
            public int TrajetID;
            public string source;
            public string destination;
            public DateTime trajetDate;
            public TimeSpan tempsTrajet;
            public double prix;
            public int nombrePlace;
            public string status;
            public string marque;
            public string model;
            public CTrajetsOfferts(string nom, string prenom,
                Bitmap imageUtilisateur, int TrajetID, string source, string destination, DateTime trajetDate,
                TimeSpan tempsTrajet, double prix, int nombrePlace, string status, string marque, string model)
            {
                this.nom = nom;
                this.prenom = prenom;
                this.imageUtilisateur = imageUtilisateur;
                this.TrajetID = TrajetID;
                this.source = source;
                this.destination = destination;
                this.trajetDate = trajetDate;
                this.tempsTrajet = tempsTrajet;
                this.prix = prix;
                this.nombrePlace = nombrePlace;
                this.status = status;
                this.marque = marque;
                this.model = model;
            }

        }




       

    }
}
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
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class TrajetsListViewRecherche : Activity
    {
        string connection =MainActivity.connectionString;
        List<demande> tabletrajets = new List<demande>();

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource

            SetContentView(Resource.Layout.TrajetsListViewRecherche);
            ImageView im1 = FindViewById<ImageView>(Resource.Id.imageView1);
            string date = Intent.GetStringExtra("date");
            string depart = Intent.GetStringExtra("depart");
            string arriver = Intent.GetStringExtra("arriver");

            GetTrajets(date, depart, arriver);
            im1.Click += delegate
            {
                StartActivity(typeof(Home));
            };
            ListView listeTrajets = FindViewById<ListView>(Resource.Id.listView2);

            listeTrajets.Adapter = new HomeScreenAdapter(this, tabletrajets);


            if (tabletrajets.Count == 0)
            {
                getEmptyUserTrajets(listeTrajets);
                LinearLayout ly = FindViewById<LinearLayout>(Resource.Id.EmptyTrajets);
                ly.Visibility = ViewStates.Visible;

            }
            else
            {
                LinearLayout ly = FindViewById<LinearLayout>(Resource.Id.EmptyTrajets);
                ly.Visibility = ViewStates.Invisible;
                listeTrajets.Visibility = ViewStates.Visible;
                listeTrajets.Adapter = new HomeScreenAdapter(this, tabletrajets);


            }

        }
        public void getEmptyUserTrajets(ListView l)
        {
            l.Visibility = ViewStates.Invisible;


        }

        /*void OnListClickAccep(object sender ,AdapterView.ItemClickEventArgs e)
        {
            var listeTrajets = sender as ListView;
          //  var t = tabletrajets[e.Position];
            Toast.MakeText(this, "verifier votre information", ToastLength.Short).Show();

            //   Android.Widget.Toast.MakeText(this, t.nom, Android.Widget.ToastLength.Short).Show();
        }*/
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        internal class demande
        {
            public string nom;
            public string prenom;
            public Bitmap imageUtilisateur;

            public string marque;
            public string model;
            public string source;
            public string destination;
            public double prix;
            public DateTime trajetDate;
            public int nombrePlace;
            public int DriverID;
            public int TrajetID;
            public demande(string nom, string prenom,
                Bitmap imageUtilisateur, string marque,
                string model, string source, string destination, double prix, DateTime trajetDate, int nombrePlace, int DriverID, int TrajetID)
            {
                this.nom = nom;
                this.prenom = prenom;
                this.imageUtilisateur = imageUtilisateur;
                this.marque = marque;
                this.model = model;
                this.source = source;
                this.destination = destination;
                this.prix = prix;
                this.trajetDate = trajetDate;
                this.nombrePlace = nombrePlace;
                this.DriverID = DriverID;
                this.TrajetID = TrajetID;
            }

        }
        internal class HomeScreenAdapter : BaseAdapter<demande>
        {
            List<demande> items;
            Activity context;
            private Bitmap imageUtilisateur;

            public HomeScreenAdapter(Activity context, List<demande> items)
                : base()
            {
                this.context = context;
                this.items = items;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override demande this[int position]
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
                    view = context.LayoutInflater.Inflate(Resource.Layout.listeRecherche, null);
                string fullname = item.prenom + " " + item.nom;
                string MaeMod = item.marque + "|" + item.model;
                DateTime dt = item.trajetDate;
                string myDateString = dt.ToString("yyyy-MM-dd");

                view.FindViewById<TextView>(Resource.Id.UserFullNameTextV).Text = fullname;
                view.FindViewById<CircleImageView>(Resource.Id.userImage).SetImageBitmap(item.imageUtilisateur);
                view.FindViewById<TextView>(Resource.Id.CarMarkModeleTextV).Text = MaeMod;
                view.FindViewById<TextView>(Resource.Id.sourceTextV).Text = item.source;
                view.FindViewById<TextView>(Resource.Id.destinationTextV).Text = item.destination;

                view.FindViewById<TextView>(Resource.Id.dateTrajetTextV).Text = myDateString;
                view.FindViewById<TextView>(Resource.Id.TrajetPriceTextV).Text = item.prix + " DHS";
                view.FindViewById<TextView>(Resource.Id.NbrSeatsTextV).Text = "(" + item.nombrePlace + " Place(s))";

                view.FindViewById<AppCompatButton>(Resource.Id.btnaccepted).Click += (sender, args) =>
                {
                    Intent intent = new Intent(view.Context, typeof(trajetDetailsReserve));
                    intent.PutExtra("trajetId", item.TrajetID);
                    intent.PutExtra("DriverID", item.DriverID);
                    view.Context.StartActivity(intent);
                };

                return view;
            }
        }
        public void GetTrajets(string date, string depart, string arriver)
        {


            string sql = "select U.nom, U.prenom, U.imageUtilisateur, V.marque, V.Model, T.source, T.destination, T.prix, T.dateTrajet, T.nbrPlaces, UV.id_utilisateur, T.trajet_id " +
                                 "from Utilisateur U, trajet T, UtlisateurVoiture UV, Voiture V " +
                                 "where source = @depart and destination = @arriver and dateTrajet = @date and UV.voiture_id = V.voiture_id and UV.utilisateur_voiture_id = T.utilisateur_voiture_id and UV.id_utilisateur = U.id_utilisateur";

            using (SqlConnection con = new SqlConnection(connection))
            {
                con.Open();

                using (SqlCommand command = new SqlCommand(sql, con))
                {
                    command.Parameters.AddWithValue("@depart", depart);
                    command.Parameters.AddWithValue("@arriver", arriver);
                    command.Parameters.AddWithValue("@date", date);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            byte[] imageData = (byte[])reader["imageUtilisateur"];

                            // Convert the image data to a bitmap 
                            Bitmap userpic = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);


                            tabletrajets.Add(new demande(
                            reader.GetString(0),
                            reader.GetString(1),
                            userpic,

                            reader.GetString(3),
                            reader.GetString(4),
                            reader.GetString(5),
                            reader.GetString(6),
                            reader.GetDouble(7),
                            reader.GetDateTime(8),
                            reader.GetInt32(9),
                            reader.GetInt32(10),
                            reader.GetInt32(11)
                            ));

                        }
                    }
                }

                con.Close();


            }


        }



    }
}
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.FloatingActionButton;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using static Java.Util.Jar.Attributes;
using System.Drawing;
using Android.Graphics;
using Bitmap = Android.Graphics.Bitmap;
using System.IO;
using System.Globalization;
using Java.Lang;
using Android.Graphics.Drawables;



namespace Covoituragee
{
    [Activity(Label = "ProfileActivity")]
    public class ProfileActivity : Activity
    {
        private ImageView userImage;
        private FloatingActionButton chooseProfilePictureButton;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set the layout for this activity
            SetContentView(Resource.Layout.Profile);




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




            //MainActivity.userId = Intent.GetIntExtra("id_utilisateur", 0); // replace 0 with the default value if the extra is not found

            // Get references to the user image and choose profile picture button
            userImage = FindViewById<ImageView>(Resource.Id.user_image);
            chooseProfilePictureButton = FindViewById<FloatingActionButton>(Resource.Id.choose_profile_picture_button);

            // Attach click event handler for the choose profile picture button
            chooseProfilePictureButton.Click += ChooseProfilePictureButton_Click;

            // Get reference to the log out ImageView
            ImageView logOutImageView = FindViewById<ImageView>(Resource.Id.log_out);

            // Attach click event handler for the log out ImageView
            logOutImageView.Click += delegate {
                // Create a dialog box
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetMessage("vous voulez vous déconnecter ?");
                builder.SetCancelable(false);

                // Add the Yes button
                builder.SetPositiveButton("Oui", delegate {
                    // Handle the user's decision to log out
                    // e.g. clear user session, navigate to login page, etc.
                    Intent intent = new Intent(this, typeof(LoginActivity));
                    StartActivity(intent);
                    Finish(); // Optional: finish the current activity so the user can't navigate back to it using the back button
                });

                // Add the No button
                builder.SetNegativeButton("Non", delegate {
                    // Dismiss the dialog box and continue with the app
                });

                // Show the dialog box
                AlertDialog alert = builder.Create();
                alert.Show();
            };

            AppCompatButton changeInfosButton = FindViewById<AppCompatButton>(Resource.Id.btn_change_infos);
            changeInfosButton.Click += (sender, e) => {
                Intent intent = new Intent(this, typeof(ChangeUserInfosActivity));
                intent.PutExtra("id_utilisateur", MainActivity.userId); // pass the user ID to the ChangeUserInfosActivity
                StartActivity(intent);
            };
            TextView name = FindViewById<TextView>(Resource.Id.second_name);
            TextView lastname = FindViewById<TextView>(Resource.Id.second_lastname);
            TextView phonenumber = FindViewById<TextView>(Resource.Id.second_phonenumber);
            TextView email = FindViewById<TextView>(Resource.Id.second_email);
            TextView age = FindViewById<TextView>(Resource.Id.second_age);
            TextView sexe = FindViewById<TextView>(Resource.Id.second_sexe);
            TextView membresince = FindViewById<TextView>(Resource.Id.second_membersince);
            TextView ratingText = FindViewById<TextView>(Resource.Id.rating_text);


            int userID = MainActivity.userId;

            if (userID != -1)
            {
                // retrieve the user's information from the database using user ID
                string sql = $"SELECT nom, prenom, DateNaissance, NumTel, sexe, email, DateInscription, imageUtilisateur FROM Utilisateur WHERE id_utilisateur = {userID}";
                string ratingQuery = $"SELECT AVG(Rating) FROM Rating WHERE UtilisateurId = {userID}";

                using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
                {
                    con.Open();

                    using (SqlCommand command = new SqlCommand(sql, con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // display user's information
                                name.Text = reader.GetString(0);
                                lastname.Text = reader.GetString(1);
                                phonenumber.Text = reader.GetString(3);
                                email.Text = reader.GetString(5);
                                age.Text = reader.GetString(2);
                                sexe.Text = reader.GetString(4);
                                DateTime dateInscription = reader.GetDateTime(6);
                                CultureInfo culture = new CultureInfo("fr-FR"); // French culture
                                membresince.Text = dateInscription.ToString("dddd d MMMM yyyy", culture);

                                // display user's image
                                byte[] imageData = null;
                                object imageObject = reader["imageUtilisateur"];
                                if (imageObject != DBNull.Value)
                                {
                                    imageData = (byte[])imageObject;
                                }

                                if (imageData != null && imageData.Length > 0)
                                {
                                    Bitmap bmp = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
                                    userImage.SetImageBitmap(bmp);
                                }
                                else
                                {
                                    userImage.SetImageResource(Resource.Drawable.profile_image);
                                }

                            }
                        }
                    }
                    // Get the average rating for the user
                    using (SqlCommand ratingCommand = new SqlCommand(ratingQuery, con))
                    {
                        object result = ratingCommand.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            // double averageRating = Convert.ToDouble(result);
                            //ratingText.Text = averageRating.ToString("0.0");
                            var b = Convert.ToInt32(result);
                            ratingText.Text = b+"/5";
                        }
                    }
                    con.Close();
                }
            }
            else
            {
                // Handle the case where the user ID is not retrieved correctly
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetMessage("Invalid user ID");
                builder.SetPositiveButton("OK", (sender, args) =>
                {
                    // do something
                });
                AlertDialog alert = builder.Create();
                alert.Show();
            }

        }

        private void ChooseProfilePictureButton_Click(object sender, EventArgs e)
        {
            // Create an intent to pick an image from gallery
            Intent galleryIntent = new Intent(Intent.ActionPick);
            galleryIntent.SetType("image/*");
            galleryIntent.SetAction(Intent.ActionGetContent);

            // Create a chooser intent to show the gallery option
            Intent chooserIntent = Intent.CreateChooser(galleryIntent, "Select Picture");

            // Start the activity and wait for a result
            StartActivityForResult(chooserIntent, 1);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok && requestCode == 1)
            {
                // Get the selected image URI
                Android.Net.Uri imageUri = data.Data;

                // Convert the image to a byte array
                byte[] imageData = ConvertImageToByteArray(imageUri);

                // Update the user's image in the database
                UpdateUserImageInDatabase(imageData);
            }
        }


        private byte[] ConvertImageToByteArray(Android.Net.Uri uri)
        {
            // Load the image from the URI
            using (var inputStream = ContentResolver.OpenInputStream(uri))
            {
                Bitmap bmp = BitmapFactory.DecodeStream(inputStream);

                // Convert the image to a byte array
                using (MemoryStream stream = new MemoryStream())
                {
                    bmp.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                    return stream.ToArray();
                }
            }
        }


        private void UpdateUserImageInDatabase(byte[] imageData)
        {
            // If imageData is null, use the default image
            if (imageData == null)
            {
                BitmapDrawable drawable = (BitmapDrawable)GetDrawable(Resource.Drawable.profile_image);
                Bitmap bitmap = drawable.Bitmap;
                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                    imageData = stream.ToArray();
                }
            }

            // Define the update query
            string sql = $"UPDATE Utilisateur SET imageUtilisateur = @ImageData WHERE id_utilisateur = {MainActivity.userId}";

            // Update the user's image in the database
            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                con.Open();

                using (SqlCommand command = new SqlCommand(sql, con))
                {
                    command.Parameters.AddWithValue("@ImageData", imageData);
                    command.ExecuteNonQuery();
                }

                con.Close();
            }

            // Update the user's image in the UI
            Bitmap bmp = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            userImage.SetImageBitmap(bmp);
        }



    }
}
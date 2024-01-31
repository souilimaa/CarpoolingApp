using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Data.SqlClient;


namespace Covoituragee
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Login);
            EditText emailEditText = FindViewById<EditText>(Resource.Id.emailEditText);



            TextView textView = FindViewById<TextView>(Resource.Id.text_view2);

            // Set a click listener for the TextView
            textView.Click += TextView_Click;

            TextView textViewforgotpassword = FindViewById<TextView>(Resource.Id.text_view3);

            // Set a click listener for the TextView
            textViewforgotpassword.Click += async delegate
            {
                // Get the email entered by the user
                string email = emailEditText.Text;

                // Check if the email is empty
                if (string.IsNullOrEmpty(email))
                {
                    Toast.MakeText(this, "Le champ Email ne peut pas être vide", ToastLength.Short).Show();
                    return;
                }

                try
                {
                    using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
                    {
                        await con.OpenAsync();

                        using (SqlCommand cmd = new SqlCommand("SELECT * FROM Utilisateur WHERE email = @Email", con))
                        {
                            cmd.Parameters.AddWithValue("@Email", email);
                            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    // Email already exists in the database, start the ForgotPasswordActivity
                                    // Create a new intent to start the ChangePasswordActivity
                                    Intent changePasswordIntent = new Intent(this, typeof(ForgotPasswordActivity));

                                    // Pass the email entered by the user as an extra to the intent
                                    changePasswordIntent.PutExtra("email", email);

                                    // Start the ChangePasswordActivity
                                    StartActivity(changePasswordIntent);
                                }
                                else
                                {
                                    // Email does not exist in the database, show an error message to the user
                                    Toast.MakeText(this, "L'adresse électronique n'existe pas", ToastLength.Short).Show();
                                }
                            }
                        }

                        await con.CloseAsync();
                    }
                }
                catch (Exception ex)
                {
                    // Handle any database exceptions
                    Toast.MakeText(this, "Une erreur s'est produite: " + ex.Message, ToastLength.Short).Show();
                }
            };


            EditText passwordEditText = FindViewById<EditText>(Resource.Id.passwordEditText);

            Button loginButton = FindViewById<Button>(Resource.Id.btn_connecter);
            loginButton.Click += async (sender, e) =>
            {
                string email = emailEditText.Text;
                string password = passwordEditText.Text;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    // Fields cannot be empty, show an error message to the user
                    Toast.MakeText(this, "Les champs Email et Mot de passe ne peuvent pas être vides", ToastLength.Short).Show();
                    return;
                }

                try
                {
                    using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
                    {
                        await con.OpenAsync();

                        using (SqlCommand cmd = new SqlCommand("SELECT * FROM Utilisateur WHERE email = @Email", con))
                        {
                            cmd.Parameters.AddWithValue("@Email", email);
                            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    int userID = reader.GetInt32(reader.GetOrdinal("id_utilisateur"));
                                    MainActivity.userId = userID;
                                    string storedPassword = reader.GetString(reader.GetOrdinal("MotPasse"));
                                    if (password == storedPassword)
                                    {
                                        // Passwords match, login successful

                                        // Store the user ID in a shared preference
                                     //   ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                                       // ISharedPreferencesEditor editor = prefs.Edit();
                                        //editor.PutInt("id_utilisateur", userID);
                                        //editor.Apply();
                                        // Pass the user ID to the next activity
                                        Intent intent = new Intent(this, typeof(Home));
                                        StartActivity(intent);
                                    }
                                    else
                                    {
                                        // Passwords don't match, login failed
                                        // Show an error message to the user
                                        Toast.MakeText(this, "Mot de passe incorrect", ToastLength.Short).Show();
                                    }
                                }
                                else
                                {
                                    // User not found, login failed
                                    // Show an error message to the user
                                    Toast.MakeText(this, "Utilisateur non trouvé", ToastLength.Short).Show();
                                }
                            }
                        }

                        await con.CloseAsync();
                    }
                }
                catch (Exception ex)
                {
                    // Handle any database exceptions
                    Toast.MakeText(this, "Une erreur s'est produite: " + ex.Message, ToastLength.Short).Show();
                }
            };
        }

        private void TextView_Click(object sender, EventArgs e)
        {
            // Create a new Intent to start the RegistrationActivity
            Intent intent = new Intent(this, typeof(SignUpActivity));

            // Start the new activity
            StartActivity(intent);
        }
    }
}
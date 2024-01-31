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
using System.Text;
using System.Text.RegularExpressions;

namespace Covoituragee
{
    [Activity(Label = "ForgotPasswordActivity")]
    public class ForgotPasswordActivity : Activity
    {
        Button nextButton;
        TextView newPasswordTextView;
        EditText newPasswordEditText;
        TextView confirmPasswordTextView;
        EditText confirmPasswordEditText;
        Button changePasswordButton;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.forgotPassword);

            nextButton = FindViewById<Button>(Resource.Id.button_next);
            newPasswordTextView = FindViewById<TextView>(Resource.Id.text_view_new_password);
            newPasswordEditText = FindViewById<EditText>(Resource.Id.edit_text_new_password);
            confirmPasswordTextView = FindViewById<TextView>(Resource.Id.text_view_confirm_password);
            confirmPasswordEditText = FindViewById<EditText>(Resource.Id.edit_text_confirm_password);
            changePasswordButton = FindViewById<Button>(Resource.Id.button_change_password);
            EditText cinEditText = FindViewById<EditText>(Resource.Id.edit_text_cin);


            // Hide the views that should not be visible initially
            newPasswordTextView.Visibility = ViewStates.Gone;
            newPasswordEditText.Visibility = ViewStates.Gone;
            confirmPasswordTextView.Visibility = ViewStates.Gone;
            confirmPasswordEditText.Visibility = ViewStates.Gone;
            changePasswordButton.Visibility = ViewStates.Gone;


            Button backArrowImageView = FindViewById<Button>(Resource.Id.farrow);

            backArrowImageView.Click += (sender, e) =>
            {
               

                // Get the email entered by the user
                string email = Intent.GetStringExtra("email");

                // Start the LoginActivity intent with the email field as an extra
                Intent loginIntent = new Intent(this, typeof(LoginActivity));
                loginIntent.PutExtra("email", email);
                StartActivity(loginIntent);

                // Finish the current activity
                Finish();
            };


            nextButton.Click += async delegate {
                // Get the email entered by the user
                string email = Intent.GetStringExtra("email");

                // Get the CIN entered by the user
                string cin = cinEditText.Text;

                // Check if the CIN is empty
                if (string.IsNullOrEmpty(cin))
                {
                    Toast.MakeText(this, "Le champ CIN ne peut pas être vide", ToastLength.Short).Show();
                    return;
                }

                try
                {
                    using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
                    {
                        await con.OpenAsync();

                        using (SqlCommand cmd = new SqlCommand("SELECT * FROM Utilisateur WHERE email = @Email AND cin = @CIN", con))
                        {
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@CIN", cin);
                            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    // CIN is valid, show the views that should be visible
                                    newPasswordTextView.Visibility = ViewStates.Visible;
                                    newPasswordEditText.Visibility = ViewStates.Visible;
                                    confirmPasswordTextView.Visibility = ViewStates.Visible;
                                    confirmPasswordEditText.Visibility = ViewStates.Visible;
                                    changePasswordButton.Visibility = ViewStates.Visible;
                                }
                                else
                                {
                                    // CIN is invalid, show an error message to the user
                                    Toast.MakeText(this, "CIN non valide", ToastLength.Short).Show();
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
            // Set a click listener for the changePasswordButton
            // Set a click listener for the changePasswordButton
            changePasswordButton.Click += async delegate
            {
                // Get the email entered by the user
                string email = Intent.GetStringExtra("email");

                // Get the new password entered by the user
                string newPassword = newPasswordEditText.Text;

                // Get the confirm password entered by the user
                string confirmPassword = confirmPasswordEditText.Text;

                // Validate that the new password and confirm password are not empty or null
                if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
                {
                    Toast.MakeText(this, "Les champs Nouveau mot de passe et Confirmer le mot de passe ne peuvent pas être vides", ToastLength.Short).Show();
                    return;
                }

                // Validate that the new password and confirm password match
                if (newPassword != confirmPassword)
                {
                    Toast.MakeText(this, "Les mots de passe ne correspondent pas", ToastLength.Short).Show();
                    return;
                }

                // Validate the new password format
                if (newPassword.Length < 8 ||
                    !Regex.IsMatch(newPassword, @"[a-zA-Z]") ||
                    !Regex.IsMatch(newPassword, @"\d") ||
                    !Regex.IsMatch(newPassword, @"[\W_]"))
                {
                    // display error message if password is invalid
                    Toast.MakeText(this, "Le mot de passe doit comporter au moins 8 caractères, des lettres et des chiffres, et au moins un caractère spécial.", ToastLength.Short).Show();
                    return;
                }

                try
                {
                    using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
                    {
                        await con.OpenAsync();

                        using (SqlCommand cmd = new SqlCommand("UPDATE Utilisateur SET MotPasse = @NewPassword WHERE email = @Email", con))
                        {
                            cmd.Parameters.AddWithValue("@NewPassword", newPassword);
                            cmd.Parameters.AddWithValue("@Email", email);
                            int rowsAffected = await cmd.ExecuteNonQueryAsync();

                            if (rowsAffected > 0)
                            {
                                // Password updated successfully, go back to the LoginActivity
                                Toast.MakeText(this, "Mise à jour réussie du mot de passe", ToastLength.Short).Show();
                                StartActivity(typeof(LoginActivity));
                            }
                            else
                            {
                                // Something went wrong, show an error message to the user
                                Toast.MakeText(this, "Une erreur s'est produite, le mot de passe n'a pas été mis à jour", ToastLength.Short).Show();
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
    }
}

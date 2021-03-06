﻿using System;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using InsuranceCompanyWebApp.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace InsuranceCompanyWebApp.Account
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterHyperLink.NavigateUrl = "Register";
            // Enable this once you have account confirmation enabled for password reset functionality
            //ForgotPasswordHyperLink.NavigateUrl = "Forgot";
            var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            if (!String.IsNullOrEmpty(returnUrl))
            {
                RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
            }
        }

        protected void LogIn(object sender, EventArgs e)
        {
            if (IsValid)
            {
                // Validate the user password
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var signinManager = Context.GetOwinContext().GetUserManager<ApplicationSignInManager>();

                // This doen't count login failures towards account lockout
                // To enable password failures to trigger lockout, change to shouldLockout: true
                var result = signinManager.PasswordSignIn(Email.Text, Password.Text, RememberMe.Checked, shouldLockout: false);
               // String a="";
                switch (result)
                {
                    
                    case SignInStatus.Success:

                        //SqlConnection connection = new SqlConnection();
                        //connection.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                        //string sqlString = "SELECT admin FROM AspNetUsers WHERE Email = @email";
                        //SqlCommand command = new SqlCommand(sqlString, connection);
                        //command.Parameters.AddWithValue("@email", Email.Text);

                       // SqlDataAdapter adapter = new SqlDataAdapter(command);
                        //DataSet ds = new DataSet();

                        //try
                        //{
                        //    connection.Open();
                        //    SqlDataReader reader = command.ExecuteReader();
                        //    if (reader.Read()) {
                        //        a = reader["admin"].ToString();
                        //    }
                           
                            
                        //}
                        //catch (Exception er) {  }
                        //finally
                        //{
                        //    connection.Close();
                        //}

                       // if (a == "True")
                        IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                       // else Response.Redirect("/About.aspx");
                        break;
                    case SignInStatus.LockedOut:
                        Response.Redirect("/Account/Lockout");
                        break;
                    case SignInStatus.RequiresVerification:
                        Response.Redirect(String.Format("/Account/TwoFactorAuthenticationSignIn?ReturnUrl={0}&RememberMe={1}",
                                                        Request.QueryString["ReturnUrl"],
                                                        RememberMe.Checked),
                                          true);
                        break;
                    case SignInStatus.Failure:
                    default:
                        FailureText.Text = "Invalid login attempt";
                        ErrorMessage.Visible = true;
                        break;
                }
            }
        }
    }
     
}
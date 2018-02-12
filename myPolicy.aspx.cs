﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Microsoft.AspNet.Identity;

namespace InsuranceCompanyWebApp
{
    public partial class myPolicy : System.Web.UI.Page
    {
        private String UserID;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (this.User != null && this.User.Identity.IsAuthenticated)
            {
                UserID = HttpContext.Current.User.Identity.GetUserId();
            }

            SqlConnection connection = new SqlConnection();
            connection.ConnectionString=ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlStringImot = "Select polisi.br_polisa, polisi.datum_izdavanje, imoti.tip_imot, imoti.vrednost, lokacii.grad, lokacii.ulica, lokacii.broj "
                + "FROM polisi, osiguruvanje_imot, imoti, lokacii "
                + "WHERE polisi.br_polisa = osiguruvanje_imot.br_polisa and osiguruvanje_imot.imot_id = imoti.imot_id "
                + "and imoti.lokacija = lokacii.lokacija_id AND baratel = @user_id ";
            string sqlStringVozilo = "Select polisi.br_polisa, polisi.datum_izdavanje, "
                + "vozila.seriski_br, vozila.registracija, vozila.godina, vozila.model, vozila.proizvoditel, vozila.br_vrati "
                + "FROM polisi, osiguruvanje_vozilo, vozila WHERE polisi.br_polisa = osiguruvanje_vozilo.br_polisa "
                + "and osiguruvanje_vozilo.seriski_br = vozila.seriski_br AND baratel = @user_id";


            SqlCommand commandImot = new SqlCommand(sqlStringImot, connection);
            commandImot.Parameters.AddWithValue("@user_id", UserID);

            SqlDataAdapter adapterImot = new SqlDataAdapter(commandImot);

            SqlCommand commandVozilo = new SqlCommand(sqlStringVozilo, connection);
            commandVozilo.Parameters.AddWithValue("@user_id", UserID);

            SqlDataAdapter adapterVozilo = new SqlDataAdapter(commandVozilo);

            DataSet dsI = new DataSet();
            DataSet dsV = new DataSet();

            try
            {
                connection.Open();

                adapterImot.Fill(dsI,"Imoti");
                GridView1.DataSource= dsI;
                GridView1.DataBind();

                adapterVozilo.Fill(dsV,"Vozila");
                GridView2.DataSource = dsV;
                GridView2.DataBind();

            }
            catch(Exception ex)
            {
                Label1.Text = ex.Message;
            }
            finally
            {
                connection.Close();
            }

        
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = GridView1.SelectedRow;
            string tip = row.Cells[0].Text;
            string br_polisa = GridView1.DataKeys[GridView1.SelectedIndex].Value.ToString();
            IspolniDetali(tip, br_polisa);
        }

        protected void IspolniDetali(string tip, string br_polisa)
        {

            SqlConnection connection2 = new SqlConnection();
            connection2.ConnectionString = ConfigurationManager.ConnectionStrings["defaultconnection"].ConnectionString;

            SqlCommand command = new SqlCommand();
            //string sqlStringDetali = "Select br_polisa From polisi Where br_polisa = @br_polisa";

            if (tip == "Куќа")
            {
                string sqlStringKukja = "Select br_spratovi as 'Број на Спратови' From polisi, osiguruvanje_imot, imoti, kukji "
                + "Where polisi.br_polisa = osiguruvanje_imot.br_polisa and osiguruvanje_imot.imot_id = imoti.imot_id and imoti.imot_id = kukji.imot_id "
                + "and polisi.br_polisa = @br_polisa AND baratel = @user_id";

                command.Connection = connection2;
                command.CommandText = sqlStringKukja;
                command.Parameters.AddWithValue("@br_polisa", br_polisa);
                command.Parameters.AddWithValue("@user_id", UserID);

            }
            else
            {
                string sqlStringStan = "Select vlez as Влез, sprat as Спрат, br_stan as 'Број на Стан' From polisi, osiguruvanje_imot, imoti, stanovi "
                + "Where polisi.br_polisa = osiguruvanje_imot.br_polisa and osiguruvanje_imot.imot_id = imoti.imot_id and imoti.imot_id = stanovi.imot_id "
                + "and polisi.br_polisa = @br_polisa AND baratel = @user_id";

                command.Connection = connection2;
                command.CommandText = sqlStringStan;
                command.Parameters.AddWithValue("@br_polisa", br_polisa);
                command.Parameters.AddWithValue("@user_id", UserID);
            
            }


                SqlDataAdapter adapterTip = new SqlDataAdapter(command);
                DataSet dsTip = new DataSet();

                try
                {
                    connection2.Open();

                    adapterTip.Fill(dsTip, "Tipovi");
                    GridView3.DataSource = dsTip;
                    GridView3.DataBind();
                }
                catch (Exception ex)
                {
                    Label1.Text = ex.Message;
                }
                finally
                {
                    connection2.Close();
                }

            }

        protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            String d = e.Row.Cells[1].Text.ToString();
            String[]  sub = d.Split(' ');
            if (d!="Датум на издавање")
            e.Row.Cells[1].Text = sub[0];

            if (e.Row.RowIndex > -1)
            {

                SqlConnection con = new SqlConnection();
                con.ConnectionString = ConfigurationManager.ConnectionStrings["defaultconnection"].ConnectionString;

                SqlCommand checkPlateno = new SqlCommand();
                checkPlateno.CommandType = System.Data.CommandType.Text;
                checkPlateno.Connection = con;
                checkPlateno.CommandText = "SELECT SUM(iznos) plateno FROM transakcii WHERE br_polisa = @br_polisa";
                checkPlateno.Parameters.AddWithValue("@br_polisa", e.Row.Cells[0].Text.ToString());

                int plateno = 0;

                try
                {
                    con.Open();
                    SqlDataReader reader = checkPlateno.ExecuteReader();
                    if (reader.Read())
                    {
                        int.TryParse(reader["plateno"].ToString(), out plateno);
                    }
                }
                catch (Exception er) { Label1.Text = er.ToString(); }
                finally { con.Close(); }

                int god=2018 ;
                int.TryParse(e.Row.Cells[4].Text, out god);
                if (god*3 - plateno < 1)
                {

                    ((LinkButton)e.Row.FindControl("btnPlateno1")).Text = "✔";

                    ((LinkButton)e.Row.FindControl("btnPlateno1")).Style.Add("color", "green");

                }
                else
                {
                    ((LinkButton)e.Row.FindControl("btnPlateno1")).Text = "✘";
                    ((LinkButton)e.Row.FindControl("btnPlateno1")).Style.Add("color", "red");
                }
            }

       }


        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            String d = e.Row.Cells[3].Text.ToString();
            String[] sub = d.Split(' ');
            if (d!="Датум на издавање")
            e.Row.Cells[3].Text = sub[0];

            String tip = e.Row.Cells[0].Text.ToString();
            if (tip == "True") {
                e.Row.Cells[0].Text = "Куќа";

            }
            else if (tip == "False") { e.Row.Cells[0].Text = "Стан"; }

            if (e.Row.RowIndex > -1)
            {

                SqlConnection con = new SqlConnection();
                con.ConnectionString = ConfigurationManager.ConnectionStrings["defaultconnection"].ConnectionString;

                SqlCommand checkPlateno = new SqlCommand();
                checkPlateno.CommandType = System.Data.CommandType.Text;
                checkPlateno.Connection = con;
                checkPlateno.CommandText = "SELECT SUM(iznos) plateno FROM transakcii WHERE br_polisa = @br_polisa";
                checkPlateno.Parameters.AddWithValue("@br_polisa", GridView1.DataKeys[e.Row.RowIndex].Value.ToString());

                int plateno = 0;

                try
                {
                    con.Open();
                    SqlDataReader reader = checkPlateno.ExecuteReader();
                    if (reader.Read())
                    {
                        int.TryParse(reader["plateno"].ToString(),out plateno);
                    }
                }
                catch (Exception er) { Label1.Text = er.ToString(); }
                finally { con.Close(); }

                int vrednost;
                int.TryParse(e.Row.Cells[2].Text, out vrednost);
                if (vrednost / 100 - plateno < 1)
                {

                  ((LinkButton)e.Row.FindControl("btnPlateno")).Text = "✔";

                    ((LinkButton)e.Row.FindControl("btnPlateno")).Style.Add("color", "green");
                   
                }
                else
                {
                    ((LinkButton)e.Row.FindControl("btnPlateno")).Text = "✘";
                    ((LinkButton)e.Row.FindControl("btnPlateno")).Style.Add("color", "red");
                }
            }
        }

        protected void btnPlateno_Click(object sender, EventArgs e)
        {

            LinkButton btn = sender as LinkButton;
            Session["polisa_id"] = btn.CommandArgument;
            Response.Redirect("/Transactions.aspx");
        }

        protected void btnPlateno1_Click(object sender, EventArgs e)
        {

            LinkButton btn = sender as LinkButton;
            Session["polisa_id"] = btn.CommandArgument;
            Response.Redirect("/Transactions.aspx");
        }
    }

}
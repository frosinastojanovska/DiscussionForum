﻿using Dapper;
using DiscussionForum.Domain.DomainModel;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace DiscussionForum.Site
{
    public partial class Confirmation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["code"] != null)
                {
                    SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnection"].ToString());

                    string sql = $"SELECT * FROM Users WHERE ConfirmationCode='{Request.QueryString["code"]}'";
                    var user = connection.Query<User>(sql).FirstOrDefault();

                    if (user != null)
                    {
                        user.Confirmed = true;
                        string updateQuery = $"UPDATE Users SET Confirmed = '{user.Confirmed}' WHERE ID='{user.ID}'";
                        connection.Execute(updateQuery);
                        lblText.Text = "<h3>Your account is confirmed!</h3>";
                    }
                    else
                    {
                        lblText.Text = "<h3>User with that confirmation link does not exists!</h3>";
                    }
                }
            }
        }
    }
}
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Crawler
{


    class DBmanager
    {
        public DBmanager()
        {
            conn = new MySqlConnection("server=localhost;user=root;database=crawler;port=3306;");
            

        }
        MySqlConnection conn;
        public void insertLinks(string link , string content)
        {
            if (conn.State != ConnectionState.Open)
            conn.Open();

            string sql = "INSERT INTO myLinks (url, content)" +
                "VALUES ( @url , @content ) ";
            MySqlCommand cmd = new MySqlCommand(sql, conn);


            cmd.CommandType =  CommandType.Text;
            cmd.Parameters.AddWithValue("@url", link);
            cmd.Parameters.AddWithValue("@content", content); 
            //cmd. += onStatementCompleted;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteScalar();
            
            cmd.Dispose();
            conn.Close();
        }
        List<String> links = new List<string>();

     public   void selectAllLinks()
        {
            conn.Open();
            string sql = "SELECT * from mylinks ;";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            var cursor = cmd.ExecuteReader();

            while (cursor.Read())
            {
                links.Add(cursor.GetString(1));
                Console.WriteLine(cursor.GetString(1));
            }
            cmd.Dispose();
            conn.Close();
        }
        private void onStatementCompleted(object sender, StatementCompletedEventArgs e)
        {
            MessageBox.Show("InsertedSuc");
           
        }
    }

    public partial class DBform : Form
    {
        public DBform()
        {
            InitializeComponent();

        }

        DBmanager db = new DBmanager(); 

       

        private void ConnectToDB()
        { 
 
           // SqlDataReader sread = new SqlDataReader();
           // conn.Open();
          //  string sql = "CREATE DATABASE SavedLinks";
          //  SqlCommand cmd = new SqlCommand(sql, conn);
         //   cmd.ExecuteNonQuery();
 
      
        }


        private void button1_Click(object sender, EventArgs e)
        {
       
        }

        private void button2_Click(object sender, EventArgs e)
        {
         db.   selectAllLinks();
        }

        private void button3_Click(object sender, EventArgs e)
        {
          db.  insertLinks("ASDASDFASdeterwt","");
        }

    }
}

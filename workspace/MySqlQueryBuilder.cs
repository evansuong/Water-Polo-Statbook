using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace Water_Polo_Statbook

{
    /// <summary>
    /// handles are mysql queries and returns data based on queries passed in
    /// </summary>
    public class MySqlQueryBuilder
    {

        // mysql connection reference
        MySqlConnection con;

        public MySqlQueryBuilder(MySqlConnection con)
        {
            this.con = con;
        }

        /// <summary>
        /// Executed query and fills datatable with data from database
        /// </summary>
        /// <param name="qry">query to be executed</param>
        /// <returns>filled datatable</returns>
        public DataTable Execute_DataTable_Qry(string qry)
        {
            try
            {
                con.Open();
                MySqlDataAdapter sda = new MySqlDataAdapter(qry, con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return null;
        }

        /// <summary>
        /// executes a query to fill a dataset 
        /// </summary>
        /// <param name="qry">query to be executed</param>
        /// <returns>filled dataset from waterpolo_db</returns>
        public DataSet Execute_DataSet_Query(string qry)
        {
            try
            {
                con.Open();
                MySqlDataAdapter sda = new MySqlDataAdapter(qry, con);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                con.Close();
                return ds;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            if (con.State == ConnectionState.Open)
                con.Close();
            return null;
        }


        /// <summary>
        /// executes a specified query passed in
        /// </summary>
        /// <param name="qry">query to be executed</param>
        public void Execute_Query(string qry)
        {
            try
            {
                con.Open();
                MySqlCommand msc = new MySqlCommand(qry, con);
                msc.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            if (con.State == ConnectionState.Open)
                con.Close();
        }
    }
}

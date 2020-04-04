using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Water_Polo_Statbook
{


    /// <summary>
    /// Interaction logic for AddTeamWindow.xaml
    /// 
    /// Window to edit what teams the database holds including
    ///     - add new team
    ///     - delete team
    ///     - update team info
    /// </summary>
    public partial class EditTeamWindow : Window
    {

        // SQL objects
        MySqlConnection con;

        // reference to calling window
        private Window callingWindow;

        // reference to team object
        private MyTeam myTeam;

        // reference to player object
        private MyPlayer myPlayer;

        // message constants
        private const string SELECT_PLAYER_MSG = "Please select a player";
        private const string STRING_FORMAT_MSG = "Please ensure that {0} is a number";
        private const string DISABLE_FOREIGN_KEY_QRY = "set FOREIGN_KEY_CHECKS = 0";
        private const string UPDATE_PLAYER_QRY = "update player set player_num={0}, player_name='{1}', player_pos='{2}', player_year={3}, player_height={4}, player_weight={5} where id={6}";
        private const string INSERT_PLAYER_QRY = "insert into player values ({0}, '{1}', '{2}', {3}, {4}, {5}, {6}, NULL)";
        private const string DELETE_PLAYER_QRY = "delete from player where id={0}";
        private const string FILL_TB_MSG = "Please fill all text boxes";
        private const string SEARCH_PLAYER_BY_NAME_QRY = "select * from player where player_name like '{0}%' and team_id={1}";
        private const string SEARCH_PLAYER_BY_ID_QRY = "select * from player where id={0}";


        public EditTeamWindow(Window callingWindow, MyTeam myTeam, MyPlayer myPlayer, MySqlConnection con)
        {
            InitializeComponent();
            this.callingWindow = callingWindow;
            this.myTeam = myTeam;
            this.myPlayer = myPlayer;
            this.con = con;

            Load_Table_Data();
        }


        /* ------------- EVENT HANDLERS --------------------- */
        private void AddBTN_Click(object sender, RoutedEventArgs e)
        {
            if (Is_TB_Empty())
            {
                MessageBox.Show(FILL_TB_MSG);
                return;
            }
            Add_Player();
            Clear_All();
        }

        private void RemoveBTN_Click(object sender, RoutedEventArgs e)
        {
            Remove_Player();
            Clear_All();
        }

        private void UpdateBTN_Click(object sender, RoutedEventArgs e)
        {
            if (PlayersDT.SelectedCells.Count < 1)
            {
                MessageBox.Show(SELECT_PLAYER_MSG);
            }

            if (Is_TB_Empty())
            {
                MessageBox.Show(FILL_TB_MSG);
                return;
            }
            Update_Player();
            Clear_All();
        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            // close this window and reopen the main window
            this.Close();
            callingWindow.Show();
        }

        private void PlayersDT_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Set_Highlighted_Player_Attributes();
            Fill_All();
        }

        private void PlayerSearchTB_KeyUp(object sender, KeyEventArgs e)
        {
            Search_Player();
        }


        /* ---------------- FUNCTIONAL METHODS ---------------------*/
        /// <summary>
        /// Load most recent data into the player datagrid
        /// </summary>
        private void Load_Table_Data()
        {
            try
            {
                // select all players 
                string qry = "select * from player where team_id=" + myTeam.GetId();
                con.Open();

                // build data adapter and use to fill data table
                MySqlDataAdapter sda = new MySqlDataAdapter(qry, con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Close();
                PlayersDT.ItemsSource = dt.DefaultView;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        /// <summary>
        /// adds player to database player datatable
        /// </summary>
        private void Add_Player()
        {

            // parse player number
            string playerNumStr = NumberTB.Text.ToString();
            int playerNum = 0;

            try
            {
                playerNum = Int32.Parse(playerNumStr);
            }
            catch (FormatException ex)
            {
                string formatError = string.Format(STRING_FORMAT_MSG, "Player Number");
                MessageBox.Show(formatError);
                return;
            }

            // parse player name
            string playerName = NameTB.Text.ToString();

            // parse player position into a char
            string playerPos = PositionCB.SelectedItem.ToString();
            playerPos = playerPos.Substring(playerPos.IndexOf(":") + 2);

            // parse player year
            string playerYearStr = YearCB.SelectedItem.ToString();
            playerYearStr = playerYearStr.Substring(playerYearStr.IndexOf(":") + 2);
            int playerYear = Int32.Parse(playerYearStr);

            // parse player weight and height to integers
            string playerHeightStr = HeightTB.Text.ToString();
            string playerWeightStr = WeightTB.Text.ToString();
            int playerHeight, playerWeight;

            try
            {
                playerHeight = Int32.Parse(playerHeightStr);
                playerWeight = Int32.Parse(playerWeightStr);
            }
            catch (FormatException ex)
            {
                string formatError = string.Format(STRING_FORMAT_MSG, "Player Weight/Height");
                MessageBox.Show(formatError);
                return;
            }

            // insert player into player data table
            try
            {
                con.Open();
                string qry = string.Format(INSERT_PLAYER_QRY, playerNum, playerName, playerPos, playerYear, playerHeight, playerWeight, myTeam.GetId());

                MySqlCommand msc = new MySqlCommand(qry, con);
                msc.ExecuteNonQuery();
                con.Close();

                // refresh tabe 
                Load_Table_Data();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            if (con.State == ConnectionState.Open)
                con.Close();
        }

        /// <summary>
        /// removes player from database player datatable
        /// </summary>
        private void Remove_Player()
        {
            // get player id
            int playerId = myPlayer.GetId();

            try
            {
                con.Open();

                // disable foreign key checks
                MySqlCommand msc = new MySqlCommand(DISABLE_FOREIGN_KEY_QRY, con);
                msc.ExecuteNonQuery();

                // remove player with id from player data table
                string qry = string.Format(DELETE_PLAYER_QRY, playerId);
                msc.CommandText = qry;
                msc.ExecuteNonQuery();
                con.Close();

                // refresh table
                Load_Table_Data();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            if (con.State == ConnectionState.Open)
                con.Close();
        }

        /// <summary>
        /// updates player info in player database
        /// </summary>
        private void Update_Player()
        {
            try
            {
                // PARSE PLAYER INFO FROM TEXTBOXES
                // parse player number
                string playerNumStr = NumberTB.Text.ToString();
                int playerNum = 0;

                try
                {
                    playerNum = Int32.Parse(playerNumStr);
                }
                catch (FormatException ex)
                {
                    string formatError = string.Format(STRING_FORMAT_MSG, "Player Number");
                    MessageBox.Show(formatError);
                    return;
                }

                // parse player name
                string playerName = NameTB.Text.ToString();

                // parse player position into a char
                string playerPos = PositionCB.SelectedItem.ToString();
                playerPos = playerPos.Substring(playerPos.IndexOf(":") + 2);

                // parse player year
                string playerYearStr = YearCB.SelectedItem.ToString();
                playerYearStr = playerYearStr.Substring(playerYearStr.IndexOf(":") + 2);
                int playerYear = Int32.Parse(playerYearStr);

                // parse player weight and height to integers
                string playerHeightStr = HeightTB.Text.ToString();
                string playerWeightStr = WeightTB.Text.ToString();
                int playerHeight, playerWeight;

                try
                {
                    playerHeight = Int32.Parse(playerHeightStr);
                    playerWeight = Int32.Parse(playerWeightStr);
                }
                catch (FormatException ex)
                {
                    string formatError = string.Format(STRING_FORMAT_MSG, "Player Weight/Height");
                    MessageBox.Show(formatError);
                    return;
                }

                // UPDATE PLAYER INFO BASED ON PLAYER ID
                con.Open();
                string qry = string.Format(UPDATE_PLAYER_QRY, playerNum, playerName, playerPos, playerYear, playerHeight, playerWeight, myPlayer.GetId());

                MySqlCommand msc = new MySqlCommand(qry, con);
                msc.ExecuteNonQuery();
                con.Close();

                // refresh table
                Load_Table_Data();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            if (con.State == ConnectionState.Open)
                con.Close();
        }


        /// <summary>
        /// sets myplayer attributes from textbox inputs
        /// </summary>
        private void Set_Highlighted_Player_Attributes()
        {
            // get all player attributes from data grid
            if (PlayersDT.SelectedItems.Count == 1)
            {
                var items = PlayersDT.SelectedItems;
                foreach (DataRowView item in items)
                {
                    // get playerId
                    string playerIdStr = item["id"].ToString();
                    int playerId = Int32.Parse(playerIdStr);

                    try
                    {
                        // build dataset from player id
                        con.Open();
                        string qry = string.Format(SEARCH_PLAYER_BY_ID_QRY, playerId);
                        MySqlDataAdapter sda = new MySqlDataAdapter(qry, con);
                        DataSet ds = new DataSet();
                        sda.Fill(ds);
                        con.Close();

                        myPlayer.Set_Attributes(ds);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
            }
            if (con.State == ConnectionState.Open)
                con.Close();
        }

        /// <summary>
        /// clears all textboxes when operation is done to player
        /// </summary>
        private void Clear_All()
        {
            NumberTB.Text = "";
            NameTB.Text = "";
            PositionCB.SelectedIndex = -1;
            YearCB.SelectedIndex = -1;
            HeightTB.Text = "";
            WeightTB.Text = "";
        }

        /// <summary>
        /// fill all textboxes with selected player info
        /// </summary>
        private void Fill_All()
        {
            if (myPlayer.HasAttributes())
            {
                // fill textboxes with player info
                NumberTB.Text = myPlayer.GetNum().ToString();
                NameTB.Text = myPlayer.GetName();
                PositionCB.Text = myPlayer.GetPos();
                YearCB.Text = myPlayer.GetYear().ToString();
                HeightTB.Text = myPlayer.GetHeight().ToString();
                WeightTB.Text = myPlayer.GetWeight().ToString();
            }
            else
            {
                MessageBox.Show(SELECT_PLAYER_MSG);
            }
           
        }

        /// <summary>
        /// checks if any textboxes are left empty
        /// </summary>
        /// <returns></returns>
        private bool Is_TB_Empty()
        {
            if (NumberTB.Text.ToString() == "" ||
                NameTB.Text.ToString() == "" ||
                PositionCB.Text.ToString() == "" ||
                YearCB.Text.ToString() == "" ||
                HeightTB.Text.ToString() == "" ||
                WeightTB.Text.ToString() == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Search_Player()
        {
            // get student id from search box
            string playerName = PlayerSearchTB.Text.ToString();

            try
            {
                // search player info by name
                string qry = string.Format(SEARCH_PLAYER_BY_NAME_QRY, playerName, myTeam.GetId());
                con.Open();

                // fill data table with player info that was found
                MySqlDataAdapter sda = new MySqlDataAdapter(qry, con);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                // fill mu player with player info that was found
                DataSet ds = new DataSet();
                sda.Fill(ds);
                con.Close();

                myPlayer.Set_Attributes(ds);
                PlayersDT.ItemsSource = dt.DefaultView;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            if (con.State == ConnectionState.Open)
                con.Close();
        }
    }
}

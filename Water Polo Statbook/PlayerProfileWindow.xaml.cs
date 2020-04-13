using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Water_Polo_Statbook
{
    /// <summary>
    /// Interaction logic for PlayerProfileWindow.xaml
    /// </summary>
    public partial class PlayerProfileWindow : Window
    {
        // calling window reference
        private Window callingWindow;
        // mysql connection reference
        private MySqlQueryBuilder build;
        // myplayer reference
        private MyPlayer myPlayer;
        //my team reference
        private MyTeam myTeam;

        // checks if profile is in edit mode
        private bool editmode;

        // string constants
        private const string SELECT_PLAYER_QRY = "select * from player where id={0}";
        private const string SELECT_TOTALSTATS_QRY = "select * from player_stats where player_id={0}";
        private const string SELECT_GAMESTATS_QRY = "select opp_team, total_gol, total_att, total_ast, total_blk, total_stl, total_exl, total_tov from game_stats inner join game on game_stats.game_id = game.id where player_id={0}";
        private const string UPDATE_PLAYER_QRY = "update player set player_num={0}, player_name='{1}', player_pos='{2}', player_year={3}, player_height={4}, player_weight={5} where id={6}";
        
        public PlayerProfileWindow(Window callingWindow, MyTeam myTeam, MyPlayer myPlayer, MySqlQueryBuilder build)
        {
            this.callingWindow = callingWindow;
            this.build = build;
            this.myPlayer = myPlayer;
            this.myTeam = myTeam;
            this.editmode = false;


            InitializeComponent();
            Load_Totalstat_Table();
            Load_StatsPG_Table();
            Load_GameStats_Table();
            Set_Labels();
            Hide_Labels(editmode);
        }

        /* find out how to fix this, maybe replace the labels with text boxes or something */
        private void EditBTN_Click(object sender, RoutedEventArgs e)
        {
            if (editmode)
            {
                // update player info and return to normal view
                EditBTN.Content = "EDIT";
                editmode = false;

                Update_Player_Attributes();
                Set_Labels();
                Hide_Labels(editmode);
            }
            else
            {
                // show editor view 
                EditBTN.Content = "UPDATE";
                editmode = true;
                Hide_Labels(editmode);
            }
        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            callingWindow.Show();
            this.Close();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            Set_Labels();
        }


        /* ---------------- HELPER METHODS ---------------- */
        private void Load_Totalstat_Table()
        {
            // get all stats from totalstats table 
            string qry = string.Format(SELECT_TOTALSTATS_QRY, myPlayer.GetId());
            TotalStatsDG.ItemsSource = build.Execute_DataTable_Qry(qry).DefaultView;
        }

        private void Load_StatsPG_Table()
        {
            // create table and name columns
            DataTable dt = new DataTable();
            dt.Columns.Add("ppg");
            dt.Columns.Add("shot_pct");
            dt.Columns.Add("mpg");
            dt.Columns.Add("apg");
            dt.Columns.Add("bpg");
            dt.Columns.Add("spg");
            dt.Columns.Add("epg");
            dt.Columns.Add("tpg");

            int gamesPlayed = myTeam.GetGamesPlayed();

            // create row and fill rows with stat averages
            DataRow player = dt.NewRow();
            player["ppg"] = Calculate_Avg(myPlayer.GetTotalGoals(), gamesPlayed);
            player["mpg"] = Calculate_Avg(myPlayer.GetTotalAttempts(), gamesPlayed);
            player["shot_pct"] = Calculate_Avg(myPlayer.GetTotalGoals(), myPlayer.GetTotalAttempts()) + "%"; 
            player["apg"] = Calculate_Avg(myPlayer.GetTotalAssists(), gamesPlayed);
            player["bpg"] = Calculate_Avg(myPlayer.GetTotalBlocks(), gamesPlayed);
            player["spg"] = Calculate_Avg(myPlayer.GetTotalSteals(), gamesPlayed);
            player["epg"] = Calculate_Avg(myPlayer.GetTotalExclusions(), gamesPlayed);
            player["tpg"] = Calculate_Avg(myPlayer.GetTotalTurnovers(), gamesPlayed);

            dt.Rows.Add(player);

            // fill stat per game data grid with stat data table
            StatsPerGameDG.ItemsSource = dt.DefaultView;
        }

        private string Calculate_Avg(int stat, int gp)
        {
            if (gp == 0)
            {
                return "0.0";
            }
            else if (stat == 0)
            {
                return "0.0";
            }
            else
            {
                return string.Format("{0:0.00}", stat / (float)gp);
            }
        }

        private void Load_GameStats_Table()
        {
            // get all stats from individual games of this player
            string qry = string.Format(SELECT_GAMESTATS_QRY, myPlayer.GetId());
            GameStatsDG.ItemsSource = build.Execute_DataTable_Qry(qry).DefaultView;
        }


        private void Update_Player_Attributes()
        {
            // get text from textboxes
            int playerNum = Int32.Parse(NumberTB.Text.ToString());
            string playerName = NameTB.Text.ToString();
            string playerPos = PosCB.Text.ToString();
            int playerYear = Int32.Parse(YearCB.Text.ToString());
            int playerHeight = Int32.Parse(HeightTB.Text.ToString());
            int playerWeight = Int32.Parse(WeightTB.Text.ToString());

            // update player info 
            string updqry = string.Format(UPDATE_PLAYER_QRY, playerNum, playerName, playerPos, playerYear, playerHeight, playerWeight, myPlayer.GetId());
            build.Execute_Query(updqry);

            // update myplayer data fields
            string selqry = string.Format(SELECT_PLAYER_QRY, myPlayer.GetId());
            myPlayer.Set_Attributes(build.Execute_DataSet_Query(selqry));
        }

        private void Set_Labels()
        {
            // give value to all properties in model object
            MyModelObject ControlDC = new MyModelObject()
            {
                Name = myPlayer.GetName(),
                Num = myPlayer.GetNum().ToString(),
                Pos = myPlayer.GetPos(),
                Year = myPlayer.GetYear().ToString(),
                Height = myPlayer.GetHeight().ToString(),
                Weight = myPlayer.GetWeight().ToString()
            };

            // set all data context bindings for labels in team main window
            NameLBL.DataContext = ControlDC;
            NameTB.DataContext = ControlDC;
            NumberLBL.DataContext = ControlDC;
            NumberTB.DataContext = ControlDC;
            PosLBL.DataContext = ControlDC;
            PosCB.DataContext = ControlDC;
            YearLBL.DataContext = ControlDC;
            YearCB.DataContext = ControlDC;
            HeightLBL.DataContext = ControlDC;
            HeightTB.DataContext = ControlDC;
            WeightLBL.DataContext = ControlDC;
            WeightTB.DataContext = ControlDC;

        }

        private void Hide_Labels(bool editing)
        {
            // check if edit mode is on
            if(editing)
            {
                // if edit mode is on hide labels and show textboxes
                NumberLBL.Visibility = Visibility.Hidden;
                NameLBL.Visibility = Visibility.Hidden;
                PosLBL.Visibility = Visibility.Hidden;
                YearLBL.Visibility = Visibility.Hidden;
                HeightLBL.Visibility = Visibility.Hidden;
                WeightLBL.Visibility = Visibility.Hidden;
                NumberTB.Visibility = Visibility.Visible;
                NameTB.Visibility = Visibility.Visible;
                PosCB.Visibility = Visibility.Visible;
                YearCB.Visibility = Visibility.Visible;
                HeightTB.Visibility = Visibility.Visible;
                WeightTB.Visibility = Visibility.Visible;

            }
            else
            {
                // if edit mode is off hide textboxes and show labels 
                NumberTB.Visibility = Visibility.Hidden;
                NameTB.Visibility = Visibility.Hidden;
                PosCB.Visibility = Visibility.Hidden;
                YearCB.Visibility = Visibility.Hidden;
                HeightTB.Visibility = Visibility.Hidden;
                WeightTB.Visibility = Visibility.Hidden;
                NameLBL.Visibility = Visibility.Visible;
                NumberLBL.Visibility = Visibility.Visible;
                PosLBL.Visibility = Visibility.Visible;
                YearLBL.Visibility = Visibility.Visible;
                HeightLBL.Visibility = Visibility.Visible;
                WeightLBL.Visibility = Visibility.Visible;
            }

            this.UpdateLayout();
        }
    }
}

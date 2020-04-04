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
    /// Interaction logic for TeamProfileWindow.xaml
    /// </summary>
    public partial class TeamProfileWindow : Window
    {
        // mysql connection reference
        private MySqlConnection con;
        // myteam reference
        private MyTeam myTeam;
        // calling window reference
        private Window callingWindow;

        // query constants
        private const string SELECT_TEAM_GAMESTATS_QRY = "select wins, losses, games_played, league_wins, league_losses, league_games_played from team_stats where team_id={0}";
        private const string SELECT_TEAM_TOTALSTATS_QRY = "select total_gol, total_ast, total_blk, total_stl, total_exl, total_tov from team_stats where team_id={0}";
        public TeamProfileWindow(Window callingWindow, MyTeam myTeam, MySqlConnection con)
        {
            this.callingWindow = callingWindow;
            this.myTeam = myTeam;
            this.con = con;
            InitializeComponent();

            Load_Gamestat_Table();
            Load_Totalstat_Table();
        }

        /// <summary>
        /// loads all stats per game for top data table
        /// </summary>
        private void Load_Gamestat_Table()
        {
            // load team game stats data
            string qry = string.Format(SELECT_TEAM_GAMESTATS_QRY, myTeam.GetId());
            con.Open();

            MySqlDataAdapter sda = new MySqlDataAdapter(qry, con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            con.Close();

            // add columns not in datatable 
            dt.Columns.Add("win_pct");
            dt.Columns.Add("league_win_pct");
            dt.Columns.Add("shot_pct");

            dt.Rows[0]["win_pct"] = myTeam.GetWinPct();
            dt.Rows[0]["league_win_pct"] = myTeam.GetLeagueWinPct();
            dt.Rows[0]["shot_pct"] = myTeam.GetShotPct();

            GameStatsDG.ItemsSource = dt.DefaultView;
        }

        /// <summary>
        /// loads total stats for bottom data table
        /// </summary>
        private void Load_Totalstat_Table()
        {
            // load team total stats data
            string qry = string.Format(SELECT_TEAM_TOTALSTATS_QRY, myTeam.GetId());
            con.Open();

            MySqlDataAdapter sda = new MySqlDataAdapter(qry, con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            con.Close();

            // add columns not in datatable 
            dt.Columns.Add("ppg");
            dt.Columns.Add("apg");
            dt.Columns.Add("bpg");
            dt.Columns.Add("spg");
            dt.Columns.Add("epg");
            dt.Columns.Add("tpg");

            dt.Rows[0]["ppg"] = myTeam.GetPPG();
            dt.Rows[0]["apg"] = myTeam.GetAPG();
            dt.Rows[0]["bpg"] = myTeam.GetBPG();
            dt.Rows[0]["spg"] = myTeam.GetSPG();
            dt.Rows[0]["epg"] = myTeam.GetEPG();
            dt.Rows[0]["tpg"] = myTeam.GetTPG();

            TotalStatsDG.ItemsSource = dt.DefaultView;
        }
    }
}

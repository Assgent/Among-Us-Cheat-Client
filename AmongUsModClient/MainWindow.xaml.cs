using System;
using System.Windows;
using System.Windows.Data;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.Generic;

using HamsterCheese.AmongUsMemory;

namespace AmongUsModClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ICheatClientWindow
    {
        public readonly static string IS_IMPOSTER_OFFSETS = "GameAssembly.dll+01468910,5C,0,34,28"; //Offsets frequently change which is why I went for this implementation
        public readonly static string IS_DEAD_OFFSETS = "GameAssembly.dll+01468910,5C,0,34,29";
        public readonly static string KILL_TIMER_OFFSETS = "GameAssembly.dll+01468910,5C,0,44";

        public static readonly string[] PLAYER_INFO_GRID_COLUMN_HEADERS = { "Name", "Color", "Is Imposter", "Is Dead", "Using Vent", "ID" };
        public static readonly string[] PLAYER_INFO_GRID_COLUMN_BINDINGS = { "Name", "Color", "IsImposter", "IsDead", "InVent", "PlayerID" };

        //======================

        public delegate void SetInformationSourceDelegate(List<Player> source);
        public Delegate UpdateInformationDelegate { get; private set; }

        private CheatClient Client;

        private readonly List<Player> ReportedDeaths = new List<Player>();

        public MainWindow()
        {
            UpdateInformationDelegate = new SetInformationSourceDelegate(UpdateInformation);

            Loaded += MainWindow_OnLoad;
            Closed += MainWindow_OnClose;
            InitializeComponent();
        }

        private void MainWindow_OnLoad(object sender, RoutedEventArgs e)
        {
            bool amongUsLoaded = (bool)new LoadingWindow().ShowDialog(); //Block thread until Among Us is started

            if (amongUsLoaded)
            {
                InitWindowAttributes();

                Client = new CheatClient(this);
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void MainWindow_OnClose(object sender, EventArgs e)
        {
            CloseApplication();
        }

        private void InitWindowAttributes()
        {
            PlayerInfoDataGrid.AutoGenerateColumns = false; //Configure Player Info grid

            for (byte i = 0; i < PLAYER_INFO_GRID_COLUMN_BINDINGS.Length; i++)
            {
                PlayerInfoDataGrid.Columns.Add(new DataGridTextColumn()
                {
                    Header = PLAYER_INFO_GRID_COLUMN_HEADERS[i],
                    Binding = new Binding(PLAYER_INFO_GRID_COLUMN_BINDINGS[i])
                });
            }
        }

        private async Task CloseDeathAlert(int duration)
        {
            await Task.Delay(duration);

            AlertTextBox.Text = string.Empty;
        }

        private void ShowAlert(string message, int duration)
        {
            message = "[Notification] " + message;

            AlertTextBox.Text = message;

            _ = CloseDeathAlert(duration); //Async logic so window updates twice
        }

        public void UpdateInformation(List<Player> source)
        {
            UpdatePlayerInformation(source);
        }

        private void UpdatePlayerInformation(List<Player> players)
        {
            Console.WriteLine("Updating player information...");

            ResetKillCooldown();

            MapGrid.Children.Clear();

            if (players.Count > 0)
            {
                foreach (Player player in players)
                {
                    if (player.IsDead && !ReportedDeaths.Contains(player))
                    {
                        ShowAlert(player.Name + " has been killed!", 4000);
                        ReportedDeaths.Add(player);
                    }
                    else if (player.InVent)
                    {
                        ShowAlert(player.Name + " is using a vent!", 4000);
                    }

                    RadioButton point = player.CreateUIPoint(player.Position); //This is very unorthodox.
                    MapGrid.Children.Add(point);
                }

                PlayerInfoDataGrid.ItemsSource = players;
                PlayerInfoDataGrid.Items.Refresh();
            }
            else
            {
                PlayerInfoDataGrid.ItemsSource = null;
            }
        }

        //==================

        private void ForceImposter(bool value)
        {
            Cheese.mem.WriteMemory(IS_IMPOSTER_OFFSETS, "byte", (value ? 1 : 0).ToString());
        }

        private void ForceDeath(bool value)
        {
            Cheese.mem.WriteMemory(IS_DEAD_OFFSETS, "byte", (value ? 1 : 0).ToString());
        }

        private void ResetKillCooldown()
        {
            if (Cheese.mem.ReadFloat(KILL_TIMER_OFFSETS) > 0.0f)
                Cheese.mem.WriteMemory(KILL_TIMER_OFFSETS, "float", 0.0f.ToString());
        }

        //======================================

        private void ForceImposterButton_Click(object sender, RoutedEventArgs e)
        {
            ForceImposter(true);
        }

        private void ForceCrewmateButton_Click(object sender, RoutedEventArgs e)
        {
            ForceImposter(false);
        }

        private void ForceDeathButton_Click(object sender, RoutedEventArgs e)
        {
            ForceDeath(true);
        }

        private void ForceAliveButton_Click(object sender, RoutedEventArgs e)
        {
            ForceDeath(false);
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Created by Assgent.\n\nQ & A:\nQ: Why isn't anything working?\nA: Game pointer offsets can change unexpectedly, breaking the program. If this happens, try searching for an updated version of this program or repair it yourself using Cheat Engine!\n\nQ: What are the other files for?\nA: \"AmongUsMemory.dll\" and \"Memory.dll\" are essential libraries that this program needs in order to run! You can read more about them here: https://bit.ly/36kbd8m", "About");
        }

        private void ExitApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            CloseApplication();
        }

        public void CloseApplication()
        {
            Console.WriteLine("Shutting down!");

            Client.IsCanceled = true;

            Environment.Exit(0);
        }
    }
}

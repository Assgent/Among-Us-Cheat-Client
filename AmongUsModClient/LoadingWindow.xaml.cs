using System;
using System.Windows;
using System.Threading.Tasks;

using HamsterCheese.AmongUsMemory;

namespace AmongUsModClient
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public LoadingWindow()
        {
            Loaded += LoadingWindow_OnLoad;
            InitializeComponent();
        }

        private async void LoadingWindow_OnLoad(object sender, EventArgs e)
        {
            Console.WriteLine("Waiting for Among Us to start up...");
            DialogResult = await CheckForGameStart();
            Close();
        }

        private async Task<bool> CheckForGameStart()
        {
            while (true)
            {
                Console.WriteLine("Running check...");

                if (Cheese.Init())
                {
                    Console.WriteLine("Check passed! Returning to main menu.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Check failed. Trying again in two seconds...");
                    await Task.Delay(2000);
                }
            }
        }
    }
}

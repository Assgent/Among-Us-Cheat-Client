using System;
using System.Threading;
using System.Windows.Threading;
using System.Collections.Generic;

using HamsterCheese.AmongUsMemory;

namespace AmongUsModClient
{
    public class CheatClient
    {
        public const int INFORMATION_UPDATE_RATE = 300; //In milliseconds
        public const uint MAX_ERRORS = 100;

        //========

        private readonly ICheatClientWindow Caller;

        private readonly List<Player> Players = new List<Player>();

        private uint ErrorCount = 0;

        private readonly Dispatcher UserDispatcher = Dispatcher.CurrentDispatcher;

        public bool IsCanceled = false;

        public CheatClient(ICheatClientWindow caller)
        {
            Caller = caller;

            Thread t = new Thread(new ThreadStart(SetPlayerInfo));
            t.Start();
        }

        private void SetPlayerInfo()
        {
            while (!IsCanceled)
            {
                Thread.Sleep(INFORMATION_UPDATE_RATE);

                List<PlayerData> playerDatas = null;
                try
                {
                    playerDatas = Cheese.GetAllPlayers();
                }
                catch (Exception)
                {
                    ErrorCount++;

                    if (ErrorCount > MAX_ERRORS)
                    {
                        Caller.CloseApplication(); //Close program
                    }
                    else
                    {
                        continue;
                    }
                }

                Players.Clear();

                foreach (PlayerData playerData in playerDatas)
                {
                    if (playerData.PlayerInfo == null) //If data is garbage, skip
                    {
                        continue;
                    }

                    PlayerInfo playerInfo = playerData.PlayerInfo.Value;
                    PlayerControl playerControl = playerData.Instance;

                    Player newPlayer = new Player()
                    {
                        PlayerID = playerInfo.PlayerId,
                        Name = Utils.ReadString(playerInfo.PlayerName),
                        Color = new PlayerColor()
                        {
                            Name = Player.COLOR_NAMES[playerInfo.ColorId],
                            Color = Player.COLORS[playerInfo.ColorId]
                        },
                        IsImposter = playerInfo.IsImpostor != 0,
                        IsDead = playerInfo.IsDead != 0,
                        InVent = playerControl.inVent != 0
                    };

                    if (playerData.IsLocalPlayer)
                        newPlayer.Position = playerData.GetMyPosition();
                    else
                        newPlayer.Position = playerData.GetSyncPosition();

                    Players.Add(newPlayer);
                }

                UserDispatcher.Invoke(Caller.UpdateInformationDelegate, Players);
            }
        }
    }

    public interface ICheatClientWindow
    {
        Delegate UpdateInformationDelegate { get; }
        void UpdateInformation(List<Player> source);
        void CloseApplication();
    }
}

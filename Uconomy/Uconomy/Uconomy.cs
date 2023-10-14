using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using Steamworks;
using System;

namespace fr34kyn01535.Uconomy
{
    public class Uconomy : RocketPlugin<UconomyConfiguration>
    {
        public DatabaseManager Database;
        public static Uconomy Instance;

        protected override void Load()
        {
            Instance = this;
            Database = new DatabaseManager();
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
        }

        public delegate void PlayerBalanceUpdate(UnturnedPlayer player, decimal amt);
        public event PlayerBalanceUpdate OnBalanceUpdate;
        public delegate void PlayerBalanceCheck(UnturnedPlayer player, decimal balance);
        public event PlayerBalanceCheck OnBalanceCheck;
        public delegate void PlayerPay(UnturnedPlayer sender, UnturnedPlayer receiver, decimal amt);
        public event PlayerPay OnPlayerPay;

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList(){
                {"command_balance_show","Your current balance is: {0} {1}"},
                {"command_pay_invalid","Invalid arguments"},
                {"command_pay_error_pay_self","You cant pay yourself"},
                {"command_pay_error_invalid_amount","Invalid amount"},
                {"command_pay_error_cant_afford","Your balance does not allow this payment"},
                {"command_pay_error_player_not_found","Failed to find player"},
                {"command_pay_private","You paid {0} {1} {2}"},
                {"command_pay_console","You received a payment of {0} {1} "},
                {"command_pay_other_private","You received a payment of {0} {1} from {2}"},
                };
            }
        }

        internal void HasBeenPayed(UnturnedPlayer sender, UnturnedPlayer receiver, decimal amt)
        {
            if (OnPlayerPay != null)
                OnPlayerPay(sender, receiver, amt);
        }

        internal void BalanceUpdated(string SteamID, decimal amt)
        {
            if (OnBalanceUpdate != null)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(SteamID)));
                OnBalanceUpdate(player, amt);
            }
        }

        internal void OnBalanceChecked(string SteamID, decimal balance)
        {
            if (OnBalanceCheck != null)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(SteamID)));
                OnBalanceCheck(player, balance);
            }
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            //setup account
            Database.CheckSetupAccount(player.CSteamID);
        }
    }
}
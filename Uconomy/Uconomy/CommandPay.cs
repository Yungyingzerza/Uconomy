using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;

namespace fr34kyn01535.Uconomy
{
    public class CommandPay : IRocketCommand
    {
        public string Help
        {
            get { return "Pays a specific player money from your account"; }
        }

        public string Name
        {
            get { return "pay"; }
        }

        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Both;
            }
        }

        public string Syntax
        {
            get { return "<player> <amount>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string> { "pagar" }; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "uconomy.pay" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (command.Length != 2)
            {
                UnturnedChat.Say(caller, Uconomy.Instance.Translations.Instance.Translate("command_pay_invalid"));
                return;
            }

            UnturnedPlayer otherPlayer = UnturnedPlayer.FromName(command[0]);
            if (otherPlayer != null)
            {
                if (caller == otherPlayer)
                {
                    UnturnedChat.Say(caller, Uconomy.Instance.Translations.Instance.Translate("command_pay_error_pay_self"));
                    return;
                }

                decimal amount = 0;
                if (!Decimal.TryParse(command[1], out amount) || amount <= 0)
                {
                    UnturnedChat.Say(caller, Uconomy.Instance.Translations.Instance.Translate("command_pay_error_invalid_amount"));
                    return;
                }

                if (caller is ConsolePlayer)
                {
                    Uconomy.Instance.Database.IncreaseBalance(otherPlayer.Id, amount);
                    UnturnedChat.Say(otherPlayer.CSteamID, Uconomy.Instance.Translations.Instance.Translate("command_pay_console", amount, Uconomy.Instance.Configuration.Instance.MoneyName));
                }
                else
                {

                    decimal myBalance = Uconomy.Instance.Database.GetBalance(caller.Id);
                    if ((myBalance - amount) <= 0)
                    {
                        UnturnedChat.Say(caller, Uconomy.Instance.Translations.Instance.Translate("command_pay_error_cant_afford"));
                        return;
                    }
                    else
                    {
                        Uconomy.Instance.Database.IncreaseBalance(caller.Id, -amount);
                        UnturnedChat.Say(caller, Uconomy.Instance.Translations.Instance.Translate("command_pay_private", otherPlayer.CharacterName, amount, Uconomy.Instance.Configuration.Instance.MoneyName));
                        Uconomy.Instance.Database.IncreaseBalance(otherPlayer.Id, amount);
                        UnturnedChat.Say(otherPlayer.CSteamID, Uconomy.Instance.Translations.Instance.Translate("command_pay_other_private", amount, Uconomy.Instance.Configuration.Instance.MoneyName, caller.DisplayName));
                        Uconomy.Instance.HasBeenPayed((UnturnedPlayer)caller, otherPlayer, amount);
                    }
                }

            }
            else
            {
                UnturnedChat.Say(caller, Uconomy.Instance.Translations.Instance.Translate("command_pay_error_player_not_found"));
            }
        }
    }
}
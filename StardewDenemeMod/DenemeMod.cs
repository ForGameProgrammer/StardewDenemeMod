using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace StardewDenemeMod
{
    class DenemeMod : Mod
    {
        const int ADD_MONEY = 2000;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            switch (e.Button)
            {
                case SButton.F8:
                    Game1.player.Money += ADD_MONEY;
                    Game1.player.totalMoneyEarned += ADD_MONEY;
                    ShowBannerMessage($"{ADD_MONEY} Para eklendi.");
                    break;
                case SButton.F5:
                    Game1.player.stamina = Game1.player.MaxStamina;
                    ShowBannerMessage($"Stamina yenilendi.");
                    break;
                case SButton.Multiply:
                    Game1.isTimePaused = !Game1.isTimePaused;
                    ShowBannerMessage($"Zaman {(Game1.isTimePaused ? "durduruldu" : "devam ediyor")}.");
                    break;
                case SButton.Add:
                    //Game1.player.addedSpeed += 1;
                    Game1.player.temporarySpeedBuff += 1;
                    ShowBannerMessage("1 Hiz arttirildi.");
                    break;
                case SButton.Subtract:
                    //Game1.player.addedSpeed -= 1;
                    Game1.player.temporarySpeedBuff += 1;
                    ShowBannerMessage("1 Hiz azaltildi.");
                    break;
                default:
                    break;
            }

            // print button presses to the console window
            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
        }

        private void ShowBannerMessage(string message)
        {
            Game1.addHUDMessage(new HUDMessage(message, 2));
        }
    }
}

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
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardewValley.Util;

namespace StardewDenemeMod
{
    class DenemeMod : Mod
    {
        const int ADD_MONEY = 2000;

        int TimeofDay = 0;
        int Speed = 0;

        bool AutoWater = false;
        bool InstantFishing = false;
        bool InstantBalta = false;
        bool isTimeFreezed = false;

        List<Response> ResponsesHile;
        List<string> Secenekler;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            SecenekleriEkle();
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            var player = Game1.player;

            player.addedSpeed = Speed;
            player.team.sharedDailyLuck.Value = 0.115d;

            if (isTimeFreezed) Game1.timeOfDay = TimeofDay;

            var location = player.currentLocation;
            if (AutoWater && (location.IsFarm || location.IsGreenhouse))
            {
                foreach (HoeDirt dirt in location.terrainFeatures.Values.OfType<HoeDirt>())
                {
                    if (dirt.crop != null)
                        dirt.state.Value = HoeDirt.watered;
                }
            }

            if (InstantFishing)
            {
                if (player.CurrentTool is FishingRod fishingRod)
                {
                    if (fishingRod.isCasting)
                        fishingRod.castingPower = 1.01f;

                    if (fishingRod.isFishing && !fishingRod.hit && fishingRod.timeUntilFishingBite > 0)
                        fishingRod.timeUntilFishingBite = 0;
                }
            }

            if (InstantBalta)
            {
                Vector2 tile = new Vector2((int)player.GetToolLocation().X / Game1.tileSize, (int)player.GetToolLocation().Y / Game1.tileSize);

                if (player.CurrentTool is Axe && player.currentLocation.terrainFeatures.ContainsKey(tile))
                {
                    TerrainFeature obj = player.currentLocation.terrainFeatures[tile];
                    if (obj is Tree tree && tree.health.Value > 1)
                        tree.health.Value = 1;
                    else if (obj is FruitTree fruitTree && fruitTree.health.Value > 1)
                        fruitTree.health.Value = 1;
                }
            }


        }


        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            switch (e.Button)
            {
                case SButton.F5:
                    CanVeStaminaYenile();
                    break;
                case SButton.F6:
                    SansEkle();
                    break;
                case SButton.F7:
                    ToggleZaman();
                    break;
                case SButton.F8:
                    ParaEkle();
                    break;
                case SButton.Multiply:
                    ZamanIleri();
                    break;
                case SButton.Divide:
                    ZamanGeri();
                    break;
                case SButton.Add:
                    Hizlandir();
                    break;
                case SButton.Subtract:
                    Yavaslat();
                    break;
                case SButton.NumPad9:
                    ToggleSulama();
                    break;
                case SButton.NumPad8:
                    BitkileriYetistir();
                    break;
                case SButton.NumPad7:
                    ToggleBalik();
                    break;
                case SButton.NumPad6:
                    ToggleAxe();
                    break;
                case SButton.NumPad0:
                    Game1.player.currentLocation.createQuestionDialogue("Hile secin", ResponsesHile.ToArray(), MenuSecildi);
                    break;
                default:
                    break;
            }

            //Get Farm as Location
            //Game1.getLocationFromName("Farm")

            // Concrete code for spawning:
            //Game1.getLocationFromName("Farm").dropObject(new StardewValley.Object(itemId, 1, false, -1, 0), new Vector2(x, y) * 64f, Game1.viewport, true, (Farmer)null);

            //You can add items found in ObjectInformation using:
            //Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(int parentSheetIndex, int initialStack, [bool isRecipe = false], [int price = -1], [int quality = 0]));

        }

        private void SecenekleriEkle()
        {
            string[] hileler = new string[] {
            "Can ve Stamina yenile",
            "Sansi arttir",
            "Zaman durdur/devam et",
            "Para ekle",
            "Zamani ileri al",
            "Zamani geri al",
            "Kosma hizini arttir",
            "Kosma hizini azalt",
            "Otomatik sulamayi ac/kapa",
            "Bitkileri ve Agaclari yetistir",
            "Hizli balik ac/kapa",
            "Hizli balta ac/kapa"
            };
            Secenekler = new List<string>();
            Secenekler.AddRange(hileler);
            ResponsesHile = new List<Response>();
            Secenekler.ForEach(secenek =>
            {
                ResponsesHile.Add(new Response(secenek, secenek));
            });
        }

        private void MenuSecildi(Farmer farmer, string secilen)
        {
            if (secilen == null || secilen == "") return;
            if (farmer != Game1.player) return;

            switch (secilen)
            {
                case "Can ve Stamina yenile":
                    CanVeStaminaYenile();
                    break;
                case "Sansi arttir":
                    SansEkle();
                    break;
                case "Zaman durdur/devam et":
                    ToggleZaman();
                    break;
                case "Para ekle":
                    ParaEkle();
                    break;
                case "Zamani ileri al":
                    ZamanIleri();
                    break;
                case "Zamani geri al":
                    ZamanGeri();
                    break;
                case "Kosma hizini arttir":
                    Hizlandir();
                    break;
                case "Kosma hizini azalt":
                    Yavaslat();
                    break;
                case "Otomatik sulamayi ac/kapa":
                    ToggleSulama();
                    break;
                case "Bitkileri ve Agaclari yetistir":
                    BitkileriYetistir();
                    break;
                case "Hizli balik ac/kapa":
                    ToggleBalik();
                    break;
                case "Hizli balta ac/kapa":
                    ToggleAxe();
                    break;
                default:
                    break;
            }
        }

        #region Yardımcı Fonksiyonlar

        private void BitkileriYetistir()
        {
            var location = Game1.player.currentLocation;

            foreach (HoeDirt dirt in location.terrainFeatures.Values.OfType<HoeDirt>())
            {
                if (dirt.crop != null)
                    dirt.crop.growCompletely();
            }

            foreach (Tree tree in location.terrainFeatures.Values.OfType<Tree>())
            {
                tree.growthStage.Value = Tree.treeStage;
            }

            foreach (FruitTree fruitTree in location.terrainFeatures.Values.OfType<FruitTree>())
            {
                fruitTree.fruitsOnTree.Value = FruitTree.maxFruitsOnTrees;
            }

            if (location is BuildableGameLocation buildableLocation)
            {
                foreach (Building building in buildableLocation.buildings)
                {
                    if (building.daysOfConstructionLeft.Value > 0)
                        building.dayUpdate(0);
                    if (building.daysUntilUpgrade.Value > 0)
                        building.dayUpdate(0);
                }
            }


        }

        private void ToggleBalik()
        {
            InstantFishing = !InstantFishing;
            BannerGoster($"Hizli balik tutma {(InstantFishing ? "acik" : "kapali")}.");
        }

        private void ToggleAxe()
        {
            InstantBalta = !InstantBalta;
            BannerGoster($"Hizli balta {(InstantBalta ? "acik" : "kapali")}.");
        }

        private void SansEkle()
        {
            Game1.player.LuckLevel += 5;
            BannerGoster($"Sansiniz 5 arttirildi. Suanda sansiniz {Game1.player.LuckLevel}");
        }

        private void ToggleSulama()
        {
            AutoWater = !AutoWater;
            BannerGoster($"Otomatik sulama {(AutoWater ? "acik" : "kapali")}.");
        }

        private void ZamanGeri()
        {
            Game1.timeOfDay -= 100;
            BannerGoster($"Zaman geri alındı.");
        }

        private void ZamanIleri()
        {
            Game1.timeOfDay += 100;
            BannerGoster($"Zaman ileri alındı.");
        }

        private void CanVeStaminaYenile()
        {
            Game1.player.stamina = Game1.player.MaxStamina;
            Game1.player.health = Game1.player.maxHealth;
            BannerGoster($"Can ve Stamina yenilendi.");
        }

        private void ToggleZaman()
        {
            isTimeFreezed = !isTimeFreezed;
            TimeofDay = Game1.timeOfDay;
            BannerGoster($"Zaman {(isTimeFreezed ? "durduldu" : "devam ediyor")}.");
        }

        private void ParaEkle()
        {
            Game1.player.Money += ADD_MONEY;
            Game1.player.totalMoneyEarned += ADD_MONEY;
            BannerGoster($"{ADD_MONEY} Para eklendi.");
        }

        private void Hizlandir()
        {
            Speed += 5;
            BannerGoster($"Hiz 5 arttirildi. Hiziniz {Speed}");
        }

        private void Yavaslat()
        {
            Speed = Math.Max(0, Speed - 5);
            BannerGoster($"Hiz 5 azaltildi. Hiziniz {Speed}");
        }

        private void BannerGoster(string message)
        {
            Game1.addHUDMessage(new HUDMessage(message, 2));
        }

        #endregion
    }
}

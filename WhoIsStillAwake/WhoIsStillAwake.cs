using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Minigames;
using WhoIsStillAwakeMod.Numbers;

namespace WhoIsStillAwakeMod
{
    public class WhoIsStillAwake : Mod
    {
        /// <summary>
        /// User configuration set in the mod
        /// </summary>
        public static UserConfig Config { get; set; }
        /// <summary>
        /// Stardew Valley modding API
        /// </summary>
        public static IModHelper SMAPI { get; private set; }

        /// <summary>
        /// SMAPI provided API for logging
        /// </summary>
        public static IMonitor Logger { get; private set; }

        /// <summary>
        /// Whether or not the rendered event has been added
        /// </summary>
        private bool hasAddedRenderedEvent = false;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<UserConfig>();
            SMAPI = Helper;
            Logger = Monitor;

            Helper.Events.GameLoop.DayStarted += OnDayStartedHandler;
            Helper.Events.GameLoop.DayEnding += OnDayEndingHandler;
        }

        /// <summary>
        /// Only try to start the mod if the game is a multiplayer game
        /// </summary>
        private void OnDayStartedHandler(object sender, DayStartedEventArgs args)
        {
            if (Context.IsMultiplayer)
            {
                Helper.Events.GameLoop.TimeChanged += OnTimeChangedHandler;
            }
        }

        /// <summary>
        /// At the end of each day, remove the mod handlers if we are in multiplayer
        /// </summary>
        private void OnDayEndingHandler(object sender, DayEndingEventArgs args)
        {
            if (Context.IsMultiplayer)
            {
                Helper.Events.GameLoop.TimeChanged -= OnTimeChangedHandler;
                Helper.Events.Display.Rendered -= OnRenderedHandler;

                hasAddedRenderedEvent = false;
            }
        }

        /// <summary>
        /// We only start the mod when the user configuration has been met
        /// </summary>
        private void OnTimeChangedHandler(object sender, TimeChangedEventArgs args)
        {
            if (args.NewTime < Config.TimeToStartDisplaying)
                return;

            if (!hasAddedRenderedEvent)
            {
                Helper.Events.GameLoop.TimeChanged -= OnTimeChangedHandler;
                Helper.Events.Display.Rendered += OnRenderedHandler;

                hasAddedRenderedEvent = true;
            }
        }

        /// <summary>
        /// Render the time if all of the configured conditions have been met
        /// </summary>
        private void OnRenderedHandler(object sender, RenderedEventArgs args)
        {
            if (!Context.IsWorldReady)
                return;
            else if (Config.ShowOutsideMinigames && Game1.currentMinigame is null)
                DrawFarmersInBed(args.SpriteBatch);
            else if (Config.ShowWhilePlayingJunimoCarts && Game1.currentMinigame is MineCart)
                DrawFarmersInBed(args.SpriteBatch);
            else if (Config.ShowWhilePlayingJourneyOfThePrairieKing && Game1.currentMinigame is AbigailGame)
                DrawFarmersInBed(args.SpriteBatch);

        }

        /// <summary>
        /// Draws the farmers in bed
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw on</param>
        private void DrawFarmersInBed(SpriteBatch spriteBatch)
        {
            if (Config.ShowNumberOfFarmersInBed)
                NumberDrawer.DrawNumberOfFarmersInBed(spriteBatch);
        }
    }
}

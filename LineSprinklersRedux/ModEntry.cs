global using SObject = StardewValley.Object;
using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.Objects;
using StardewValley.GameData.BigCraftables;
using Microsoft.Xna.Framework.Graphics;
using LineSprinklersRedux.Framework.Data;
using StardewValley.GameData.Machines;

using LineSprinklersRedux.Framework;
using HarmonyLib;


/* TODO NEXT 
 * 
 * 1. Cleanup Rotation Code to leverage the Direction Enum, & ModData to determine spriteIndex.
 * 2. Patch IsSprinkler
 * 3. Patch GetSprinklerTiles
 * 4. Test and see if it just works?
 * 5. Add Config for Directions, Keybinds
 * 
 * Others:
 * - Abstract out Asset Loading
 */


namespace LineSprinklersRedux
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        internal static IModHelper? IHelper;
        internal static IMonitor? IMonitor;

        internal static List<ObjectInformation>? SprinklersObjectsInfo;

        public static string ModDataKeySpace => $"{IHelper.ModRegistry.ModID}";

        // TODO: Move to a ModConfig
        private KeybindList RotateKey { get; set; } = KeybindList.Parse("MouseMiddle");

        /*********
        ** Accessors
        *********/
        /// <summary>The unique assets for which <see cref="IAssetLoader.CanLoad{T}"/> was called.</summary>
        private readonly HashSet<IAssetName> LoadedAssets = new();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            IHelper = Helper;
            IMonitor = Monitor;
            SprinklersObjectsInfo = Helper.Data.ReadJsonFile<List<ObjectInformation>>("assets/data.json");
            if (SprinklersObjectsInfo == null)
            {
                this.Monitor.Log("Could not load Sprinkler Information from data.json, is this mod correctly installed?");
                SprinklersObjectsInfo = new List<ObjectInformation>();
                return;
            }

            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var harmony = new Harmony(ModManifest.UniqueID);

            BaseGamePatches.Apply(harmony);
        }

        /// <inheritdoc cref="IInputEvents.ButtonsChanged"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (this.RotateKey.JustPressed())
            {
                Vector2 tile = this.Helper.Input.GetCursorPosition().GrabTile;
                var obj = Game1.player.currentLocation.getObjectAtTile((int)tile.X, (int)tile.Y);
                if (obj == null) return;

                if (obj.HasContextTag("LineSprinkler"))
                {
                    Sprinkler.Rotate(obj);
                }

            }
        }

        /// <inheritdoc cref="IGameLoopEvents.DayStarted"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (!Context.IsMainPlayer)
            {
                return;
            }

            this.Monitor.Log($"Day Started.", LogLevel.Debug);
        }

        /// <inheritdoc cref="IContentEvents.AssetRequested"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/BigCraftables"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, BigCraftableData>().Data;
                    foreach (var item in SprinklersObjectsInfo ??= new List<ObjectInformation>())
                    {
                        var id = $"{this.ModManifest.UniqueID}_{item.Id}";
                        if (item.Object == null)
                        {
                            this.Monitor.Log($"Could not register object {id}, Object is null.");
                            continue;
                        }
                        item.Object.DisplayName = Helper.Translation.Get($"{item.Id}.DisplayName");
                        item.Object.Description = Helper.Translation.Get($"{item.Id}.Description");
                        item.Object.Texture = string.Format(item.Object.Texture, this.ModManifest.UniqueID);
                        data[id] = item.Object;
                    }
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    foreach (var item in SprinklersObjectsInfo ??= new List<ObjectInformation>())
                    {
                        var id = $"{this.ModManifest.UniqueID}_{item.Id}";
                        if (item.Recipe == null) continue;

                        data[id] = string.Format(item.Recipe, id);
                    }
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo($"/Mods/{this.ModManifest.UniqueID}/Objects"))
            {
                e.LoadFromModFile<Texture2D>("assets/LineSprinklers.png", AssetLoadPriority.Exclusive);
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Machines"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsDictionary<string, MachineData>();
                    var recyclingMachine = editor.Data["(BC)20"];
                    foreach (var item in SprinklersObjectsInfo ??= new List<ObjectInformation>())
                    {
                        if (item.RecyclerOutput == null) continue;
                        var id = $"{this.ModManifest.UniqueID}_{item.Id}";
                        MachineOutputRule recyclerRule = new()
                        {
                            Id = id,
                            UseFirstValidOutput = true,
                            MinutesUntilReady = 60,
                            Triggers = new(),
                            OutputItem = new(),
                        };
                        recyclerRule.Triggers.Add(new MachineOutputTriggerRule
                        {
                            Trigger = MachineOutputTrigger.ItemPlacedInMachine,
                            RequiredItemId = id,
                            RequiredCount = 1,
                        });
                        recyclerRule.OutputItem.Add(new MachineItemOutput
                        {
                            ItemId = item.RecyclerOutput,
                        });
                        recyclingMachine.OutputRules.Add(recyclerRule);
                    }
                });
            }
        }
    }
}
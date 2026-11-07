using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using System.Reflection;

namespace SleepOnAnyBed;

internal sealed class ModEntry : Mod
{
    private static readonly MethodInfo? StartSleepMethod = typeof(GameLocation).GetMethod("startSleep", BindingFlags.Instance | BindingFlags.NonPublic);
    private Point? promptedBedTile;

    public override void Entry(IModHelper helper)
    {
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady || !this.CanOpenSleepPrompt())
            return;

        if (this.HasNativeSleepHandling(Game1.currentLocation))
        {
            this.promptedBedTile = null;
            return;
        }

        Point? bedTile = this.GetCurrentBedTile(Game1.currentLocation, Game1.player);
        if (bedTile is null)
        {
            this.promptedBedTile = null;
            return;
        }

        if (this.promptedBedTile == bedTile)
            return;

        this.promptedBedTile = bedTile;
        this.ShowSleepPrompt();
    }

    private bool CanOpenSleepPrompt()
    {
        return Game1.currentLocation is not null
            && Game1.player is not null
            && Game1.activeClickableMenu is null
            && !Game1.newDay
            && Game1.shouldTimePass(false)
            && Game1.player.hasMoved
            && !Game1.player.passedOut;
    }

    private bool HasNativeSleepHandling(GameLocation location)
    {
        return location is FarmHouse or IslandFarmHouse;
    }

    private Point? GetCurrentBedTile(GameLocation location, Farmer player)
    {
        Rectangle bounds = player.GetBoundingBox();
        bounds.Inflate(Game1.tileSize / 2, Game1.tileSize / 2);

        int left = bounds.Left / Game1.tileSize;
        int right = (bounds.Right - 1) / Game1.tileSize;
        int top = bounds.Top / Game1.tileSize;
        int bottom = (bounds.Bottom - 1) / Game1.tileSize;

        for (int y = top; y <= bottom; y++)
        {
            for (int x = left; x <= right; x++)
            {
                if (BedFurniture.IsBedHere(location, x, y))
                    return new Point(x, y);
            }
        }

        Point standingTile = player.TilePoint;
        if (BedFurniture.IsBedHere(location, standingTile.X, standingTile.Y))
            return standingTile;

        bounds = player.GetBoundingBox();
        bounds.Inflate(4, 4);

        foreach (Point homeTile in this.GetNpcHomeTiles(location))
        {
            Rectangle homeArea = new(homeTile.X * Game1.tileSize, homeTile.Y * Game1.tileSize, Game1.tileSize, Game1.tileSize);
            if (bounds.Intersects(homeArea))
                return homeTile;
        }

        return null;
    }

    private IEnumerable<Point> GetNpcHomeTiles(GameLocation location)
    {
        string locationName = location.NameOrUniqueName;
        string rawLocationName = location.Name;

        foreach (CharacterData data in Game1.characterData.Values)
        {
            if (data.Home is null)
                continue;

            foreach (CharacterHomeData home in data.Home)
            {
                if (home.Location == locationName || home.Location == rawLocationName)
                    yield return home.Tile;
            }
        }
    }

    private void ShowSleepPrompt()
    {
        GameLocation location = Game1.currentLocation;
        location.createQuestionDialogue(
            Game1.content.LoadString("Strings\\Locations:FarmHouse_Bed_GoToSleep"),
            location.createYesNoResponses(),
            this.AfterSleepQuestion,
            speaker: null
        );
    }

    private void AfterSleepQuestion(Farmer who, string whichAnswer)
    {
        if (whichAnswer != "Yes")
            return;

        GameLocation location = Game1.currentLocation;
        if (StartSleepMethod is null)
        {
            this.Monitor.Log("Couldn't find Stardew Valley's private GameLocation.startSleep method.", LogLevel.Error);
            return;
        }

        who.isInBed.Value = true;
        who.sleptInTemporaryBed.Value = true;
        Game1.displayFarmer = false;

        StartSleepMethod.Invoke(location, null);
    }
}

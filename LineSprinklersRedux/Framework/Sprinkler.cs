using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

namespace LineSprinklersRedux.Framework
{
    public static class Sprinkler
    {


        private static readonly string directionKey = $"{ModEntry.ModDataKeySpace}/Direction";
        private static readonly string sprinklerTypeKey = $"{ModEntry.ModDataKeySpace}/Type";

        public enum Direction
        {
            Right,
            Down,
            Left,
            Up,
        }

        public static IEnumerable<Vector2> GetCoverage(SObject sprinkler)
        {
            var tile = sprinkler.TileLocation;

            int range = Range(sprinkler);
            Direction direction = getDirectionFromModData(sprinkler);
            switch (direction)
            {
                case Direction.Right:
                    for (int i = 1; i <= range; i++) {
                        yield return new Vector2(tile.X + i, tile.Y);
                    }
                    break;
                case Direction.Down:
                    for (int i = 1; i <= range; i++) {
                        yield return new Vector2(tile.X, tile.Y + i);
                    }
                    break;
                case Direction.Left:
                    for (int i = 1; i <= range; i++)
                    {
                        yield return new Vector2(tile.X + (i * -1), tile.Y);
                    }
                    break;
                case Direction.Up:
                    for (int i = 1; i <= range; i++)
                    {
                        yield return new Vector2(tile.X, tile.Y + (i * -1));
                    }
                    break;

            }
        }

        public static void Rotate(SObject sprinkler)
        {
            var current = getDirectionFromModData(sprinkler);
            var next = cycleDirection(current);
            ModEntry.IMonitor!.Log($"current: {current}, next: {next}", LogLevel.Debug);

            setDirectionInModData(sprinkler, next);
            // TODO: Magic Strings are Brittle :-(
            if (sprinkler.HasContextTag("BasicLineSprinkler"))
            {
                sprinkler.ParentSheetIndex = (int)next;
            }
            else if (sprinkler.HasContextTag("QualityLineSprinkler"))
            {
                sprinkler.ParentSheetIndex = (int)next + 4;
            }
            else if (sprinkler.HasContextTag("IridiumLineSprinkler"))
            {
                sprinkler.ParentSheetIndex = (int)next + 8;
            }
            ModEntry.IMonitor!.Log($"ParentSheetIndex: {sprinkler.ParentSheetIndex}", LogLevel.Debug);
        }

        private static Direction cycleDirection(Direction direction)
        {
            if (direction == Direction.Up)
            {
                return Direction.Right;
            }
            return ++direction;
        }

        private static Direction getDirectionFromModData(SObject sprinkler)
        {
            ModEntry.IMonitor!.Log($"Using modDataKey: {directionKey}", LogLevel.Debug);
            if (!sprinkler.modData.ContainsKey(directionKey))
            {
                return Direction.Right;
            }

            var data = sprinkler.modData[directionKey] ?? "Right";
            if (Enum.TryParse(typeof(Direction), data, out var ret))
            {
                return (Direction)ret!;
            }
            else
            {
                ModEntry.IMonitor!.Log($" Failed to parse Direction from ModData, assuming Right. modData: {data}", LogLevel.Debug);
                return Direction.Right;
            }

        }

        private static void setDirectionInModData(SObject sprinkler, Direction dir)
        {
            sprinkler.modData[directionKey] = dir.ToString();
        }

        private static int Range(SObject sprinkler) {
            if (sprinkler.HasContextTag("BasicLineSprinkler"))
            {
                return 4;
            }
            else if (sprinkler.HasContextTag("QualityLineSprinkler"))
            {
                return 8;
            }
            else if (sprinkler.HasContextTag("IridiumLineSprinkler"))
            {
                return 24;
            }
            return 0;
        }
    }
}


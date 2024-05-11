using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;
using StardewValley;
using StardewValley.GameData.BigCraftables;
using LineSprinklersRedux.Framework.Data;
using System;

namespace LineSprinklersRedux.Framework
{
    public static class Sprinkler
    {


        public static bool IsLineSprinkler(SObject obj)
        {
            if (obj == null) return false;
            return obj.HasContextTag(ModConstants.MainContextTag);
        }
        public static IEnumerable<Vector2> GetCoverage(SObject sprinkler)
        {
            var tile = sprinkler.TileLocation;

            int range = CustomFields.GetRange(sprinkler);
            SprinklerDirection direction = ModData.GetDirection(sprinkler);
            switch (direction)
            {
                case SprinklerDirection.Right:
                    for (int i = 1; i <= range; i++) {
                        yield return new Vector2(tile.X + i, tile.Y);
                    }
                    break;
                case SprinklerDirection.Down:
                    for (int i = 1; i <= range; i++) {
                        yield return new Vector2(tile.X, tile.Y + i);
                    }
                    break;
                case SprinklerDirection.Left:
                    for (int i = 1; i <= range; i++)
                    {
                        yield return new Vector2(tile.X + (i * -1), tile.Y);
                    }
                    break;
                case SprinklerDirection.Up:
                    for (int i = 1; i <= range; i++)
                    {
                        yield return new Vector2(tile.X, tile.Y + (i * -1));
                    }
                    break;

            }
        }

        public static void Rotate(SObject sprinkler)
        {
            var current = ModData.GetDirection(sprinkler);
            ModData.SetDirection(sprinkler, current.Cycle());
            SetSpriteFromRotation(sprinkler);
        
        
        }

        public static void SetSpriteFromRotation(SObject sprinkler)
        {
            var dir = ModData.GetDirection(sprinkler);
            int baseSprite = CustomFields.GetBaseSprite(sprinkler);
            sprinkler.ParentSheetIndex = baseSprite + (int)dir;
        }

        public static void ApplySprinkler(SObject sprinkler)
        {
            GameLocation location = sprinkler.Location;
            int delayBeforeAnimationStart = Game1.random.Next(1000);
            int num = (int)sprinkler.TileLocation.X;
            int num2 = (int)sprinkler.TileLocation.Y;
            switch (ModData.GetDirection(sprinkler))
            {
                case SprinklerDirection.Up:
                    location.temporarySprites.Add(new TemporaryAnimatedSprite(29, sprinkler.TileLocation * 64f + new Vector2(0f, -48f), Color.White * 0.5f, 4, flipped: false, 60f, 100)
                    {
                        delayBeforeAnimationStart = delayBeforeAnimationStart,
                        id = num * 4000 + num2
                    });
                    break;
                case SprinklerDirection.Right:
                    location.temporarySprites.Add(new TemporaryAnimatedSprite(29, sprinkler.TileLocation * 64f + new Vector2(48f, 0f), Color.White * 0.5f, 4, flipped: false, 60f, 100)
                    {
                        rotation = MathF.PI / 2f,
                        delayBeforeAnimationStart = delayBeforeAnimationStart,
                        id = num * 4000 + num2
                    });
                    break;
                case SprinklerDirection.Down:
                    location.temporarySprites.Add(new TemporaryAnimatedSprite(29, sprinkler.TileLocation * 64f + new Vector2(0f, 48f), Color.White * 0.5f, 4, flipped: false, 60f, 100)
                    {
                        rotation = MathF.PI,
                        delayBeforeAnimationStart = delayBeforeAnimationStart,
                        id = num * 4000 + num2
                    });
                    break;
                case SprinklerDirection.Left:
                    location.temporarySprites.Add(new TemporaryAnimatedSprite(29, sprinkler.TileLocation * 64f + new Vector2(-48f, 0f), Color.White * 0.5f, 4, flipped: false, 60f, 100)
                    {
                        rotation = 4.712389f,
                        delayBeforeAnimationStart = delayBeforeAnimationStart,
                        id = num * 4000 + num2
                    });
                    break;
            }
        }
    }
}


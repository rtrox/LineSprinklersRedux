using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;
using StardewValley;
using StardewValley.GameData.BigCraftables;
using LineSprinklersRedux.Framework.Data;
using System;
using StardewValley.ItemTypeDefinitions;

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
            if (HasPressureNozzle(sprinkler))
            {
                range *= 2;
            }

            SprinklerDirection direction = ModData.GetDirection(sprinkler);
            switch (direction)
            {
                case SprinklerDirection.Right:
                    for (int i = 1; i <= range; i++)
                    {
                        yield return new Vector2(tile.X + i, tile.Y);
                    }
                    break;
                case SprinklerDirection.Down:
                    for (int i = 1; i <= range; i++)
                    {
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

        public static void DrawAttachments(SObject sprinkler, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            Microsoft.Xna.Framework.Rectangle boundingBoxAt = sprinkler.GetBoundingBoxAt(x, y);
            if (sprinkler.heldObject.Value != null)
            {
                Vector2 vector3 = Vector2.Zero;
                if (sprinkler.heldObject.Value.QualifiedItemId == "(O)913")
                {
                    vector3 = new Vector2(0f, -20f);
                }

                // Override texture for Pressure Nozzles to be directional.
                ParsedItemData heldItemData = ItemRegistry.GetDataOrErrorItem(sprinkler.heldObject.Value.QualifiedItemId);
                var sourceRect = heldItemData.GetSourceRect(1);
                if (HasPressureNozzle(sprinkler))
                {
                    heldItemData = ItemRegistry.GetDataOrErrorItem(ModConstants.OverlayDummyItemID);
                    sourceRect = heldItemData.GetSourceRect(spriteIndex: (int)ModData.GetDirection(sprinkler));
                }

                spriteBatch.Draw(
                    heldItemData.GetTexture(),
                    Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + 32 + ((sprinkler.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), y * 64 + 32 + ((sprinkler.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0)) + vector3),
                    sourceRect,
                    Color.White * alpha,
                    0f,
                    new Vector2(8f, 8f),
                    (sprinkler.scale.Y > 1f) ? sprinkler.getScale().Y : 4f,
                    sprinkler.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    (float)(sprinkler.isPassable() ? boundingBoxAt.Top : boundingBoxAt.Bottom) / 10000f + 1E-05f);
            }
            if (sprinkler.SpecialVariable == 999999)
            {
                if (sprinkler.heldObject.Value != null && sprinkler.heldObject.Value.QualifiedItemId == "(O)913")
                {
                    Torch.drawBasicTorch(spriteBatch, (float)(x * 64) - 2f, y * 64 - 32, (float)boundingBoxAt.Bottom / 10000f + 1E-06f);
                }
                else
                {
                    Torch.drawBasicTorch(spriteBatch, (float)(x * 64) - 2f, y * 64 - 32 + 12, (float)(boundingBoxAt.Bottom + 2) / 10000f);
                }
            }
        }
        private static Texture2D PressureNozzleSpriteFromRotation(SObject sprinkler)
        {
            var dir = ModData.GetDirection(sprinkler);
            var data = ItemRegistry.GetDataOrErrorItem($"{ModConstants.OverlayDummyItemID}_{dir}");
            return data.GetTexture();
        }

        private static bool HasPressureNozzle(SObject sprinkler)
        {
            if (!IsLineSprinkler(sprinkler)) return false;
            if (sprinkler.heldObject.Value != null)
            {
                return sprinkler.heldObject.Value.QualifiedItemId == "(O)915";
            }
            return false;
        }
    }
}


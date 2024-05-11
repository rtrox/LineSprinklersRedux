using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Machines;
using StardewValley.GameData.Objects;

namespace LineSprinklersRedux.Framework.Data
{
    public class ObjectInformation
    {
        public string? Id { get; set; }

        public BigCraftableData? Object { get; set; }

        public string? Recipe { get; set; }

        public string? RecyclerOutput { get; set; }
    }
}

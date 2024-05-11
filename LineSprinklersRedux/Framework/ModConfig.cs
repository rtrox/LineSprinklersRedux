using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineSprinklersRedux.Framework
{
    internal class ModConfig
    {
        public KeybindList RotateSprinklerKeybindList { get; set; } = new(SButton.MouseMiddle);

    }
}

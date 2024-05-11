using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineSprinklersRedux.Framework
{
    internal class ModConstants
    {
        public static string ModKeySpace => $"{ModEntry.IHelper!.ModRegistry.ModID}";

        // ContextTags
        public static string MainContextTag = $"{ModKeySpace}_LineSprinklers";

    }
}

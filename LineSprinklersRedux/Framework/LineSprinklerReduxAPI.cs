using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineSprinklersRedux.Framework
{
    public class LineSprinklerReduxAPI: ILineSprinklersReduxAPI
    {
        public void PlaySprinklerAnimation(SObject sprinkler, int delayBeforeAnimationStart)
        {
            Sprinkler.ApplySprinklerAnimation(sprinkler, delayBeforeAnimationStart);
        }

        public void ApplySprinkler(SObject sprinkler)
        {
            Sprinkler.ApplySprinkler(sprinkler);
        }

        public bool IsLineSprinkler(SObject sprinkler)
        {
            return Sprinkler.IsLineSprinkler(sprinkler);
        }
    }
}

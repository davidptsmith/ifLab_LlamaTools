using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ifLab_LlamaTools.Core.ComponentInputs
{
    public static class StandardComponentInputs
    {
        //standard inputs for gaps - maybe back out into a class static input utilities 
        public static InputObject LinedGap = new InputObject("____________", "____________", "____________", GH_ParamAccess.item, true);
        public static InputObject EmptyGap = new InputObject("      ", "      ", "      ", GH_ParamAccess.item, true);

    }
}

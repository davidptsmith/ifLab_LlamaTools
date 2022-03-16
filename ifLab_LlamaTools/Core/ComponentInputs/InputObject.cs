using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;

namespace ifLab_LlamaTools.Core.ComponentInputs
{
    public enum InputDataTypes
    {
        Number = 0,
        String = 1,
        Bool = 2,
    }
    public class InputObject
    {
        public InputObject(string name, string nickname, string description, GH_ParamAccess paramAccess, bool optional)
        {
            Name = name;
            Nickname = nickname;
            Desscription = description;
            ParamAccess = paramAccess;
            Optional = optional; 
        }

        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Desscription { get; set; }
        public GH_ParamAccess ParamAccess { get; set; }
        public int ParamNumber { get; set; }
        public bool Optional { get; set; } = false;

        public InputDataTypes InputType{ get; set; }

      
    }

  
}

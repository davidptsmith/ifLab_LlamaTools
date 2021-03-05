using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace ifLab_LlamaTools
{
    public class ifLab_LlamaToolsInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "ifLabLlamaTools";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("1f8012b0-66bf-4d5e-b7f0-29853539dba1");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}

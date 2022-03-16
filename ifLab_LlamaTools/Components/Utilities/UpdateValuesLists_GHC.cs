using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper;
using System.Drawing;


namespace ifLab_LlamaTools.Components.Utilities
{
    public class UpdateValuesLists_GHC : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        /// <summary>
        /// Initializes a new instance of the Ghc_UpdateValuesLists class.
        /// </summary>
        public UpdateValuesLists_GHC()
          : base("UpdateValuesLists", "Update Values Lists",
              "Reverts all values lists back to defualt- usefull for the auto populate values lists",
              "LLama Tools", "Layout Tools")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "Run", "Run", GH_ParamAccess.item); 
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool runBool = false;
            bool runBool2 = false; 
            DA.GetData(0, ref runBool);

            Component = this;
            GrasshopperDocument = this.OnPingDocument();


            if (runBool)
            {
                runBool2 = true;
                runBool = false; 
            }

            Rhino.RhinoApp.Wait();


            if (runBool2)
            {
                var doc = Rhino.RhinoDoc.ActiveDoc;

                
                var valueList = new Grasshopper.Kernel.Special.GH_ValueList();

                //fore each component on the canvas
                foreach (IGH_DocumentObject obj in GrasshopperDocument.Objects)
                {
                    //if that component is selected
                    if (obj.Name == valueList.Name)
                    {
                        

                        // set the variable vl to the source object as a grasshopper value list 
                        Grasshopper.Kernel.Special.GH_ValueList vl = obj as Grasshopper.Kernel.Special.GH_ValueList;
                        vl.NickName = "list";
                        
                       // vl.ClearData();
                        obj.ExpireSolution(true);
                    }
                }

              

                runBool2 = false; 
            }



        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("58fc9d36-217a-4fc7-ad63-7d3ba79a9155"); }
        }
    }
}
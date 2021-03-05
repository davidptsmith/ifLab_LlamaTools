using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace ifLab_LlamaTools.Components.Utilities
{
    public class TextScaling : GH_Component
    {

       


        /// <summary>
        /// Initializes a new instance of the Ghc_TextScaling class.
        /// </summary>
        public TextScaling()
          : base("Text Scaling Toggle", "Text Scaling",
              "This allows for the toggling of model space and viewport text scaling",
              "LLama Tools", "Utilities")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddBooleanParameter("Model Space Scaling", "Model Space", "Enable or Disable model space scaling", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Layout Space Scaling", "Layout Space ", "Enable or Disable Model Space Scaling", GH_ParamAccess.item);

            pManager[0].Optional = true; 
            pManager[1].Optional = true; 


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
            bool modelSpace = true;
            bool layoutSpace = true;


            var doc = Rhino.RhinoDoc.ActiveDoc;

            modelSpace = doc.ModelSpaceAnnotationScalingEnabled;
            layoutSpace = doc.LayoutSpaceAnnotationScalingEnabled; 



            DA.GetData(0, ref modelSpace);
            DA.GetData(1, ref layoutSpace);



            doc.ModelSpaceAnnotationScalingEnabled = modelSpace;
            doc.LayoutSpaceAnnotationScalingEnabled = layoutSpace; 


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
            get { return new Guid("f2904f87-7f32-45b0-8955-8594e0b75ee9"); }
        }
    }
}
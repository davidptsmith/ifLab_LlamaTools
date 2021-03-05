using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino;

namespace ifLab_LlamaTools.Components.Layouts
{
    public class GetLayouts : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Ghc_GetLayouts class.
        /// </summary>
        public GetLayouts()
          : base("Get Layouts", "Get Layouts",
              "Gets the name of all layouts in the document",
              "LLama Tools", "Layout Tools")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Refresh", "Refresh", "Refresh Value List", GH_ParamAccess.item); 

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter("LayoutList", "Layout List", "Layouts in document", GH_ParamAccess.list); 
        }


        List<string> pageNames = new List<string>();


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            //set active doc, set active page unit system to millimeters, get active Views
            var doc = RhinoDoc.ActiveDoc;
            bool runBool = false; 
            DA.GetData(0,ref runBool);

            // get document page views

            
            var pageViews = doc.Views.GetPageViews();
            bool runBool_2 = false;
            if (runBool)
            {
                runBool_2 = true;
                runBool = false; 

            }

            RhinoApp.Wait(); 
            if (runBool_2)
            
            {
                
                pageNames.Clear();
                foreach (var pageView in pageViews)
                {
                    pageNames.Add(pageView.PageName);
                }

                
            }

            runBool_2 = false; 

            DA.SetDataList(0, pageNames);
            
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
            get { return new Guid("a36c0c3d-f1b4-41cb-64c4-0b38da9fe2ed"); }
        }
    }
}
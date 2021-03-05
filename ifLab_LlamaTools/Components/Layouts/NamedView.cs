using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino; 

namespace ifLab_LlamaTools.Components.Layouts
{
    public class NamedView : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the NamedView class.
        /// </summary>
        public NamedView()
          : base("Named View", "NamedView",
              "Update a detail to be a named View",
              "LLama Tools", "Test")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run Function", "Run", "Input boolean to run function", GH_ParamAccess.item);
            pManager.AddTextParameter("Detail Names", "Detail Names", "Detail Names in which to act", GH_ParamAccess.list);
            pManager.AddTextParameter("Named Views", "Named Views", "Named Views in which to act", GH_ParamAccess.list);
            pManager.AddTextParameter("Display Mode", "Display Mode", "Display Mode Name to use", GH_ParamAccess.item);

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
            List<string> detailNames = new List<string>();
            List<string> namedViews = new List<string>();
            string displayMode = "WireFame"; 

            DA.GetData(0, ref runBool);
            DA.GetDataList(1, detailNames);
            DA.GetDataList(2, namedViews);
            DA.GetData(3, ref displayMode); 

            //set active doc, set active page unit system to millimeters, get active Views
            var doc = RhinoDoc.ActiveDoc;
            doc.PageUnitSystem = Rhino.UnitSystem.Millimeters;

            // get document page views
           
            var pageViews = doc.Views.GetPageViews();

            if (runBool)
            {
                int indexValue = 0;
                foreach ( string detailName in detailNames)
                {
                    var myNamedView = namedViews[indexValue]; 

                    foreach (var pageView in pageViews)
                        {
                        var details = pageView.GetDetailViews();

                        for (int j = 0; j < details.Length; j++)
                        {
                        
                            var detail = details[j];

                            if (detail.Name == detailName)
                            {

                                var viewportDisplay = Rhino.Display.DisplayModeDescription.FindByName(displayMode);
                                detail.Viewport.DisplayMode = viewportDisplay;
                                detail.CommitViewportChanges();
                                pageView.Redraw();

                                var myViewIndex = doc.NamedViews.FindByName(myNamedView);
                                var myView = doc.NamedViews[myViewIndex];
                                var location = myView.Viewport.CameraLocation;
                                var target = myView.Viewport.CameraDirection;

                                detail.Viewport.CameraUp = Rhino.Geometry.Vector3d.ZAxis;
                                detail.Viewport.SetCameraLocation(location, false);
                                detail.Viewport.SetCameraDirection(target, true);
                                detail.CommitViewportChanges();
                                detail.CommitChanges();
                                pageView.Redraw();

                            }

                        

                        }
                       
                    doc.Views.Redraw();

                }

                indexValue++;



                }
                
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
            get { return new Guid("5d6d650e-b629-4445-a5c3-93a0edfc7a8f"); }
        }
    }
}
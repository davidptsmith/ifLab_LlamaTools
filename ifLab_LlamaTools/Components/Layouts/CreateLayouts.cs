using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace ifLab_LlamaTools.Components.Layouts
{
    public class CreateLayouts : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;


        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public CreateLayouts()
          : base("Create Layouts", "Create Layouts",
              "Description",
              "Llama Tools", "Layout Tools")
        {
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "Run", "Create Layouts", GH_ParamAccess.item);
            pManager.AddTextParameter("Layout Names", "Names", "Layout Names", GH_ParamAccess.list);
            pManager.AddNumberParameter("Layout Width", "Width", "Layout Width", GH_ParamAccess.item);
            pManager.AddNumberParameter("Layout Height", "Height", "layout Height", GH_ParamAccess.item);
            pManager.AddNumberParameter("Detail Offset", "Offest", "Detail offset from page edge", GH_ParamAccess.item);
            //pManager.AddTextParameter();
            pManager.AddTextParameter("Display Mode", "[] Display", "the display mode used for the viewport", GH_ParamAccess.item);
            pManager.AddPointParameter("Target Points", "Target Points", "Targest to centre the viewport on", GH_ParamAccess.list);
            pManager.AddNumberParameter("Scale", "Scale", "Scale of the viewport (1:input value", GH_ParamAccess.item);




            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

         //   pManager.AddTextParameter("LayoutList", "Layout List", "Layouts in document", GH_ParamAccess.list);
        }


        List<string> pageNames = new List<string>();



        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            bool runBool = false;
            bool runBool_2 = false;

            DA.GetData(0, ref runBool);

            List<string> layoutNames = new List<string>();
            DA.GetDataList(1, layoutNames);

            double l_Width = 0;
            DA.GetData(2, ref l_Width);

            double l_Height = 0;
            DA.GetData(3, ref l_Height);

            double offset = 0;
            DA.GetData(4, ref offset);

            string displayMode = "";
            DA.GetData(5, ref displayMode);

            List<Point3d> targetPoints = new List<Point3d>();
            DA.GetDataList(6, targetPoints);

            double scale = 1;
            DA.GetData(7, ref scale);






            //// get display styles 


            var displayModes = Rhino.Display.DisplayModeDescription.GetDisplayModes();
            List<string> displayNames = new List<string>();

            foreach (var dM in displayModes)
            {
                displayNames.Add(dM.EnglishName);
            }


            ////// Auto Update Value list    - need to change input list, location of active doc, and add gh component to and grasshopper document to the start of script

            var inputList2 = displayNames;
            int inputNumber = 5;

            Component = this;
            GrasshopperDocument = this.OnPingDocument();

            foreach (IGH_Param source in this.Component.Params.Input[inputNumber].Sources)
            {
                if (source is Grasshopper.Kernel.Special.GH_ValueList && source.NickName != "DisplayModes")
                {
                    // set the variable vl to the source object as a grasshopper value list 
                    Grasshopper.Kernel.Special.GH_ValueList vl = source as Grasshopper.Kernel.Special.GH_ValueList;

                    vl.NickName = "DisplayModes";

                    //clear the current contents of the value list
                    vl.ListItems.Clear();

                    // for each data item add value and key to value list 
                    for (int i = 0; i < inputList2.Count; i++)
                    {

                        // select the type of value list to display 
                        vl.ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.DropDown;

                        // add the items to the value list 
                        vl.ListItems.Add(new Grasshopper.Kernel.Special.GH_ValueListItem(inputList2[i], "\"" + inputList2[i] + "\""));

                    }



                }

            }

            /////






            if (runBool)
            {
                runBool_2 = true;
                runBool = false;
            }
            Rhino.RhinoApp.Wait();
            if (runBool_2)
            {

                var doc = Rhino.RhinoDoc.ActiveDoc;
                int indexValue = 0;


                var pageViews = doc.Views.GetPageViews();



                //find the page views with the same name as template view
                foreach (var pageView in pageViews)
                {
                    for (int k = 0; k < layoutNames.Count; k++)
                    {
                        if (pageView.PageName == layoutNames[k])
                        {
                            pageView.Close();
                        }
                    }
                }




                foreach (var layoutName in layoutNames)
                {



                    var pageview = doc.Views.AddPageView(layoutName, l_Width, l_Height);

                    var pt1 = new Point2d(0 + offset, 0 + offset);
                    var pt2 = new Point2d(l_Width - offset, l_Height - offset);

                    var detail = pageview.AddDetailView("newDetail", pt1, pt2, Rhino.Display.DefinedViewportProjection.Top);





                    //if( namedView != "" )
                    {
                        //var myViewIndex = doc.NamedViews.FindByName(detailNames[indexValue]);
                        //var myView = doc.NamedViews[myViewIndex];
                        //var location = myView.Viewport.CameraLocation;
                        //var target = myView.Viewport.CameraDirection;

                        //detail.Viewport.CameraUp = Rhino.Geometry.Vector3d.ZAxis;
                        //detail.Viewport.SetCameraLocation(location, true);
                        //detail.Viewport.SetCameraDirection(target, true);

                        //detail.CommitViewportChanges();
                        //detail.CommitChanges();
                        //pageView.Redraw();

                    }


                    if (targetPoints.Count > 0)
                    {
                        var myFocalPoint = targetPoints[indexValue];

                        detail.DetailGeometry.SetScale(1, doc.ModelUnitSystem, 1 / scale, doc.PageUnitSystem);
                        detail.CommitChanges();
                        detail.Viewport.SetCameraTarget(myFocalPoint, true);

                    }



                    if (displayMode != "")
                    {
                        var viewportDisplay = Rhino.Display.DisplayModeDescription.FindByName(displayMode);
                        detail.Viewport.DisplayMode = viewportDisplay;
                        detail.CommitViewportChanges();
                        detail.CommitChanges();
                    }

                    indexValue++;
                }

                runBool_2 = false;

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
            get { return new Guid("a36c0c3d-f1b4-44cb-94c4-0b38da9fe2ed"); }
        }
    }
}
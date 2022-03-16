using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino; 


namespace ifLab_LlamaTools.Components.Layouts
{
    public class UpdateDetailsOnLayouts_GHC : GH_Component
    {


        GH_Document GrasshopperDocument;
        IGH_Component Component;



        /// <summary>
        /// Initializes a new instance of the Ghc_EditDetail class.
        /// </summary>
        public UpdateDetailsOnLayouts_GHC()
          : base("Update Details on Layouts", "Update Details",
              "Edit Details by Name",
              "LLama Tools", "Layout Tools")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddBooleanParameter("Run Function", "Run", "Input boolean to run function", GH_ParamAccess.item);
            pManager.AddTextParameter("Layout Names", "[] Layout Names", "layouts to search for detail", GH_ParamAccess.list);
            pManager.AddTextParameter("Detail Names", "Detail Names", "Detail Names in which to act", GH_ParamAccess.list);
            pManager.AddTextParameter("Named Views", "Named Views", "Named Views in which to act", GH_ParamAccess.list);
            pManager.AddTextParameter("Display Mode", "[] Display Mode", "Display Mode Name to use", GH_ParamAccess.item);
            pManager.AddPointParameter("Target", "Target", "Detail Target", GH_ParamAccess.list);
            pManager.AddNumberParameter("Scale", "Scale", "Scale of Viewport", GH_ParamAccess.item);

            //pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;

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
            List<Point3d> targetPoints = new List<Point3d>();
            double scaleFactor = 0;

            List<string> layoutNames = new List<string>();


            DA.GetData(0, ref runBool);
            DA.GetDataList(2, detailNames);
            DA.GetDataList(3, namedViews);
            DA.GetData(4, ref displayMode);
            DA.GetDataList(5, targetPoints);
            DA.GetData(6, ref scaleFactor);

            DA.GetDataList(1, layoutNames);

           // bool noPointsInList = false;

            //if (targetPoints.Count == 0)
            //{
            //    targetPoints.Add(Point3d.Origin);
            //    noPointsInList = true;
            //}

            //set active doc, set active page unit system to millimeters, get active Views
            var doc = RhinoDoc.ActiveDoc;
            doc.PageUnitSystem = Rhino.UnitSystem.Millimeters;

         
            // get document page views

            var views = doc.Views.GetPageViews();
            List<string> viewNames = new List<string>();
            foreach (var view in views)
            {
                viewNames.Add(view.PageName);
            }

            ////// Auto Update Value list    - need to change input list, location of active doc, and add gh component to and grasshopper document to the start of script

            var inputList = viewNames;

            Component = this;
            GrasshopperDocument = this.OnPingDocument();

            foreach (IGH_Param source in this.Component.Params.Input[1].Sources)
            {
                if (source is Grasshopper.Kernel.Special.GH_ValueList && source.NickName != "Layouts")
                {
                    // set the variable vl to the source object as a grasshopper value list 
                    Grasshopper.Kernel.Special.GH_ValueList vl = source as Grasshopper.Kernel.Special.GH_ValueList;

                    vl.NickName = "Layouts";

                    //clear the current contents of the value list
                    vl.ListItems.Clear();

                    if (inputList.Count == 0)
                    {
                        vl.ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.CheckList;
                        vl.ListItems.Add(new Grasshopper.Kernel.Special.GH_ValueListItem("No Layouts", null));
                    }

                        // for each data item add value and key to value list 
                        for (int i = 0; i < inputList.Count; i++)
                    {

                        // select the type of value list to display 
                        vl.ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.CheckList;

                        // add the items to the value list 
                        vl.ListItems.Add(new Grasshopper.Kernel.Special.GH_ValueListItem(inputList[i], "\"" + inputList[i] + "\""));

                    }



                }

            }

            /////



            //// get display styles 


            var displayModes = Rhino.Display.DisplayModeDescription.GetDisplayModes();
            List<string> displayNames = new List<string>();

            foreach (var dM in displayModes)
            {
                displayNames.Add(dM.EnglishName);
            }


            ////// Auto Update Value list    - need to change input list, location of active doc, and add gh component to and grasshopper document to the start of script

            var inputList2 = displayNames;

            Component = this;
            GrasshopperDocument = this.OnPingDocument();

            foreach (IGH_Param source in this.Component.Params.Input[4].Sources)
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
                        vl.ListItems.Add(new Grasshopper.Kernel.Special.GH_ValueListItem(inputList2[i], "\"" +  inputList2[i] + "\""));

                    }



                }

            }

            /////




            bool runBool_2 = false;
            if (runBool)
            {
                runBool_2 = true;
                runBool = false;
            }


            if (runBool_2)
            {
                int indexValue = 0;

                foreach (var layoutName in layoutNames)
                {
                    foreach (var pageView in views)
                    {

                        if (pageView.PageName == layoutName)
                        {

                            var details = pageView.GetDetailViews();

                            foreach (var detailName in detailNames)
                            {


                            

                            for (int j = 0; j < details.Length; j++)
                            {

                                var detail = details[j];

                                if (detail.Name == detailName)
                                {



                                        //if (noPointsInList)
                                        //{
                                        //    myFocalPoint = detail.Viewport.CameraTarget;

                                        //}

                                        if (scaleFactor != 0)
                                        {
                                            detail.DetailGeometry.SetScale(1, doc.ModelUnitSystem, 1 / scaleFactor, doc.PageUnitSystem);
                                            detail.CommitChanges();
                                        }

                                        if (targetPoints.Count > 0)
                                    {
                                        var myFocalPoint = targetPoints[indexValue];
                                        detail.Viewport.SetCameraTarget(myFocalPoint, true);
                                        detail.CommitViewportChanges();
                                    }

                                    if (displayMode != null)
                                    {
                                        var viewportDisplay = Rhino.Display.DisplayModeDescription.FindByName(displayMode);
                                        detail.Viewport.DisplayMode = viewportDisplay;
                                        detail.CommitViewportChanges();
                                    }
                                    pageView.Redraw();


                                        if(namedViews.Count > 0)
                                        {
                                            
                                            var myViewIndex = doc.NamedViews.FindByName(namedViews[indexValue]);
                                            var myView = doc.NamedViews[myViewIndex];
                                           
                                           
                                            var location = myView.Viewport.CameraLocation;
                                            var target = myView.Viewport.CameraDirection;
                                            var tagetP = myView.Viewport.TargetPoint;
                                            bool isPer = myView.Viewport.IsPerspectiveProjection;
                                            bool isTwo = myView.Viewport.IsTwoPointPerspectiveProjection;
                                            bool isPara = myView.Viewport.IsParallelProjection;
                                            
                                            

                                            var projection = Rhino.Display.DefinedViewportProjection.Perspective; 

                                            if(isPer)
                                            {
                                                projection = Rhino.Display.DefinedViewportProjection.Perspective;
                                            }
                                            else if (isTwo)
                                            {
                                                projection = Rhino.Display.DefinedViewportProjection.TwoPointPerspective;
                                            }
                                            else if (isPara)
                                            {
                                                projection = Rhino.Display.DefinedViewportProjection.Top;
                                            }


                                            detail.Viewport.SetProjection(projection, namedViews[indexValue] , false);

                                           // detail.Viewport.CameraUp = myView.Viewport.CameraUp;
                                            detail.Viewport.SetCameraLocation(location, true);
                                           
                                            detail.Viewport.SetCameraTarget(tagetP, false);
                                           detail.Viewport.SetCameraDirection(target, false);

                                            detail.CommitViewportChanges();
                                            detail.CommitChanges();
                                            pageView.Redraw();
                                           
                                           

                                        }

                                     




                                    }





                                }


                                indexValue++;

                            }
                         
                            



                        }
                    }
                    indexValue = 0; 
                }
               
            }
            runBool_2 = false;
        }





            //    foreach (string detailName in detailNames)
            //    {
            //        var myNamedView = namedViews[indexValue];

            //        foreach (var pageView in pageViews)
            //        {
            //            var details = pageView.GetDetailViews();

            //            for (int j = 0; j < details.Length; j++)
            //            {

            //                var detail = details[j];

            //                if (detail.Name == detailName)
            //                {

            //                    var myFocalPoint = targetPoints[indexValue];

            //                    if (noPointsInList)
            //                    {
            //                         myFocalPoint = detail.Viewport.CameraTarget;
                                  
            //                    }
                                
                                

            //                    detail.DetailGeometry.SetScale(1, doc.ModelUnitSystem, 1/scaleFactor, doc.PageUnitSystem);
            //                    detail.CommitChanges();
            //                    detail.Viewport.SetCameraTarget(myFocalPoint, true);
            //                    detail.CommitViewportChanges();
                                

            //                    var viewportDisplay = Rhino.Display.DisplayModeDescription.FindByName(displayMode);
            //                    detail.Viewport.DisplayMode = viewportDisplay;
            //                    detail.CommitViewportChanges();
            //                    pageView.Redraw();

            //                    if( myNamedView.Length>0)
            //                    {
            //                        var myViewIndex = doc.NamedViews.FindByName(myNamedView);
            //                        var myView = doc.NamedViews[myViewIndex];
            //                        var location = myView.Viewport.CameraLocation;
            //                        var target = myView.Viewport.CameraDirection;

            //                        detail.Viewport.CameraUp = Rhino.Geometry.Vector3d.ZAxis;
            //                        detail.Viewport.SetCameraLocation(location, true);
            //                        detail.Viewport.SetCameraDirection(target, true);
            //                        detail.CommitViewportChanges();
            //                        detail.CommitChanges();
            //                        pageView.Redraw();
            //                    }
            //                    else
            //                    {

            //                    }
                             

            //                }



            //            }

            //            doc.Views.Redraw();

            //        }

            //        indexValue++;



            //    }

            //}

          //  runBool_2 = false; 



      //  }

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
            get { return new Guid("d94da05f-149d-4da2-b2fe-e902ea78f48e"); }
        }
    }
}
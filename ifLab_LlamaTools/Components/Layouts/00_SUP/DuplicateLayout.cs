//using Grasshopper.Kernel;
//using Rhino;
//using Rhino.Geometry;
//using System;
//using System.Collections.Generic;


//namespace ifLab_LlamaTools.Components.Layouts
//{

//    public class DuplicateLayout : GH_Component
//    {

//        GH_Document GrasshopperDocument;
//        IGH_Component Component;


//        /// <summary>
//        /// Initializes a new instance of the GhcCreateLayout class.
//        /// </summary>
//        public DuplicateLayout()
//          : base("Duplicate Layout", "Duplicate Layout",
//              "Duplicate Layout",
//              "LLama Tools", "Layout Tools")
//        {
//        }


//        /// <summary>
//        /// Registers all the input parameters for this component.
//        /// </summary>
//        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
//        {
//            pManager.AddBooleanParameter("Run Function", "Run", "Input boolean to run function", GH_ParamAccess.item);
//            pManager.AddTextParameter("Template Layout Name", "[] Template Layout ", "Input Layout name that you wish to duplicate (Value List Compatible)", GH_ParamAccess.item);
//            pManager.AddTextParameter("Duplicate Layout Names", "Copy Names", "Input copy layout names", GH_ParamAccess.list);
//            pManager.AddTextParameter("______________", "______________", "___", GH_ParamAccess.item); 
//            pManager.AddTextParameter("Name of Detail to replicate", "Name of Detail", "input the name of the detail that you with to replicate", GH_ParamAccess.item);
//            pManager.AddPointParameter("Target Point", "Target Point", "Input the target points to focus on", GH_ParamAccess.list);
//            pManager.AddNumberParameter("Scale Factor", "Scale", "input the scale factor", GH_ParamAccess.item);
//            pManager.AddTextParameter("Dispaly Mode", "Display", "Text param that defines the display mode used", GH_ParamAccess.item);
//            pManager.AddTextParameter("Text User Name", "Text Name", "User Name of the text object to be replaced (Case Sensitive)", GH_ParamAccess.item);
//            pManager.AddTextParameter("Copy Names", "Copy Names", "The replacement name for the copied text object", GH_ParamAccess.list);

//            //pManager[0].Optional = true;
//             pManager[1].Optional = true;
//            // pManager[2].Optional = true;
//            pManager[3].Optional = true;
//            pManager[4].Optional = true;
//            pManager[5].Optional = true;
//            pManager[6].Optional = true;
//            pManager[7].Optional = true;
//            pManager[8].Optional = true;
//            pManager[9].Optional = true;
//        }

//        /// <summary>
//        /// Registers all the output parameters for this component.
//        /// </summary>
//        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
//        {
//            //pManager.AddTextParameter("Output Message", "Output Message", "Output Message", GH_ParamAccess.item);

//        }

//        /// <summary>
//        /// This is the method that actually does the work.
//        /// </summary>
//        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
//        protected override void SolveInstance(IGH_DataAccess DA)
//        {


//            bool runBoolean = false;
//            string templateName = "Page 1";
//            List<string> copyLayoutNames = new List<string>();
//            string namedView = "top";
//            List<Point3d> targetPoints = new List<Point3d>();
//            double scaleFactor = 1;
//            string displayMode = "";

//            string textName = "";
//            List<string> copyText = new List<string>(); 
            



//            DA.GetData(0, ref runBoolean);
//            DA.GetData(1, ref templateName);
//            DA.GetDataList<string>(2, copyLayoutNames);
//            DA.GetData(4, ref namedView);
//            DA.GetDataList<Point3d>(5, targetPoints);
//            DA.GetData(6, ref scaleFactor);
//            DA.GetData(7, ref displayMode);
//            DA.GetData(8, ref textName);
//            DA.GetDataList<string>(9, copyText);


//            bool runBoolean_2 = false; 
//            if(runBoolean)
//            {
//                runBoolean_2 = true;
//                runBoolean = false; 
//            }

//            RhinoApp.Wait(); 
//            //test for number of of names and number of points being equal 


//            if(copyLayoutNames.Count != targetPoints.Count && targetPoints.Count > 0)
//            {

//                runBoolean_2 = false;
//                DA.SetData(0, "Number of Points Must Match Number of Copy Views");
//            }


//            var doc = Rhino.RhinoDoc.ActiveDoc;

//            // get document page views

//            var views = doc.Views.GetPageViews();
//            List<string> viewNames = new List<string>();
//            foreach (var view in views)
//            {
//                viewNames.Add(view.PageName);
//            }

//            ////// Auto Update Value list    - need to change input list, location of active doc, and add gh component to and grasshopper document to the start of script

//            var inputList = viewNames;

//            Component = this;
//            GrasshopperDocument = this.OnPingDocument();

//            foreach (IGH_Param source in this.Component.Params.Input[1].Sources)
//            {
//                if (source is Grasshopper.Kernel.Special.GH_ValueList && source.NickName != "Layouts")
//                {
//                    // set the variable vl to the source object as a grasshopper value list 
//                    Grasshopper.Kernel.Special.GH_ValueList vl = source as Grasshopper.Kernel.Special.GH_ValueList;

//                    vl.NickName = "Layouts"; 

//                    //clear the current contents of the value list
//                    vl.ListItems.Clear();

//                    if (inputList.Count == 0)
//                    {
//                        vl.NickName = "Layouts";
//                        vl.ListItems.Add(new Grasshopper.Kernel.Special.GH_ValueListItem("No Layouts", null));
//                        vl.ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.CheckList;
//                    }
//                    // for each data item add value and key to value list 
//                    for (int i = 0; i < inputList.Count; i++)
//                    {

//                        // select the type of value list to display 
//                        vl.ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.DropDown;

//                        // add the items to the value list 
//                        vl.ListItems.Add(new Grasshopper.Kernel.Special.GH_ValueListItem(inputList[i], "\"" + inputList[i] + "\""));

//                    }



//                }

//            }

//            /////


//            //// get display styles 

            
//            var displayModes = Rhino.Display.DisplayModeDescription.GetDisplayModes();
//            List<string> displayNames = new List<string>(); 

//            foreach ( var dM in displayModes)
//            {
//                displayNames.Add(dM.EnglishName); 
//            }

           
//            ////// Auto Update Value list    - need to change input list, location of active doc, and add gh component to and grasshopper document to the start of script

//            var inputList2 = displayNames;

//            Component = this;
//            GrasshopperDocument = this.OnPingDocument();

//            foreach (IGH_Param source in this.Component.Params.Input[6].Sources)
//            {
//                if (source is Grasshopper.Kernel.Special.GH_ValueList && source.NickName != "DisplayModes")
//                {
//                    // set the variable vl to the source object as a grasshopper value list 
//                    Grasshopper.Kernel.Special.GH_ValueList vl = source as Grasshopper.Kernel.Special.GH_ValueList;

//                    vl.NickName = "DisplayModes";

//                    //clear the current contents of the value list
//                    vl.ListItems.Clear();

//                    // for each data item add value and key to value list 
//                    for (int i = 0; i < inputList2.Count; i++)
//                    {

//                        // select the type of value list to display 
//                        vl.ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.DropDown;

//                        // add the items to the value list 
//                        vl.ListItems.Add(new Grasshopper.Kernel.Special.GH_ValueListItem(inputList2[i], "\"" + inputList2[i] + "\""));

//                    }



//                }

//            }

//            /////




//            if (runBoolean_2)
//            {
//                //set active doc, set active page unit system to millimeters, get active Views
//                 doc = RhinoDoc.ActiveDoc;
//                doc.PageUnitSystem = Rhino.UnitSystem.Millimeters;
                 
//                // get the number o pages to copy and get document page views
//                int numberOfPages = copyLayoutNames.Count;
//                var pageViews = doc.Views.GetPageViews();

               

//               //find the page views with the same name as template view
//                foreach (var pageView in pageViews)
//                {
//                    for (int k = 0; k < copyLayoutNames.Count; k++)
//                    {
//                        if (pageView.PageName == copyLayoutNames[k])
//                        {
//                            pageView.Close();
//                        }
//                    }

//                    if (templateName == pageView.PageName)
//                    {

//                        int indexValue = 0;
//                        for (int i = 0; i < numberOfPages; i++)
//                        {
                            
//                            var newPage = pageView.Duplicate(true);
//                            newPage.PageName = copyLayoutNames[indexValue];

                           

//                            //Try to add a text updater //

//                            // set active detail 
//                            newPage.SetPageAsActive();
//                            var activeViewId = newPage.ActiveViewportID;
//                          if(textName != "")
//                            { 
                           
//                            // set up a object search filter for current viewport
//                            Rhino.DocObjects.ObjectEnumeratorSettings settings = new Rhino.DocObjects.ObjectEnumeratorSettings();
//                            settings.NameFilter = textName;
//                            settings.ObjectTypeFilter = Rhino.DocObjects.ObjectType.Annotation; 
//                            List<Rhino.DocObjects.TextObject> textItems = new List<Rhino.DocObjects.TextObject>(); 
//                            foreach (Rhino.DocObjects.TextObject textItem in doc.Objects.GetObjectList(settings))
//                               textItems.Add(textItem);

//                            Rhino.Geometry.TextEntity textEntity;

//                            int myIndex = 0;

                                
//                                    foreach (var textItem in textItems)
//                                    {
//                                        if (textItem.IsActiveInViewport(Rhino.Display.RhinoViewport.FromId(activeViewId)))
//                                        {
//                                            textEntity = textItem.Geometry as Rhino.Geometry.TextEntity;
//                                            string myString = copyText[indexValue];
//                                            textEntity.RichText = myString;
//                                            textItem.CommitChanges();
//                                        }
//                                        textItem.CommitChanges();
//                                        myIndex++;
//                                    }

//                           }


//                            var details = newPage.GetDetailViews();
//                            for (int j = 0; j<details.Length; j++)
//                           {
//                                var detail = details[j];
                              
//                                if  (detail.Name == namedView)
//                                {
//                                    var myFocalPoint = targetPoints[indexValue];
                                   
//                                    detail.DetailGeometry.SetScale(1, doc.ModelUnitSystem, 1/scaleFactor, doc.PageUnitSystem);
//                                    detail.CommitChanges();
//                                    detail.CommitViewportChanges(); 
//                                    if (myFocalPoint != null)
//                                    {
//                                        detail.Viewport.SetCameraTarget(myFocalPoint, true);
//                                        detail.CommitChanges();
//                                        detail.CommitViewportChanges();
//                                    }
//                                    if (displayMode != "")
//                                    {
//                                        var viewportDisplay = Rhino.Display.DisplayModeDescription.FindByName(displayMode);
//                                        detail.Viewport.DisplayMode = viewportDisplay;
//                                        detail.CommitViewportChanges();
//                                    }
//                                    newPage.Redraw();

//                                }


//                            }
                        

//                            indexValue++; 
//                        }


//                    }

//                }

//                doc.Views.Redraw();

//                ///// Add Text editor here
//                ///

               

//              //  DA.SetData(0, "sucess");


//            }


//            runBoolean_2 = false;
            

//            return;



//        }

//        /// <summary>
//        /// Provides an Icon for the component.
//        /// </summary>
//        protected override System.Drawing.Bitmap Icon
//        {
//            get
//            {
//                //You can add image files to your project resources and access them like this:
//                // return Resources.IconForThisComponent;
//                return ifLab_LlamaTools.Resources.IconResources.DuplicateLayout;
//            }
//        }

//        /// <summary>
//        /// Gets the unique ID for this component. Do not change this ID after release.
//        /// </summary>
//        public override Guid ComponentGuid
//        {
//            get { return new Guid("638df8ab-da9b-49a9-a32d-b21c21ee15a9"); }
//        }
//    }
//}
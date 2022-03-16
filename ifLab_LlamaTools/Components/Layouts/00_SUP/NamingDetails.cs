//using System;
//using System.Collections.Generic;

//using Grasshopper.Kernel;
//using Rhino.Geometry;

//namespace ifLab_LlamaTools.Components.Layouts
//{
//    public class NamingDetails : GH_Component
//    {
//        /// <summary>
//        /// Initializes a new instance of the NamingDetails class.
//        /// </summary>
//        public NamingDetails()
//          : base("NamingDetails", "Naming Details",
//              "This component names details with a title and a scale ",
//              "LLama Tools", "Layout Tools")
//        {
//        }

//        /// <summary>
//        /// Registers all the input parameters for this component.
//        /// </summary>
//        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
//        {
//            pManager.AddBooleanParameter("Run?", "Run?", "Run Function", GH_ParamAccess.item);
//        }

//        /// <summary>
//        /// Registers all the output parameters for this component.
//        /// </summary>
//        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
//        {
//            // pManager.AddTextParameter()

//        }

//        /// <summary>
//        /// This is the method that actually does the work.
//        /// </summary>
//        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
//        protected override void SolveInstance(IGH_DataAccess DA)
//        {


//            bool runBool = false;
//            bool runBool_2 = false;
//            List<string> layoutNames = new List<string>();
//            List<string> scale = new List<string>();
//            List<string> namedView = new List<string>();
//            List<int> numberOfViews = new List<int>();


//            int cornerIndex = 0;
//            string detailTitle = "Title";
//            double offsetValue = 15;  

//            string subTitle = "Sub Title";

//            string styleName = "Default"; 



//            DA.GetData(0, ref runBool);


//            if (runBool)
//            {
//                runBool_2 = true;
//                runBool = false;

//            }
//            Rhino.RhinoApp.Wait();
//            if (runBool_2)
//            {
//                var doc = Rhino.RhinoDoc.ActiveDoc;


//                var pageViews = doc.Views.GetPageViews();

//                int myIndex = 0;
//                foreach (var pageView in pageViews)
//                {

//                    pageView.SetPageAsActive();

//                    // set up a object search filter for current viewport
//                    Rhino.DocObjects.ObjectEnumeratorSettings settings = new Rhino.DocObjects.ObjectEnumeratorSettings();
//                    settings.ViewportFilter = pageView.ActiveViewport;
//                    settings.ObjectTypeFilter = Rhino.DocObjects.ObjectType.Detail;

//                    List<Rhino.DocObjects.DetailViewObject> details = new List<Rhino.DocObjects.DetailViewObject>();


//                    foreach (Rhino.DocObjects.DetailViewObject detail in doc.Objects.GetObjectList(settings))
//                        details.Add(detail);


//                    numberOfViews.Add(details.Count);
//                    foreach (var detail in details)
//                    {
//                       // namedView.Add(detail.Viewport.Name);
////
//                       // pageView.SetActiveDetail(detail.Id);
//                       // doc.Views.ActiveView.TitleVisible = true;
                       

//                       var bBox =  detail.DetailGeometry.GetBoundingBox(Rhino.Geometry.Plane.WorldXY);

//                      var bCorners =  bBox.GetCorners();

//                        var titlePlane = Rhino.Geometry.Plane.WorldXY; 
//                        titlePlane.Origin = bCorners[cornerIndex];

                        
                       

//                        Rhino.Geometry.TextEntity.Create(detailTitle, titlePlane, doc.DimStyles.FindName(styleName), true, 20, 0); 
//                        // detail.DetailGeometry;
//                        // namedView.Add(detail.)
//                    }

//                }






//                myIndex++;
//            }





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
//                return null;
//            }
//        }

//        /// <summary>
//        /// Gets the unique ID for this component. Do not change this ID after release.
//        /// </summary>
//        public override Guid ComponentGuid
//        {
//            get { return new Guid("21da60c5-4f06-4432-8ae6-e8576acac103"); }
//        }
//    }
//}
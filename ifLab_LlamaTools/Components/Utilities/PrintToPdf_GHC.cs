using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino;
using Rhino.FileIO; 


namespace ifLab_LlamaTools.Components.Utilities
{
    public class PrintToPdf_GHC : GH_Component
    {

        GH_Document GrasshopperDocument;
        IGH_Component Component;

        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public PrintToPdf_GHC()
          : base("Print to Pdf", "Print To Pdf",
              "Prints all layouts to Pdf",
              "LLama Tools", "Utilities")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "Run", "Run this function", GH_ParamAccess.item);
            
            pManager.AddTextParameter("Folder Location", "Folder Location", "This is the output folder location", GH_ParamAccess.item);

            pManager.AddBooleanParameter("Exclude/Include", "Exclude/Include", "Set to True to pring all except the selected layouts, false to print just selected", GH_ParamAccess.item); 

            pManager.AddTextParameter("Layouts", "Layouts", "The name of the layout to Exclude or Include in the export", GH_ParamAccess.list); 

            pManager.AddTextParameter("Combinded Name", "Combinded Name", "Name used if combining pdf in one document", GH_ParamAccess.item);

            pManager.AddBooleanParameter("Combinded?", "Combined?", "Is the out put file combined or individual", GH_ParamAccess.item); 

            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true; 

            // add boolean for include exclude- this will allow the user to select if they are excluded pages from the print or only printing these pages


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
            string folderLocation = "";
            bool runBool = false;
            bool runBool_2 = false;

            bool exclude = false; 

            bool combine = false;
            string combinedName = "Combined"; 


            DA.GetData(0, ref runBool);
            DA.GetData(1, ref folderLocation);

            DA.GetData(2, ref exclude); 

            List<string> excludeNames = new List<string>();
            DA.GetDataList(3, excludeNames); 

            DA.GetData(4, ref combinedName); 
            if(combinedName == "")
            { combinedName = "Combined"; }

            DA.GetData(5, ref combine);



            //// Auto Update Value list
            //set active doc, set active page unit system to millimeters, get active Views
            var doc = RhinoDoc.ActiveDoc;
            doc.PageUnitSystem = Rhino.UnitSystem.Millimeters;

            // get document page views

            var pageViews = doc.Views.GetPageViews();
            List<string> viewNames = new List<string>();
            foreach (var view in pageViews)
            {
                viewNames.Add(view.PageName);
            }


            var inputList = viewNames;

            Component = this;
            GrasshopperDocument = this.OnPingDocument();

            foreach (IGH_Param source in this.Component.Params.Input[3].Sources)
            {
                if (source is Grasshopper.Kernel.Special.GH_ValueList && source.NickName != "Layouts")
                { // set the variable vl to the source object as a grasshopper value list 
                    Grasshopper.Kernel.Special.GH_ValueList vl = source as Grasshopper.Kernel.Special.GH_ValueList;
                   

                    // Create Nickname 
                    vl.NickName = "Layouts";

                    //clear the current contents of the value list
                    vl.ListItems.Clear();

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



            if (runBool)
            {
                runBool_2 = true;
                runBool = false; 
            }
            Rhino.RhinoApp.Wait(); 

            if(runBool_2)
            {
                runBool_2 = false; 
                 doc = Rhino.RhinoDoc.ActiveDoc;

                if ( folderLocation.EndsWith(@"\") != true)
                {
                    folderLocation = folderLocation + @"\"; 
                }

                 pageViews = doc.Views.GetPageViews();
                List<Rhino.Display.RhinoPageView> pViews = new List<Rhino.Display.RhinoPageView>();
                pViews.AddRange(pageViews);

                if (exclude)
                {
                    for (int i = 0; i < pViews.Count; i++)
                    {
                        foreach (var excludeName in excludeNames)
                            if (pViews[i].PageName == excludeName)
                            {
                                pViews.RemoveAt(i);

                            }
                    }
                }
                else
                {
                    for (int i = 0; i < pViews.Count; i++)
                    {
                        foreach (var excludeName in excludeNames)
                            if (pViews[i].PageName != excludeName)
                            {
                                pViews.RemoveAt(i);

                            }
                    }

                }

                if (combine != true)
                {
                    foreach (var pView in pViews)
                    {
                        

                        var fileName = folderLocation + pView.PageName + ".pdf";
                        var pdf = Rhino.FileIO.FilePdf.Create();
                        var settings = new Rhino.Display.ViewCaptureSettings(pView, 300);
                        pdf.AddPage(settings);
                        pdf.Write(fileName);
                    }


                }
               else
                {
                    var fileName = folderLocation + combinedName + ".pdf";
                    var pdf = Rhino.FileIO.FilePdf.Create();
                    
                    foreach (var pView in pViews)
                    {
                        
                        var settings = new Rhino.Display.ViewCaptureSettings(pView, 300);
                        pdf.AddPage(settings);
                    }
                    pdf.Write(fileName);
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
            get { return new Guid("abc038cf-53c2-472b-911c-1e4ac8526458"); }
        }
    }
}
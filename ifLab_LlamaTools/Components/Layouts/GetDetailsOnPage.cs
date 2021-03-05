using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino; 

namespace ifLab_LlamaTools.Components.Layouts
{
    public class GetDetailsOnPage : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

    
        /// <summary>
        /// Initializes a new instance of the Ghc_GetDetailsOnPage class.
        /// </summary>
        public GetDetailsOnPage()
          : base("Get Details On Page", "Get Details",
              "Get Details on Page",
              "LLama Tools", "Layout Tools")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "Run", "Get details on a page", GH_ParamAccess.item);
            pManager.AddTextParameter("Layout Name", "[] Layout", "The name of the layouts to get details on (Value List Compatible)", GH_ParamAccess.item);

            pManager[1].Optional = true; 
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Detail Names", "Names", "Names of the details on current page", GH_ParamAccess.list); 
        }

        List<string> detailNames = new List<string>(); 

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool runBool = false;
            bool runBool_2 = false;
            DA.GetData(0, ref runBool);

            string layoutName = "";
            DA.GetData(1, ref layoutName);

            var doc = Rhino.RhinoDoc.ActiveDoc;

            // get document page views

            var views = doc.Views.GetPageViews();
            List<string> viewNames = new List<string>();
            foreach (var view in views)
            {
                viewNames.Add(view.PageName);
            }


            ////// Auto Update Value list

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

                    // for each data item add value and key to value list 
                    for (int i = 0; i < inputList.Count; i++)
                    {

                        // select the type of value list to display 
                        vl.ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.DropDown;

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
                Rhino.RhinoApp.Wait(); 
            }


            if(runBool_2)
            {
                detailNames.Clear(); 
                 doc = Rhino.RhinoDoc.ActiveDoc;

                var pageViews = doc.Views.GetPageViews(); 

                foreach ( var pageView in pageViews)
                {
                    if(pageView.PageName == layoutName)
                    {

                        var detailViews = pageView.GetDetailViews();
                        foreach(var detailView in detailViews)
                        {
                            detailNames.Add(detailView.Name);

                        }
                        

                    }


                }

                
                runBool_2 = false;  
            }
            DA.SetDataList(0, detailNames); 

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
                return ifLab_LlamaTools.Resources.IconResources.Artboard_1_copy_8;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("746f70ae-0d4c-48fb-b042-10a547a2619b"); }
        }
    }
}
using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino;
using ifLab_LlamaTools.Core.ComponentInputs;

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
        /// Use this class to set up all the inputs, this will allow you to call them by their name in intellisense 
        /// </summary>
        private static class ComponentInputs
        {
            // list of inputs for the component 
            public static InputObject Refresh = new InputObject("Refresh", "Refresh", "Refresh the output list", GH_ParamAccess.item, false);
            public static InputObject LayoutNames = new InputObject("[] Layout Names", "[] Layout", "Layout Names", GH_ParamAccess.item, true);


        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager[pManager.AddBooleanParameter(ComponentInputs.Refresh.Name, ComponentInputs.Refresh.Nickname, ComponentInputs.Refresh.Desscription, ComponentInputs.Refresh.ParamAccess)].Optional = true;
            ComponentInputs.LayoutNames.ParamNumber = pManager.AddTextParameter(ComponentInputs.LayoutNames.Name, ComponentInputs.LayoutNames.Nickname, ComponentInputs.LayoutNames.Desscription, ComponentInputs.LayoutNames.ParamAccess);
            pManager[ComponentInputs.LayoutNames.ParamNumber].Optional = ComponentInputs.LayoutNames.Optional;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Detail Names", "Names", "Names of the details on current page (null meaning detail is present by does not have a name)", GH_ParamAccess.list); 
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            bool runBool = false;
            DA.GetData(ComponentInputs.Refresh.Name, ref runBool);

            string layoutName = "";
            DA.GetData(ComponentInputs.LayoutNames.Name, ref layoutName);

            #region Set Up Globals

            Component = this;
            GrasshopperDocument = this.OnPingDocument();

            var doc = Rhino.RhinoDoc.ActiveDoc;

           List<string> detailNames = new List<string>(); 
            #endregion

            //add the names to a value list that will allow for the selection of a template view 
            InputFunctions.AutoPopulate_PageViews(doc, this.Component.Params.Input[ComponentInputs.LayoutNames.ParamNumber].Sources, "LayoutNames", Grasshopper.Kernel.Special.GH_ValueListMode.CheckList);


         if(runBool)
                detailNames.Clear(); 

                var pageViews = doc.Views.GetPageViews(); 
                
                foreach ( var pageView in pageViews)
                {
                    if(pageView.PageName == layoutName)
                    {
                        var detailViews = pageView.GetDetailViews();
                    foreach (var detailView in detailViews)
                    {
                      
                            detailNames.Add(detailView.Name);
                       
                    }
                    }
                }

                runBool = false;  
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
                return null;
                //return ifLab_LlamaTools.Resources.IconResources.Artboard_1_copy_8;
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
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using ifLab_LlamaTools.Core.ComponentInputs;
using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// Use this class to set up all the inputs, this will allow you to call them by their name in intellisense 
        /// </summary>
        private static class ComponentInputs
        {
            // list of inputs for the component 
            public static InputObject Run = new InputObject("Run", "Run", "Run the function to create layouts", GH_ParamAccess.item, false);
            public static InputObject LayoutNames =new InputObject("Layout Names", "Names", "Layout Names", GH_ParamAccess.item, false);
            public static InputObject TemplateLayouts = new InputObject("[] Template Layouts", "[] Template", "Use this sheet as a template for the creation of layouts.", GH_ParamAccess.item, true);
            public static InputObject LayoutWidth = new InputObject("Layout Width", "Width", "Layout Width", GH_ParamAccess.item, true);
            public static InputObject LayoutHeight = new InputObject("Layout Height", "Height", "layout Height", GH_ParamAccess.item, true);


            // add detail name as a new param  - either to create with name or edit name 

            public static InputObject DetailName = new InputObject("Detail Name", "Name", "Name of the detail to create or name of the detail to update.", GH_ParamAccess.item, true);
            public static InputObject DetialOffset = new InputObject("Detail Margin", "Margin", "Detail offset from  page edge", GH_ParamAccess.item, true);
            public static InputObject DisplayModes = new InputObject("Display Mode", "[] Display", "the display mode used for the viewport", GH_ParamAccess.item, true);
            public static InputObject TargetPoints = new InputObject("Target Points", "Target Points", "Target to centre the viewport on", GH_ParamAccess.item, true);
            public static InputObject Scale = new InputObject("Scale", "Scale", "Scale of the viewport(1:input value", GH_ParamAccess.item, true);

        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager[  pManager.AddBooleanParameter(ComponentInputs.Run.Name, ComponentInputs.Run.Nickname, ComponentInputs.Run.Desscription, ComponentInputs.Run.ParamAccess )].Optional = false;
            pManager[pManager.AddTextParameter(ComponentInputs.LayoutNames.Name, ComponentInputs.LayoutNames.Nickname, ComponentInputs.LayoutNames.Desscription, ComponentInputs.LayoutNames.ParamAccess)].Optional = false; ;

            ///use this setup to establish components- it allows for the ability to set optional and store the input value in the class 
            StandardComponentInputs.LinedGap.ParamNumber = pManager.AddTextParameter(StandardComponentInputs.LinedGap.Name, StandardComponentInputs.LinedGap.Nickname, StandardComponentInputs.LinedGap.Desscription, StandardComponentInputs.LinedGap.ParamAccess);
            pManager[StandardComponentInputs.LinedGap.ParamNumber].Optional = true;


            ComponentInputs.TemplateLayouts.ParamNumber = pManager.AddTextParameter(ComponentInputs.TemplateLayouts.Name, ComponentInputs.TemplateLayouts.Nickname, ComponentInputs.TemplateLayouts.Desscription, ComponentInputs.TemplateLayouts.ParamAccess);
            pManager[ComponentInputs.TemplateLayouts.ParamNumber].Optional = true; ;

            ComponentInputs.LayoutWidth.ParamNumber = pManager.AddNumberParameter(ComponentInputs.LayoutWidth.Name, ComponentInputs.LayoutWidth.Nickname, ComponentInputs.LayoutWidth.Desscription, ComponentInputs.LayoutWidth.ParamAccess);
            pManager[ComponentInputs.LayoutWidth.ParamNumber].Optional = true; 

            ComponentInputs.LayoutHeight.ParamNumber = pManager.AddNumberParameter(ComponentInputs.LayoutHeight.Name, ComponentInputs.LayoutHeight.Nickname, ComponentInputs.LayoutHeight.Desscription, ComponentInputs.LayoutHeight.ParamAccess);
            pManager[ComponentInputs.LayoutHeight.ParamNumber].Optional = true;

            ComponentInputs.DetialOffset.ParamNumber = pManager.AddNumberParameter(ComponentInputs.DetialOffset.Name, ComponentInputs.DetialOffset.Nickname, ComponentInputs.DetialOffset.Desscription, ComponentInputs.DetialOffset.ParamAccess);
            pManager[ComponentInputs.DetialOffset.ParamNumber].Optional = true;


            //pManager[pManager.AddTextParameter(StandardComponentInputs.EmptyGap.Name, StandardComponentInputs.EmptyGap.Nickname, StandardComponentInputs.EmptyGap.Desscription, StandardComponentInputs.EmptyGap.ParamAccess)].Optional = true; ;
            ComponentInputs.DetailName.ParamNumber = pManager.AddTextParameter(ComponentInputs.DetailName.Name, ComponentInputs.DetailName.Nickname, ComponentInputs.DetailName.Desscription, ComponentInputs.DetailName.ParamAccess);
            pManager[ComponentInputs.DetailName.ParamNumber].Optional = true;

            ComponentInputs.DisplayModes.ParamNumber = pManager.AddTextParameter(ComponentInputs.DisplayModes.Name, ComponentInputs.DisplayModes.Nickname, ComponentInputs.DisplayModes.Desscription, ComponentInputs.DisplayModes.ParamAccess);
            pManager[ComponentInputs.DisplayModes.ParamNumber].Optional = true;

            ComponentInputs.TargetPoints.ParamNumber = pManager.AddPointParameter(ComponentInputs.TargetPoints.Name, ComponentInputs.TargetPoints.Nickname, ComponentInputs.TargetPoints.Desscription, ComponentInputs.TargetPoints.ParamAccess);
            pManager[ComponentInputs.TargetPoints.ParamNumber].Optional = true;

            ComponentInputs.Scale.ParamNumber = pManager.AddNumberParameter(ComponentInputs.Scale.Name, ComponentInputs.Scale.Nickname, ComponentInputs.Scale.Desscription, ComponentInputs.Scale.ParamAccess);
            pManager[ComponentInputs.Scale.ParamNumber].Optional = true; 

        }



        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

         //   pManager.AddTextParameter("LayoutList", "Layout List", "Layouts in document", GH_ParamAccess.list);
        }

    

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            #region Set Up Globals

            Component = this;
            GrasshopperDocument = this.OnPingDocument();

            var doc = Rhino.RhinoDoc.ActiveDoc;

            #endregion

            #region Assign Inputs

            bool runBool = false;
            DA.GetData(ComponentInputs.Run.Name, ref runBool);

            string layoutName = "";
            DA.GetData(ComponentInputs.LayoutNames.Name, ref layoutName);
         

            string templateName = "";
            DA.GetData(ComponentInputs.TemplateLayouts.Name, ref templateName);

            double layoutWidth = 0;
            DA.GetData(ComponentInputs.LayoutWidth.Name, ref layoutWidth);

            double layoutHeight = 0;
            DA.GetData(ComponentInputs.LayoutHeight.Name, ref layoutHeight);



            //Details 

            string detailName = "";
            DA.GetData(ComponentInputs.DetailName.Name, ref detailName);

            double offset = 0;
            DA.GetData(ComponentInputs.DetialOffset.Name, ref offset);

            string displayMode = "";
            DA.GetData(ComponentInputs.DisplayModes.Name, ref displayMode);

            //   List<Point3d> targetPoints = new List<Point3d>();
            Point3d targetPoint = new Point3d();
            DA.GetData(ComponentInputs.TargetPoints.Name,ref  targetPoint);

            double scale = 1;
            DA.GetData(ComponentInputs.Scale.Name, ref scale);


            #endregion

            #region Set up auto populate value lists 


            //auto populate the display modes intot the connected value list
            InputFunctions.AutoPopulate_DisplayModes(doc, this.Component.Params.Input[ComponentInputs.DisplayModes.ParamNumber].Sources, "DisplayModes", GH_ValueListMode.DropDown); 


            //add the names to a value list that will allow for the selection of a template view 
            InputFunctions.AutoPopulate_PageViews(doc, this.Component.Params.Input[ComponentInputs.TemplateLayouts.ParamNumber].Sources,"LayoutNames", Grasshopper.Kernel.Special.GH_ValueListMode.DropDown);



            #endregion


            //run the function
            if (runBool)
            {

             //get all to current layouts in the document 
             var pageViews = doc.Views.GetPageViews();
                //construct a new layout object, the class used to create the layouts 
                LayoutObject LayoutObject = new LayoutObject(doc,
                                                         layoutName,
                                                         templateName,
                                                         layoutWidth,
                                                         layoutHeight,
                                                         offset,
                                                         targetPoint,
                                                         scale,
                                                         displayMode,
                                                         detailName,
                                                         "",
                                                         "",
                                                         pageViews
                                                         );

                LayoutObject.CloseDuplicateLayouts();
                LayoutObject.AddPageView();
                LayoutObject.AddDetailToPageView();
                LayoutObject.SetDetailDisplayMode();
                LayoutObject.SetTargetPointOfDetail();

                LayoutObject.CommitViewChanges();

        
               // runBool = false;


            }

          InputFunctions.ResetButton(this.Component.Params.Input[ComponentInputs.Run.ParamNumber].Sources);
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
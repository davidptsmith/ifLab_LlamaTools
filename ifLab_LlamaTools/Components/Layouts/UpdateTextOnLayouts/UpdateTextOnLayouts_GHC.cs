using Grasshopper.Kernel;
using ifLab_LlamaTools.Core.ComponentInputs;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ifLab_LlamaTools.Components.Layouts.EditTextOnLayouts
{
    public class UpdateTextOnLayout_GHC : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the EditText_GHC class.
        /// </summary>
        public UpdateTextOnLayout_GHC()
          : base("Update Text On Layouts", "Update Text on Layouts",
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
            public static InputObject LayoutNames = new InputObject("[] Layout Names", "[] Layouts", "The layouts to update text values on", GH_ParamAccess.item, true);
            public static InputObject TextNames = new InputObject("Text Name", "Text Name", "The name of the text element to replace the value with in properties", GH_ParamAccess.item, false);
            public static InputObject TextValues = new InputObject("Text Values", "Text Value", "The value of the text to be updated", GH_ParamAccess.item, true);


        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            ComponentInputs.Run.ParamNumber = pManager.AddBooleanParameter(ComponentInputs.Run.Name, ComponentInputs.Run.Nickname, ComponentInputs.Run.Desscription, ComponentInputs.Run.ParamAccess);
            pManager[ComponentInputs.Run.ParamNumber].Optional = true;

            ComponentInputs.LayoutNames.ParamNumber = pManager.AddTextParameter(ComponentInputs.LayoutNames.Name, ComponentInputs.LayoutNames.Nickname, ComponentInputs.LayoutNames.Desscription, ComponentInputs.LayoutNames.ParamAccess);
            pManager[ComponentInputs.LayoutNames.ParamNumber].Optional = true;

            ComponentInputs.TextNames.ParamNumber = pManager.AddTextParameter(ComponentInputs.TextNames.Name, ComponentInputs.TextNames.Nickname, ComponentInputs.TextNames.Desscription, ComponentInputs.TextNames.ParamAccess);
            pManager[ComponentInputs.TextNames.ParamNumber].Optional = true;

            ComponentInputs.TextValues.ParamNumber = pManager.AddTextParameter(ComponentInputs.TextValues.Name, ComponentInputs.TextValues.Nickname, ComponentInputs.TextValues.Desscription, ComponentInputs.TextValues.ParamAccess);
            pManager[ComponentInputs.TextValues.ParamNumber].Optional = true; ;

        }

       
 
        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }


        GH_Document GrasshopperDocument;
        IGH_Component Component;


        List<string> textOnInputViews = new List<string>(); 


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            Component = this;
            GrasshopperDocument = this.OnPingDocument();

            var doc = Rhino.RhinoDoc.ActiveDoc;


            bool runBool = false;
            DA.GetData(ComponentInputs.Run.Name, ref runBool);

            string layoutName = "";
            DA.GetData(ComponentInputs.LayoutNames.Name, ref layoutName);

            string textName = "";
            DA.GetData(ComponentInputs.TextNames.Name, ref textName);


            string textValues = "";
            DA.GetData(ComponentInputs.TextValues.Name, ref textValues);


            //get all to current layouts in the document 
            var pageViews = doc.Views.GetPageViews();
            List<string> pageViewNames = new List<string>();
            foreach (var view in pageViews)
            {
                pageViewNames.Add(view.PageName);
            }

            //add the names to a value list that will allow for the selection of a template view 
            InputFunctions.AutoPopulate_PageViews(doc, this.Component.Params.Input[ComponentInputs.LayoutNames.ParamNumber].Sources, "LayoutNames", Grasshopper.Kernel.Special.GH_ValueListMode.CheckList);

            //get all the layouts in the input 

            List<string> TextOnSheets = getTextNamesFromPageNames(doc, this.Component.Params.Input[ComponentInputs.LayoutNames.ParamNumber].Sources, pageViews);
           
        
                InputFunctions.AutoPopulateInputValuesList(this.Component.Params.Input[ComponentInputs.TextNames.ParamNumber].Sources, "Text On Sheets", TextOnSheets.Distinct().ToList(), Grasshopper.Kernel.Special.GH_ValueListMode.CheckList);
            while (runBool)
            {

                //get layout by name 
                try
                {
                    var selectedPage = pageViews.Where(x => x.PageName == layoutName).First();
                    if (selectedPage == null) return;

                    selectedPage.SetPageAsActive();

                    // set up a object search filter for current viewport
                    Rhino.DocObjects.ObjectEnumeratorSettings settings = new Rhino.DocObjects.ObjectEnumeratorSettings();
                    settings.ViewportFilter = selectedPage.ActiveViewport;
                    settings.ObjectTypeFilter = Rhino.DocObjects.ObjectType.Annotation;

                    var textToUpdate = doc.Objects.GetObjectList(settings).Where(text => text.Name == textName); 

                    foreach(var text in textToUpdate)
                    {
                        //blockTextObjects.Add(blockObject as Rhino.DocObjects.TextObject);
                        var textBlockObject = text as Rhino.DocObjects.TextObject;

                        Rhino.Geometry.TextEntity textEntity;

                        textEntity = textBlockObject.Geometry as Rhino.Geometry.TextEntity;
                      
                        textEntity.RichText = textValues;


                        textBlockObject.CommitChanges();


                    }





                }
                catch
                {

                }

           //get text on layout by name 

           //replace text with value 


           runBool = false;
            }
            InputFunctions.ResetButton(this.Component.Params.Input[ComponentInputs.Run.ParamNumber].Sources);
        }

        private List<string> getTextNamesFromPageNames(Rhino.RhinoDoc doc, IList<IGH_Param> sources, Rhino.Display.RhinoPageView[] pageViews)
        {
            List<string> selectedSheetNames = new List<string>(); 

            //get the inputs from a component  
            foreach (IGH_Param source in sources)
            {
                //        if (source is Grasshopper.Kernel.Special.GH_ValueList && source.NickName != valueListName)
                if (source is Grasshopper.Kernel.Special.GH_ValueList)
                {
                    // set the variable vl to the source object as a grasshopper value list 
                    Grasshopper.Kernel.Special.GH_ValueList valueList = source as Grasshopper.Kernel.Special.GH_ValueList;

                    selectedSheetNames.AddRange( valueList.SelectedItems.Select(x => x.Name));



                }

            }
            List<string> textNames = new List<string>(); 
            foreach(var layoutName in selectedSheetNames)
            {
                //get the list of selected pages view names 
                try
                {

                    var textPage = pageViews.Where(x => x.PageName == layoutName).First();

                    Rhino.DocObjects.ObjectEnumeratorSettings textonpagesettings = new Rhino.DocObjects.ObjectEnumeratorSettings();
                    textonpagesettings.ViewportFilter = textPage.ActiveViewport;
                    textonpagesettings.ObjectTypeFilter = Rhino.DocObjects.ObjectType.Annotation;

                    textNames.AddRange(doc.Objects.GetObjectList(textonpagesettings).Where(text => text.Name != null).Select(x => x.Name));

                }
                catch { }
            }

            return textNames; 
            //get the text on this layout 


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
            get { return new Guid("be2c615c-72fe-4357-9539-25978734c8db"); }
        }
    }
}
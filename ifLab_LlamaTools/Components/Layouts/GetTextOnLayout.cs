using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper;
using System.Drawing;

namespace ifLab_LlamaTools.Components.Layouts
{
    public class GetTextOnLayout : GH_Component
    {

        GH_Document GrasshopperDocument;
        IGH_Component Component;

        /// <summary>
        /// Initializes a new instance of the GhcGetTextOnLayout class.
        /// </summary>
        public GetTextOnLayout()
          : base("Get Text On Layout", "Get Text",
              "Description",
              "LLama Tools", "Layout Tools")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddBooleanParameter("Run", "Run", "This runs function", GH_ParamAccess.item);
            pManager.AddTextParameter("Layout Name", "[] LayoutName", "Name of the layout to get text from (Value List Compatible)", GH_ParamAccess.list);
            //Maybe update this into a list parameter and output a data tree
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Names of Text Items", "Text Names", "Name of text items on current layout", GH_ParamAccess.list);
            pManager.AddTextParameter("Current Text", "Current Text", "The Current Text in the retreived text item", GH_ParamAccess.list);

           
        }




        List<string> outputTextNames = new List<string>();
        List<string> outputText = new List<string>();


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

          
            


            bool runBool = false;
            bool runBool_2 = false;
            List<string> layoutNames = new List<string>();
            

            DA.GetData(0, ref runBool);
            DA.GetDataList(1,  layoutNames);

            // test 
            //instantiate  new value list

           
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

                    if (inputList.Count == 0)
                    {
                        vl.NickName = "Layouts";
                        vl.ListItems.Add(new Grasshopper.Kernel.Special.GH_ValueListItem("No Layouts", "No Layouts"));
                        vl.ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.CheckList;
                    }

                    // for each data item add value and key to value list 
                    for (int i = 0; i < inputList.Count; i++)
                    {
                       
                        // select the type of value list to display 
                        vl.ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.DropDown;

                        // add the items to the value list 
                        vl.ListItems.Add(new Grasshopper.Kernel.Special.GH_ValueListItem(inputList[i], inputList[i]));

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
                 doc = Rhino.RhinoDoc.ActiveDoc;
                
                int myIndex = 0; 
                foreach (var layoutName in layoutNames)
                {
                    var pageViews = doc.Views.GetPageViews();

                    foreach(var pageView in pageViews)
                    {
                        if(pageView.PageName == layoutName)
                        {
                            pageView.SetPageAsActive();

                            // set up a object search filter for current viewport
                            Rhino.DocObjects.ObjectEnumeratorSettings settings = new Rhino.DocObjects.ObjectEnumeratorSettings();
                            settings.ViewportFilter = pageView.ActiveViewport;
                            settings.ObjectTypeFilter = Rhino.DocObjects.ObjectType.Annotation;


                            List<Rhino.DocObjects.TextObject> textItems = new List<Rhino.DocObjects.TextObject>();


                            foreach (Rhino.DocObjects.TextObject textItem in doc.Objects.GetObjectList(settings))
                                textItems.Add(textItem);
                            outputText.Clear();
                            outputTextNames.Clear();
                            foreach ( var textItem in textItems)
                            {
                                if( textItem.Name != null)
                                {
                                    outputTextNames.Add(textItem.Name);
                                    outputText.Add(textItem.TextGeometry.RichText);
                                }
                            

                            }


                        }
                    }
                    
                    myIndex++;
                }

               
                runBool_2 = false; 
            }

            DA.SetDataList(0, outputTextNames);
            DA.SetDataList(1, outputText);


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
                return ifLab_LlamaTools.Resources.IconResources.Artboard_1_copy_4;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("5f46556c-ad6c-435b-bce2-784414908419"); }
        }
    }
}
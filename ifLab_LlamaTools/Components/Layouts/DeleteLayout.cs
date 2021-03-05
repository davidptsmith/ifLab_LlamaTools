using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino; 

namespace ifLab_LlamaTools.Components.Layouts
{
    public class DeleteLayout : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;

        /// <summary>
        /// Initializes a new instance of the Ghc_DeleteLayout class.
        /// </summary>
        public DeleteLayout()
          : base("Delete Layout", "Delete Layout",
              "Delete a layout from the document",
              "LLama Tools", "Layout Tools")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run Function", "Delete Selected?", "Input boolean to run function", GH_ParamAccess.item);
            pManager.AddTextParameter("[] Layout Names", "Layout Names", "Name of Layouts to delete (Values List Compatible)", GH_ParamAccess.list);
            pManager.AddTextParameter("[] Except Layouts", "Except", "Layouts not deleted through delete all (Values List Compatible", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Delete All?", "Delete All?", "Delete All", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
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
            List<string> layoutNames = new List<string>();
            List<string> exceptName = new List<string>();
            bool deleteAll = false; 

            DA.GetData(0, ref runBool);
            DA.GetDataList(1, layoutNames);
            DA.GetDataList(2,  exceptName); 
            DA.GetData(3, ref deleteAll); 
            

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



            //////// Auto Update Value list

            var inputList = viewNames;

            Component = this;
            GrasshopperDocument = this.OnPingDocument();

            foreach (IGH_Param source in this.Component.Params.Input[1].Sources)
            {
                if (source is Grasshopper.Kernel.Special.GH_ValueList && source.NickName != "Layouts")
                {
                    // set the variable vl to the source object as a grasshopper value list 
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

            ///

            //// Auto Update Value list

          //  var inputList = viewNames;

            Component = this;
            GrasshopperDocument = this.OnPingDocument();

            foreach (IGH_Param source in this.Component.Params.Input[2].Sources)
            {
                if (source is Grasshopper.Kernel.Special.GH_ValueList && source.NickName != "Layouts" )
                {
                    // set the variable vl to the source object as a grasshopper value list 
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




            bool deleteAll_2 = false; 

            if (deleteAll)
            {
                deleteAll_2 = true;
                deleteAll = false; 
            }
            deleteAll = false;
            RhinoApp.Wait(); 

            if(deleteAll_2)
            {
                List<string> deleteList = new List<string>(); 
                foreach (var pageView in pageViews)
                {
                    deleteList.Add(pageView.PageName);
                    
                }


                foreach (string exceptItem in exceptName)
                {
                    deleteList.Remove(exceptItem);

                }
                
                foreach (var pageView in pageViews)
                {
                    for (int k = 0; k < deleteList.Count; k++)
                    {
                        if (pageView.PageName == deleteList[k])
                        {
                            pageView.Close();
                        }
                    }
                }
                // pageView.Close();
            }

           

            bool runBool_2 = false; 
            if(runBool)
            {
                runBool_2 = true;
                runBool = false; 

            }
            if (runBool_2)
            {
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

                doc.Views.Redraw();
               

            }
            runBool_2 = false;
            deleteAll_2 = false; 
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
                return ifLab_LlamaTools.Resources.IconResources.DeleyeLayout_08;
                
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("89cb6a3c-28eb-4597-bb4c-09d72f43570a"); }
        }
    }
}
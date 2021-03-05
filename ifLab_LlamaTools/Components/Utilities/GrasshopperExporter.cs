using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino; 


namespace ifLab_LlamaTools.Components.Utilities
{
    public class GrasshopperExporter : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Ghc_GrasshopperExporter class.
        /// </summary>
        public GrasshopperExporter()
      : base("Exporter", "Export Objects",
          "Exports Objects to specified file format",
          "LLama Tools", "Utilities")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {


            pManager.AddBooleanParameter("Run?", "Run", "Run Boolean", GH_ParamAccess.item);
            pManager.AddGeometryParameter("Object", "Objects", "Input Objects to transfer to Inventor", GH_ParamAccess.tree);
            pManager.AddTextParameter("File Location", "File Location", "Folder location for objects to be saved", GH_ParamAccess.item);
            pManager.AddTextParameter("File type", "File Type", "The File Type used to transfer the geometry to inventor ", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Keep Bake", "Bake", "Keep the exported elements in a new layer", GH_ParamAccess.item);  

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

            // Set up Variables 
            bool runBool = false;
            bool runBool2 = false;

            // List<Rhino.Geometry.GeometryBase> inputBreps = new List<Rhino.Geometry.GeometryBase>();
            GH_Structure<IGH_GeometricGoo> inputBreps = new GH_Structure<IGH_GeometricGoo>(); 
            

            string @fileLocationDis = "";

            string tag = "Exported_Objects";
            string exportType = ".stp";

            bool keepBake = true; 


            // Assign Variables to inputs
            DA.GetData(0, ref runBool);
            DA.GetDataTree(1, out inputBreps);
            DA.GetData(2, ref @fileLocationDis);
            DA.GetData(3, ref exportType);
            DA.GetData(4, ref keepBake); 





            // This is the itterative function that will drive the inventor application, 
            //each part needs to be opened, created from sketch, folded, exported, and closed 
            // then the itteration count will move through each number of panels (these will be concatenated with export and have corresponding csv and autocad doc. 

            ////////////////////////////////////////////////////////////////////        setup up new methods here     ///////////////////////////////////////////

            if (runBool)
            {
                runBool2 = true;
                runBool = false;

            }
            Rhino.RhinoApp.Wait();
           // int itt = 0;
            if (runBool2)
            {
                //set active doc, set active page unit system to millimeters, get active Views
                var doc = RhinoDoc.ActiveDoc;
                doc.PageUnitSystem = Rhino.UnitSystem.Millimeters;


                
                for (int j =0; j < inputBreps.Branches.Count; j++)
                {
                    var breps = inputBreps[j]; 

                   

                    if (doc == null)
                    { return; }


                    //<Custom additional code>
                    int ensureLayer(string lay)
                    {

                        int i = doc.Layers.Find(lay, true);
                        if (i < 0)
                            return doc.Layers.Add(lay, System.Drawing.Color.Black);
                        else
                            return i;
                    }

                    
                    List<Guid> brepGuids = new List<Guid>(); 

                    foreach (var brep in breps)
                    {

                        Rhino.DocObjects.ObjectAttributes att = new Rhino.DocObjects.ObjectAttributes();
                        att.Name = tag;
                        att.LayerIndex = ensureLayer(tag);


                        Rhino.Geometry.GeometryBase myBrep = brep as Rhino.Geometry.GeometryBase; 
                       // Rhino.Geometry.GeometryBase myBrep = brep.

                        var bakedBrep = doc.Objects.Add(myBrep, att);
                        brepGuids.Add(bakedBrep); 
                    }
                    

                    doc.Objects.Select(brepGuids,true);
                    

                    //List<Guid> guidList = new List<Guid>();

                    Rhino.DocObjects.Tables.ObjectTable ot = Rhino.RhinoDoc.ActiveDoc.Objects;


                    //     int nSelected = ot.Select(guidList);

                    string cmd = "-_Export " + @fileLocationDis + "\\Part_" + j + exportType + " _Enter";
                    Rhino.RhinoApp.RunScript(cmd, false);


                    if(keepBake != true)
                    {
                        ot.Delete(brepGuids, false);
                        var layer = doc.Layers.Find(tag, true);
                        doc.Layers.Delete(layer, true); 
                    }

                    brepGuids.Clear(); 

                   // itt++;
                }

                runBool2 = false; 
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
                return ifLab_LlamaTools.Resources.IconResources.ExportCAD_11;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("68479119-9555-4af0-a8f0-80e4f53c77c8"); }
        }
    }
}
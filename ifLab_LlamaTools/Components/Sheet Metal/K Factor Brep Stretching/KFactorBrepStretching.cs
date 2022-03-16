using Grasshopper.Kernel;
using ifLab_LlamaTools.Core.ComponentInputs;
using Rhino.Geometry;
using Rhino.Geometry.Morphs;
using System;
using System.Collections.Generic;

namespace ifLab_LlamaTools.Components.Sheet_Metal
{
    public class KFactorBrepStretching : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the KFactorBrepStretching class.
        /// </summary>
        public KFactorBrepStretching()
          : base("KFactorBrepStretching", "Brep Stretching",
              "Description",
               "Llama Tools", "Sheet Metal")
        {
        }

        /// <summary>
        /// Use this class to set up all the inputs, this will allow you to call them by their name in intellisense 
        /// </summary>
        private static class ComponentInputs
        {
            // list of inputs for the component 
            public static InputObject Breps = new InputObject("Input Breps", "Breps", "Unrolled, filleted breps for stretching", GH_ParamAccess.item, false);
            public static InputObject Axis = new InputObject("Stretch Axis", "Axis", "The axix to stretch the brep on", GH_ParamAccess.list, false);
            public static InputObject lengths = new InputObject("Actual Lengths", "Length", "the actual length of the axis", GH_ParamAccess.list, false);
       

        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            ComponentInputs.Breps.ParamNumber = pManager.AddBrepParameter(ComponentInputs.Breps.Name, ComponentInputs.Breps.Nickname, ComponentInputs.Breps.Desscription, ComponentInputs.Breps.ParamAccess);
            pManager[ComponentInputs.Breps.ParamNumber].Optional = false;

            ComponentInputs.Axis.ParamNumber = pManager.AddLineParameter(ComponentInputs.Axis.Name, ComponentInputs.Axis.Nickname, ComponentInputs.Axis.Desscription, ComponentInputs.Axis.ParamAccess);
            pManager[ComponentInputs.Axis.ParamNumber].Optional = false;

            ComponentInputs.lengths.ParamNumber = pManager.AddNumberParameter(ComponentInputs.lengths.Name, ComponentInputs.lengths.Nickname, ComponentInputs.lengths.Desscription, ComponentInputs.lengths.ParamAccess);
            pManager[ComponentInputs.lengths.ParamNumber].Optional = false;
        }
        /// <summary>
        /// Use this class to set up all the inputs, this will allow you to call them by their name in intellisense 
        /// </summary>
        private static class ComponentOutputs
        {
            // list of inputs for the component 
            public static InputObject Breps = new InputObject(" Breps", "Breps", "Unrolled, filleted breps for stretching", GH_ParamAccess.item, false);
            public static InputObject points = new InputObject(" pts", "pts", "Unrolled, filleted breps for stretching", GH_ParamAccess.list, false);
            public static InputObject lines = new InputObject("line", "line", "Unrolled, filleted breps for stretching", GH_ParamAccess.list, false);
    


        }
        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter(ComponentOutputs.Breps.Name, ComponentOutputs.Breps.Nickname, ComponentOutputs.Breps.Desscription, ComponentOutputs.Breps.ParamAccess);
            pManager.AddBrepParameter(ComponentOutputs.points.Name, ComponentOutputs.points.Nickname, ComponentOutputs.points.Desscription, ComponentOutputs.points.ParamAccess);
            pManager.AddLineParameter(ComponentOutputs.lines.Name, ComponentOutputs.lines.Nickname, ComponentOutputs.lines.Desscription, ComponentOutputs.lines.ParamAccess);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            var doc = Rhino.RhinoDoc.ActiveDoc;

            //estalish inputs 
            Brep brepToStretch = new Brep();
            DA.GetData(ComponentInputs.Breps.Name, ref brepToStretch); 

            List<Line> stretchingAxis = new List<Line>();
            DA.GetDataList(ComponentInputs.Axis.Name, stretchingAxis); 

            List<double> actualAxisLength = new List<double>();
            DA.GetDataList(ComponentInputs.lengths.Name, actualAxisLength);



            List<Point3d> outPoints = new List<Point3d>();

            //check for issues at the start 
            if (stretchingAxis.Count != actualAxisLength.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The list of axis and lengths are not equal and the componenet cannot run");
                return; 
             }
            if(!brepToStretch.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The supplied brep is not valid, the component could not run");
                return;
            }
            string test = "";
            var ss =     brepToStretch.IsValidTopology(out test);
         

            KfactorBrepStrechingObj strechingObj = new KfactorBrepStrechingObj(brepToStretch, stretchingAxis, actualAxisLength);

            //strechingObj.GetStretchingFaces();

            //strechingObj.StretchFaces();

            //strechingObj.ReassembleFaces();
        


            //explode the brep and get the faces and their centre point as a list 
            //  var brepFaces = brepToStretch.Faces;


         //   Brep outBrep = new Brep(); 



                //run stretch 
             //   Rhino.Geometry.Morphs.StretchSpaceMorph stretchMorph = new StretchSpaceMorph(startPoint, endPoint, actualLength);   /////// cant use inputs here - will need to get this from brep
                                                                                                                                    //  if(stretchMorph.IsValid && i == 0)
                                                                                                                                    //   stretchMorph.Morph(closestFace);

                //stretchMorph.PreserveStructure = true;


                //List<Brep> listOfFaces = new List<Brep>();
                //for (int f = 0; f < brepSrfs.Count; f++)
                //{
                //    if(f != closestFaceIndex)
                //        listOfFaces.Add(brepSrfs[f]);
                //}

                //var joinedBreps = Brep.JoinBreps(listOfFaces.ToArray(), doc.ModelRelativeTolerance);

                ////get the extension distance 
                //Line extensionLine = new Line(startPoint, endPoint);
                //extensionLine.Extend(0, actualLength - extensionLine.Length);
                //var extensionVector = new Vector3d(extensionLine.PointAt(1) - extensionLine.PointAt(0));

                //List<Brep> finalBreps = new List<Brep>(); 

                ////get the closest edge to the end point of the axis 
                //foreach (var joinedBrep in joinedBreps)
                //{
                //    var extensionPtStart = extensionLine.PointAt(0);
                //    var extensionPtEnd = extensionLine.PointAt(1);

                //    var closestPointStart = joinedBrep.ClosestPoint(extensionPtStart);
                //    var closestPointEnd =  joinedBrep.ClosestPoint(extensionPtEnd);

                //    var distanceToStart = extensionPtStart.DistanceTo(closestPointStart);
                //    var distanceToEnd = extensionPtStart.DistanceTo(closestPointEnd);

                //    //move this brep along this axis 
                //    if (distanceToStart < distanceToEnd)
                //    {
                //        joinedBrep.Transform(Transform.Translation(new Vector3d(closestPointStart - extensionPtStart)));
                //    }
                //    else
                //    {
                //        joinedBrep.Transform(Transform.Translation(new Vector3d(closestPointEnd- extensionPtEnd)));
                //    }
                //    //if the distance does not == 0 then move the brep along this vector 

                //    finalBreps.Add(joinedBrep);
                //}
             

                ////join all the breps together again 
                //finalBreps.Add(closestFace);


                // brepToStretch = Brep.JoinBreps(finalBreps, doc.ModelAbsoluteTolerance)[0];  //might want to check that there is only one output

                //continue the loop

           



            //assign breps and text dots to outputs 
            DA.SetData(ComponentOutputs.Breps.Name, strechingObj.OutputBrep);
            DA.SetDataList(ComponentOutputs.points.Name, strechingObj.outputBreps);
            DA.SetDataList(ComponentOutputs.lines.Name, stretchingAxis);

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
            get { return new Guid("ee5261ee-6f6a-447b-a595-afd0b2aeb4f0"); }
        }
    }
}
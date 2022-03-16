using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Morphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ifLab_LlamaTools.Components.Sheet_Metal
{
    public class KfactorBrepStrechingObj
    {
        public KfactorBrepStrechingObj(Brep stretchingBrep, List<Line> strechingAxis, List<double> axisLength)
        {
            var aj = stretchingBrep.Faces[0].AdjacentEdges();

            StretchingBrep = stretchingBrep;
            StrechingAxis = strechingAxis;
            AxisLength = axisLength;

            var af = stretchingBrep.Faces[0].AdjacentFaces();
            var af2 = StretchingBrep.Faces[0].AdjacentFaces();

          var outbrep =  RunStretchingLoops(StretchingBrep);

            this.OutputBrep = outbrep; 
        }

        public Brep StretchingBrep { get; set; }
        public List<Line> StrechingAxis { get; set; }
        public List<double> AxisLength { get; set; }

        List<Brep> brepsToStretch = new List<Brep>();
        List<Brep> nonStretchBreps = new List<Brep>();

        List<BrepObjs> brepObjs = new List<BrepObjs>();

        public List<Brep> outputBreps = new List<Brep>();
        public Brep OutputBrep { get; set; }

        internal void GetStretchingFaces()
        {
            var brepSrfs = new List<Brep>();
            List<Point3d> brepSrfsCentroids = new List<Point3d>();

            var af2 = StretchingBrep.Faces[0].AdjacentFaces();

            int j = 0;
            foreach (var brepFaces in StretchingBrep.Faces)
            {
                brepSrfsCentroids.Add(AreaMassProperties.Compute(brepFaces).Centroid);
                var extractedFaces = StretchingBrep.Faces.ExtractFace(j);
                brepSrfs.Add(extractedFaces);




                BrepObjs brep = new BrepObjs(extractedFaces);
                brep.FaceIndex = brepFaces.FaceIndex; 
                var faces = brepFaces.AdjacentFaces();
                brep.AdjacentFaces = brepFaces.AdjacentFaces().ToList();


            //    var newFaces = StretchingBrep.Faces[brepFaces.FaceIndex].AdjacentFaces();
                brepObjs.Add(brep);

                var newFaces = StretchingBrep.Faces[brepFaces.FaceIndex].AdjacentEdges();

                j++;
            }


            nonStretchBreps.AddRange(brepSrfs);

            //loop through internal curves 
            for (int i = 0; i < StrechingAxis.Count; i++)
            {
                var line = StrechingAxis[i];
                var actualLength = StrechingAxis[i];

                var startPoint = line.PointAt(0);
                var endPoint = line.PointAt(1);

                var midPoint = line.PointAt(0.5);


                PointCloud ptCloud = new PointCloud(brepSrfsCentroids);
                var closestFaceIndex = ptCloud.ClosestPoint(midPoint);
                var closestFace = brepSrfs[closestFaceIndex];

                brepObjs[closestFaceIndex].isStretched = true; 
                brepObjs[closestFaceIndex].stretchIndex = i; 

                brepsToStretch.Add(closestFace);
               
                nonStretchBreps.Remove(closestFace);
            }

        }

        internal void StretchFaces()
        {
            var brepStretch = brepObjs.Where(x => x.isStretched == true).OrderBy(x => x.stretchIndex).ToList();
          

            for (int i = 0; i < StrechingAxis.Count; i++)
            {
                var line = StrechingAxis[i];
                var actualLength = AxisLength[i];

                var startPoint = line.PointAt(0);
                var endPoint = line.PointAt(1);

                var midPoint = line.PointAt(0.5);

                var brepToStretch = brepStretch[i];

                var faces = StretchingBrep.Faces[brepToStretch.FaceIndex].AdjacentFaces();

                Rhino.Geometry.Morphs.StretchSpaceMorph stretchMorph = new StretchSpaceMorph(startPoint, endPoint, actualLength);
                stretchMorph.PreserveStructure = true;
                stretchMorph.Morph(brepToStretch.Brep);

            }
        }

        internal void ReassembleFaces()
        {

            Brep joinedBrep = new Brep();

            // loop through input face list objects - (create and object and flag if it is a stretch or not 
            //loop through the edges to find the closest edge
            //move the object to this face 
            //join and continue
            for (int i = 0; i < brepObjs.Count; i++)
            {
            

                var brep = brepObjs[i];
                if(i == 0)
                {
                    //create a new brep with this object 
                    joinedBrep = brep.Brep; 
                }
                else
                {
                   // loop through adjacent faces adding them to the new joined brep

                    
                        var adjacentBrep = brepObjs[i].Brep;
                        //get the edges of the current brep
                        var currentBrepVerts = joinedBrep.Vertices;

                        var adjacentBrepVerts = adjacentBrep.Vertices;

                        Point3d closestCurrentPoint = new Point3d();
                        Point3d closestAdjacentPoint = new Point3d();
                        double? distance = null;

                        foreach (var currentVert in currentBrepVerts)
                        {

                       var curves =  joinedBrep.DuplicateNakedEdgeCurves(true, false);

                     

                            foreach (var curve in curves)
                            {

                            double t = 0;
                            curve.ClosestPoint(currentVert.Location, out t);

                            var point = curve.PointAt(t);

                                var testDistance = currentVert.Location.DistanceTo(point);
                                if (distance == null || testDistance < distance)
                                {
                                    distance = testDistance;
                                    closestAdjacentPoint = point;
                                    closestCurrentPoint = currentVert.Location;
                                }
                            }

                        }

                        //get the edges of the adjacent Face

                        //get closest vert to the end point

                        //move in this direction

                        adjacentBrep.Translate(new Vector3d(closestCurrentPoint - closestAdjacentPoint));

                        joinedBrep.Join(adjacentBrep, 0.01, true);

                        //join faces together - this may need to be creating a new brep 
                   
                }
                //loop through adjacent faces adding them to the new joined brep 

                //foreach(int j in brep.AdjacentFaces)
                //{
                //    var adjacentBrep = brepObjs[j].Brep;
                //    //get the edges of the current brep
                //    var currentBrepVerts = brep.Brep.Vertices;

                //    var adjacentBrepVerts = adjacentBrep.Vertices;

                //    Point3d closestCurrentPoint = new Point3d();
                //    Point3d closestAdjacentPoint = new Point3d();
                //    double? distance = null; 

                //    foreach (var currentVert in currentBrepVerts)
                //    {
                //        foreach (var adjacentPoint in adjacentBrepVerts)
                //        {
                //            var testDistance = currentVert.Location.DistanceTo(adjacentPoint.Location); 
                //            if(distance == null || testDistance < distance)
                //            {
                //                distance = testDistance;
                //                closestAdjacentPoint = adjacentPoint.Location;
                //                closestCurrentPoint = currentVert.Location;
                //            }
                //        }

                //    }

                //    //get the edges of the adjacent Face

                //    //get closest vert to the end point

                //    //move in this direction

                //    adjacentBrep.Translate(new Vector3d(closestCurrentPoint - closestAdjacentPoint));

                //    joinedBrep.Join(adjacentBrep, 0.01, true);

                //    //join faces together - this may need to be creating a new brep 
                //}

                outputBreps.Add(brep.Brep);
            }

            // outputBreps.AddRange(nonStretchBreps);
            OutputBrep = joinedBrep; 
        }

        private Brep RunStretchingLoops(Brep inputBrep) /// input brep - maybe duplicate the stretching brep
        {
            Brep constBrep = inputBrep;

            int k = 0;
            foreach(var lineAxis in StrechingAxis)
            {
                var midPoint = lineAxis.PointAt(0.5); 
                var stretchingFace = getStretchingFaceFromBrep(constBrep, midPoint);

                if (stretchingFace == null) continue;

                //get all the adjacent Face Elements 
                var adjacentFaces = constBrep.Faces[stretchingFace.FaceIndex].AdjacentFaces(); 
                var adjacentEdges = constBrep.Faces[stretchingFace.FaceIndex].AdjacentEdges();

                List<BrepFaceObjs> brepFaceObjs = new List<BrepFaceObjs>(); 

                for (int i = 0; i < adjacentFaces.ToList().Count; i++)
                {
                    var adjacentFace = constBrep.Faces[adjacentFaces[i]];

                    BrepFaceObjs faceObjs = new BrepFaceObjs(adjacentFace.FaceIndex);
                    faceObjs.faceTreeIndex.Add(adjacentFaces[i]);
                    faceObjs.faceTreeIndex.AddRange( adjacentFace.AdjacentFaces().Where(x => x != stretchingFace.FaceIndex));
                    brepFaceObjs.Add(faceObjs); 
                }

                //extract faces that form all the faces except what will be stretched 
                List<Brep> otherBreps = new List<Brep>();
                foreach (var otherFaces in brepFaceObjs)
                {

                   var unjoinedbBrep = constBrep.DuplicateSubBrep(otherFaces.faceTreeIndex);
                    otherBreps.Add(unjoinedbBrep); 
                }

                //get set up stretching axis and vector 


                var line = lineAxis;
                    var actualLength = AxisLength[k];

                    var startPoint = line.PointAt(0);
                    var endPoint = line.PointAt(1);

                  

                    var brepToStretch = stretchingFace.DuplicateFace(false);

                
                //stretch the folding piece 

                    Rhino.Geometry.Morphs.StretchSpaceMorph stretchMorph = new StretchSpaceMorph(startPoint, endPoint, actualLength);
                    stretchMorph.PreserveStructure = true;
                    stretchMorph.Morph(brepToStretch);



                Brep brepToMove = new Brep();
                double longestDistance = 0; 

                //move the adjacent brep closest to the end of the axis 
                foreach (var otherBrep in otherBreps)
                {
                    var distnace = endPoint.DistanceTo(otherBrep.ClosestPoint(endPoint));
                   if(distnace > longestDistance)
                    {
                        brepToMove = otherBrep;
                        longestDistance = distnace; 
                    }
                }


                var myline = new Line(startPoint.X, startPoint.Y, startPoint.Z, endPoint.X, endPoint.Y, endPoint.Z);
                myline.Extend(0, myline.Length - actualLength);



                brepToMove.Translate(new Vector3d(endPoint - myline.PointAt(1)));
                //rejoin all the pieces 

                foreach (var b in otherBreps)
                {

             var bv=       brepToStretch.Join(b, 0.01, true);
                    if(!bv)
                    {

                    }
                    else
                    {
                        outputBreps.Add(brepToStretch);

                    }
                }

                //update constBrep
                constBrep = brepToStretch; 

                k++;
            }
            return constBrep; 
        }

        class BrepFaceObjs
        {
            public BrepFaceObjs(int fi)
            {
                BaseFaceIndex = fi;
                
            }

            public int BaseFaceIndex { get; set; }
            public List<int> faceTreeIndex = new List<int>(); 
        }

        private BrepFace getStretchingFaceFromBrep(Brep constBrep, Point3d midPoint)
        {
            double dist = 100000000;
            BrepFace outface = null; 
            foreach (var brepFace in constBrep.Faces)
            {

                double distance = midPoint.DistanceTo(brepFace.DuplicateFace(false).ClosestPoint(midPoint)); 
                if (distance < dist)
                {
                    outface = brepFace;
                    dist = distance; 

                }
            }

            return outface; 
        }

        private class BrepObjs
        {
            public BrepObjs(Brep brep)
            {
                Brep = brep;
            }

            public bool isStretched { get; set; }
            public Brep Brep { get; set; }

            public int stretchIndex { get; set; }

            public int FaceIndex { get; set; }

            public List<int> AdjacentFaces { get; set; }

        }


    }


}
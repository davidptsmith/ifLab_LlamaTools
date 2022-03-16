using Grasshopper.Kernel;

using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ifLab_LlamaTools.Components.Utilities
{
    public class AdvancedText_GHC : GH_Component
    {


        GH_Document GrasshopperDocument;
        IGH_Component Component;


        /// <summary>
        /// Initializes a new instance of the Ghc_AdvancedText class.
        /// </summary>
        public AdvancedText_GHC()
          : base("AdvancedText", "Advanced Text",
              "Advanced text orients to the viewport",
              "LLama Tools", "Utilities")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddPointParameter("Location", "Location", "Point location for the text", GH_ParamAccess.list);
            pManager.AddTextParameter("Text", "Text", "Text", GH_ParamAccess.list);
            pManager.AddTextParameter("________", "________", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Size", "Size", "Size of Text", GH_ParamAccess.item);
            pManager.AddColourParameter("Colour", "Colour", "Colour of text", GH_ParamAccess.item);
            pManager.AddTextParameter("Font", "Font", "Font used for text", GH_ParamAccess.item);
            pManager.AddTextParameter("Text Alignment", "[] Alignment", "Alignment of text (Auto Value List) ", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Bold", "Bold", "Set to true to bold text", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Italic?", "Italic", "Set to true to make text italic", GH_ParamAccess.item);


            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
            pManager[8].Optional = true;




        }


        List<string> _text = new List<string>();
        List<Point3d> _points = new List<Point3d>();



        string font = "Lucida Console";
        Color color = Color.Black;
        double size = 25;
        bool bold = false;
        bool italic = false;

        TextHorizontalAlignment displayAlignment = TextHorizontalAlignment.Center;



        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            for (int i = 0; i < _text.Count; i++)
            {
                Plane plane;
                args.Viewport.GetCameraFrame(out plane);
                plane.Origin = _points[i];

                double pixelsPerUnit;
                args.Viewport.GetWorldToScreenScale(_points[i], out pixelsPerUnit);

                args.Display.Draw3dText(_text[i], color, plane, size / pixelsPerUnit, font, bold, italic, displayAlignment, TextVerticalAlignment.Middle);
                // add justsitrifation, bold, italic, etc

            }
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
            _text.Clear();
            _points.Clear();


            font = "Lucida Console";
            color = Color.Black;
            size = 25;
            bold = false;
            italic = false;

            displayAlignment = TextHorizontalAlignment.Center;






            Color myColour = Color.Black;
            double mySize = 0;
            string myFont = "";
            string myAlignment = "";
            bool myBold = false;
            bool myItalic = false;


            ////// Auto Update Value list    - need to change input list, location of active doc, and add gh component to and grasshopper document to the start of script

            List<string> textAlignment = new List<string>()
        { "Left", "Centre", "Right"};

            var inputList2 = textAlignment;

            Component = this;
            GrasshopperDocument = this.OnPingDocument();

            foreach (IGH_Param source in this.Component.Params.Input[6].Sources)
            {
                if (source is Grasshopper.Kernel.Special.GH_ValueList && source.NickName != "Text Alignment")
                {
                    // set the variable vl to the source object as a grasshopper value list 
                    Grasshopper.Kernel.Special.GH_ValueList vl = source as Grasshopper.Kernel.Special.GH_ValueList;

                    vl.NickName = "Text Alignment";

                    //clear the current contents of the value list
                    vl.ListItems.Clear();

                    // for each data item add value and key to value list 
                    for (int i = 0; i < inputList2.Count; i++)
                    {

                        // select the type of value list to display 
                        vl.ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.DropDown;

                        // add the items to the value list 
                        vl.ListItems.Add(new Grasshopper.Kernel.Special.GH_ValueListItem(inputList2[i], "\"" + inputList2[i] + "\""));

                    }



                }

            }

            /////




            DA.GetDataList(0, _points);
            DA.GetDataList(1, _text);

            DA.GetData(3, ref mySize);
            DA.GetData(4, ref myColour);
            DA.GetData(5, ref myFont);
            DA.GetData(6, ref myAlignment);
            DA.GetData(7, ref myBold);
            DA.GetData(8, ref italic);





            if (myFont != "")
            {
                font = myFont;
            }

            if (myColour != null)
            {
                color = myColour;

            }

            if (mySize != 0)
            {
                size = mySize;
            }

            if (myAlignment == "Left")
            {
                displayAlignment = TextHorizontalAlignment.Left;
            }

            if (myAlignment == "Centre")
            {
                displayAlignment = TextHorizontalAlignment.Center;
            }
            if (myAlignment == "Right")
            {
                displayAlignment = TextHorizontalAlignment.Right;
            }


            if (myBold == true)
            {
                bold = myBold;
            }

            if (myItalic == true)
            {
                italic = myItalic;
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
            get { return new Guid("dd438075-b5bf-412a-817c-3c62cc565632"); }
        }
    }
}
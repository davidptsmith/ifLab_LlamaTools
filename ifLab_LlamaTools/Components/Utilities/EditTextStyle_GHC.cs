using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace ifLab_LlamaTools.Components.Utilities
{
    public class EditTextStyle_GHC : GH_Component
    {
        GH_Document GrasshopperDocument;
        IGH_Component Component;



        /// <summary>
        /// Initializes a new instance of the Ghc_EditTextStyle class.
        /// </summary>
        public EditTextStyle_GHC()
          : base("Edit Text Style", "Edit Text Style",
              "This component allows you to live update dimension styles",
              "LLama Tools", "Utilities")
        {
        }


        bool runHorFunc = false;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddTextParameter("Style Name", "[] Style Name", "Style Name to Edit", GH_ParamAccess.item);
            pManager.AddNumberParameter("Text Scale", "Text Scale", "Text Scale", GH_ParamAccess.item); 
            pManager.AddTextParameter("Font", "[] Font", "Font", GH_ParamAccess.item);
            pManager.AddNumberParameter("Text Height", "Text Height", "Text Height", GH_ParamAccess.item);
            pManager.AddNumberParameter("Text Gap", "Text Gap", "Text Gap", GH_ParamAccess.item); 
            pManager.AddTextParameter("Alignment", "Alignment", "Text Alignmet", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Horizontal to View?", "Horizontal?", "Orient text horizontal to view?", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
           pManager[5].Optional = true;
            pManager[6].Optional = true; 

           
            if (pManager[6].SourceCount > 0)
                runHorFunc = true; 


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

           

            var doc = Rhino.RhinoDoc.ActiveDoc;

            string styleName = "";
            DA.GetData(0, ref styleName);



             var myStyle = doc.DimStyles.FindName(styleName);



            //// get display styles 


            var style = doc.DimStyles;
            List<string> styleNames = new List<string>();

            foreach (var dM in style)
            {
                styleNames.Add(dM.Name);
            }


            ////// Auto Update Value list    - need to change input list, location of active doc, and add gh component to and grasshopper document to the start of script

            var inputList2 = styleNames;
            int inputNumber = 0;

            Component = this;
            GrasshopperDocument = this.OnPingDocument();

            foreach (IGH_Param source in this.Component.Params.Input[inputNumber].Sources)
            {
                if (source is Grasshopper.Kernel.Special.GH_ValueList && source.NickName != "Text Styles")
                {
                    // set the variable vl to the source object as a grasshopper value list 
                    Grasshopper.Kernel.Special.GH_ValueList vl = source as Grasshopper.Kernel.Special.GH_ValueList;

                    vl.NickName = "Text Styles";

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




            // scale text
            double scaleValue = myStyle.DimensionScale;
            DA.GetData(1, ref scaleValue);

            myStyle.DimensionScale = scaleValue;

            // update font 
            List<string> fonts = new List<string>();
            

            var fontTabl = Rhino.DocObjects.Font.AvailableFontFaceNames();

            for ( int i = 0; i< fontTabl.Length; i++)
            {
               
                fonts.Add(fontTabl[i]);

            }
           // DA.SetData(0, fonts);

            //////// Auto Update Value list !!!! Change Input list as and change PARAM INPUT INDEX


            List<string> autoFonts = new List<string>();
            string[] names = new string[]
                {"Arial", "Calibri", "Century Gothic", "Comic Sans MS", "Courier New", "Myriad Pro", "SansSerif", "Times New Roman"};
            autoFonts.AddRange(names);

            var inputList = autoFonts;

            Component = this;
            GrasshopperDocument = this.OnPingDocument();

            foreach (IGH_Param source in this.Component.Params.Input[2].Sources)
            {
                if (source is Grasshopper.Kernel.Special.GH_ValueList && source.NickName != "Fonts")
                {
                    // set the variable vl to the source object as a grasshopper value list 
                    Grasshopper.Kernel.Special.GH_ValueList vl = source as Grasshopper.Kernel.Special.GH_ValueList;

                    //clear the current contents of the value list
                    vl.ListItems.Clear();
                    vl.NickName = "Fonts";

                    // for each data item add value and key to value list 
                    for (int i = 0; i < inputList.Count; i++)
                    {

                        // select the type of value list to display 
                        vl.ListMode = Grasshopper.Kernel.Special.GH_ValueListMode.CheckList;

                        // add the items to the value list 
                        vl.ListItems.Add(new Grasshopper.Kernel.Special.GH_ValueListItem(inputList[i], "\"" +  inputList[i] + "\"" ));

                    }



                }

            }







            string fontName = "Arial";
            DA.GetData(2, ref fontName);

            var myFont = new Rhino.DocObjects.Font(fontName); 

            if (fontName != "") myStyle.Font = myFont;
            

            // update height 
            double textHeight = myStyle.TextHeight;
            DA.GetData(3, ref textHeight);

            myStyle.TextHeight = textHeight;
            

            // update gap
            double textGap = myStyle.TextGap;
            DA.GetData(4, ref textGap);

            myStyle.TextGap = textGap;


            // update alignment - value list? 
            string alignment = "";
            
            DA.GetData(5, ref alignment);

            if (alignment == "Left" || alignment == "left")
                myStyle.TextHorizontalAlignment = Rhino.DocObjects.TextHorizontalAlignment.Left;
            else if (alignment == "Center" || alignment == "center")
                myStyle.TextHorizontalAlignment = Rhino.DocObjects.TextHorizontalAlignment.Center;
            else if (alignment == "Right" || alignment == "right")
                myStyle.TextHorizontalAlignment = Rhino.DocObjects.TextHorizontalAlignment.Right;

            //if (runHorFunc)
            //{
                // update Horizontal to View

                bool horToView = true;

                DA.GetData(6, ref horToView);

                if (horToView == true)
                {
                    myStyle.DimTextOrientation = Rhino.DocObjects.TextOrientation.InView;
                    myStyle.TextOrientation = Rhino.DocObjects.TextOrientation.InView;
                    

                }
                else if (horToView != true)
                {
                   
                    myStyle.DimTextOrientation = Rhino.DocObjects.TextOrientation.InPlane ;
                    myStyle.TextOrientation = Rhino.DocObjects.TextOrientation.InPlane;
                   
                }

            //}



            doc.DimStyles.Modify(myStyle, myStyle.Index, true);





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
                // return ifLab_LlamaTools.Resources.IconResources.EditText;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("940512ad-59ef-4db2-aca0-e4aa6fba2e31"); }
        }
    }
}
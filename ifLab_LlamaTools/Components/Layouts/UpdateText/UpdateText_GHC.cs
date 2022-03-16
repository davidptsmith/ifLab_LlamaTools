using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino;
using ifLab_LlamaTools.Core.ComponentInputs;

namespace ifLab_LlamaTools.Components.Layouts
{
    public class UpdateText : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public UpdateText()
          : base("Update Text", "Update Text",
              "Find within a Rhino document and update it across all sheets or in current view",
              "LLama Tools", "Layout Tools")
        {
        }

        /// <summary>
        /// Use this class to set up all the inputs, this will allow you to call them by their name in intellisense 
        /// </summary>
        private static class ComponentInputs
        {
            // list of inputs for the component 
            public static InputObject UpdateAll = new InputObject("Update All", "Update All", "This will update all instances of the text in the document", GH_ParamAccess.item, true);
            public static InputObject UpdateCurrent = new InputObject("Update Current View", "Update Current", "This will update only text instances in the current view", GH_ParamAccess.item, true);
            public static InputObject TextNames = new InputObject("Text Name", "Text Name", "The name of the text element to replace the value with in properties", GH_ParamAccess.list, true);
            public static InputObject TextValues = new InputObject("Text Values", "Text Value", "The value of the text to be updated", GH_ParamAccess.list, true);


        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            ComponentInputs.UpdateAll.ParamNumber = pManager.AddBooleanParameter(ComponentInputs.UpdateAll.Name, ComponentInputs.UpdateAll.Nickname, ComponentInputs.UpdateAll.Desscription, ComponentInputs.UpdateAll.ParamAccess);
            pManager[ComponentInputs.UpdateAll.ParamNumber].Optional = true;

            ComponentInputs.UpdateCurrent.ParamNumber = pManager.AddBooleanParameter(ComponentInputs.UpdateCurrent.Name, ComponentInputs.UpdateCurrent.Nickname, ComponentInputs.UpdateCurrent.Desscription, ComponentInputs.UpdateCurrent.ParamAccess);
            pManager[ComponentInputs.UpdateCurrent.ParamNumber].Optional = true;

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
            
            pManager.AddTextParameter("Text Names", "Text Names", "Output Text Names", GH_ParamAccess.list);
            pManager.AddTextParameter("Update Text", "Update Text", "List of text to update text names too", GH_ParamAccess.list);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool updateAll = false;
            bool updateCurrent = false; 
            List<string> textNames = new List<string>();
            List<string> updateTexts = new List<string>();

            DA.GetData(0, ref updateAll);
            DA.GetData(1, ref updateCurrent);
            DA.GetDataList(2, textNames);
            DA.GetDataList(3, updateTexts);


            

            if ( textNames.Count != updateTexts.Count)
            {
                updateAll = false;
                updateCurrent = false;
                throw new Exception("Number of Names and update text must be equal");

            }


            if(updateAll)
            {
                RhinoDoc doc = RhinoDoc.ActiveDoc;
                int myIndex = 0;
                
                //search block objects for text references 
                Rhino.DocObjects.ObjectEnumeratorSettings settingsBlock = new Rhino.DocObjects.ObjectEnumeratorSettings();
                settingsBlock.ActiveObjects = true;
                settingsBlock.ObjectTypeFilter = Rhino.DocObjects.ObjectType.InstanceDefinition;

                var blockItems = doc.InstanceDefinitions.GetList(true);

                //List<Rhino.DocObjects.TextObject> blockTextObjects = new List<Rhino.DocObjects.TextObject>(); 

                foreach ( var blockItem in blockItems)
                {
                    
                    var blockObjects = blockItem.GetObjects();

                    foreach (var blockObject in blockObjects)
                    {

                        for (int n = 0; n < textNames.Count; n++)
                        {
                            if (blockObject.Name == textNames[n] && blockObject.ObjectType == Rhino.DocObjects.ObjectType.Annotation)
                            {
                                //blockTextObjects.Add(blockObject as Rhino.DocObjects.TextObject);
                                var textBlockObject = blockObject as Rhino.DocObjects.TextObject;

                                Rhino.Geometry.TextEntity textEntity;

                                textEntity = textBlockObject.Geometry as Rhino.Geometry.TextEntity;
                                string myString = updateTexts[n];
                                textEntity.RichText = myString;

                                
                                textBlockObject.CommitChanges();


                            }
                        }

                        blockObject.CommitChanges(); 
                    }
                      
                }

                
                
                // Regular rhino text objects

                foreach ( var  textName in textNames)
                {

                    // set up a object search filter for current viewport
                    Rhino.DocObjects.ObjectEnumeratorSettings settings = new Rhino.DocObjects.ObjectEnumeratorSettings();
                    settings.NameFilter = textName;
                    settings.ObjectTypeFilter = Rhino.DocObjects.ObjectType.Annotation;
                    

                    List<Rhino.DocObjects.TextObject> textItems = new List<Rhino.DocObjects.TextObject>();

                    foreach (Rhino.DocObjects.TextObject textItem in doc.Objects.GetObjectList(settings))
                        textItems.Add(textItem);

                    Rhino.Geometry.TextEntity textEntity;

                   // int myIndex = 0;
                    foreach (var textItem in textItems)
                    {

                        textEntity = textItem.Geometry as Rhino.Geometry.TextEntity;
                        string myString = updateTexts[myIndex];
                        textEntity.RichText = myString;
                        textItem.CommitChanges();
                       
                    }
                     myIndex++;
                }



                DA.SetDataList(0, textNames);
                DA.SetDataList(1, updateTexts);




            }
            if (updateCurrent)
            {
                RhinoDoc doc = RhinoDoc.ActiveDoc;
                var activeView = doc.Views.ActiveView.ActiveViewport;

                int myIndex = 0; 

                foreach ( var textName  in textNames)
                {
                  
                    // set up a object search filter for current viewport
                    Rhino.DocObjects.ObjectEnumeratorSettings settings = new Rhino.DocObjects.ObjectEnumeratorSettings();
                    settings.NameFilter = textName;
                    settings.ObjectTypeFilter = Rhino.DocObjects.ObjectType.Annotation;
                    settings.ViewportFilter = activeView; 

                    List<Rhino.DocObjects.TextObject> textItems = new List<Rhino.DocObjects.TextObject>();
                    foreach (Rhino.DocObjects.TextObject textItem in doc.Objects.GetObjectList(settings))
                        textItems.Add(textItem);

                    Rhino.Geometry.TextEntity textEntity;

                    //int myIndex = 0;
                    foreach (var textItem in textItems)
                    {
                        
                            textEntity = textItem.Geometry as Rhino.Geometry.TextEntity;
                            string myString = updateTexts[myIndex];
                            textEntity.RichText = myString;
                            textItem.CommitChanges();
                        
                        textItem.CommitChanges();
                       
                    }
                    myIndex++;
                }


                DA.SetDataList(0, textNames);
                DA.SetDataList(1, updateTexts); 

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
            get { return new Guid("0b111dda-c730-47a6-92fc-c3187497368e"); }
        }
    }
}
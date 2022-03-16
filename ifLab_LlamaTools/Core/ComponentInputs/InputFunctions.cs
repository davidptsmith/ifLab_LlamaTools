using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Rhino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ifLab_LlamaTools.Core.ComponentInputs
{
    public static class InputFunctions
    {
        /// <summary>
        /// Auto populates a value list with a series of data, use the value list name to stop it from resetting 
        /// </summary>
        /// <param name="sources">List of sources from the input parameter</param>
        /// <param name="valueListName">name to validate the value list</param>
        /// <param name="listOfValues">values that you wish to populate the value list with</param>
        /// <param name="valueListDisplayMode">what type of value list you want it to finish displaying as</param>
        public static void AutoPopulateInputValuesList(IList<IGH_Param> sources, string valueListName, List<string> listOfValues, GH_ValueListMode valueListDisplayMode)
        {
            foreach (IGH_Param source in sources)
            {
        //        if (source is Grasshopper.Kernel.Special.GH_ValueList && source.NickName != valueListName)
                if (source is Grasshopper.Kernel.Special.GH_ValueList )
                {
                    // set the variable vl to the source object as a grasshopper value list 
                    Grasshopper.Kernel.Special.GH_ValueList valueList = source as Grasshopper.Kernel.Special.GH_ValueList;


                    var listOfItems = valueList.ListItems.Select(x => x.Name).ToList(); 
                    if ( listOfItems.SequenceEqual(listOfValues))
                    {
                        return; 
                    }
                    valueList.NickName = valueListName;

                    //clear the current contents of the value list
                    valueList.ListItems.Clear();

                    // for each data item add value and key to value list 
                    for (int i = 0; i < listOfValues.Count; i++)
                    {

                        // add the items to the value list 
                        var valueListItem = new Grasshopper.Kernel.Special.GH_ValueListItem(listOfValues[i], "\"" + listOfValues[i] + "\"");

                        valueList.ListItems.Add(valueListItem);
                        valueList.SelectItem(i);
                    }
                    valueList.SelectItem(0);

                    // select the type of value list to display 
                    valueList.ListMode = valueListDisplayMode;


                }

            }
        }

        /// <summary>
        /// Functions used to try and reset the button to stop issues with the button compression toggle
        /// </summary>
        /// <param name="sources">List of input sources to find the button from</param>
        public static void ResetButton(IList<IGH_Param> sources)
        {
            foreach (IGH_Param source in sources)
            {
                if (source is Grasshopper.Kernel.Special.GH_BooleanToggle)
                {
                    var toggle = source as GH_BooleanToggle;

                    toggle.Value = false; 


                    //source.ClearData();
                    //toggle.ExpirePreview(true);
                    //toggle.ButtonDown = false;
                    //toggle.Locked = true; 
                    //var btn = toggle.ButtonDown;
                    
                }
            }
        }

        //add a create element function (maybe auto populate the toggles - that way people dont use buttons, 
        //set the state of the toggle to false once the layouts are created or the function is complete


        public static void AutoPopulate_PageViews(RhinoDoc doc, IList<IGH_Param> sources, string ValueListName, GH_ValueListMode valueListDisplayMode)
        {
            //get all to current layouts in the document 
            var pageViews = doc.Views.GetPageViews();
            List<string> pageViewNames = new List<string>();
            foreach (var view in pageViews)
            {
                pageViewNames.Add(view.PageName);
            }



            //add the names to a value list that will allow for the selection of a template view 
            InputFunctions.AutoPopulateInputValuesList(sources,
                                            ValueListName,
                                            pageViewNames,
                                            valueListDisplayMode);

        }

        public static void AutoPopulate_DisplayModes(RhinoDoc doc, IList<IGH_Param> sources, string ValueListName, GH_ValueListMode valueListDisplayMode)
        {
            //// get display styles in the document 
            var displayModes = Rhino.Display.DisplayModeDescription.GetDisplayModes();
            List<string> displayNames = new List<string>();
            foreach (var dM in displayModes)
            {
                displayNames.Add(dM.EnglishName);
            }

            //add the names to a value list that will allow for the selection of a template view 
            InputFunctions.AutoPopulateInputValuesList(sources,
                                            ValueListName,
                                            displayNames,
                                            valueListDisplayMode);

        }
    }
}

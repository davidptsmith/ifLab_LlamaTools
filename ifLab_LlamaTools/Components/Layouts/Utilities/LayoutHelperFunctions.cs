using Rhino.DocObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ifLab_LlamaTools.Components.Layouts.Utilities
{
    /// <summary>
    /// static utilites class for helping with layout functions 
    /// </summary>
    public static class LayoutHelperFunctions
    {
        /// <summary>
        /// Get the name of the display modes within a document 
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDisplayModeNames()
        {
            //// get display styles in the document 
            var displayModes = Rhino.Display.DisplayModeDescription.GetDisplayModes();
            List<string> displayNames = new List<string>();
            foreach (var dM in displayModes)
            {
                displayNames.Add(dM.EnglishName);
            }
            return displayNames;
        }

        /// <summary>
        /// Get the list of desplaymode objects from within a document 
        /// </summary>
        /// <returns></returns>
        public static Rhino.Display.DisplayModeDescription[] GetDisplayModes()
        {
            return Rhino.Display.DisplayModeDescription.GetDisplayModes();
        }

        /// <summary>
        /// Get the names of the page view (layout) elements within the document
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<string> GetPageViewNames(Rhino.RhinoDoc doc)
        {

            //get all to current layouts in the document 
            var pageViews = doc.Views.GetPageViews();
            List<string> pageViewNames = new List<string>();
            foreach (var view in pageViews)
            {
                pageViewNames.Add(view.PageName);
            }
            return pageViewNames;
        }

        /// <summary>
        /// Gets the page view (Layout) elements from within a document 
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Rhino.Display.RhinoPageView[] GetPageViews(Rhino.RhinoDoc doc)
        {
            return doc.Views.GetPageViews();
        }

        /// <summary>
        /// Get the names of the details on a page 
        /// </summary>
        /// <param name="pageView">the page view (layout) element to search for details on</param>
        /// <returns></returns>
        public static List<string> GetDetailNamesOnPageView(Rhino.Display.RhinoPageView pageView)
        {
            var detailViews = pageView.GetDetailViews();
            List<string> detailNames = new List<string>();
            foreach (var detailView in detailViews)
            {
                detailNames.Add(detailView.Name);

            }
            return detailNames;
        }

        /// <summary>
        /// Get the detail elements on a page view (layout)
        /// </summary>
        /// <param name="pageView">the page view (layout) to search for detials on</param>
        /// <returns></returns>
        public static DetailViewObject[] GetDetailsOnPageView(Rhino.Display.RhinoPageView pageView)
        {
            return pageView.GetDetailViews();
        }
    }
}

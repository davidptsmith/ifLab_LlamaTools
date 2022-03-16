using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Display;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace ifLab_LlamaTools.Components.Layouts
{
   public class LayoutObject
    {
        public LayoutObject(string layoutName)
        {
            LayoutName = layoutName;
        }
        /// <summary>
        /// defualt contstructor taking the input values 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="layoutName"></param>
        /// <param name="templateName"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="offset"></param>
        /// <param name="targetPoint"></param>
        /// <param name="scaleFactor"></param>
        /// <param name="displayMode"></param>
        /// <param name="nameOfDetail"></param>
        /// <param name="textToReplaceName"></param>
        /// <param name="textToReplace"></param>
        /// <param name="pageViews"></param>
        public LayoutObject(RhinoDoc doc,
                            string layoutName,
                            string templateName,
                            double width,
                            double height,
                            double offset,
                            Point3d targetPoint,
                            double scaleFactor,
                            string displayMode,
                            string nameOfDetail,
                            string textToReplaceName,
                            string textToReplace,
                            RhinoPageView[] pageViews) : this(layoutName)
        {
            this.doc = doc;

            TemplateName = templateName;

            //check for the template string - if the template is there sue the sheet 
            if (!string.IsNullOrEmpty(templateName))
            {
                try
                {

                    //get the template sheet from the name 
                     templateView = doc.Views.GetPageViews().Where(x => x.PageName ==templateName).First();


                    if (! string.IsNullOrEmpty(nameOfDetail))
                    {
                        DetailName = nameOfDetail;
                    }
                 


                    //if this is found set the width and height 
                    if (templateView != null)
                    {
                        Width = templateView.PageWidth;
                        Height = templateView.PageHeight;

                        
                     
                    }   
                }
                catch 
                {

                    
                }
            }
            else
            {
                //if no template sheet, get this from the inputs if they have a value
                if(width != 0)
                    Width = width;
                if(width != 0)
                    Height = height;
            }


            if(targetPoint != null)
                TargetPoint = targetPoint;
            
            if(scaleFactor != 0)
                ScaleFactor = scaleFactor;
            else
            {
                ScaleFactor = 1; 
            }


            this.offset = offset; 

            TextToReplaceName = textToReplaceName;
            TextToReplace = textToReplace;

            this.displayMode= displayMode; 
            
            PageViews = pageViews;
        }

        private RhinoDoc doc { get; set; }

        private string LayoutName { get; set; }

        private string TemplateName { get; set; }

      
        public RhinoPageView templateView { get; set; }

        private double Width { get; set; } = 420;
        private double Height { get; set; } = 297;

        private double offset { get; set; } =  0;

        private Point3d TargetPoint { get; set; }
        private double ScaleFactor { get; set; }

        private string DetailName { get; set; }
        private string TextToReplaceName { get; set; }
        private string TextToReplace { get; set; }
        private RhinoPageView[] PageViews { get;  set; }

        private string displayMode { get; set; }
        
        private RhinoPageView pageView; 
        
        private DetailViewObject detail { get; set; }  // need to work with a list here 

        /// <summary>
        /// Close layers on the call of the command - extract this to a new core functions as this same function can be used for delete layouts 
        /// </summary>
        internal void CloseDuplicateLayouts()
        {

            //find the page views with the same name as template view
            foreach (var pageView in PageViews)
            {
                if (pageView.PageName == LayoutName)
                {
                    pageView.Close();
                }
            }
        }


        /// <summary>
        /// adds Page views to the documment 
        /// </summary>
        internal void AddPageView()
        {
            if (templateView == null)
            {
                pageView = doc.Views.AddPageView(LayoutName, Width, Height);

            }
            else
            {
                pageView = templateView.Duplicate(true);

                try
                {
                    detail = pageView.GetDetailViews().Where(detail => detail.Name == DetailName).First();
                }
                catch
                {
                    throw new Exception("Could not find a detail with that name.");

                }




            }
                pageView.PageName = LayoutName;
        }


        /// <summary>
        /// Adds details to the page 
        /// </summary>
        internal void AddDetailToPageView()
        {
            if (templateView == null)
            {
                var pt1 = new Point2d(0 + offset, 0 + offset);
                var pt2 = new Point2d(Width - offset, Height - offset);

                detail = pageView.AddDetailView("newDetail", pt1, pt2, Rhino.Display.DefinedViewportProjection.Top);
                detail.Name = "AutoDetail";
            }
        }

        /// <summary>
        /// Sets the display mode of the detail
        /// </summary>
        internal void SetDetailDisplayMode()
        {

            if (displayMode != "" && detail != null)
            {
                var viewportDisplay = Rhino.Display.DisplayModeDescription.FindByName(displayMode);
                if (viewportDisplay == null) return; 
                detail.Viewport.DisplayMode = viewportDisplay;
                detail.CommitViewportChanges();
                detail.CommitChanges();
            }
        }

        /// <summary>
        /// Sets the target point of the detail - the element to focus on 
        /// </summary>
        internal void SetTargetPointOfDetail()
        {
               detail.DetailGeometry.SetScale(ScaleFactor, doc.ModelUnitSystem, 1 , doc.ModelUnitSystem);
               detail.CommitChanges();

            if (TargetPoint != new Point3d())
            {

                var myFocalPoint = TargetPoint;
      
                detail.Viewport.SetCameraTarget(myFocalPoint, true);
                detail.CommitChanges();
                detail.CommitViewportChanges();
            }
        }



        /// <summary>
        /// used to ensure that the changes to a detail are committed to the document 
        /// </summary>
        internal void CommitViewChanges()
        {
            detail.CommitChanges();
            detail.CommitViewportChanges();
        }


        ///Things to add:
        ///selection of detail view (named views and built in projections
        ///Extract set scale of the details 
        ///maybe even add a function for naming details on the sheets (top left to bottom right?)
    }
}

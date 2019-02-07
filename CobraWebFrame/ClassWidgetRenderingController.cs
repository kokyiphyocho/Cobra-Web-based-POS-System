using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;
using CobraFoundation;
using CobraFrame;
using CobraWebFrame;
using System.Reflection;

namespace CobraWebFrame
{
    public abstract class WidgetRenderingController
    {
        protected FormInfoManager         clFormInfoManager;
        protected ComponentController     clComponentController;
        protected CompositeFormInterface  clCompositeForm;

        public abstract void RenderWidget(String paRenderMode = null);

        public WidgetRenderingController(CompositeFormInterface paCompositeForm, FormInfoManager paFormInfoManager, ComponentController paComponentController)
        {
            clCompositeForm = paCompositeForm;
            clFormInfoManager = paFormInfoManager;
            clComponentController = paComponentController;
        }        
    }

    public class WidgetRenderingEngine<TWidControl> where TWidControl : WidgetControlInterface, new()
    {
        protected FormInfoManager           clFormInfoManager;
        protected ComponentController       clComponentController;
        protected CompositeFormInterface    clCompositeForm;

        public WidgetRenderingEngine(CompositeFormInterface paCompositeForm, FormInfoManager paFormInfoManager, ComponentController paComponentController)
        {
            clCompositeForm = paCompositeForm;
            clFormInfoManager = paFormInfoManager;
            clComponentController = paComponentController;            
        }

        public static WidgetRenderingEngine<TWidControl> CreateInstance(CompositeFormInterface paCompositeForm, FormInfoManager paFormInfoManager, ComponentController paComponentController)
        {
            WidgetRenderingEngine<TWidControl> lcWidgetRenderingEngine;

            lcWidgetRenderingEngine = new WidgetRenderingEngine<TWidControl>(paCompositeForm, paFormInfoManager, paComponentController);         

            return (lcWidgetRenderingEngine);
        }

        public void RenderWidget(String paRederMode = "")
        {
            TWidControl     lcWidControl;
            FieldInfoRow    lcFieldInfoRow;            
                        
            lcWidControl = new TWidControl();
            lcWidControl.SCI_ParentForm = clCompositeForm;

            if (clFormInfoManager.FieldInfoManager.FieldInfoList != null)
            {
                lcFieldInfoRow = new FieldInfoRow(null);

                foreach (DataRow lcDataRow in clFormInfoManager.FieldInfoManager.FieldInfoList.Rows)
                {
                    lcFieldInfoRow.Row = lcDataRow;
                    ApplyBehaviour(lcWidControl, lcFieldInfoRow);
                }

                lcWidControl.RenderChildMode(clComponentController, paRederMode);
            }
        }

        protected void ApplyBehaviour(TWidControl paWidControl, FieldInfoRow paFieldInfoRow)
        {            
            PropertyInfo              lcPropertyInfo;
            object                    lcValue;
            object                    lcOutputValue;            

            try
            {
                if ((paFieldInfoRow != null) && ((lcPropertyInfo = paWidControl.GetType().GetProperty(paFieldInfoRow.ColumnName)) != null))
                {
                    lcValue = clFormInfoManager.TranslateStringEx(paFieldInfoRow.DefaultValue);                    
                    if ((!String.IsNullOrWhiteSpace(paFieldInfoRow.DefaultValue)) && (General.TryChangeType(lcValue, lcPropertyInfo.PropertyType, out lcOutputValue)))
                        lcPropertyInfo.SetValue(paWidControl, lcOutputValue, null);
                }
            }
            catch(Exception paException) { General.WriteExceptionLog(paException,"ApplyBehaviour"); }               
        }

    }
    
    public class WidgetAjaxGridModeRenderingController<TWidControl> 
                 where TWidControl : WidgetControlInterface, AjaxWidgetControlInterface, AjaxWidgControlPagingInterface, new()
           
    {
        private TWidControl         clWidControl;
        private FormInfoManager     clFormInfoManager;

        public TWidControl WidgetControl        { get { return (clWidControl); } }

        public WidgetAjaxGridModeRenderingController()
        {
            clWidControl = new TWidControl();
            clFormInfoManager = ApplicationFrame.GetInstance().ActiveFormInfoManager;
        }

        protected void ApplyBehaviour(TWidControl paWidControl, FieldInfoRow paFieldInfoRow)
        {

            PropertyInfo lcPropertyInfo;
            object lcValue;
            object lcOutputValue;

            try
            {
                if ((paFieldInfoRow != null) && ((lcPropertyInfo = paWidControl.GetType().GetProperty(paFieldInfoRow.ColumnName)) != null))
                {
                    lcValue = clFormInfoManager.TranslateStringEx(paFieldInfoRow.DefaultValue);
                    if ((!String.IsNullOrWhiteSpace(paFieldInfoRow.DefaultValue)) && (General.TryChangeType(lcValue, lcPropertyInfo.PropertyType, out lcOutputValue)))
                        lcPropertyInfo.SetValue(paWidControl, lcOutputValue, null);
                }
            }
            catch (Exception paException) { General.WriteExceptionLog(paException, "ApplyBehaviour"); }
        }

        public String GetSerializedAjaxData()
        {         
            FieldInfoRow            lcFieldInfoRow;
            HtmlTextWriter          lcHtmlTextWriter;
            StringWriter            lcStringWriter;
            StringBuilder           lcStringBuilder;
            ComponentController     lcComponentController;            

            lcStringBuilder         = new StringBuilder();
            lcStringWriter          = new StringWriter(lcStringBuilder);
            lcHtmlTextWriter        = new HtmlTextWriter(lcStringWriter);
            lcComponentController   = new ComponentController(lcHtmlTextWriter);
            
            if (clFormInfoManager.FieldInfoManager.FieldInfoList != null)
            {
                lcFieldInfoRow = new FieldInfoRow(null);

                foreach (DataRow lcDataRow in clFormInfoManager.FieldInfoManager.FieldInfoList.Rows)
                {
                    lcFieldInfoRow.Row = lcDataRow;
                    ApplyBehaviour(clWidControl, lcFieldInfoRow);
                }

                clWidControl.RenderAjaxMode(lcComponentController);

                return(lcStringWriter.ToString());
            }
            else return(null);            
        }

        public Dictionary<String, object> GetAjaxPagingInfo()
        {
            return (clWidControl.GetAjaxPagingInfo());
        }
    }

}

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace CobraFrame
{
    public class ComponentController
    {
        public enum ElementType
        {
            Form, Composite, StatusBar, MessageBar, MessageDisplay, LogInID, UserName, Password, LogInButton, ControlBar, PageStatus, StatusControl,
            FilterControl, Control, Container, Element, WidgetIcon, MessageBoxTemplate, MessageRepository,
            AjaxLoaderPopUp, AjaxLoaderStatusDisplay, Grid, GridRow, GridCell, GridItem, Item, Info, 
            InputBlock, InputTitle, InputRow, InputLabel, InputBox, InputTabBar, ToolBar, Button, Box,
            UnitPrice, Quantity, Discount, Subtotal, Total, Summary, Overlay, GoogleMap, Row, PopUp, Timer, Buffer, SearchBox, Panel, List,
            SlideSelectionControl, ImageSlideSelectionControl, ColorSlideSelectionControl, CalendarControl, LoadingOverlay, Block, Title, Label, Figure, DateBox,         
        }

        public enum ElementAttribute
        {
            ea_ServiceRequestToken,
            ea_FormName, ea_EncodedFormName, ea_KeyColumnName, ea_ColumnName, ea_DataGroupName, ea_LinkColumn, ea_MirrorColumn, ea_KeyValue,
            ea_Type, ea_Mandatory, ea_PageIndex, ea_CheckOnText, ea_CheckOffText, ea_InputMode, ea_QueryName, ea_Attribute, ea_Status, ea_Template, ea_Title, ea_Separator,            
            ea_FormProtocolList,
            ea_StatusText,
            ea_Name,
            ea_Mode, ea_Text,
            ea_FormStack,
            ea_LandingPage,
            ea_Language,
            ea_PluginMode,
            ea_FormMode, 
            ea_ControlMode,
            ea_OriginalValue,
            ea_Group,                    
            ea_ComponentID,
            ea_DataID,
            ea_Action,
            ea_Appearance,
            ea_Default,
            ea_GifDisplayMode,
            ea_MaxFileSize,
            ea_MaxFileCount,
            ea_ExtensionList,
            ea_Messages,
            ea_MessageCode,
            ea_AjaxMessages,            
            ea_Parameter,
            ea_KeyField,
            ea_IdentifierColumn,
            ea_ReadOnlyMode,
            ea_AdditionalData,
            ea_HasValue,
            ea_TimeOut,
            ea_TotalRows,
            ea_TotalPages,
            ea_PageSize,
            ea_DemoMode,
            ea_Parent,
            ea_Code,
            ea_Root,
            ea_DisplayMode,
            ea_Command,            
            ea_Button1,
            ea_Button2,
            ea_Edition,
            ea_LowerBound,
            ea_UpperBound,
            ea_DataList,
            ea_CurrencyCode,
            ea_Filter,
            ea_LocalNumberMode,
            ea_LocalDigits,
            ea_Decimal,     
            ea_Limit,    
            ea_OptionList,
            ea_Value,
            ea_desktopbackgroundcss,
            ea_Persist,
            ea_Lastmodified,
            ea_Count,
            ea_SystemConfig,            
            ea_Hidden,
            ea_Dynamic,
            ea_AdminMode,

            gpos_SystemItemLimit,
            gpos_SystemTableLimit,
            gpos_SystemTableGroupLimit,
            gpos_AllowShortSell,
            gpos_MultiPaymentMode,
            gpos_ReceiptPrintMode,
            gpos_TaxApplicable,
            gpos_TaxInclusive,
            gpos_TaxPercent,
            gpos_EntryType,
            gpos_EntryAttribute,
            gpos_Category,
            gpos_ItemID,
            gpos_ItemStatus,
            gpos_ItemCode,
            gpos_ItemText,
            gpos_Attribute,
            gpos_MajorUnitName,
            gpos_MinorUnitName,
            gpos_UnitName,
            gpos_UnitMode,
            gpos_UnitRelationship,
            gpos_MajorPrice,
            gpos_MinorPrice,
            gpos_MajorMSP,
            gpos_MinorMSP,

            gpos_ReceiptID,
            gpos_ReceiptAmount,
            gpos_Reference,
            gpos_DisplayName,
            gpos_TransactionState,

            //gpos_salereceiptcount,
            //gpos_purchasereceiptcount,
            //gpos_stockinreceiptcount,
            //gpos_stockoutreceiptcount,
            //gpos_cancelreceiptcount,

            gmap_instantload, gmap_Latitude, gmap_Longitude, gmap_maptype, gmap_zoom, gmap_showmarker
        }

        public enum ElementMessageTemplate
        {
            ma_ExceedMaxFileCount,
            ma_NoFileSelected, ma_InvalidFileType, ma_FileTooLarge, 
            ma_UploadSuccessful, ma_UploadFail, 
            ma_DeleteConfirmation, ma_DeleteSuccessful, ma_DeleteFail            
        }

        HtmlTextWriter clHtmlTextWriter;

        public HtmlTextWriter HtmlTextWriter { get { return (clHtmlTextWriter); } }

        public ComponentController(HtmlTextWriter paHtmlTextWriter)
        {
            clHtmlTextWriter = paHtmlTextWriter;
        }

        public void AddAttribute(HtmlAttribute paKey, String paValue)
        {
            if (!String.IsNullOrEmpty(paValue))
                clHtmlTextWriter.AddAttribute(paKey.ToString().Replace("_", "-").ToLower(), paValue);
        }

        public void AddAttribute(String paKey, String paValue)
        {
            if ((!String.IsNullOrEmpty(paKey)) && (!String.IsNullOrEmpty(paValue)))
                clHtmlTextWriter.AddAttribute(paKey.ToString().Replace("_", "-").ToLower(), paValue);
        }

        public void AddAttribute(HtmlAttribute paKey, String paValue, bool paEncode)
        {
            if (!String.IsNullOrEmpty(paValue))
                clHtmlTextWriter.AddAttribute(paKey.ToString().Replace("_", "-").ToLower(), paValue, paEncode);
        }

        public void AddElementType(ElementType paElementType)
        {
            AddAttribute("sa-elementtype", paElementType.ToString().ToLower());
        }

        public void AddElementAttribute(ElementAttribute paElementAttribute, String paValue)
        {
            if (!String.IsNullOrEmpty(paValue))
                AddAttribute(paElementAttribute.ToString().ToLower(), paValue);
        }

        public void AddElementAttribute(String paElementAttribute, String paValue)
        {
            if ((!String.IsNullOrEmpty(paElementAttribute)) && (!String.IsNullOrEmpty(paValue)))
                AddAttribute(paElementAttribute.ToLower(), paValue);
        }

        public void AddMessageTemplate(ElementMessageTemplate paElementMessageTemplate, String paValue)
        {
            if (!String.IsNullOrEmpty(paValue))
                AddAttribute(paElementMessageTemplate.ToString().ToLower(), paValue);
        }

        public void AddBareAttribute(String paAttribute, String paValue)
        {
            if ((!String.IsNullOrEmpty(paAttribute)) && (!String.IsNullOrEmpty(paValue)))
                clHtmlTextWriter.AddAttribute(paAttribute, paValue);
        }

        public void AddStyle(CSSStyle paStyle, String paValue)
        {
            if (!String.IsNullOrEmpty(paValue))
                clHtmlTextWriter.AddStyleAttribute(paStyle.ToString().Replace("_", "-").ToLower(), paValue);
        }

        public void AddStyle(String paStyle, String paValue)
        {
            if ((!String.IsNullOrEmpty(paStyle)) && (!String.IsNullOrEmpty(paValue)))
                clHtmlTextWriter.AddStyleAttribute(paStyle.ToString().Replace("_", "-").ToLower(), paValue);
        }

        public void RenderBeginTag(HtmlTag paHtmlTextWriterTag)
        {
            clHtmlTextWriter.RenderBeginTag(paHtmlTextWriterTag.ToString());
        }

        public void RenderEndTag()
        {
            clHtmlTextWriter.RenderEndTag();
        }

        public void Write(String paHtmlData)
        {
            clHtmlTextWriter.Write(paHtmlData);
        }

        public void RenderHyperLink(String paTextStr, String paHyperLink)
        {
            AddAttribute(HtmlAttribute.Href, paHyperLink);
            RenderBeginTag(HtmlTag.A);
            Write(paTextStr);
            RenderEndTag();
        }

        public static String ReplaceHtmlQuote(String paTextString)
        {
            return (paTextString.Replace("\"", "&quot;"));
        }

        static public String UnicodeStr(int paUnicode)
        {
            return ("&#" + paUnicode.ToString() + ";");
        }
    }
}



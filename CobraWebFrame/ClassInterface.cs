using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CobraFrame;

namespace CobraWebFrame
{
    public interface WidgetControlInterface
    {
        void RenderChildMode(ComponentController paComponentController, String paRenderMode = null);
        CompositeFormInterface SCI_ParentForm { get; set; }
    }

    public interface AjaxWidgetControlInterface
    {
        void RenderAjaxMode(ComponentController paComponentController);
    }

    public interface AjaxWidgControlPagingInterface
    {
        Dictionary<String, object> GetAjaxPagingInfo();
    }

    public interface CompositeFormInterface
    {        
        void RenderToolBar(ComponentController paComponentController);        
    }
}

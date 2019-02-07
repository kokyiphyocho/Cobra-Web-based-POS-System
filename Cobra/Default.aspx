<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Cobra.Default" ClientIDMode="Static"%>
<%@ Register assembly="CobraWebControls" namespace="CobraWebControls" tagprefix="cwc" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">    
    <cwc:CompositeForm ID="CompositeForm1" runat="server" SC_FormName="@[GLOBALMETA::__FormName::0]"/>    
    <cwc:CompositeAjaxLoaderPopUp ID="CompositeAjaxLoaderPopUp1" runat="server" />
    <cwc:CompositeMessageBox ID="CompositeMessageBox1" runat="server" />    
</asp:Content>


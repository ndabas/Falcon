<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Report1.aspx.cs" Inherits="FalconWeb.Reports.Report1" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:DropDownList ID="DevicesDropDownList" runat="server" DataSourceID="DevicesDataSource"
            DataTextField="VehicleNumber" DataValueField="IMEI">
        </asp:DropDownList>
        <asp:TextBox ID="FromDateTextBox" runat="server"></asp:TextBox>
        <ajaxToolkit:CalendarExtender ID="FromDateCalendar" runat="server" TargetControlID="FromDateTextBox" Format="dd-MMM-yyyy H:mm" />
        <asp:TextBox ID="ToDateTextBox" runat="server"></asp:TextBox>
        <ajaxToolkit:CalendarExtender ID="ToDateCalendar" runat="server" TargetControlID="ToDateTextBox" Format="dd-MMM-yyyy H:mm" />
        <asp:Button ID="GoButton" runat="server" Text="Go" OnClick="GoButton_Click" /><br />
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt"
            InteractiveDeviceInfos="(Collection)" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt"
            Width="100%">
            <LocalReport ReportPath="Report\Report1.rdlc">
                <DataSources>
                    <rsweb:ReportDataSource DataSourceId="ReportDataSource" Name="DataSet1" />
                </DataSources>
            </LocalReport>
        </rsweb:ReportViewer>
        <asp:ObjectDataSource ID="ReportDataSource" runat="server" SelectMethod="MovementReport"
            TypeName="FalconWeb.Report.ReportData">
            <SelectParameters>
                <asp:ControlParameter ControlID="DevicesDropDownList" Name="IMEI" PropertyName="SelectedValue"
                    Type="String" />
                <asp:ControlParameter ControlID="FromDateTextBox" Name="from" PropertyName="Text"
                    Type="DateTime" />
                <asp:ControlParameter ControlID="ToDateTextBox" Name="to" PropertyName="Text" Type="DateTime" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <asp:ObjectDataSource ID="DevicesDataSource" runat="server" SelectMethod="GetDevices"
            TypeName="FalconWeb.Report.ReportData"></asp:ObjectDataSource>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
    </div>
    </form>
</body>
</html>

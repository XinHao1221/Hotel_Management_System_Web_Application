﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddFloor.aspx.cs" Inherits="Hotel_Management_System.Hotel_Configuration_Management.Floor.AddFloor" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <link rel="stylesheet" href="../../StyleSheet/InputStyle.css" />
    <link rel="stylesheet" href="../../StyleSheet/PopupWindow.css" />

    <style>

        

        .formBtnCancel{
            margin-left:15px;
        }

        .content{
            width:100%;
            height:100%;
            opacity:<%= Name %>;
            z-index:-1;
        }

        

    </style>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <%-- Page content --%>
            <div class="content">
                <table style="width:100%;">
                    <tr>
                        <td class="formLabel">
                            Floor Name
                        </td>
                        <td class="tableSeperator"></td>
                        <td>
                            <asp:TextBox ID="txtFloorName" runat="server" CssClass="inputStyle" placeholder="Name" Width="50%"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="formLabel">Floor Number </td>
                        <td></td>
                        <td>
                            <asp:DropDownList ID="ddlFloorNumber" runat="server" CssClass="dropDownStyle" Width="10%"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="formLabel" style="vertical-align:top; padding-top:15px;">Description </td>
                        <td></td>
                        <td>
                            <asp:TextBox ID="txtDescription" runat="server" Rows="8" TextMode="MultiLine" Width="50%" CssClass="inputMultiLineTxtBox" placeholder="Type floor description here..."></asp:TextBox>
                        </td>
                    </tr>

                    <tr>
                        <td class="formLabel">Status</td>
                        <td></td>
                        <td>
                            <asp:DropDownList ID="DropDownList1" runat="server" CssClass="dropDownStyle">
                            <asp:ListItem>Active</asp:ListItem>
                            <asp:ListItem>Suspend</asp:ListItem>
                            </asp:DropDownList></td>
                    </tr>
                
                    <tr>
                        <td></td>
                        <td>
                        
                        </td>
                    </tr>
                </table>

                <div class="bottomBar">

                    <center>
                        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="formBtnSave" ToolTip="Save"/>
                        <asp:Button ID="formBtnCancel" runat="server" Text="Reset" OnClick="btnResetForm_Click" CssClass="formBtnCancel" ToolTip="Reset"/>
                    </center>
                    
                </div>
            </div>

            <%-- Popup Window --%>
            <div class="popup">
                <asp:Panel ID="PanelCancel" runat="server" Visible="False" CssClass="popupWindow">

                    <%-- Popup Window Title --%>
                    <p style="color:red;" class="popupTitle">Reset Text Field?</p>

                    <%-- Popup Window Body --%>
                    <div class="popupBody">

                        <p>All text fields will be reset!</p>

                    </div>

                    <div>&nbsp;</div>

                    <asp:Button ID="btnPopupCancel" runat="server" Text="Cancel" CssClass="popUpCancelBtn" OnClick="btnPopupCancel_Click"/>
                
                    <asp:Button ID="btnPopupConfirm" runat="server" Text="Confirm" CssClass="popUpDeleteBtn" OnClick="btnPopupConfirm_Click"/>
                </asp:Panel>
            </div>
            
        </div>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="SELECT * FROM [Floor]"></asp:SqlDataSource>
    </form>
</body>
</html>

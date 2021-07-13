﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddPreference.ascx.cs" Inherits="Hotel_Management_System.Front_Desk.Guest.AddPreference" %>

<link rel="stylesheet" href="../../StyleSheet/SubFormStyle.css" />
<link rel="stylesheet" href="../../StyleSheet/RepeaterTable.css" />

<div style="margin-left:8%; margin-top:25px;">
    <div style="float:left;">
        <div class="subFormLabel requiredFieldLabel">
            Preference
        </div>
        <div style="float:left;">
            <div>
                <asp:TextBox ID="txtPreference" runat="server" CssClass="inputStyle" placeholder="" Width="100%"></asp:TextBox>
            </div>
            <div>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtPreference" ErrorMessage="Please enter a value." ValidationGroup="add" CssClass="validatorStyle"></asp:RequiredFieldValidator>
            </div>
        </div>
    </div>

    <div style="float:left; width:5%;">
        &nbsp;
    </div>

    <div style="float:left;">

        <asp:Button ID="btnSavePreference" runat="server" Text="Add" CssClass="subFormBtnSave" OnClick="btnSavePreference_Click" ValidationGroup="add" ToolTip="Add Preference"/>

    </div>

</div>

<div style="clear:both">&nbsp;</div>

<%--Repeater table header--%>
<div style="width:80%; margin:auto;">
    <div style="float:left; width:8%; text-align:center;" class="subFormRepeaterHeader">
        No
    </div>
    <div style="float:left; width:72%;" class="subFormRepeaterHeader">
        Preference
    </div>
    <div style="float:left; width:15%;" class="subFormRepeaterHeader">
        Date Added
    </div>
    <div style="float:left; width:5%;" class="subFormRepeaterHeader">
        &nbsp;
    </div>
</div> 

<%--Repeater Table content--%>
<asp:Repeater ID="Repeater1" runat="server">

    <ItemTemplate>
        <div style="width:80%; margin:auto;">
            <div style="float:left; width:8%; text-align:center;" class="subFormTableContent">
                <asp:Label ID="lblNumber" runat="server" Text='<%# Container.ItemIndex + 1 %>'></asp:Label>
            </div>
            <div style="float:left; width:72%;" class="subFormTableContent">
                <asp:Label ID="lblPreference" runat="server" Text='<%# Eval("preference") %>'></asp:Label>
            </div>
            <div style="float:left; width:15%;" class="subFormTableContent">
                <asp:Label ID="lblDate" runat="server" Text='<%# Eval("date") %>'></asp:Label>
            </div>
            <div style="float:left; width:2%;" class="subFormTableContent">
                &nbsp;
            </div>
            <div style="float:left; width:3%;" class="subFormTableContent">
                <asp:ImageButton ID="IBDeletePreference" runat="server" OnClick="IBDeletePreference_Click" ImageUrl="~/Image/delete_icon.png" CssClass="subFormIcon" ToolTip="Delete"/>
            </div>
        </div>  
    </ItemTemplate>

    <AlternatingItemTemplate>
        <div style="width:80%; margin:auto;">
            <div style="float:left; width:8%; text-align:center;" class="subFormTableContentAlternate">
                <asp:Label ID="lblNumber" runat="server" Text='<%# Container.ItemIndex + 1 %>'></asp:Label>
            </div>
            <div style="float:left; width:72%;" class="subFormTableContentAlternate">
                <asp:Label ID="lblPreference" runat="server" Text='<%# Eval("preference") %>'></asp:Label>
            </div>
            <div style="float:left; width:15%;" class="subFormTableContentAlternate">
                <asp:Label ID="lblDate" runat="server" Text='<%# Eval("date") %>'></asp:Label>
            </div>
            <div style="float:left; width:2%;" class="subFormTableContentAlternate">
                &nbsp;
            </div>
            <div style="float:left; width:3%;" class="subFormTableContentAlternate">
                <asp:ImageButton ID="IBDeletePreference" runat="server" OnClick="IBDeletePreference_Click" ImageUrl="~/Image/delete_icon.png" CssClass="subFormIcon" ToolTip="Delete"/>
            </div>
        </div> 
    </AlternatingItemTemplate>
    
</asp:Repeater>

<div style="width: 80%; margin: auto; clear:both;">

    <div class="subFormTableContent" style="padding-left:2%;">
        <asp:Label ID="lblNoItemFound" runat="server" Text="No item found!" Visible="false"></asp:Label>
    </div>           
</div>

<%--Popup Window--%>
<div class="popup">
    <asp:Panel ID="PopupDelete" runat="server" Visible="False" CssClass="popupWindow">

        <%-- Popup Window Title --%>
        <p style="color:red;" class="popupTitle">Delete</p>

        <%-- Popup Window Body --%>
        <div class="popupBody">

            <asp:Label ID="lblPopupDeleteContent" runat="server" Text="Label"></asp:Label>

        </div>

        <div>&nbsp;</div>

        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="popUpCancelBtn" OnClick="btnCancel_Click"/>
                
        <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="popUpDeleteBtn" OnClick="btnDelete_Click"/>
    </asp:Panel>
</div>

<%-- Popup Cover --%>
<asp:Panel ID="PopupCover" runat="server" CssClass="popupCoverStyle" Visible="false">
    &nbsp;
</asp:Panel>
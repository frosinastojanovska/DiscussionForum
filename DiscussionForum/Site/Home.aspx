﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site/SiteMaster.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="DiscussionForum.Site.Home" %>


<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row row-separated">
        <div class="col-xs-8">
            <ul class="nav nav-pills">
                <li id="latest" class="active">Latest</li>
                <li id="top">Top</li>
                <li id="most-popular">Most Popular</li>
            </ul>
        </div>
        <div class="col-xs-4">
            <asp:DropDownList ID="ddlCategories" runat="server" CssClass="form-control"></asp:DropDownList>
        </div>
    </div>
    <div class="row row-separated">
        <asp:Table ID="tableTopics" runat="server" class="display" CellSpacing="0" Width="100%">
        </asp:Table>
    </div>
</asp:Content>

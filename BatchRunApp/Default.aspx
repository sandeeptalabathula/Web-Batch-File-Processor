<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="BatchRunApp._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <style>
        a.buttonStyledLink
        {
            font-family: Verdana;
            font-size: 8pt;
            display: block;
            padding: 0.5em;
            line-height: 1;
            background-color: #94B8E9;
            border: 1px solid black;
            color: #000;
            text-decoration: none;
            text-align: center;
        }
        a.buttonStyledLink:link
        {
            font-family: Verdana;
            font-size: 8pt;
            display: block;
            padding: 0.5em;
            line-height: 1;
            background-color: #94B8E9;
            border: 1px solid black;
            color: #000;
            text-decoration: none;
            text-align: center;
        }
        a.buttonStyledLink:visited
        {
            font-family: Verdana;
            font-size: 8pt;
            display: block;
            padding: 0.5em;
            line-height: 1;
            background-color: #94B8E9;
            border: 1px solid black;
            color: #000;
            text-decoration: none;
            text-align: center;
        }
        a.buttonStyledLink:hover
        {
            background-color: #369;
            text-decoration: underline;
            color: Black;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Quick New Batch Run
    </h2>
    <hr />
    <p style="font-size: 13px;">
        Choose Batch file to run:
        <asp:FileUpload runat="server" ID="batchFileLoader" />
        <br />
        <br />
        <asp:LinkButton ID="LinkButton1" CssClass="buttonStyledLink" Width="15em" runat="server" OnClick="LinkButton1_Click">Click here to run this Batch</asp:LinkButton>
    </p>
    <asp:Panel ID="pnlResult" runat="server" Visible="false">
            <h2>Batch Run Result</h2>
            <hr />
            <span style="color: Green; font-size: medium">Congrats! Your batch run successfully</span><br />
            <br />
            <asp:HyperLink ID="hlnkResults" runat="server" Target="_blank" Text="Click here to View the generated log"></asp:HyperLink>
        </asp:Panel>
    <br />
    <h2>
        Previous Batch Runs
    </h2>
    <hr />
    <br />
    <asp:GridView ID="GridView1" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
        AutoGenerateColumns="false" Width="800px" 
        onrowcommand="GridView1_RowCommand" RowStyle-Wrap="true">
        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        <EditRowStyle BackColor="#999999" />
        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#F7F6F3" Height="15px" ForeColor="#333333" />
        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
        <SortedAscendingCellStyle BackColor="#E9E7E2" />
        <SortedAscendingHeaderStyle BackColor="#506C8C" />
        <SortedDescendingCellStyle BackColor="#FFFDF8" />
        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
        <Columns>
            <asp:BoundField DataField="FileName" ItemStyle-Font-Bold="true" HeaderText="Batch Name" ItemStyle-Width="30%"></asp:BoundField>
            <asp:BoundField DataField="FilePath" HeaderText="Batch File Path" ItemStyle-Width="50%">
            </asp:BoundField>
            <asp:TemplateField ItemStyle-Width="15%">
                <ItemTemplate>
                    <asp:LinkButton ID="lnkButtonRunBatchGrid" CssClass="buttonStyledLink" Width="9em" runat="server" CommandName="RunBatch"
                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "FilePath") + "," + ((GridViewRow) Container).RowIndex + ", "+ 
                            DataBinder.Eval(Container.DataItem, "FileName") %> '>Run this Batch</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <br />
    <br />
</asp:Content>

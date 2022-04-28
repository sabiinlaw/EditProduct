<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddProductForm.aspx.cs" Inherits="EditProductApp.AddProductForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="../EditProductStyle.css" />
</head>
<body>
    <form id="addform" runat="server">
        <div>
            <h1>Add Product</h1>
            <p>Here you can add a product</p>
        </div>
        <asp:ValidationSummary ID="validationSummary" runat="server" ShowModelStateErrors="true" />
        <div>
            <label class="info">Name:</label><input type="text" class="text" id="name" runat="server" /></div>
        <div>
            <label class="info">Description:</label><input type="text" class="text" id="description" runat="server" /></div>
        <div>
            <asp:button id="SaveButton" class="button" runat="server" Text="Save" OnClick="SaveButton_Click" />
        </div>
    </form>
</body>
</html>

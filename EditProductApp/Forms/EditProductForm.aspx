<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditProductForm.aspx.cs" Inherits="EditProductApp.EditProductForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="../EditProductStyle.css" />
</head>
<body>
    <form id="editform" runat="server">
        <div>
            <h1>Edit Product</h1>
            <p>Here you can make some changes</p>
        </div>
        <asp:ValidationSummary ID="validationSummary" runat="server" ShowModelStateErrors="true" />
        <div>
            <label class="info">Name:</label><asp:textbox type="text" class="text" id="name" runat="server" text="<%# sessionProduct.Name%>" AutoPostBack="True" OnTextChanged="TextBox_TextChanged"></asp:textbox>
        </div>
        <div>
            <label style="color:red" class="warning" id="labelWarningName" runat="server" />
        </div>
        <div>
            <label class="info">Description:</label><asp:textbox type="text" class="text" id="description" runat="server" text="<%# sessionProduct.Description%>" AutoPostBack="True" OnTextChanged="TextBox_TextChanged"></asp:textbox>
        </div>
        <div>
            <label style="color:red" class="warning" id="labelWarningDesc" runat="server" />
        </div>
        <div>
            <asp:button id="SaveButton" class="button" runat="server" Text="Save" OnClick="SaveButton_Click" />
        </div>
        <div>
            <label style="color:red" class="warning" id="labelWarningGeneral" runat="server" />
        </div>
    </form>
</body>
</html>

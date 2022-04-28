<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserLogInForm.aspx.cs" Inherits="EditProductApp.UserLogInForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="../EditProductStyle.css" />
</head>
<body>
    <form id="loginform" runat="server">
        <div>
            <h1>Please LogIn</h1>
        </div>
        <asp:ValidationSummary ID="validationSummary" runat="server" ShowModelStateErrors="true" />
        <div>
            <label class="info">Name:</label><input type="text" class="text" id="name" runat="server" /></div>
        <div>
            <asp:button id="SaveButton" class="button" runat="server" Text="LogIn" OnClick="LogInButton_Click" />
        </div>
    </form>
</body>
</html>

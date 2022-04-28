<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductList.aspx.cs"
    Inherits="EditProductApp.ProductList" %>
<%@ Import Namespace="EditProductApp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="../EditProductStyle.css" />
</head>
<body>
<form id="productListform" runat="server">
    <h1>Products</h1>
    <table>
        <thead>
            <tr>
                <th>Name</th>
                <th>Description</th>
                <th>Actions</th>
            </tr>
        </thead>
        <tbody>
            <asp:Repeater ID="ItemsRepeater" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("Name") %></td>
                            <td><%# Eval("Description") %></td>
                            <td>
                                <asp:button class='editButton' runat='server' Text="Edit" CommandArgument='<%#Eval("ProductId")%>' OnClick='EditButton_Click'/>
                            </td>
                        </tr>
                    </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
    
    <div>
        <asp:button id="AddProduct" class="button" runat="server" Text="Add" OnClick="AddButton_Click" />
    </div>
</form>
</body>
</html>

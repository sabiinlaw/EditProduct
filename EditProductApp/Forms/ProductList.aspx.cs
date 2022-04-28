using System;
using System.Web.UI.WebControls;

namespace EditProductApp
{
    public partial class ProductList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = (User)Session["user"];
            if (currentUser == null)
            {
                Response.Redirect("UserLogInForm.aspx");
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            ItemsRepeater.DataSource = ProductRepository.GetRepository().GetAllProducts();
            ItemsRepeater.DataBind();
        }
        protected void EditButton_Click(object sender, EventArgs e)
        {
            if (sender is Button edit)
            {
                string url = "EditProductForm.aspx?productId=" + edit.CommandArgument;
                Response.Redirect(url);
            }
        }
        protected void AddButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddProductForm.aspx");
        }

    }
}
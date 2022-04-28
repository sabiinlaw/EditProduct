using System;
using System.Web.ModelBinding;

namespace EditProductApp
{
    public partial class AddProductForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = (User)Session["user"];
            if (currentUser == null)
            {
                Response.Redirect("UserLogInForm.aspx");
            }
        }
        protected void SaveButton_Click(object sender, EventArgs e)
        {

            if (IsPostBack)
            {
                Product product = new Product();

                if (TryUpdateModel(product, new FormValueProvider(ModelBindingExecutionContext)))
                {
                    product.ProductId = ProductRepository.GetRepository().GetAllProducts().Count + 1;
                    product.ProductUpdateTimeStamp = DateTime.Now;
                    ProductRepository.GetRepository().AddProduct(product);
                    Response.Redirect("ProductList.aspx");
                }
            }
        }
    }
}
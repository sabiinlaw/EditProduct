using System;
using System.Web.ModelBinding;

namespace EditProductApp
{
    public partial class UserLogInForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void LogInButton_Click(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                User user = new User();

                if (TryUpdateModel(user, new FormValueProvider(ModelBindingExecutionContext)))
                {
                    user.UserId = UserRepository.GetRepository().GetAllUsers().Count + 1;
                    UserRepository.GetRepository().AddUser(user);
                    Session["user"] = user;
                    Response.Redirect("ProductList.aspx");
                }
            }
        }
    }
}
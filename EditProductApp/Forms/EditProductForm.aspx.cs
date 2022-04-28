using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Web.ModelBinding;
using System.Web.UI.WebControls;

namespace EditProductApp
{
    public partial class EditProductForm : System.Web.UI.Page
    {
        public Product sessionProduct { get; set; }
        private readonly static int SessionExpirationTime = 5;
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = (User)Session["user"];
            if (currentUser != null)
            {
                var user = currentUser.Name + currentUser.UserId;
                int.TryParse(Request.QueryString["productId"], out int productId);
                var product = ProductRepository.GetRepository().GetProduct(productId);
                if (product != null)
                {
                    //save original product values when user hit Edit at first time
                    sessionProduct = Session["originalProduct"] as Product;
                    if (sessionProduct == null)
                    {
                        sessionProduct = product;
                        Session["originalProduct"] = product;
                    }

                    Product newProduct = new Product() { ProductId = product.ProductId };
                    if (TryUpdateModel(newProduct, new FormValueProvider(ModelBindingExecutionContext)))
                    {
                        sessionProduct = newProduct;
                    }

                    this.DataBind();

                    var userProductQueueCollection = HttpRuntime.Cache.Get("userProductQueue") as Dictionary<int, Queue<string>> ?? new Dictionary<int, Queue<string>>();
                    var userSessionStorage = HttpRuntime.Cache.Get("userProductSessions") as Dictionary<int, Dictionary<string, DateTime>> ?? new Dictionary<int, Dictionary<string, DateTime>>();
                    var userProductQueue = userProductQueueCollection.Count > 0 && userProductQueueCollection.ContainsKey(product.ProductId) ? userProductQueueCollection[product.ProductId] : new Queue<string>();

                    // add users that want to edit products to Queue (collection that track users for each product)
                    // also keep 2 more collections:
                    //  a) mainUserProduct - helps to find who has blocking marker as Main User for current product
                    //  b) userProductSessions - keeps "edit expiration time" for any user that what to edit the same product
                    if (userProductQueue.Count == 0 || !userProductQueue.Contains(user))
                    {
                        var mainUserProductCollection = HttpRuntime.Cache.Get("mainUserProduct") as Dictionary<int, string> ?? new Dictionary<int, string>();
                        var mainUser = mainUserProductCollection != null && mainUserProductCollection.ContainsKey(product.ProductId) ? mainUserProductCollection[product.ProductId] : null;
                        if (mainUser != user)
                        {
                            if (userSessionStorage.ContainsKey(product.ProductId) && userSessionStorage[product.ProductId].ContainsKey(user))
                            {
                                var currentProductUserSessions = userSessionStorage[product.ProductId];

                                if (currentProductUserSessions.FirstOrDefault(x => x.Key == user).Value > DateTime.Now.AddMinutes(SessionExpirationTime))
                                {
                                    currentProductUserSessions.Remove(user);
                                }
                            }
                            else
                            {
                                if (userSessionStorage.Count == 0 || !userSessionStorage.ContainsKey(product.ProductId) || !userSessionStorage[product.ProductId].Any(x => x.Key == user))
                                {
                                    if (userSessionStorage.ContainsKey(product.ProductId) && !userSessionStorage[product.ProductId].ContainsKey(user))
                                    {
                                        userSessionStorage[product.ProductId].Add(user, DateTime.Now.AddMinutes(SessionExpirationTime * (userProductQueue.Count + 1)));
                                    }
                                    else
                                    {
                                        userSessionStorage.Add(product.ProductId, new Dictionary<string, DateTime>() {
                                            { user, DateTime.Now.AddMinutes(SessionExpirationTime * (userProductQueue.Count + 1)) }
                                        });
                                    }

                                    if (mainUser == null)
                                    {
                                        mainUserProductCollection.Add(product.ProductId, user);
                                        HttpRuntime.Cache.Insert("mainUserProduct", mainUserProductCollection);
                                    }
                                    else
                                    {
                                        userProductQueue.Enqueue(user);
                                        if (userProductQueueCollection.ContainsKey(product.ProductId))
                                        {
                                            userProductQueueCollection[product.ProductId] = userProductQueue;
                                        }
                                        else
                                        {
                                            userProductQueueCollection.Add(product.ProductId, userProductQueue);
                                        }
                                    }
                                }
                            }
                        }

                        HttpRuntime.Cache.Insert("userProductQueue", userProductQueueCollection);
                        HttpRuntime.Cache.Insert("userProductSessions", userSessionStorage);
                    }
                }
            }
            else
            {
                Response.Redirect("UserLogInForm.aspx");
            }
        }
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                if (sessionProduct != null)
                {
                    bool allowToSave = true;
                    bool mainSessionIsExpired = false;
                    bool saveWithoutInform = true;
                    int timeToSave = SessionExpirationTime;

                    var user = ((User)Session["user"]).Name + ((User)Session["user"]).UserId;
                    var originalProduct = Session["originalProduct"] as Product;
                    var mainUserProductCollection = HttpRuntime.Cache.Get("mainUserProduct") as Dictionary<int, string>;
                    var mainUser = mainUserProductCollection[originalProduct.ProductId];
                    var userSessionStorage = HttpRuntime.Cache.Get("userProductSessions") as Dictionary<int, Dictionary<string, DateTime>>;

                    var currentProductUserSessionStoarge = userSessionStorage[originalProduct.ProductId];
                    if (currentProductUserSessionStoarge.Any(x=>x.Key == user))
                    {
                        // check if current User has blocking marker and authority to be first to save or
                        // allow to save any user that are in Queue if Main User blockin marker time is expired
                        var mainUserSessionTime = currentProductUserSessionStoarge[mainUser];
                        if (mainUser != null && user != mainUser)
                        {
                            var timeDiff = (mainUserSessionTime - DateTime.Now);
                            timeToSave = timeDiff.Minutes * 60 + timeDiff.Seconds;
                            if (timeToSave > 0)
                            {
                                allowToSave = false;
                            }
                            else
                            {
                                mainSessionIsExpired = true;
                            }
                        }
                    }

                    if (allowToSave)
                    {
                        //get real product values from DB and compare them with current user session values and current product state to define if it's a Main User (if Yes - allow to save)
                        var realProduct = ProductRepository.GetRepository().GetProduct(originalProduct.ProductId);
                        var currentProductTimeStamp = HttpRuntime.Cache.Get(sessionProduct.ProductId + "_timestamp_" + user) as string;
                        var sessionProductNameValue = HttpRuntime.Cache.Get(sessionProduct.ProductId + "_name_" + user) as string;
                        var sessionProductDescriptionValue = HttpRuntime.Cache.Get(sessionProduct.ProductId + "_description_" + user) as string;


                        //allow to do regular save as Main User or inform users in Queue if they try to save the same product but it was already updated in DB by Main User 
                        saveWithoutInform = (originalProduct?.Name == realProduct?.Name && originalProduct?.Description == realProduct?.Description &&
                                            sessionProduct?.Name == sessionProductNameValue && sessionProduct?.Description == sessionProductDescriptionValue) ||
                                            realProduct?.ProductUpdateTimeStamp.Ticks.ToString() == currentProductTimeStamp ||
                                            mainSessionIsExpired;

                        //regular Save
                        if (saveWithoutInform)
                        {
                            //save edit product with updated time
                            sessionProduct.ProductUpdateTimeStamp = DateTime.Now;
                            ProductRepository.GetRepository().UpdateProduct(sessionProduct);

                            Session["originalProduct"] = null; //clear presaved original product for user that saves 
                            mainUserProductCollection.Remove(sessionProduct.ProductId); //remove Main User that has blocking marker for editing product

                            //check if Queue has more users that wanted to edit same product 
                            // if Yes then set next user from Queue as Main User
                            var userProductQueueCollection = HttpRuntime.Cache.Get("userProductQueue") as Dictionary<int,Queue<string>>;
                            var userProductQueue = userProductQueueCollection.Count > 0 && userProductQueueCollection.ContainsKey(originalProduct.ProductId) ? userProductQueueCollection[originalProduct.ProductId] : new Queue<string>();
                            if (userProductQueue.Count > 0)
                            {
                                if (!mainSessionIsExpired)
                                {
                                    mainUserProductCollection.Add(originalProduct.ProductId, userProductQueue.Dequeue());
                                    HttpRuntime.Cache.Insert("mainUserProduct", mainUserProductCollection);
                                }
                                else
                                {
                                    userProductQueue.Dequeue();
                                }
                                HttpRuntime.Cache.Insert("userProductQueue", userProductQueueCollection);
                            }

                            //clear current user Session for current Product
                            if (currentProductUserSessionStoarge.Any(x => x.Key == user))
                            {
                                userSessionStorage[originalProduct.ProductId].Remove(user);
                                if (mainSessionIsExpired)
                                {
                                    userSessionStorage[originalProduct.ProductId].Remove(mainUser);
                                }
                                HttpRuntime.Cache.Insert("userProductSessions", userSessionStorage);

                            }

                            Response.Redirect("ProductList.aspx");
                        }
                        else //notify users in Queue what fields were changed if they try to overwrite the same product
                        {
                            HttpRuntime.Cache.Insert(sessionProduct.ProductId + "_timestamp_" + user, realProduct.ProductUpdateTimeStamp.Ticks.ToString());
                            if (originalProduct.Description != realProduct.Description && sessionProductDescriptionValue != realProduct.Description)
                            {
                                labelWarningDesc.InnerText = String.Format("Description is saved with '{0}' by main user.", realProduct.Description);
                            }
                            if (originalProduct.Name != realProduct.Name && sessionProductNameValue != null && sessionProductNameValue != realProduct.Name)
                            {
                                labelWarningName.InnerText = String.Format("Name is saved with '{0}' by main user.", realProduct.Name);
                            }

                            labelWarningGeneral.InnerText = String.Format("The product you try to save has been already saved by main user. Do you want to overwrite his changes?", realProduct.Name, realProduct.Description);
                        }
                    }
                    else //notify users in Queue if they try to save product which is currently under editing
                    {
                        labelWarningDesc.InnerText = null;
                        labelWarningName.InnerText = null;
                        labelWarningGeneral.InnerText = String.Format("The product you try to save is also editing by main user. Please wait {0} second(s)", timeToSave);
                    }
                }
            }
        }
        protected void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox currentTextBox)
            {
                var user = ((User)Session["user"]).Name + ((User)Session["user"]).UserId;
                var key = sessionProduct.ProductId + "_" + currentTextBox.ID + "_" + user;
                var mainUserProductCollection = HttpRuntime.Cache.Get("mainUserProduct") as Dictionary<int, string>;
                var mainUser = mainUserProductCollection[sessionProduct.ProductId];
                var cachedValue = HttpRuntime.Cache.Get(sessionProduct.ProductId +"_" + currentTextBox.ID + "_" + mainUser) as string;

                //update Main User if missed
                if (mainUser == null)
                {
                    mainUserProductCollection.Add(sessionProduct.ProductId, user);
                    HttpRuntime.Cache.Insert("mainUserProduct", mainUserProductCollection);
                }

                // cache changed properties for any user and any product
                if (cachedValue == null || cachedValue != currentTextBox.Text)
                {
                    HttpRuntime.Cache.Insert(key, currentTextBox.Text);
                    HttpRuntime.Cache.Insert(sessionProduct.ProductId + "_timestamp_" + user, DateTime.Now.Ticks.ToString());
                }

                // notify users in Queue if they edit the same fields as Main User
                if (mainUser != null && user != mainUser && currentTextBox.Text != cachedValue)
                {
                    if (cachedValue != null)
                    {
                        if (currentTextBox.ID == "description")
                        {
                            labelWarningDesc.InnerText = String.Format("Description is changed to '{0}' by main user.", cachedValue);
                        }
                        else if (currentTextBox.ID == "name")
                        {
                            labelWarningName.InnerText = String.Format("Name is changed to '{0}' by main user.", cachedValue);
                        }
                    }
                }
            }
        }
    }
}
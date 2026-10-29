using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using ShopEasy.Models;
using System.Diagnostics.CodeAnalysis;

namespace ShopEasy.Controllers
{


    public class CustomerController : Controller
    {
        private readonly ShopEasyContext Context;
        public CustomerController(ShopEasyContext _context)
        {
            Context = _context;
        }

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("CustomerID");
            var pdt = Context.ProductTypes
                .Include(e => e.CategoryModel)
                .Include(r => r.wishlist.Where(w => w.username == username))
                .Include(x => x.Cart.Where(c => c.username == username))
                .ToList();
            var categories = Context.ProductCategories.Select(x => new SelectListItem
            {
                Value = x.CategoryId.ToString(),
                Text = x.CategoryName
            })
                 .ToList();

           
            ViewBag.Wishlist = Context.wishlists.Count(x => x.username == username);
            ViewBag.cart = Context.Carts.Count(x => x.username == username);

            ViewBag.type = categories;

            return View(pdt);

        }
        public IActionResult Viewproducts()
        {
            var products = Context.ProductTypes.ToList();
            var data = Context.ProductCategories.Select(x => new SelectListItem
            {
                Value = x.CategoryId.ToString(), // What get sents to the controller/Database
                Text = x.CategoryName // What user sees

            })
                   .ToList();

            ViewBag.data = data;
            return View("customerproduct", products);
        }

        public IActionResult Filter(string cat)
        {
            var data1 = Context.ProductCategories.Select(x => new SelectListItem
            {
                Value = x.CategoryId.ToString(),
                Text = x.CategoryName
            })
                .ToList();
            int catid = int.Parse(cat);



            ViewBag.type = data1;

            var username = HttpContext.Session.GetString("CustomerID");

            ViewBag.Wishlist = Context.wishlists.Count(x => x.username == username);
            ViewBag.cart = Context.Carts.Count(x => x.username == username);



            if (catid == 0)
            {
               
               var  pdt = Context.ProductTypes
                    .Include(e => e.CategoryModel)
                    .Include(r => r.wishlist.Where(w => w.username == username))
                    .Include(x => x.Cart.Where(c => c.username == username))
                    .ToList();
                return View("Index", pdt);
            }
            else
            {
               var  pdt = Context.ProductTypes
                    .Include(e => e.CategoryModel)
                    .Include(r => r.wishlist.Where(w => w.username == username))
                    .Include(x => x.Cart.Where(c => c.username == username)).Where(output => output.CategoryID == catid)
                    .ToList();
                return View("Index", pdt);
            }
          
        }

        public IActionResult ProductDetails(int id)  // While clicking on eye icon
        {
            
            var details = Context.ProductTypes.Include(x => x.CategoryModel).FirstOrDefault(p => p.pid == id);
            var username = HttpContext.Session.GetString("CustomerID");
            ViewBag.Wishlist = Context.wishlists.Count(x => x.username == username);
            ViewBag.cart = Context.Carts.Count(x => x.username == username);
            var pdt = Context.Carts.FirstOrDefault(x => x.username == username && x.Pid == id);
            if (pdt != null)
            {
                ViewBag.Message = "1";
            }
            else
            {
                ViewBag.Message = "0";
            }

            var stock = Context.Stocks.FirstOrDefault(x => x.Pid == id);  // showing stock quantity
            if (stock == null || stock.Quantity == 0)
            {
                ViewBag.msg = "0";
            }
            else
            {
                ViewBag.msg = "1";
            }

            var review = Context.Feedbacks.Join(Context.CustomerRegs,          //feedback table
                f => f.username,
                c => c.Email,
                (f, c) => new
                {
                    Pid = f.Pid,
                    Username = c.UserName,
                    Rating = f.Rating,
                    Comment = f.feedback
                }).Where(x => x.Pid == id).ToList();


            List<Feedback> fd = new List<Feedback>();
            foreach (var item in review)
            {
                fd.Add(new Feedback
                {
                    Rating=item.Rating,
                    feedback=item.Comment,
                    Customer=new CustomerReg { UserName=item.Username}

                });
            }
            ViewBag.Count = fd.Count;
            ViewBag.Rating = fd;
           
            return View("Details", details);
        }

        public IActionResult AddToCart(int id)
        {
            var username = HttpContext.Session.GetString("CustomerID");

            
            var pdt = Context.Carts.FirstOrDefault(x => x.username == username && x.Pid == id);
            if (pdt == null)
            {
                //Context.Carts.Add(new Cart
                //{
                //    username = username,
                //    Pid = id,
                //});
                Cart obj = new Cart { Pid = id, username = username };
                Context.Carts.Add(obj);

                Context.SaveChanges();
            }



            return RedirectToAction("Index");
        }


        public IActionResult AddtoCartFromwishlist(int id)    // to add products to cart from wishlist while clicking wish icon
        {
            var username = HttpContext.Session.GetString("CustomerID");
            
            var pdt = Context.Carts.FirstOrDefault(x => x.username == username && x.Pid == id);
            if (pdt == null)
            {
                //Context.Carts.Add(new Cart
                //{
                //    username = username,
                //    Pid = id,
                //});
                Cart obj = new Cart { Pid = id, username = username };
                Context.Carts.Add(obj);

                Context.SaveChanges();
            }



            return RedirectToAction("ViewWishlist");
        }




        public IActionResult WishList(int id)
        {
            var username = HttpContext.Session.GetString("CustomerID");
            var pdt = Context.wishlists.FirstOrDefault(x => x.username == username && x.Pid == id); //Check whether this CUSTOMER already has this PRODUCT in the wishlist.
            if (pdt == null) //Check whether the wishlist record was NOT found.
            {
                wishlist obj = new wishlist { Pid = id, username = username };
                Context.wishlists.Add(obj);
                Context.SaveChanges();
                //  ViewBag.wish = "0";

            }
            else  //This customer already has this product in wishlist.
            {
                Context.wishlists.Remove(pdt);
                Context.SaveChanges();
                //  ViewBag.wish = "1";
            }
            return RedirectToAction("Index");
        }

        public IActionResult ViewWishlist()
        {
            var username = HttpContext.Session.GetString("CustomerID");
            ViewBag.Wishlist = Context.wishlists.Count(x => x.username == username);
            ViewBag.cart = Context.Carts.Count(x => x.username == username);

            var wishlist = Context.ProductTypes
      .Include(x => x.CategoryModel)
      .Include(x => x.wishlist.Where(w => w.username == username))
      .Include(x => x.Cart.Where(c => c.username == username))
      .Where(r => r.wishlist.Any(w => w.username == username))
      .ToList();
            //var wishlist = Context.ProductTypes.Include(x => x.wishlist).Include(c=>c.Cart).Where(r => r.wishlist.username == username).ToList();

            return View("wishlist", wishlist);

        }

        public IActionResult Removewishlist(int id)
        {
            var username = HttpContext.Session.GetString("CustomerID");
            var remove = Context.wishlists.FirstOrDefault(x => x.Pid == id && x.username == username);
            Context.wishlists.Remove(remove);
            Context.SaveChanges();

            return RedirectToAction("ViewWishlist");

        }

        public IActionResult ViewCart()
        {
            var username = HttpContext.Session.GetString("CustomerID");
            ViewBag.Wishlist = Context.wishlists.Count(x => x.username == username);
            ViewBag.cart = Context.Carts.Count(x => x.username == username);

            //var cart = Context.ProductTypes.Include(x => x.Cart).Include(a=>a.wishlist).Where(r => r.Cart.username == username).ToList();
            var cart = Context.ProductTypes
             .Include(x => x.CategoryModel)
             .Include(x => x.Cart.Where(c => c.username == username))
             .Include(x => x.wishlist.Where(w => w.username == username))
             .Where(r => r.Cart.Any(c => c.username == username))
             .ToList();
            return View("cart", cart);
        }

        public IActionResult Removefromcart(int id)
        {
            var username = HttpContext.Session.GetString("CustomerID");
            var remove = Context.Carts.FirstOrDefault(x => x.Pid == id && x.username == username);
            Context.Carts.Remove(remove);
            Context.SaveChanges();
            return RedirectToAction("ViewCart");
        }

        public IActionResult Profile()
        {
            var username = HttpContext.Session.GetString("CustomerID");
            var data = Context.CustomerRegs.Join(Context.LoginTbs, c => c.Email, l => l.Email, (c, l) => new  // joining two tables and select only the required data

            {

                c.RegId,    // customerreg Records
                l.Email,   // LoginTbs Records
                l.Password,
                c.UserName

            }).Where(r => r.Email == username);  //  use this Join when we cannot bind two models directly using include. // Find the currently logged in customer using where


            CustomerReg obj = new CustomerReg
            {                            // 
                UserName = data.FirstOrDefault().UserName,
                Email = data.FirstOrDefault().Email,

                loginModel = new LoginTb
                {
                    Email = data.FirstOrDefault().Email,
                    Password = data.FirstOrDefault().Password
                }

            };
            ViewBag.Wishlist = Context.wishlists.Count(x => x.username == username);
            ViewBag.cart = Context.Carts.Count(x => x.username == username);

            //To Display address 

            ViewBag.address = Context.Addresses.Where(x => x.Username == username).ToList();




            return View("editprofile", obj);
        }
        [HttpPost]
        public IActionResult Updateprofile(CustomerReg obj,string btn,string Address)
        {

            if (btn=="Update")
            {
                var customer = Context.CustomerRegs.FirstOrDefault(x => x.Email == obj.Email);  //Find the customer record using Email
                if (customer == null)
                    return NotFound();

                customer.UserName = obj.UserName;
                //   customer.Email = obj.Email;


                var login = Context.LoginTbs.FirstOrDefault(x => x.Email == obj.Email); //Find the corresponding login record.
                                                                                        //CustomerReg and LoginTb are separate tables
                                                                                        // We use the same Email to find the login record.


                if (login == null)
                    return NotFound();
                login.Password = obj.loginModel.Password;

                Context.Update(customer);
                Context.Update(login);

                Context.SaveChanges();   // EF sends UPDATE statements only for changed values
            }
            else if (btn=="address")
            {
                //var username = HttpContext.Session.GetString("CustomerID");
                Addresses obj1 = new Addresses
                {
                    Username = obj.Email,     // obj.Email → identifies which customer owns the address.
                                              
                   Address = Address       // Address   → address entered by the customer.
                };
                Context.Addresses.Add(obj1);
                Context.SaveChanges();
            }


            return RedirectToAction("Profile");
        }
        public IActionResult Buynow(int id)
        {
            var username = HttpContext.Session.GetString("CustomerID");

            var data = Context.ProductTypes.Where(x=>x.pid==id).ToList();     // Find the product whose pid matches the id received
                                                                              // from the "Buy Now" button.
            TempData.Keep("msg");
          
            var address = Context.Addresses.Where(x => x.Username == username).ToList();  // Go to the Addresses table and get the addresses that already exist for this customer.

            ViewBag.Add = address;
            ViewBag.Wishlist = Context.wishlists.Count(x => x.username == username);
            ViewBag.cart = Context.Carts.Count(x => x.username == username);



            return View("order",data);
        }
        [HttpPost]
        public IActionResult PlaceOrder(int Pid,int quantity,int totalprice, int addressid)
        {
            var username = HttpContext.Session.GetString("CustomerID");
            var stock = Context.Stocks.FirstOrDefault(x => x.Pid == Pid); // To check the available quantity while customer place order
            if (stock==null)
          
            {
                TempData["msg"] = "Insufficient Stock!";
                return RedirectToAction("Buynow", new { id = Pid });
            }

            
            if (stock.Quantity > quantity)
     
            {
               

                Order Obj = new Order
                {
                    username = username,
                    pid = Pid,
                    quantity = quantity,
                    totalprice = totalprice,
                    orderdate = DateTime.Now.ToString(),
                    status = 0,                                 // pending
                    addressId = addressid

                };

                // Send success message to the next page
                TempData["msg"] = "Order placed successfully!";


                Context.Orders.Add(Obj);
                Context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["msg"] = "Insufficient Stock!";
                return RedirectToAction("Buynow", new { id = Pid });
            }
        
          

        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Index");
        }

        public IActionResult MyOrders()
        {
            var username = HttpContext.Session.GetString("CustomerID");

            //var myord = Context.Orders.Include(x => x.ProductType ).Where(r => r.username == username).ToList();

            ViewBag.Wishlist = Context.wishlists.Count(x => x.username == username);
            ViewBag.cart = Context.Carts.Count(x => x.username == username);
            var orders = Context.Orders.Join(
            Context.ProductTypes,
              o => o.pid,
              p => p.pid,
             (o, p) => new
             {
                 Username = o.username,
                 Orderid = o.Oid,
                 Productid = p.pid,
                 Productname = p.ProductName,
                 Productprice =p.Price,
                 Quantity = o.quantity,
                 Totalprice = o.totalprice,
                 Orderdate = o.orderdate,
                 Image = p.ImagePath,
                 Status = o.status
             })
               
                .OrderByDescending(x => x.Orderid).Where(x => x.Username == username)


               .ToList();
           
            List<Order> myord = new List<Order>();


            foreach (var item in orders)       //take from item
            {

                myord.Add(new Order
                {
                    ProductType = new ProductType { ProductName = item.Productname, ImagePath = item.Image,Price=item.Productprice },
                    username = item.Username,
                    quantity = item.Quantity,
                    totalprice = item.Totalprice,
                    orderdate = item.Orderdate,
                    Oid=item.Orderid,
                    status=item.Status,
                
                });

            }

            return View("Myorders",myord);
        }

        public IActionResult OrderDetails(int id)
        {
            var username = HttpContext.Session.GetString("CustomerID");
            var order = Context.Orders.Join(Context.ProductTypes,
                o=>o.pid,
                p=>p.pid,
                (o,p)=>new
                {
                    Username = o.username,
                    Orderid = o.Oid,
                    Productid = p.pid,
                    Productname = p.ProductName,
                    Productprice = p.Price,
                    Quantity = o.quantity,
                    Totalprice = o.totalprice,
                    Orderdate = o.orderdate,
                    Image = p.ImagePath,
                    Status = o.status,
                    Addresses = o.addressId

                }).Join(Context.Addresses,
                o=>o.Addresses,
                a=>a.AddressId,
                (o,a)=>new
                {
                    Username = o.Username,
                    Orderid = o.Orderid,
                    Productid = o.Productid,
                    Productname = o.Productname,
                    Productprice = o.Productprice,
                    Quantity = o.Quantity,
                    Totalprice = o.Totalprice,
                    Orderdate = o.Orderdate,
                    Image = o.Image,
                    Status = o.Status,
                    Address=a.Address
                }).Where(x=>x.Username==username && x.Orderid==id).FirstOrDefault();


          
            ViewBag.order = order;

            var feedback = Context.Feedbacks
      .FirstOrDefault(x => x.Oid == id && x.username == username);

            ViewBag.Feedback = feedback;
            ViewBag.Rating = feedback?.Rating ?? 0;     // 0 = no feedback yet
            ViewBag.Comment = feedback?.feedback;
            ViewBag.Wishlist = Context.wishlists.Count(x => x.username == username);
            ViewBag.cart = Context.Carts.Count(x => x.username == username);



            return View("Orderdetails",order);
        }

        [HttpPost]
        public IActionResult SubmitFeedback(int Pid,int Orderid,int Rating,string Comment)  //Parameters passed from view of orderdetails
            {
            var username = HttpContext.Session.GetString("CustomerID");
            Feedback obj = new Feedback
            {
                username = username,
                Pid = Pid,
                Oid = Orderid,
                Rating = Rating,
                feedback = Comment
            };
            Context.Feedbacks.Add(obj);
            Context.SaveChanges();

            return RedirectToAction("OrderDetails",new {id=Orderid});
        }

}
    

}

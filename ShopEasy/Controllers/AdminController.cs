using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using ShopEasy.Models;

namespace ShopEasy.Controllers
{

    public class AdminController : Controller
    {
        private readonly ShopEasyContext Context;
        public AdminController(ShopEasyContext _shopEasyContext)
        {
            Context = _shopEasyContext;
        }
        public IActionResult Index()
        {
            var order = Context.Orders.Join(               // To show order request send from customer to seller
               Context.ProductTypes,
               o => o.pid,
               p => p.pid,
               (o, p) => new                               //combine two tables and take only needed item 
               {
                   Username = o.username,
                   Orderid = o.Oid,
                   Productname = p.ProductName,
                   Owner = p.Owner,
                   quantity = o.quantity,
                   price = o.totalprice,
                   orderdate = o.orderdate,
                   Image = p.ImagePath,
                   st = o.status
               }).Where(res => res.Owner == "CompanyOwner" && res.st == 0).ToList();

            List<Order> ord = new List<Order>();      //create object to call in viewbag


            foreach (var item in order)       //take from item
            {

                ord.Add(new Order
                {
                    ProductType = new ProductType { ProductName = item.Productname, Owner = item.Owner, ImagePath = item.Image },
                    username = item.Username,
                    quantity = item.quantity,
                    totalprice = item.price,
                    orderdate = item.orderdate,
                    Oid = item.Orderid

                });

            }

            ViewBag.ord = ord;
            var orderhistory = Context.Orders.Join(               // To show order request send from customer to seller
              Context.ProductTypes,
              o => o.pid,
              p => p.pid,
              (o, p) => new                               //combine two tables and take only needed item 
              {
                  Username = o.username,
                  Order = o.Oid,
                  Productname = p.ProductName,
                  Owner = p.Owner,
                  quantity = o.quantity,
                  price = o.totalprice,
                  orderdate = o.orderdate,
                  Image = p.ImagePath,
                  st = o.status
              }).Where(res => res.Owner == "CompanyOwner"&& res.st != 0).ToList();

            List<Order> ordHis = new List<Order>();      //create object to call in viewbag


            foreach (var item in orderhistory)       //take from item
            {

                ordHis.Add(new Order
                {
                    ProductType = new ProductType { ProductName = item.Productname, Owner = item.Owner, ImagePath = item.Image },
                    username = item.Username,
                    quantity = item.quantity,
                    totalprice = item.price,
                    orderdate = item.orderdate
                });

            }

            ViewBag.ordHis = ordHis;


            return View();
        }
        public IActionResult ApproveOrder(int id)  //Order Id is passed to approve 
        {
            var ord = Context.Orders.FirstOrDefault(p => p.Oid == id);
            if (ord != null)
            {
                ord.status = 1;
            }
            Context.Orders.Update(ord);
            Context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult ViewSeller() // To view the seller 
        {

            var data = Context.SellerRegs.Join(Context.LoginTbs, r => r.Email, l => l.Email,
               (r, l) => new { r.CompanyName, r.SId, r.SellerLiscenceNo, r.SellerName, l.Status })
                .Where(obj => obj.Status == 0).ToList();

            List<SellerReg> ob = new List<SellerReg>();
            foreach (var item in data)
            {
                ob.Add(new SellerReg
                {
                    CompanyName = item.CompanyName,
                    SId = item.SId,
                    SellerLiscenceNo = item.SellerLiscenceNo,
                    SellerName = item.SellerName
                });
            }





            return View("List", ob);
        }
        public IActionResult Accept(int id) //To accept the pending seller request by status
        {
            var data = Context.SellerRegs.FirstOrDefault(r => r.SId == id);
            var logindata = Context.LoginTbs.FirstOrDefault(r => r.Email == data.Email);
            logindata.Status = 1;
            Context.Update(logindata);
            Context.SaveChanges();
            return RedirectToAction("ViewSeller");
        }

        public IActionResult RegSeller()  //Registration for seller
        {
            return View(Context.SellerRegs.ToList());
        }

        public IActionResult ManageCategory()  // To manage category of products
        {
            ViewBag.category = Context.ProductCategories.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Save(ProductCategory category)  //
        {
            Context.ProductCategories.Add(category);
            Context.SaveChanges();

            return RedirectToAction("ManageCategory");
        }

        public IActionResult ViewCustomers()
        {
            var data = Context.CustomerRegs.ToList();
            return View("Customer", data);
        }


        public IActionResult Delete(int id) //To delete products from database
        {

            var data = Context.ProductCategories.FirstOrDefault(m => m.CategoryId == id);
            Context.ProductCategories.Remove(data); 
            Context.SaveChanges();
            return RedirectToAction("ManageCategory");
        }
        public IActionResult ManageProduct()   //Open Manageproduct page
        {
            var data = Context.ProductCategories.Select(x => new SelectListItem
            {
                Value = x.CategoryId.ToString(),
                Text = x.CategoryName

            })
                .ToList();    //Get the products from database


            ViewBag.data = data;  // Store it to the viewbag
            return View();       //show
        }
        [HttpPost]

        public IActionResult ManageProduct(ProductType P) // Process submitted form && P is the product obj coming from form
        {
            List<string> imagePaths = new List<string>(); // creates a empty list to store filenames 

            if (P.Image.Count > 0) // Check whether user selected atleast one image
            {


                foreach (var item in P.Image)  // Process each image
                {
                    var filename = "";  //for storing filename temporarily somewhere

                    if (item.Length > 0 && item != null)   // Make sure the uploaded file exists and has content
                    {

                        filename = item.FileName;
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Products", filename); //physicalpath

                        using (var stream = new FileStream(path, FileMode.Create)) //save the img
                        {
                            item.CopyTo(stream);
                        }
                        imagePaths.Add(filename); // store the filename in list
                    }

                }


            }
            string imagePathString = string.Join(",", imagePaths);
            P.ImagePath = imagePathString;
            P.Owner = "CompanyOwner";
            Context.ProductTypes.Add(P);
            Context.SaveChanges();
            return RedirectToAction("ManageProduct");


        }
        public IActionResult ViewProducts()   // View all products
        {
            var data = Context.ProductTypes.ToList(); // This data is for product viewing

            
            var data1 = Context.ProductCategories.Select(x => new SelectListItem  //Get categories for dropdown
            {
                Value = x.CategoryId.ToString(), // What get sents to the controller/Database
                Text = x.CategoryName // What user sees

            })
                 .ToList();
            


            ViewBag.data = data1;  // put categories into viewbag.data
            return View("displayimg", data);
        }

        public IActionResult Filter(int id)   //To show only the selected product category
        {
             
            var data1 = Context.ProductCategories.Select(x => new SelectListItem  // This data is for dropwdown/ Categories
            {
                Value = x.CategoryId.ToString(),
                Text = x.CategoryName

            })
                .ToList();


            ViewBag.data = data1;  // This is to keep the dropdown available even after filtering
            ViewBag.selectId = id;  

            if (id != 0)
            {
                var data = Context.ProductTypes.Where(r => r.CategoryID == id).ToList(); // Give me the matching category id selected by the user from products
                return View("displayimg", data);
              
            }
            else
            {
                var data = Context.ProductTypes.ToList();
                return View("displayimg", data);
            }
            

                
        }

        public IActionResult ManageStocks()   // ManageStocks details
        {
            {
                ViewBag.categories = Context.ProductCategories 
                    .Select(x => new SelectListItem
                    {
                        Value = x.CategoryId.ToString(),
                        Text = x.CategoryName
                    })
                    .ToList();

                var data3 = Context.Stocks.Include(r => r.ProductType); // To show the stock details in view page
              
                ViewData["stock"] = data3;
            
                return View("Stocks");
            }

        }
        [HttpGet]
        public JsonResult GetProductsByCategory(int categoryId)
        {
            var products = Context.ProductTypes .Where(p => p.CategoryID == categoryId)
                .Select(p => new
                {
                    id = p.pid,
                    name = p.ProductName
                })
                .ToList();

            return Json(products);
        }
        [HttpPost]

        public IActionResult Store(Stock stock)
        {
            var ExsitingStock = Context.Stocks.FirstOrDefault(x => x.Pid == stock.Pid); 
            if (ExsitingStock != null)
            {
                int total = ExsitingStock.Quantity + stock.Quantity;
                var dte = DateTime.Now;
                ExsitingStock.Quantity = total;
                ExsitingStock.Lastupdate = dte;

                Context.Update(ExsitingStock);
                Context.SaveChanges();
                return RedirectToAction("ManageStocks");
            }
            else   // If not insert new stock to the table
            {
                stock.Lastupdate = DateTime.Now;
                Context.Stocks.Add(stock);
                Context.SaveChanges();
                return RedirectToAction("ManageStocks");
            }
               
        }

        public IActionResult SalesReview()
        {
           
            var sales = Context.Orders
                .Join(Context.ProductTypes,
                    o => o.pid,
                    p => p.pid,
                    (o, p) => new
                    {
                        Pid = p.pid,
                        Productname = p.ProductName,
                        Price = p.Price,
                        Image = p.ImagePath,
                        Quantity = o.quantity,
                        Owner=p.Owner
                        
                    }).Join(Context.SellerRegs,
                    p=>p.Owner,
                    s=>s.Email,
                    (p,s)=> new
                    {
                        p.Pid,
                        p.Productname,
                        p.Price,
                        p.Image,
                        p.Quantity,
                        Owner = p.Owner,
                        SellerName = s.SellerName,
                        SellerEmail = s.Email,
                        SellerPhone = s.Contact
                    })
                
                .GroupBy(x => new
                {
                    x.Pid,
                    x.Productname,
                    x.Price,
                    x.Image,
                    x.SellerName,
                    x.SellerEmail,
                    x.SellerPhone,
                    x.Owner
                })
                .Select(g => new
                {
                    pid = g.Key.Pid,
                    productname = g.Key.Productname,
                    Price = g.Key.Price,
                    Image = g.Key.Image,
                    owner=g.Key.Owner,
                    sellername=g.Key.SellerName,
                    selleremail=g.Key.SellerEmail,
                    Sellerphone=g.Key.SellerPhone,
                    Quantity = g.Sum(x => x.Quantity)
                }).OrderByDescending(x=>x.Quantity)
                .ToList();

            return View(sales);
        }

        public IActionResult AdminSales()
        {
          
            var sales = Context.Orders
               .Join(Context.ProductTypes,
                   o => o.pid,
                   p => p.pid,
                   (o, p) => new
                   {
                       Pid = p.pid,
                       Productname = p.ProductName,
                       Price = p.Price,
                       Image = p.ImagePath,
                       Quantity = o.quantity,
                       Owner = p.Owner,
                   }).Where(x => x.Owner == "CompanyOwner")
               .GroupBy(x => new
               {
                   x.Pid,
                   x.Productname,
                   x.Price,
                   x.Image
               })
               .Select(g => new
               {
                   pid = g.Key.Pid,
                   productname = g.Key.Productname,
                   Price = g.Key.Price,
                   Image = g.Key.Image,
                   Quantity = g.Sum(x => x.Quantity)
               }).OrderByDescending(x => x.Quantity)
               .ToList();

            return View("AdminsalesReview",sales);
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ShopEasy.Models;
using System.Security.Cryptography;

namespace ShopEasy.Controllers
{
    public class SellerController : Controller
    {
        private readonly ShopEasyContext Context;
        public SellerController(ShopEasyContext _context)
        {
            Context = _context;
        }

        public IActionResult Index()
        {
            var seller = HttpContext.Session.GetString("SellerID");
            var pdtcount = Context.ProductTypes.Where(r => r.Owner == seller).Count();
            ViewBag.cnt = pdtcount;
            var totalstock = Context.ProductTypes.Include(r => r.Stock).Where(p => p.Owner == seller).Sum(u => u.Stock.Quantity);
            ViewBag.stock = totalstock;
            //to view the pending orders

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
                }).Where(res => res.Owner == seller && res.st == 0).ToList();

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

            //to view the order history

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
               }).Where(res => res.Owner == seller && res.st != 0).ToList();

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

        public IActionResult ManageProducts()
        {
            var seller = HttpContext.Session.GetString("SellerID");
            var products = Context.ProductTypes.Where(x => x.Owner == ("SellerID"));


            return View("List");
        }
        public IActionResult AddProduct()
        {
            var categories = Context.ProductCategories.Select(x => new SelectListItem
            {
                Value = x.CategoryId.ToString(),
                Text = x.CategoryName
            }).ToList();
            ViewBag.type = categories;
            return View();
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

            P.Owner = HttpContext.Session.GetString("SellerID");
            Context.ProductTypes.Add(P);
            Context.SaveChanges();
            return RedirectToAction("AddProduct");


        }
        public IActionResult Viewproducts()
        {
            {
                var sellerId = HttpContext.Session.GetString("SellerID"); // we used session to view products only registered by seller (their products only)
                var data = Context.ProductTypes.Where(x => x.Owner == sellerId).ToList();

                var data1 = Context.ProductCategories.Select(x => new SelectListItem
                {
                    Value = x.CategoryId.ToString(), // What get sents to the controller/Database
                    Text = x.CategoryName // What user sees

                })
                     .ToList();


                ViewBag.data = data1;  // put categories into viewbag.data
                return View("productlist", data);
            }


        }
        public IActionResult ManageStock()
        {

            {
                ViewBag.categories = Context.ProductCategories
                    .Select(x => new SelectListItem
                    {
                        Value = x.CategoryId.ToString(),
                        Text = x.CategoryName
                    })
                    .ToList();
                var sellerId = HttpContext.Session.GetString("SellerID");
                var data3 = Context.Stocks.Include(r => r.ProductType).Where(o => o.ProductType.Owner == sellerId); // To show the stock details in view page

                ViewData["SellerStocks"] = data3;

                return View("SellerStocks");
            }


        }
        [HttpGet]
        public JsonResult GetProductsByCategory(int categoryId)
        {
            var sellerId = HttpContext.Session.GetString("SellerID");
            var products = Context.ProductTypes.Where(p => p.CategoryID == categoryId && p.Owner == sellerId)
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
                return RedirectToAction("ManageStock");
            }
            else   // If not insert new stock to the table
            {
                stock.Lastupdate = DateTime.Now;
                Context.Stocks.Add(stock);
                Context.SaveChanges();
                return RedirectToAction("ManageStock");
            }

        }

        public IActionResult ApproveOrder(int id)
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

        public IActionResult Sales()
        {
            var sellerId = HttpContext.Session.GetString("SellerID");
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
                       Owner = p.Owner
                   }).Where(x => x.Owner == sellerId)
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
               }).OrderByDescending(x=>x.Quantity)
               .ToList();

            return View("Mysalesreview", sales);
        }

        public IActionResult Ratingreview()
        {
            var sellerId = HttpContext.Session.GetString("SellerID");
            var rating =Context.ProductTypes.Join(Context.Orders,
                p=>p.pid,
                or=>or.pid,
                (p,or)=>new
                {
                    Pid = p.pid,
                    Productname = p.ProductName,
                    Owner = p.Owner,
                    Oid = or.Oid,
                    Image=p.ImagePath,
                    Price=p.Price,
                    Quantity=or.quantity
                }
                ).Join(Context.Feedbacks,
                o => o.Oid,
                f => f.Oid,
                (o, f) => new
                {
                    Pid = o.Pid,
                    Productname = o.Productname,
                    Rating = f.Rating,
                    Feedback = f.feedback,
                   Owner = o.Owner,
                   Image=o.Image,
                   Price=o.Price,
                   Quantity=o.Quantity
                }).Where(x => x.Owner == sellerId)
                .GroupBy(x => new
                {
                    x.Pid,
                    x.Productname,
                    x.Image,
                    x.Price,
                    x.Quantity

                })
                .Select(g => new
                {
                    pid = g.Key.Pid,
                    productname = g.Key.Productname,
                    AverageRating = g.Average(x => x.Rating),
                    Feedbacks = g.Select(x => x.Feedback).ToList(),
                    Image=g.Key.Image,
                    Price=g.Key.Price,
                    Quantity=g.Key.Quantity
                })
                .ToList();

            return View("ProductRating", rating);
        }


        [HttpPost]
        public IActionResult Filterproduct(string id)
        {
            int catid = int.Parse(id);
            var sellerId = HttpContext.Session.GetString("SellerID");
             

            var data = Context.ProductTypes.Where(x=>x.CategoryID==catid && x.Owner == sellerId).ToList();

            var data1 = Context.ProductCategories.Select(x => new SelectListItem
            {
                Value = x.CategoryId.ToString(), 
                Text = x.CategoryName 

            })
                 .ToList();


            ViewBag.data = data1;
            return View("productlist", data);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopEasy.Models;

namespace ShopEasy.Controllers
{
    public class IndexController : Controller
        
    {
        private readonly ShopEasyContext shopEasyContext;
        public IndexController(ShopEasyContext _shopEasyContext)
        {
            shopEasyContext = _shopEasyContext;
        }

        public IActionResult Index() // To get the dropdown of categories (to get by id - check Filter method)
        {

            var pdt = shopEasyContext.ProductTypes.Include(e => e.CategoryModel).ToList();
            var categories = shopEasyContext.ProductCategories.Select(x => new SelectListItem
            {
                Value = x.CategoryId.ToString(),
                Text = x.CategoryName
            })
                 .ToList();

            ViewBag.type = categories;
         
            return View(pdt);
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult CustomerReg(CustomerComponent reg, string btn)
        {
            if (ModelState.IsValid)
            {
                if (btn == "REGISTER AS SELLER")
                {

                    LoginTb log = new LoginTb
                    {

                        Email = reg.SellerReg.Email,
                        Password = reg.LoginRef.Password,

                        TypeOfUser = false,
                        Status = 0
                    };

                    shopEasyContext.LoginTbs.Add(log);

                    shopEasyContext.SellerRegs.Add(reg.SellerReg);

                    shopEasyContext.SaveChanges();


                }
                else
                {

                    //login storage
                    LoginTb log = new LoginTb { Email = reg.CustomerReg.Email, Password = reg.LoginRef.Password, TypeOfUser = true, Status = 1 };
                    shopEasyContext.LoginTbs.Add(log);


                    shopEasyContext.CustomerRegs.Add(reg.CustomerReg);

                    shopEasyContext.SaveChanges();
                }

            }
          
            return RedirectToAction("Register");
        }

        [HttpPost]
        public IActionResult Login(LoginTb l)
        {
            if (l.Email == "admin@gmail.com" && l.Password == "admin")
            {
                return Redirect("~/Admin/Index");
            }
            else 
            {
                var seller = shopEasyContext.LoginTbs.FirstOrDefault(x => x.Email == l.Email && x.Password == l.Password && x.Status == 1);


                if (seller != null)
                {

                    if (seller.TypeOfUser==false)
                    {

                        var SellerReg = shopEasyContext.SellerRegs.FirstOrDefault(m => m.Email == seller.Email);
                        HttpContext.Session.SetString("SellerID", SellerReg.Email);
                        return Redirect("~/Seller/Index");
                    }
                    else
                    {

                        var CustomerReg = shopEasyContext.CustomerRegs.FirstOrDefault(m => m.Email == seller.Email);
                        HttpContext.Session.SetString("CustomerID", CustomerReg.Email);
                        return Redirect("~/Customer/Index");

                    }


                }
                else
                {
                    ViewBag.data = "User Invalid";
                    
                }
                return View();
            }
        }

        public IActionResult Filter(string  cat)  // retrive/filter products so we use get method (Connected to Index)
        {
            var data2 = shopEasyContext.ProductCategories.Select(x => new SelectListItem
            {
                Value = x.CategoryId.ToString(),
                Text = x.CategoryName
               

            })
                .ToList();
            ViewBag.type = data2; // while using viewbag the name should be same in both viewing and showing category by id or else it will throw null reference 


            int catid = int.Parse(cat);

            if (catid == 0)
            {
                var type1 = shopEasyContext.ProductTypes.Include(r => r.CategoryModel).ToList();
                return View("Index", type1);
            }

            else
            {
                var type = shopEasyContext.ProductTypes.Include(r => r.CategoryModel).Where(x => x.CategoryID == catid).ToList();

                return View("Index", type);

            }


           

        }
      
        
       
    }
}

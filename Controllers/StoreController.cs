using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using models.ViewModels;
using System.Net.Http.Headers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;
using Stripe.Checkout;



namespace BAG_Site.Controllers
{

    public class StoreController : Controller
    {

        private readonly MyContext dbContext;
        public interface webHostEnvironment : Microsoft.Extensions.Hosting.IHostEnvironment { };
        public StoreController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("Bag")]
        public IActionResult Bag()
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/Login");
            }
            ViewBag.LoggedUser = HttpContext.Session.GetInt32("LoggedUser");
            var bags = dbContext.Bag.Include(b => b.Art).ThenInclude(a => a.Art).ThenInclude(c => c.Creator).Include(x => x.Art).ThenInclude(y => y.Art).ThenInclude(l => l.LikedBy).FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("LoggedUser"));
            var total = 0;
            foreach (var item in bags.Art)
            {
                total += item.Art.price_data;
            }
            Console.WriteLine(total);
            ViewBag.total = total;
            return View(bags);
        }

        [HttpGet("Checkout")]
        public IActionResult Checkout()
        {
            return View();
        }

        [HttpGet("Cancel")]
        public IActionResult Cancel()
        {
            return View();
        }
        [HttpGet("OutBag/{id}")]
        public IActionResult OutBag(int id)
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/Login");
            }
            var BagtoRemove = dbContext.Bag.FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));

            var BagItemtoRemove = dbContext.Bagitems.FirstOrDefault(u => u.BagId == BagtoRemove.BagId && u.ArtId == id);
            dbContext.Remove(BagItemtoRemove);
            dbContext.SaveChanges();

            return RedirectToAction("Bag");
        }
        [HttpGet("IntoBag/{id}")]
        public IActionResult IntoBag(int id)
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/Login");
            }
            var ArtAdd = dbContext.Art.FirstOrDefault(u => u.ArtId == id);
            var BagtoAdd = dbContext.Bag.FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            if (BagtoAdd == null)
            {
                Bag newBag = new Bag
                {
                    UserId = (int)HttpContext.Session.GetInt32("LoggedUser")
                };
                dbContext.Add(newBag);
                dbContext.SaveChanges();
                Bagitem newItem = new Bagitem()
                {
                    BagId = newBag.BagId,
                    ArtId = id
                };
                dbContext.Add(newItem);
                dbContext.SaveChanges();
            }
            else
            {
                Bagitem newItem = new Bagitem()
                {
                    BagId = BagtoAdd.BagId,
                    ArtId = id
                };
                dbContext.Add(newItem);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Bag");
        }

        [HttpGet("Success")]
        public IActionResult Success()
        {
            return View();
        }
    }
    [Route("create-checkout-session")]
    [ApiController]
    public class CheckoutApiController : Controller
    {
        private readonly MyContext dbContext;
        public interface webHostEnvironment : Microsoft.Extensions.Hosting.IHostEnvironment { };
        public CheckoutApiController(MyContext context)
        {
            dbContext = context;
        }

        [HttpPost]
        public ActionResult Create()
        {
            var thisUser = dbContext.Users.Include(b => b.Bag).ThenInclude(i => i.Art).ThenInclude(a => a.Art).FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("LoggedUser"));
            var domain = "http://localhost:5000";
            SessionCreateOptions options = new SessionCreateOptions
            {

                PaymentMethodTypes = new List<string>
                {
                  "card",
                },
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                      UnitAmount = 2000,
                      Currency = "usd",
                      ProductData = new SessionLineItemPriceDataProductDataOptions
                      {
                        Name = "img.Title",
                      },
                    },
                    Quantity = 1,
                  },
                },
                Mode = "payment",
                SuccessUrl = domain + "/success",
                CancelUrl = domain + "/bag",
            };
            options.LineItems.Remove(options.LineItems.First());
            foreach (var item in thisUser.Bag.Art)
            {
                var price = item.Art.price_data;
                var name = "Digital Art:" + item.Art.Title;
                var newItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = price,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = name,
                        },
                    },
                    Quantity = 1,
                };
                options.LineItems.Add(newItem);
            }
            var service = new SessionService();
            Session session = service.Create(options);
            return Json(new { id = session.Id });
        }
    }
    namespace workspace.Controllers
    {
        [Route("api/[controller]")]
        public class StripeWebHook : Controller
        {
            const string secret = "sk_test_51IyNr3Fju3dt4jIrv4oUac1PT3V5PvtJrITS1E0zB0hyhErZL1QTFxOTsGM0F7mIbToEwalU9EwpuRt900b3a5UY001FT4T8bV";
            [HttpPost]
            public async Task<IActionResult> Index()
            {
                Console.WriteLine("This isn't working");
                var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

                Console.WriteLine(json);

                return Ok();
            }
        }
    }
}
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using models.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.AspNetCore.Hosting;


namespace BAG_Site.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyContext dbContext;
        public interface webHostEnvironment : Microsoft.Extensions.Hosting.IHostEnvironment { };
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        [HttpGet("Privacy")]
        public IActionResult Privacy()
        {
            ViewBag.LoggedUser = HttpContext.Session.GetInt32("LoggedUser");
            return View();
        }

        string[] Categories = new string[] { "Game Art", "Photography", "Stock Story Boards", "Story Boards", "Concept Art", "Graphic Design", "Drawings", "Animals" };
        
        //Index will be the Home(Landing) Page. 
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            int[] featureSelect = new int[10];
            int[] featured = new int[3];
            var first = 0;
            var second = 1;
            var third = 2;
            DateTime lastMonth = DateTime.Now;
            lastMonth = lastMonth.AddMonths(-1);
            var FeatureList = dbContext.Art.Include(u => u.Creator).Include(p => p.LikedBy).OrderByDescending(u => u.LikedBy.Count).Take(10).ToList();
            int index = 0;
            foreach (var possible in FeatureList)
            {
                foreach (var like in possible.LikedBy)
                {
                    if (like.LikedAt >= lastMonth)
                    {
                        featureSelect[index] = featureSelect[index] + 1;
                    }
                }
                index++;
            }
            for (var i = 0; i < featureSelect.Length; i++)
            {
                if (featureSelect[i] > featured[0])
                {
                    featured[2] = featured[1];
                    featured[1] = featured[0];
                    featured[0] = featureSelect[i];
                    third = second;
                    second = first;
                    first = i;
                }
                else if (featureSelect[i] > featured[1])
                {
                    featured[2] = featured[1];
                    featured[1] = featureSelect[i];
                    third = second;
                    second = i;
                }
                else if (featureSelect[i] > featured[2])
                {
                    featured[2] = featureSelect[i];
                    third = i;
                }
            }

            List<Art> FeaturedModel = new List<Art>();
            FeaturedModel.Add(FeatureList[first]);
            FeaturedModel.Add(FeatureList[second]);
            FeaturedModel.Add(FeatureList[third]);

            ViewBag.feature = FeaturedModel;
            var art = await dbContext.Art.Include(u => u.Creator).Include(p => p.LikedBy).OrderByDescending(d => d.UploadDate).ToListAsync();
            ViewBag.LoggedUser = HttpContext.Session.GetInt32("LoggedUser");
            ViewBag.Likes = dbContext.Likes.Include(u => u.User);
            ViewBag.Categories = Categories;
            if (ViewBag.LoggedUser != null)
            {
                ViewBag.User = dbContext.Users.FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            }
            return View(art);
        }

        [HttpGet("Artists")]
        public async Task<IActionResult> Artists()
        {
            var Users = dbContext.Users.Include(u => u.Art).ToList();
            var art = await dbContext.Art.Include(u => u.Creator).Include(p => p.LikedBy).OrderBy(c => c.Creator).ToListAsync();
            ViewBag.LoggedUser = HttpContext.Session.GetInt32("LoggedUser");
            ViewBag.Likes = dbContext.Likes.Include(u => u.User);
            if (ViewBag.LoggedUser != null)
            {
                ViewBag.User = dbContext.Users.FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            }
            return View(Users);
        }

        // Process the register form submission
        [HttpPost("/register")]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Checks to make sure the email is unique
                if (dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use");
                    return View("Login");
                }
                // Checks to make sure the display name is unique
                if (dbContext.Users.Any(u => u.DisplayName == user.DisplayName))
                {
                    ModelState.AddModelError("DisplayName", "That username is already in use");
                    return View("Login");
                }
                // Hashes the User's password to securely store their data
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                dbContext.Add(user);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("LoggedUser", user.UserId);
                SendVerification();
                return RedirectToAction("Index");
            }
            return View("Login");
        }
        // Proccesses the Login form
        [HttpPost("/login")]
        public IActionResult Login(LoginUser userSubmission)
        {
            if (ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                if (userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                if (result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }
                HttpContext.Session.SetInt32("LoggedUser", userInDb.UserId);
                return RedirectToAction("Index");
            }
            return View("Login");
        }

        // The Login Page
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet("Verification")]
        public IActionResult Verification()
        {
            // Will need to get user data and log the user in.
            // At that point, change the Userlvl from 0 to 1. Indicating they have verified their email
            return View();
        }

        [HttpPost("WriteComment")]
        public IActionResult WriteComment(int ArtId, string Message)
        {
            Comment newComment = new Comment();
            newComment.UserId = (int)HttpContext.Session.GetInt32("LoggedUser");
            newComment.ArtId = ArtId;
            newComment.Message = Message;
            int id = newComment.ArtId;
            dbContext.Add(newComment);
            dbContext.SaveChanges();
            return Json(newComment);
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        [HttpGet("deleteArt/{id}")]
        public IActionResult deleteArt(int id)
        {
            var thisArt = dbContext.Art.FirstOrDefault(u => u.ArtId == id);
            dbContext.Remove(thisArt);
            dbContext.SaveChanges();
            return Redirect("/Profile");
        }

        [HttpGet("Details")]
        public ActionResult Details(int id)
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                ViewBag.User = null;
            } else {
                ViewBag.User = dbContext.Users.FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            }
            ViewBag.LoggedUser = HttpContext.Session.GetInt32("LoggedUser");
            var singleArt = dbContext.Art.Include(a => a.Creator).Include(c => c.Comments).ThenInclude( z => z.User).Include(l => l.LikedBy).ThenInclude(s => s.User).Include(t => t.Tags).FirstOrDefault(u => u.ArtId == id);
            return PartialView("_Details", singleArt);
        }

        // The code necessary for emailing
        public void SendVerification()
        {
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress("quinn.s.lathe@gmail.com", "Request for Verification"));
            message.From = new MailAddress("quinnlathe@gmail.com");
            message.Subject = "Verification";
            message.Body = "<a href='http://localhost:5000'>click here to verify</a> fgdfgdfgdfgdfgdfgdfgfdg";
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
            System.Net.NetworkCredential basicCredential1 = new System.Net.NetworkCredential("quinnlathe@gmail.com", "passworw");
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential1;
            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [HttpGet("Genres/{genre}")]
        public IActionResult Genre(string genre)
        {
            ViewBag.LoggedUser = HttpContext.Session.GetInt32("LoggedUser");
            var GenreArt = dbContext.Art.Include(u => u.Creator).Include(l => l.LikedBy).Where(g => g.Genre == genre).ToList();
            return View(GenreArt);
        }
        // To add a genre. Add the genre added here. Add the name of the genre in the array at the top, as well as the array on the Profile Controller.
        [HttpGet("Genres")]
        public IActionResult Genres()
        {
            ViewBag.LoggedUser = HttpContext.Session.GetInt32("LoggedUser");
            for (var i = 0; i < Categories.Length; i++)
            {
                var genre = Categories[i];
                ViewBag.i = dbContext.Art.Include(u => u.Creator).Include(l => l.LikedBy).Where(g => g.Genre == genre).ToList();
            }
            List<List<Art>> Genres = new List<List<Art>>();
            var Photography = dbContext.Art.Include(u => u.Creator).Include(l => l.LikedBy).Where(g => g.Genre == "Photography").ToList();
            var Drawings = dbContext.Art.Include(u => u.Creator).Include(l => l.LikedBy).Where(g => g.Genre == "Drawings").ToList();
            var Graphic = dbContext.Art.Include(u => u.Creator).Include(l => l.LikedBy).Where(g => g.Genre == "Graphic Design").ToList();
            var Stock = dbContext.Art.Include(u => u.Creator).Include(l => l.LikedBy).Where(g => g.Genre == "Stock Story Boards").ToList();
            var Story = dbContext.Art.Include(u => u.Creator).Include(l => l.LikedBy).Where(g => g.Genre == "Story Boards").ToList();
            var Game = dbContext.Art.Include(u => u.Creator).Include(l => l.LikedBy).Where(g => g.Genre == "Game Art").ToList();
            var Concept = dbContext.Art.Include(u => u.Creator).Include(l => l.LikedBy).Where(g => g.Genre == "Concept Art").ToList();
            var Animals = dbContext.Art.Include(u => u.Creator).Include(l => l.LikedBy).Where(g => g.Genre == "Animals").ToList();
            Genres.Add(Photography);
            Genres.Add(Drawings);
            Genres.Add(Graphic);
            Genres.Add(Stock);
            Genres.Add(Game);
            Genres.Add(Story);
            Genres.Add(Concept);
            Genres.Add(Animals);
            ViewBag.Categories = Categories;
            if (ViewBag.LoggedUser != null)
            {
                ViewBag.User = dbContext.Users.FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            }
            return View(Genres);
        }
        [HttpPost("Search")]
        public IActionResult Search(string search){
            var searchItems = search.Split(" ");
            List<Tag> searchRes = new List<Tag>();
            for(var i = 0; i<searchItems.Length; i++){
                var temp = dbContext.Tags.Include(a => a.Art).Where( u => u.Desc.ToLower().Contains(searchItems[i].ToLower())).ToList();
                searchRes.AddRange(temp);
            }
            searchRes.OrderBy(a => a.ArtId);
            List<int> index = new List<int>();
            foreach(var tag in searchRes){
                if(index.Any( u => u == tag.ArtId)){
                    continue;
                } else {
                    index.Add(tag.ArtId);
                }
            }
            var Results = new List<Art>();
            for(var x = 0; x<index.Count; x++){
                Console.WriteLine(index[x]);
                var add = dbContext.Art.Include(u => u.Creator).Include(p => p.LikedBy).FirstOrDefault( a => a.ArtId == index[x]);
                Results.Add(add);
            }
            return View(Results);
        }
        [HttpGet("Feed")]
        public IActionResult Feed(){
            if(HttpContext.Session.GetInt32("LoggedUser") == null){
                return RedirectToAction("Index");
            }
            ViewBag.LoggedUser = HttpContext.Session.GetInt32("LoggedUser");
            var Fave = dbContext.FArtists.Where( c => c.UserId ==(int)HttpContext.Session.GetInt32("LoggedUser")).ToList();
            var MyFeed = dbContext.Art.Include(t => t.Tags).Include(l => l.LikedBy).ThenInclude(s => s.User).Include(c => c.Comments).ThenInclude( z => z.User).Include(p => p.LikedBy).Include(u => u.Creator).ThenInclude(f => f.FavArtist).Where( a => a.Creator.FavArtist.Any( i => i.UserId == HttpContext.Session.GetInt32("LoggedUser"))).OrderByDescending(d => d.UploadDate).ToList();
            return View(MyFeed);
        }
        
        // [HttpGet("SendCommission")]
        // public IActionResult SendCommission(){
        //     return View();
        // }
    }
}

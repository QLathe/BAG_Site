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


namespace BAG_Site.Controllers
{
    public class ProfileController : Controller
    {
        private readonly MyContext dbContext;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProfileController(IWebHostEnvironment hostEnvironment, MyContext context)
        {
            webHostEnvironment = hostEnvironment;
            dbContext = context;
        }

        string[] Categories = new string[]{"Game Art", "Photography", "Stock Story Boards", "Stock Images", "Story Boards", "Concept Art", "Graphic Design", "Drawings", "Animals"};

        [HttpGet("Profile")]
        public IActionResult Profile()
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            ViewBag.LoggedUser = HttpContext.Session.GetInt32("LoggedUser");
            ViewBag.User = dbContext.Users.FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            ViewBag.fav = dbContext.FArtists.Include(f => f.Target).Where(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            ViewBag.fol = dbContext.Followers.Include(f => f.Follower).Where(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            var UserArt = dbContext.Art.Include(l => l.LikedBy).ThenInclude( s => s.User).Where(u => u.CreatorId == (int)HttpContext.Session.GetInt32("LoggedUser")).OrderByDescending( d => d.UploadDate);
            ViewBag.Liked = dbContext.Likes.Where( u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            return View(UserArt);
        }
        [HttpGet("OProfile/{id}")]
        public IActionResult OProfile(int id)
        {
            var check = new int();
            if (HttpContext.Session.GetInt32("LoggedUser") != null)
            {
                ViewBag.me = dbContext.FArtists.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("LoggedUser") && u.TargetId == id);
                check = (int)HttpContext.Session.GetInt32("LoggedUser");
            }
            ViewBag.LoggedUser = HttpContext.Session.GetInt32("LoggedUser");
            ViewBag.User = dbContext.Users.FirstOrDefault(u => u.UserId == id);
            if(ViewBag.User.UserId == check){
                return RedirectToAction("Profile");
            }
            ViewBag.fav = dbContext.FArtists.Include(f => f.Target).Where(u => u.UserId == id);
            var UserArt = dbContext.Art.Include(l => l.LikedBy).ThenInclude( s => s.User).Where(u => u.CreatorId == id);
            return View(UserArt);
        }
        [HttpGet("CommissionView")]
        public IActionResult CommissionView(){
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            return View();
        }

        [HttpGet("EditProfile")]
        public IActionResult EditProfile()
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            ViewBag.User = dbContext.Users.FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            ViewBag.UserArt = dbContext.Art.Where(u => u.CreatorId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            ViewBag.LoggedUser = HttpContext.Session.Get("LoggedUser");
            return View();
        }

        [HttpPost("MentStat")]
        public IActionResult MenStat(int stat)
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            User thisUser = dbContext.Users.FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            thisUser.Mentor = stat;
            dbContext.SaveChanges();
            return RedirectToAction("EditProfile");
        }
        // Change Commission status to available or not
        [HttpGet("ComStat")]
        public IActionResult ComStat()
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            User thisUser = dbContext.Users.FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            if (thisUser.OpenForCommission == 0)
            {
                thisUser.OpenForCommission = 1;
            }
            else if (thisUser.OpenForCommission == 1)
            {
                thisUser.OpenForCommission = 0;
            }
            dbContext.SaveChanges();
            return RedirectToAction("Profile");
        }
        // Processes request to follow another user
        [HttpGet("FavoriteArtist/{Target}")]
        public IActionResult FavoriteArtist(int Target)
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/Login");
            }
            FArtist checkFollow = dbContext.FArtists.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("LoggedUser") && u.TargetId == Target);
            if( checkFollow == null){
                FArtist newFArtist = new FArtist
                {
                    UserId = (int)HttpContext.Session.GetInt32("LoggedUser"),
                    TargetId = Target
                };
                Followers newFollower = new Followers
                {
                    UserId = Target,
                    FollowerId = (int)HttpContext.Session.GetInt32("LoggedUser")
                };
                dbContext.Add(newFollower);
                dbContext.Add(newFArtist);
                dbContext.SaveChanges();
                return Redirect("/OProfile/"+Target);
            } else {
                Followers unFollow = dbContext.Followers.FirstOrDefault(u => u.UserId == Target && u.FollowerId == HttpContext.Session.GetInt32("LoggedUser"));
                dbContext.Remove(unFollow);
                dbContext.Remove(checkFollow);
                dbContext.SaveChanges();
                return Redirect("/OProfile/"+Target);
            }
        }

        [HttpGet("Liked")]
        public IActionResult Liked()
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            ViewBag.LoggedUser = (int)HttpContext.Session.GetInt32("LoggedUser");
            ViewBag.User = dbContext.Users.FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            var art = dbContext.Likes.Include(a => a.Art).ThenInclude(l => l.LikedBy).Include(r => r.Art).ThenInclude(c => c.Creator).Where(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser")).ToList();
            return View(art);
        }

        // Like someone's art and add it to liked art
        [HttpGet("LikeArt")]
        public IActionResult LikeArt(int id)
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/Login");
            }
            Liked newLike = new Liked()
            {
                UserId = (int)HttpContext.Session.GetInt32("LoggedUser"),
                ArtId = id
            };
            var toDelete = dbContext.Likes.FirstOrDefault(u => u.ArtId == id && u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            if(toDelete != null){
                dbContext.Remove(toDelete);
                dbContext.SaveChanges();
                return RedirectToAction("Profile");
            } else {
                dbContext.Add(newLike);
                dbContext.SaveChanges();
            }
                return RedirectToAction("Profile");
        }

        [HttpPost("WriteBio")]
        public IActionResult WriteBio(string Bio)
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            User UpdateUser = dbContext.Users.FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            UpdateUser.Bio = Bio;
            UpdateUser.UpdatedAt = DateTime.Now;
            dbContext.SaveChanges();
            return RedirectToAction("Profile");
        }

        // Add new skills to Artist's profile page
        [HttpPost("AddSkill")]
        public IActionResult AddSkill(Skill skill)
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            User thisUser = dbContext.Users.FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            Skill newSkill = new Skill
            {
                UserId = thisUser.UserId,
                Desc = skill.Desc,
            };
            thisUser.UpdatedAt = DateTime.Now;
            dbContext.Add(newSkill);
            dbContext.SaveChanges();
            return RedirectToAction("EditProfile");
        }

        [HttpGet("SellingHistory")]
        public IActionResult SellingHistory()
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            ViewBag.SoldArt = dbContext.Transactions;
            ViewBag.LoggedUser = HttpContext.Session.Get("LoggedUser");
            return View();
        }

        [HttpGet("Upload")]
        public IActionResult Upload()
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            ViewBag.Categories = Categories;
            ViewBag.UserId = HttpContext.Session.Get("LoggedUser");
            ViewBag.LoggedUser = HttpContext.Session.Get("LoggedUser");
            return View(new ArtViewModel());
        }
        [HttpGet("UploadPro")]
        public IActionResult UploadPro()
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            ViewBag.UserId = HttpContext.Session.Get("LoggedUser");
            ViewBag.LoggedUser = HttpContext.Session.Get("LoggedUser");
            return View(new ProfileViewModel());
        }

        [HttpPost("AddPic")]
        public async Task<IActionResult> AddPic(ProfileViewModel addPic)
        {
            if (ModelState.IsValid)
            {
                User thisUser = dbContext.Users.FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
                string uniqueFileName = UploadedPic(addPic);
                if(thisUser.ProfilePic != null){
                    thisUser.ProfilePic = null;
                }
                thisUser.ProfilePic = uniqueFileName;
                thisUser.UpdatedAt = DateTime.Now;
                await dbContext.SaveChangesAsync();
                dbContext.SaveChanges();
            }
            return RedirectToAction("Profile");
        }

        [HttpPost("AddArt")]
        public IActionResult AddArt(ArtViewModel addArt, string tags)
        {
            if (ModelState.IsValid)
            {
                User thisUser = dbContext.Users.FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
                string uniqueFileName = UploadedFile(addArt);
                Art AddArt = new Art
                {
                    CreatorId = thisUser.UserId,
                    Title = addArt.Title,
                    ImgFile = uniqueFileName,
                    Genre = addArt.Genre,
                    price_data = addArt.Price
                };
                dbContext.Add(AddArt);
                dbContext.SaveChanges();
                var addTags = tags.Split(',');
                foreach(var str in addTags ){
                    if(str != ""){
                    Tag newTag = new Tag {
                        Desc = str,
                        ArtId = AddArt.ArtId
                    };
                    dbContext.Add(newTag);
                    }
                }
                thisUser.UpdatedAt = DateTime.Now;
                dbContext.SaveChanges();
            }
            return RedirectToAction("Upload");
        }
        private string UploadedPic(ProfileViewModel art )
        {
            string uniqueFileName = null;  
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");  
                uniqueFileName = Guid.NewGuid().ToString() + "_" + art.Image.FileName;  
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);  
                using (var fileStream = new FileStream(filePath, FileMode.Create))  
                {  
                    art.Image.CopyTo(fileStream);  
                }  
            return uniqueFileName;  
        }
        private string UploadedFile(ArtViewModel art )
        {
            string uniqueFileName = null;  
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");  
                uniqueFileName = Guid.NewGuid().ToString() + "_" + art.Image.FileName;  
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);  
                using (var fileStream = new FileStream(filePath, FileMode.Create))  
                {  
                    art.Image.CopyTo(fileStream);  
                }  
            return uniqueFileName;  
        }

        [HttpGet("JobOffer/{id}")]
        public IActionResult JobOffer(int id){
            if(HttpContext.Session.GetInt32("LoggedUser") == null){
                return Redirect("/");
            }
            ViewBag.Artist = dbContext.Users.FirstOrDefault(u => u.UserId == id);
            return View();
        }

        [HttpGet("JobView")]
        public IActionResult JobView(){
            if(HttpContext.Session.GetInt32("LoggedUser") == null){
                return Redirect("/");
            }
            ViewBag.LoggedUser = HttpContext.Session.GetInt32("LoggedUser");
            var myJobs = dbContext.Jobs.Where(u => u.ArtistId == (int)HttpContext.Session.GetInt32("LoggedUser")).ToList();
            return View(myJobs);
        }
        [HttpGet("pastJobs")]
        public IActionResult pastJobs(){
            if(HttpContext.Session.GetInt32("LoggedUser") == null){
                return Redirect("/");
            }
            ViewBag.LoggedUser = HttpContext.Session.GetInt32("LoggedUser");
            var myJobs = dbContext.Jobs.Where(u => u.ArtistId == (int)HttpContext.Session.GetInt32("LoggedUser")).ToList();
            return View(myJobs);
        }

        [HttpPost("Offer")]
        public IActionResult Offer(Job JobModel){
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            ViewBag.LoggedUser = HttpContext.Session.GetInt32("LoggedUser");
            if(ModelState.IsValid){
                JobModel.UserId = (int)HttpContext.Session.GetInt32("LoggedUser");
                dbContext.Add(JobModel);
                dbContext.SaveChanges();
                return Redirect("OProfile/" + JobModel.ArtistId);
            }
            ViewBag.Artist = dbContext.Users.FirstOrDefault(u => u.UserId == JobModel.ArtistId);
            return Redirect("JobOffer/" + JobModel.ArtistId);
        }
        [HttpGet("ViewJob/{id}")]
        public IActionResult ViewJob(int id){
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            ViewBag.LoggedUser = HttpContext.Session.GetInt32("LoggedUser");
            var thisJob = dbContext.Jobs.FirstOrDefault(u => u.JobId == id);
            return View(thisJob);
        }
        [HttpGet("JobReject/{id}")]
        public IActionResult JobReject(int id){
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            var reject = dbContext.Jobs.FirstOrDefault(u => u.JobId == id);
            if(reject.Status == 0){
                reject.Status = 3;
                dbContext.SaveChanges();
            }
            return RedirectToAction("JobView");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BAG_Site.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BAG_Site.Controllers
{
    public class BoardsController : Controller
    {
        private readonly MyContext dbContext;
        public BoardsController(MyContext context)
        {
            dbContext = context;
        }
        [HttpGet("MyBoards")]
        public IActionResult MyBoards()
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            ViewBag.LoggedUser = HttpContext.Session.Get("LoggedUser");
            var boards = dbContext.Boards.Include(b => b.User).Include(u => u.Pinned).ThenInclude( u => u.Art).Where(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser")).ToList();
            return View(boards);
        }
        [HttpGet("NewBoard")]
        public IActionResult NewBoard()
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            ViewBag.LoggedUser = HttpContext.Session.Get("LoggedUser");
            return View();
        }
        [HttpGet("ViewBoard/{boardId}")]
        public IActionResult ViewBoard(int boardId)
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            var Board = dbContext.Boards.Include(p => p.Pinned).ThenInclude( u => u.Art).ThenInclude(c=>c.Creator).Include(d => d.Pinned).ThenInclude(x => x.Art).ThenInclude(l => l.LikedBy).FirstOrDefault( b => b.BoardId == boardId);
            ViewBag.LoggedUser = HttpContext.Session.Get("LoggedUser");
            return View(Board);
        }

        [HttpPost("createBoard")]
        public IActionResult createBoard(Board board)
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            Board newBoard = new Board()
            {
                Title = board.Title,
                UserId = (int)HttpContext.Session.GetInt32("LoggedUser"),
                Description = board.Description,
                Viewable = board.Viewable
            };
            dbContext.Add(newBoard);
            dbContext.SaveChanges();
            return RedirectToAction("MyBoards");
        }

        [HttpPost("BoardDesc")]
        public IActionResult BoardDesc(int id, string desc)
        {
            if (HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            Board UpdateBoard = dbContext.Boards.FirstOrDefault(u => u.BoardId == id);
            UpdateBoard.Description = desc;
            UpdateBoard.UpdatedAt = DateTime.Now;
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost("PintoBoard")]
        public IActionResult PintoBoard(int ArtId, int BoardId)
        {
            Pin newPin = new Pin
            {
                ArtId = ArtId,
                BoardId = BoardId
            };
            Board UpdateBoard = dbContext.Boards.FirstOrDefault(u => u.BoardId == BoardId);
            UpdateBoard.UpdatedAt = DateTime.Now;
            dbContext.Add(newPin);
            dbContext.SaveChanges();
            return Redirect("/");
        }

        [HttpPost("PinNote")]
        public IActionResult PinNote(int id, string note)
        {
            Pin UpdatePin = dbContext.Pins.FirstOrDefault(u => u.PinId == id);
            UpdatePin.Note = note;
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet("Unpin/{id}/{board}")]
        public IActionResult Unpin(int id, int board)
        {
            var thisBoard = dbContext.Boards.FirstOrDefault( u => u.BoardId == board);
            if(HttpContext.Session.GetInt32("LoggedUser") == thisBoard.UserId){
                Pin toDelete = dbContext.Pins.FirstOrDefault(u => u.PinId == id);
                dbContext.Remove(toDelete);
                dbContext.SaveChanges();
                return Redirect("/ViewBoard/"+board+"");
            } else {
                return Redirect("/");
            }

        }
    }
}

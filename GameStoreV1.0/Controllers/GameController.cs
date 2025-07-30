using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GameStoreV1._0.DB;
using GameStoreV1._0.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameStoreV1._0.Controllers
{
    public class GameController : Controller
    {
        private readonly ApplicationDBContext _db;
        public GameController(ApplicationDBContext _db)
        {
            this._db = _db;
        }

        public IActionResult ViewGame(string id)
        {
            Game ga = this._db.game.Find(id);
            if (ga == null)
            {
                return RedirectToAction("Index", "Home");
            }
            String sqlQuery = "select cc.CID, c.categoryName from CategoryConnect cc join Category c on c.CID = cc.CID where cc.GID = '" + id + "'";
            List<Category> categoryList = this._db.Set<Category>().FromSqlRaw(sqlQuery).ToList();
            ViewGameDetail view = new ViewGameDetail
            {
                viewedGame = ga,
                viewCategory = categoryList
            };
            var sessionUser = HttpContext.Request.Cookies["UserCookie"];
            //Check if feedback is added
            var check = this._db.feedback.Any(m => m.username == sessionUser && m.GID == id);
            if (check == true)
            {
                ViewBag.fbAdded = "true";
            }
            var query = from o in this._db.order
                        join od in this._db.orderDetail on o.OID equals od.OID
                        where o.username == sessionUser && od.GID == id
                        select new
                        {
                            o.username,
                            od.GID
                        };
            var check2 = query.Any();
            if (check2 == true)
            {
                ViewBag.gameAdded = "true";
            }
            ViewBag.UserID = sessionUser;
            ViewBag.Rate = Math.Round(Rate(id), 1);
            ViewBag.ListFeedback = feedbackViewModel(id);
            return View(view);
        }

        public IActionResult SearchGame()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SearchGame(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return View();
            }

            var filterGame = (from g in this._db.game
                              where g.gameName.Contains(searchString) && g.status == "1"
                              select new Game
                              {
                                  GID = g.GID,
                                  gameName = g.gameName,
                                  price = g.price,
                                  picture = g.picture,
                                  date = g.date,
                                  description = g.description,
                                  configuration = g.configuration,
                                  sellerName = g.sellerName,
                                  status = g.status
                              }).ToList();
            return View(filterGame);
        }

        public IActionResult CategoryList()
        {
            // Lấy danh sách các danh mục từ cơ sở dữ liệu.
            var catelist = this._db.Category.ToList();
            var glist = this._db.game.Where(m => m.status == "1").ToList();

            List<Game> gamelist = glist;
            List<Category> clist = catelist;

            viewCategorylist viewct = new viewCategorylist
            {
                GamesList = gamelist,
                Categories = clist

            };
            return View(viewct);
        }

        public IActionResult searchCategory(string id)
        {
            // Thực hiện truy vấn để lấy danh sách các trò chơi thuộc danh mục có ID là id.
            var query = (from cc in this._db.CategoryConnect
                         join g in this._db.game
                         on cc.GID equals g.GID
                         where cc.CID == id && g.status == "1"
                         select new Game
                         {
                             GID = g.GID,
                             gameName = g.gameName,
                             price = g.price,
                             picture = g.picture,
                             date = g.date,
                             description = g.description,
                             configuration = g.configuration,
                             sellerName = g.sellerName,
                             status = g.status

                         }).ToList();
            // Chuyển danh sách trò chơi tìm thấy thành một danh sách.
            List<Game> s = query;

            var name = this._db.Category.Find(id);
            ViewBag.x= name.categoryName;

            // Trả về view hiển thị danh sách các trò chơi thuộc danh mục cụ thể.
            return View(s);
        }
        [HttpPost]
        public IActionResult AddFeedback(ViewGameDetail obj)
        {
            // Lấy username từ Cookie
            var user = HttpContext.Request.Cookies["UserCookie"];

            if (string.IsNullOrEmpty(user))
            {
                TempData["RedirectGameId"] = obj.currentGame;
                return RedirectToAction("Login", "Profile");
            }

            // Kiểm tra dữ liệu hợp lệ
            if (ModelState.IsValid)
            {
                // Kiểm tra xem Feedback đã tồn tại hay chưa
                var existingFeedback = this._db.feedback.FirstOrDefault(f => f.username == user && f.GID == obj.currentGame);

                if (existingFeedback == null) // Chỉ thêm nếu chưa tồn tại
                {
                    // Tạo mới feedback
                    var newFeedback = new Feedback
                    {
                        username = user,
                        GID = obj.currentGame,
                        feedback = obj.feedback,
                        rate = obj.rate,
                        status = "1",
                        date = DateTime.Now
                    };

                    // Thêm và lưu vào database
                    this._db.feedback.Add(newFeedback);
                    this._db.SaveChanges();
                }
                else
                {
                    // Nếu đã tồn tại, có thể cập nhật hoặc thông báo lỗi
                    existingFeedback.feedback = obj.feedback;
                    existingFeedback.rate = obj.rate;
                    existingFeedback.date = DateTime.Now;

                    this._db.SaveChanges();
                }
            }

            // Quay lại trang game
            return RedirectToAction("ViewGame", "Game", new { id = obj.currentGame });
        }

        
        


        public List<Feedback> ListFeedback(string id)
        {
            //Get all feedback
            return _db.feedback.Where(x => x.GID == id).OrderBy(x => x.date).ToList();
        }

        public List<FeedbackViewModel> feedbackViewModel(string id)
        {
            var model = (from a in _db.feedback
                         join b in _db.game on a.GID equals b.GID
                         join c in _db.profile on a.username equals c.username
                         where b.GID == id
                         select new
                         {
                             GID = a.GID,
                             username = a.username,
                             feedback = a.feedback,
                             fullname = c.fullname,
                             rate = a.rate,
                             status = a.status,
                             date = a.date
                         }).AsEnumerable().Select(x => new FeedbackViewModel()
                         {
                             GID = x.GID,
                             username = x.username,
                             feedback = x.feedback,
                             fullname = x.fullname,
                             rate = x.rate,
                             status = x.status,
                             date = x.date
                         });
            return model.ToList();
        }

        /*
          Calculate the number of rates
        */
        public double Rate(string id)
        {
            var query = _db.feedback.Where(x => x.GID == id);
            int count = query.Count();
            int sum = query.Sum(x => x.rate);
            ViewBag.count = count;
            double erate = (double)sum / count;
            return erate;
        }

        public IActionResult sellerGame(string id)
        {
            var segame = (from g in this._db.game
                          where g.sellerName == id && g.status == "1"
                          select new Game
                          {
                              GID = g.GID,
                              gameName = g.gameName,
                              price = g.price,
                              picture = g.picture,
                              date = g.date,
                              description = g.description,
                              configuration = g.configuration,
                              sellerName = g.sellerName,
                              status = g.status
                          }).ToList();
            HttpContext.Session.SetString("SellerName", segame[0].sellerName);
            return View(segame);
        }
    }
}
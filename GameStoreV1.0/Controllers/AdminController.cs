using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
    public class AdminController : Controller
    {
        private readonly ApplicationDBContext _db;
        public AdminController(ApplicationDBContext _db)
        {
            this._db = _db;
        }
        public IActionResult Management()
        {
            string adminck = HttpContext.Request.Cookies["AdminCookie"];
            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var userlist = (from pf in this._db.profile
                            where pf.type == "1"
                            select new Profile
                            {
                                username = pf.username,
                                email = pf.email,
                                fullname = pf.fullname,
                                gender = pf.gender,
                                birthday = pf.birthday,
                                money = pf.money,
                                type = pf.type,
                                status = pf.status,
                                date = pf.date
                            }).ToList();

            var sellerlist = (from pf in this._db.profile

                              where pf.type == "2"
                              // Chỉ chọn những người dùng có loại tài khoản là "2" (đại diện cho người bán).
                              && pf.status != "3"
                              
                              && pf.status != "4"
                              

                              select new Profile
                              {
                               
                                  username = pf.username,
                                  email = pf.email,
                                  fullname = pf.fullname,
                                  gender = pf.gender,
                                  birthday = pf.birthday,
                                  money = pf.money,
                                  status = pf.status,
                                  date = pf.date
                              }).ToList();

            var gamelist = (from g in this._db.game
                            join p in this._db.profile on g.sellerName equals p.username
                            // Chọn từng dòng dữ liệu trong bảng game và chuyển chúng thành các đối tượng Game mới.
                            where g.status == "0" && p.status == "1"
                           || g.status == "1"
                           || g.status == "2"
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
                                status = g.status,
                                downloadFile = g.downloadFile
                            }).ToList();

            var applicationList = (from pf in
            this._db.profile
                                   where pf.type == "2"
                                   &&
                                    pf.status == "3"
                                   select
                                   new Profile
                                   {
                                       username = pf.username,
                                       email = pf.email,
                                       fullname = pf.fullname,
                                       gender = pf.gender,
                                       birthday = pf.birthday,
                                       money = pf.money,
                                       type = pf.type,
                                       status = pf.status,
                                       date = pf.date,

                                   }).ToList();

            var applicationList1 = (from pf in
            this._db.profile
                                    where pf.type == "2"
                                    &&
                                     pf.status == "4"
                                    select
                                    new Profile
                                    {
                                        username = pf.username,
                                        email = pf.email,
                                        fullname = pf.fullname,
                                        gender = pf.gender,
                                        birthday = pf.birthday,
                                        money = pf.money,
                                        type = pf.type,
                                        status = pf.status,
                                        date = pf.date,

                                    }).ToList();
            var gamelist1 = (from g in this._db.game
                             join p in this._db.profile on g.sellerName equals p.username
                             where g.status == "3" && p.status == "1"
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
                                 status = g.status,
                                 downloadFile = g.downloadFile
                             }).ToList();




            var gamelist2 = (from g in this._db.game
                             join p in this._db.profile on g.sellerName equals p.username
                             where g.status == "4" && p.status == "1"
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
                                 status = g.status,
                                 downloadFile = g.downloadFile
                             }).ToList();
            
            List<Game> GList = gamelist;          
            List<Profile> UList = userlist;           
            List<Profile> SList = sellerlist;      
            List<Profile> AList = applicationList;          
            List<Profile> AList1 = applicationList1;
            List<Game> GList1 = gamelist1;
            List<Game> GList2 = gamelist2;
            Managerview screenManager = new Managerview
            {
                ProfileList = UList,
                GamesList = GList,
                SellerList = SList,
                ApplicationList = AList,
                ApplicationList1 = AList1,
                GamesList1 = GList1,
                GamesList2 = GList2,
            };
            return View(screenManager);
        }

        public IActionResult Detail(string id)
        {
            string adminck = HttpContext.Request.Cookies["AdminCookie"];
            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            HttpContext.Session.SetString("check", "4");
            try
            {
                var information = (from pf in
            this._db.profile
                                   where pf.username == id
                                   select
                                   new Profile
                                   {
                                       username = pf.username,
                                       email = pf.email,
                                       fullname = pf.fullname,
                                       gender = pf.gender,
                                       birthday = pf.birthday,
                                       money = pf.money,
                                       type = pf.type,
                                       status = pf.status,
                                       date = pf.date,

                                   }).ToList();
                return View(information[0]);
            }
            catch (System.Exception)
            {
                return NotFound();
            }
        }
        public IActionResult Detail1(string id)
        {
            string adminck = HttpContext.Request.Cookies["AdminCookie"];

            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            HttpContext.Session.SetString("check", "5");
            try
            {
                var information = (from pf in
            this._db.profile
                                   where pf.username == id
                                   select
                                   new Profile
                                   {
                                       username = pf.username,
                                       email = pf.email,
                                       fullname = pf.fullname,
                                       gender = pf.gender,
                                       birthday = pf.birthday,
                                       money = pf.money,
                                       type = pf.type,
                                       status = pf.status,
                                       date = pf.date,

                                   }).ToList();
                return View(information[0]);
            }
            catch (System.Exception)
            {
                return NotFound();
            }
        }
        [HttpPost]
        public IActionResult Detail(string btnAccept, string btnDecline, string status, string username)
        {
            //truy vấn để tìm người dùng
            /*var profileToUpdate = this._db.profile.FirstOrDefault
            (p
             =>
             p.username
             ==
              username
              );
            if (profileToUpdate != null)
            {
                if (!string.IsNullOrEmpty(btnAccept))
                {
                    profileToUpdate.username = profileToUpdate.username;
                    profileToUpdate.password = profileToUpdate.password;
                    profileToUpdate.email = profileToUpdate.email;
                    profileToUpdate.fullname = profileToUpdate.fullname;
                    profileToUpdate.gender = profileToUpdate.gender;
                    profileToUpdate.birthday = profileToUpdate.birthday;
                    profileToUpdate.money = profileToUpdate.money;
                    profileToUpdate.type = profileToUpdate.type;
                    profileToUpdate.date = profileToUpdate.date;
                    profileToUpdate.status = "1";
                    this._db.profile.Update(profileToUpdate);
                    this._db.SaveChanges();
                    return RedirectToAction("Management", "Admin");
                }
                else if (!string.IsNullOrEmpty(btnDecline))
                {
                    profileToUpdate.username = profileToUpdate.username;
                    profileToUpdate.password = profileToUpdate.password;
                    profileToUpdate.email = profileToUpdate.email;
                    profileToUpdate.fullname = profileToUpdate.fullname;
                    profileToUpdate.gender = profileToUpdate.gender;
                    profileToUpdate.birthday = profileToUpdate.birthday;
                    profileToUpdate.money = profileToUpdate.money;
                    profileToUpdate.type = profileToUpdate.type;
                    profileToUpdate.date = profileToUpdate.date;
                    profileToUpdate.status = "4";
                    this._db.profile.Update(profileToUpdate);
                    this._db.SaveChanges();
                    return RedirectToAction("Management", "Admin");
                }

            }
            return NotFound();*/


            var profileToUpdate = this._db.profile.FirstOrDefault(p => p.username == username);
            if (profileToUpdate == null) return NotFound();

            if (!string.IsNullOrEmpty(btnAccept))
            {
                profileToUpdate.status = "1"; // Duyệt
            }
            else if (!string.IsNullOrEmpty(btnDecline))
            {
                profileToUpdate.status = "4"; // Từ chối
            }

            this._db.profile.Update(profileToUpdate);
            this._db.SaveChanges();
            return RedirectToAction("Management", "Admin");




        

        }

        public IActionResult deleteUser(string id)
        {
            string adminck = HttpContext.Request.Cookies["AdminCookie"];
            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            HttpContext.Session.SetString("check", "1");
            try
            {
                var information = (from pf in this._db.profile
                                   where pf.username == id
                                   select new Profile
                                   {
                                       username = pf.username,
                                       email = pf.email,
                                       fullname = pf.fullname,
                                       gender = pf.gender,
                                       birthday = pf.birthday,
                                       money = pf.money,
                                       type = pf.type,
                                       status = pf.status,
                                       date = pf.date
                                   }).ToList();

                ViewBag.Message3 = TempData["Message3"];
                return View(information[0]);
            }
            catch (System.Exception)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult deleteUser(string btnAccept, string btnDecline, string status, string username)
        {
            // Tìm kiếm thông tin người dùng cần cập nhật trong cơ sở dữ liệu
            var profileToUpdate = this._db.profile.FirstOrDefault(p
            =>
            p.username
            ==
            username
            );

            if (profileToUpdate != null)
            {
                if (!string.IsNullOrEmpty(btnAccept))
                {
                    profileToUpdate.username = profileToUpdate.username;
                    profileToUpdate.password = profileToUpdate.password;
                    profileToUpdate.email = profileToUpdate.email;
                    profileToUpdate.fullname = profileToUpdate.fullname;
                    profileToUpdate.gender = profileToUpdate.gender;
                    profileToUpdate.birthday = profileToUpdate.birthday;
                    profileToUpdate.money = profileToUpdate.money;
                    profileToUpdate.type = profileToUpdate.type;
                    profileToUpdate.date = profileToUpdate.date;
                    profileToUpdate.status = "2";
                    ViewBag.Message3 = "Delete account successfully";

                    if (profileToUpdate.type == "2")
                    {
                        string sqlQuery = "update game set status = '5' where sellerName = '" + profileToUpdate.username + "' and status = '1'";
                        this._db.Database.ExecuteSqlRaw(sqlQuery);
                    }

                }

                else if (!string.IsNullOrEmpty(btnDecline))
                {
                    profileToUpdate.username = profileToUpdate.username;

                    profileToUpdate.password = profileToUpdate.password;
                    profileToUpdate.email = profileToUpdate.email;
                    profileToUpdate.fullname = profileToUpdate.fullname;
                    profileToUpdate.gender = profileToUpdate.gender;
                    profileToUpdate.birthday = profileToUpdate.birthday;
                    profileToUpdate.money = profileToUpdate.money;
                    profileToUpdate.type = profileToUpdate.type;
                    profileToUpdate.date = profileToUpdate.date;
                    profileToUpdate.status = "1";
                    ViewBag.Message3 = "Recover account successfully";

                    if (profileToUpdate.type == "2")
                    {
                        string sqlQuery = "update game set status = '1' where sellerName = '" + profileToUpdate.username + "' and status = '5'";
                        this._db.Database.ExecuteSqlRaw(sqlQuery);
                    }
                }

                this._db.profile.Update(profileToUpdate);
                this._db.SaveChanges();
                return View(profileToUpdate);
            }
            return NotFound();
        }

        public IActionResult detailGame(string id)
        {
            string adminck = HttpContext.Request.Cookies["AdminCookie"];
            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            HttpContext.Session.SetString("check", "3");
            Game ga = this._db.game.Find(id);

            if (ga == null)
            {
                return RedirectToAction("Management", "Admin");
            }

            String sqlQuery = "select cc.CID, c.categoryName from CategoryConnect cc join Category c on c.CID = cc.CID where cc.GID = '" + id + "'";
            List<Category> categoryList = this._db.Set<Category>().FromSqlRaw(sqlQuery).ToList();

            ViewCategory viewCategory = new ViewCategory
            {
                GID = ga.GID,
                gameName = ga.gameName,
                price = ga.price,
                picture = ga.picture,
                date = ga.date,
                description = ga.description,
                configuration = ga.configuration,
                sellerName = ga.sellerName,
                status = ga.status,
                viewCategory = categoryList,
                downloadFile = ga.downloadFile
            };
            ViewBag.Message3 = TempData["Message3"];
            return View(viewCategory);
        }

        public IActionResult detailGame1(string id)
        {
            string adminck = HttpContext.Request.Cookies["AdminCookie"];
            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            HttpContext.Session.SetString("check", "6");
            Game ga = this._db.game.Find(id);
            if (ga == null)
            {
                return RedirectToAction("Management", "Admin");
            }
            String sqlQuery = "select cc.CID, c.categoryName from CategoryConnect cc join Category c on c.CID = cc.CID where cc.GID = '" + id + "'";

            List<Category> categoryList = this._db.Set<Category>().FromSqlRaw(sqlQuery).ToList();
            ViewCategory viewCategory = new ViewCategory
            {
                GID = ga.GID,
                gameName = ga.gameName,
                price = ga.price,
                picture = ga.picture,
                date = ga.date,
                description = ga.description,
                configuration = ga.configuration,
                sellerName = ga.sellerName,
                status = ga.status,
                downloadFile = ga.downloadFile,
                viewCategory = categoryList
            };

            return View(viewCategory);
        }
        [HttpPost]
        public IActionResult Recorvery(string username)
        {
            Profile profile = this._db.profile.Find(username);
            if (profile == null)
            {
                return NotFound();
            }
            profile.status = "3";
            this._db.SaveChanges();
            return RedirectToAction("Management", "Admin");
        }


        public IActionResult deleteGamee(string id)
        {
            var ga = this._db.game.Find(id);
            ga.GID = ga.GID;
            ga.gameName = ga.gameName;
            ga.price = ga.price;
            ga.picture = ga.picture;
            ga.date = ga.date;
            ga.description = ga.description;
            ga.configuration = ga.configuration;
            ga.sellerName = ga.sellerName;
            ga.status = "0";

            ga.downloadFile = ga.downloadFile;
            this._db.SaveChanges();
            TempData["Message3"] = "Delete game successfully";
            return RedirectToAction("detailGame", "Admin", new { id = id });
        }

        public IActionResult recoveryGame(string id)
        {
            var ga = this._db.game.Find(id);
            ga.GID = ga.GID;
            ga.gameName = ga.gameName;
            ga.price = ga.price;
            ga.picture = ga.picture;
            ga.date = ga.date;
            ga.description = ga.description;
            ga.configuration = ga.configuration;
            ga.sellerName = ga.sellerName;
            ga.status = "1";
            ga.downloadFile = ga.downloadFile;
            this._db.SaveChanges();
            TempData["Message3"] = "Recover game successfully";
            return RedirectToAction("detailGame", "Admin", new { id = id });
        }

        public IActionResult deleteGamee1(string id)
        {
            var ga = this._db.game.Find(id);
            ga.GID = ga.GID;
            ga.gameName = ga.gameName;
            ga.price = ga.price;
            ga.picture = ga.picture;
            ga.date = ga.date;
            ga.description = ga.description;
            ga.configuration = ga.configuration;
            ga.sellerName = ga.sellerName;
            ga.status = "4";
            ga.downloadFile = ga.downloadFile;
            this._db.SaveChanges();
            return RedirectToAction("Management", "Admin");
        }

        public IActionResult recoveryGame1(string id)
        {
            var ga = this._db.game.Find(id);
            ga.GID = ga.GID;
            ga.gameName = ga.gameName;
            ga.price = ga.price;
            ga.picture = ga.picture;
            ga.date = ga.date;
            ga.description = ga.description;
            ga.configuration = ga.configuration;
            ga.sellerName = ga.sellerName;
            ga.status = "1";

            ga.downloadFile = ga.downloadFile;
            this._db.SaveChanges();
            return RedirectToAction("Management", "Admin");
        }
        public static string GetMd5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public IActionResult Addnewaccount()
        {
            string adminck = HttpContext.Request.Cookies["AdminCookie"];
            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Addnewaccount(Profile p)
        {
            var pu = this._db.profile.Find(p.username);
            var pg = this._db.profile.FirstOrDefault(m => m.email == p.email);
            if (pu == null && pg == null)
            {
                p.status = "1";
                p.fullname = "";
                p.birthday = null;
                p.gender = "";
                p.password = GetMd5(p.password).ToLower();
                p.date = DateTime.Now;
                this._db.profile.Add(p);
                this._db.SaveChanges();
                ViewBag.Message1 = "Add new account successfully";
                return View();
            }
            if (pu != null && pg != null)
            {
                ViewBag.Message2 = "Both username and email already exist";
            }
            else if (pu != null)
            {
                ViewBag.Message2 = "Username already exists";
            }
            else if (pg != null)
            {
                ViewBag.Message2 = "Email already exists";
            }

            return View();

        }
        public IActionResult Revenue()
        {
            string adminck = HttpContext.Request.Cookies["AdminCookie"];
            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                string adminName = HttpContext.Request.Cookies["AdminCookie"];
                if (adminName == null)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }

            string adminName2 = HttpContext.Request.Cookies["AdminCookie"];

            string sqlquery = "select mm.[admin] as adusername,"
                + " mm.[date],"
                + " YEAR(mm.[date]) AS [year],"
                + " MONTH(mm.[date]) AS [month],"
                + " DAY(mm.[date]) AS [day],"
                + " mm.sellerMoney as sellerMoneyPlus,"
                + " mm.admoney as adMoneyPlus,"
                + " p.[money] as admoney,"
                + " od.GID,"
                + " g.gameName,"
                + " g.picture,"
                + " g.[status] from MoneyManagement mm"
                + " join OrderDetail od on od.ODID = mm.ODID"
                + " join Game g on g.GID = od.GID"
                + " join [Profile] p on p.username = mm.[admin]"
                + " where [admin] = '" + "user01" + "'"
                + "and g.[status] != '3'"
                + " and g.[status] != '4'"
                + "order by mm.[date]";

            List<AdminRevenueViewModel> adRev = new List<AdminRevenueViewModel>();

            using (var command = this._db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sqlquery;
                this._db.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        AdminRevenueViewModel item = new AdminRevenueViewModel
                        {
                            adusername = result.GetString(result.GetOrdinal("adusername")),
                            date = result.GetDateTime(result.GetOrdinal("date")),
                            year = result.GetInt32(result.GetOrdinal("year")),
                            month = result.GetInt32(result.GetOrdinal("month")),
                            day = result.GetInt32(result.GetOrdinal("day")),
                            sellerMoneyPlus = result.GetInt64(result.GetOrdinal("sellerMoneyPlus")),
                            adMoneyPlus = result.GetInt64(result.GetOrdinal("adMoneyPlus")),
                            admoney = result.GetInt64(result.GetOrdinal("adMoney")),
                            GID = result.GetString(result.GetOrdinal("GID")),
                            gameName = result.GetString(result.GetOrdinal("gameName")),
                            picture = result.GetString(result.GetOrdinal("picture")),
                            status = result.GetString(result.GetOrdinal("status"))
                        };

                        adRev.Add(item);
                    }
                }
            }
            var data = _db.moneyManagement.ToList();
            var rfs = data.GroupBy(mm => mm.username)
                             .Select(g => new RevenueFromSellers
                             {
                                 Username = g.Key,
                                 TotalSellerMoney = g.Sum(mm => mm.adMoney)
                             }).OrderByDescending(m => m.TotalSellerMoney)
                             .ToList();
            ViewBag.SellerList = rfs;
            return View(adRev);
        }
        public IActionResult DetailSellerRevenue(string id)
        {
            string adminck = HttpContext.Request.Cookies["AdminCookie"];
            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                string adminName = HttpContext.Request.Cookies["AdminCookie"];
                if (adminName == null)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }

            string sqlquery = "select mm.[username] as sellerName,"
                + " mm.[date],"
                + " YEAR(mm.[date]) AS [year],"
                + " MONTH(mm.[date]) AS [month],"
                + " DAY(mm.[date]) AS [day],"
                + " mm.admoney as sellerPlusMoney,"
                + " p.[money] as sellerMoney,"
                + " od.GID,"
                + " g.gameName,"
                + " g.picture,"
                + " g.[status] from MoneyManagement mm"
                + " join OrderDetail od on od.ODID = mm.ODID"
                + " join Game g on g.GID = od.GID"
                + " join [Profile] p on p.username = mm.username"
                + " where sellerName = '" + id + "'"
                + "and g.[status] != '3'"
                + " and g.[status] != '4'"
                + "order by mm.[date]";

            List<SellerRevenue> sellerRev = new List<SellerRevenue>();

            using (var command = this._db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sqlquery;
                this._db.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        SellerRevenue item = new SellerRevenue
                        {
                            sellerName = result.GetString(result.GetOrdinal("sellerName")),
                            date = result.GetDateTime(result.GetOrdinal("date")),
                            year = result.GetInt32(result.GetOrdinal("year")),
                            month = result.GetInt32(result.GetOrdinal("month")),
                            day = result.GetInt32(result.GetOrdinal("day")),
                            sellerPlusMoney = result.GetInt64(result.GetOrdinal("sellerPlusMoney")),
                            sellerMoney = result.GetInt64(result.GetOrdinal("sellerMoney")),
                            GID = result.GetString(result.GetOrdinal("GID")),
                            gameName = result.GetString(result.GetOrdinal("gameName")),
                            picture = result.GetString(result.GetOrdinal("picture")),
                            status = result.GetString(result.GetOrdinal("status"))
                        };

                        sellerRev.Add(item);
                    }
                }
            }

            return View(sellerRev);
        }
        public IActionResult DetailGameRevenue(string id)
        {
            string adminck = HttpContext.Request.Cookies["AdminCookie"];
            // Đây là một truy vấn LINQ để lấy danh sách các hồ sơ từ cơ sở dữ liệu.
            // Cụ thể, chúng ta đang truy vấn bảng "profile".
            if (adminck == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                string adminName = HttpContext.Request.Cookies["AdminCookie"];
                if (adminName == null)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }

            string adminName2 = HttpContext.Request.Cookies["AdminCookie"];

            string sqlquery = "select mm.[admin] as adusername,"
                + " o.username as cusName,"
                + " mm.[date],"
                + " YEAR(mm.[date]) AS [year],"
                + " MONTH(mm.[date]) AS [month],"
                + " DAY(mm.[date]) AS [day],"
                + " od.price as gamePrice,"
                + " mm.sellerMoney as sellerMoneyPlus,"
                + " mm.admoney as adMoneyPlus,"
                + " p.[money] as admoney,"
                + " od.GID,"
                + " g.gameName,"
                + " g.picture,"
                + " g.[status] from MoneyManagement mm"
                + " join OrderDetail od on od.ODID = mm.ODID"
                + " join [Order] o on o.OID = od.OID"
                + " join Game g on g.GID = od.GID"
                + " join [Profile] p on p.username = mm.username"
                + " where [admin] = 'user01'"
                + " and g.GID = '" + id + "'"
                + " and g.[status] != '3'"
                + " and g.[status] != '4'"
                + " order by mm.[date]";

            List<AdminRevenueViewModel> adRev = new List<AdminRevenueViewModel>();

            using (var command = this._db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sqlquery;
                this._db.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        AdminRevenueViewModel item = new AdminRevenueViewModel
                        {
                            adusername = result.GetString(result.GetOrdinal("adusername")),
                            cusName = result.GetString(result.GetOrdinal("cusName")),
                            date = result.GetDateTime(result.GetOrdinal("date")),
                            year = result.GetInt32(result.GetOrdinal("year")),
                            month = result.GetInt32(result.GetOrdinal("month")),
                            day = result.GetInt32(result.GetOrdinal("day")),
                            gamePrice = result.GetInt64(result.GetOrdinal("gamePrice")),
                            sellerMoneyPlus = result.GetInt64(result.GetOrdinal("sellerMoneyPlus")),
                            adMoneyPlus = result.GetInt64(result.GetOrdinal("adMoneyPlus")),
                            admoney = result.GetInt64(result.GetOrdinal("admoney")),
                            GID = result.GetString(result.GetOrdinal("GID")),
                            gameName = result.GetString(result.GetOrdinal("gameName")),
                            picture = result.GetString(result.GetOrdinal("picture")),
                            status = result.GetString(result.GetOrdinal("status"))
                        };

                        adRev.Add(item);
                    }
                }
            }

            return View(adRev);
        }

        public IActionResult toWaiting(string id)
        {
            var ga = this._db.game.Find(id);
            ga.GID = ga.GID;
            ga.gameName = ga.gameName;
            ga.price = ga.price;
            ga.picture = ga.picture;
            ga.date = ga.date;
            ga.description = ga.description;
            ga.configuration = ga.configuration;
            ga.sellerName = ga.sellerName;
            ga.status = "3";
            ga.downloadFile = ga.downloadFile;
            this._db.SaveChanges();
            return RedirectToAction("Management", "Admin");
        }

        public IActionResult changeseller(string id)
        {
            Profile profile = this._db.profile.Find(id);
            if (profile == null)
            {
                return NotFound();
            }
            profile.type = "2";
            this._db.SaveChanges();
            TempData["Message3"] = "Change account to seller successfully";
            return RedirectToAction("DeleteUser", "Admin", new { id = id });

        }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using prjPROJECT.Models;
using System.Web.Security;

namespace prjPROJECT.Controllers
{
    public class HomeController : Controller
    {
        // 建立與資料庫的連線實例
        public dbDATABASEEntities db = new dbDATABASEEntities();

        // GET: Home/Index 
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }


        // GET:Home/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST:Home/Login
        [HttpPost]
        public ActionResult Login(string fUserID, string fPwd)
        {
            // 用 LINQ 在 dbDATABASEE.dbo.tMember 中找帳號、密碼皆符合的資料
            // .FirstOrDefault() 回傳符合條件的第一筆資料，如果沒有則回傳 null
            var member = db.tMember
                .Where(m => m.fMem_UserID == fUserID && m.fMem_Pwd == fPwd)
                .FirstOrDefault();

            // 若 member 為 null，表示會員未註冊，回傳錯誤訊息至前端
            if (member == null)
            {
                ViewBag.Message = "帳密錯誤，登入失敗！";
                return View();
            }

            // 登入成功時，在 Session 中存入歡迎詞
            Session["Welcome"] = member.fMem_Name + " 嗨嗨";

            // 登入成功後 FormsAuthentication 類別中的 RedirectFromLoginPage 方法會給予 fUserID 認證，創建一個 Cookie
            // 第二個參數 true，代表 Cookie 是持久的，即使關閉瀏覽器，也不會立刻過期
            FormsAuthentication.RedirectFromLoginPage(fUserID, true);
            return RedirectToAction("HomePage", "Member");
        }


        // GET:Home/Register
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // Post:Home/Register
        [HttpPost]
        public ActionResult Register(tMember pMember)
        {
            // 檢查模型狀態是否有效。如果模型狀態無效(例如，前端傳遞的數據不符合定義的驗證規)則返回原始的 View
            if (ModelState.IsValid == false)
            {
                return View();
            }

            var member = db.tMember
                .Where(m => m.fMem_UserID == pMember.fMem_UserID)
                .FirstOrDefault();

            if (member == null)
            {
                // 將會員紀錄新增到 tMember 資料表
                db.tMember.Add(pMember);
                db.SaveChanges();

                return RedirectToAction("Login");
            }

            ViewBag.Message("此帳號已有人使用，註冊失敗");
            return View();
            
        }

    }
}
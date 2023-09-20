using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using prjPROJECT.Models; // Web 應用程式的表單驗證服務需引用此類別

namespace prjPROJECT.Controllers
{
    // [Authorize]屬性確保此控制器中的所有方法都要求用戶已經登入才能訪問
    [Authorize]
    public class MemberController : Controller
    {
        // 建立與資料庫的連線實例
        public dbDATABASEEntities db = new dbDATABASEEntities();

        // Get: Member/Index
        [HttpGet]
        public ActionResult Index()
        {
            return View("../Home/Index", "_LayoutMember");
        }


        // Get: Member/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut(); // 登出
            return RedirectToAction("Login", "Home");
        }


        // Get: Member/Account
        public ActionResult Account()
        {
            //取得登入會員的帳號並指定給 fUserID
            string fUserID = User.Identity.Name;
            var UserName = db.tMember
                .Where(m => m.fMem_UserID == fUserID)
                .ToList();
            return View(UserName);
        }


        // Get: Member/HomePage
        public ActionResult HomePage()
        {

            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using prjPROJECT.Models;
using System.Web.Security;
using System.Diagnostics;
using System.Data.Entity;

namespace prjPROJECT.Controllers
{
    // [Authorize]屬性確保此控制器中的所有方法都要求用戶已經登入才能訪問
    [Authorize]
    public class ExerciseController : Controller
    {
        // 建立與資料庫的連線實例
        public dbDATABASEEntities db = new dbDATABASEEntities();




        // GET: Exercise/Record
        [HttpGet]
        public ActionResult Record()
        {
            return Record(DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("yyyyMMdd"));
        }

        // POST: Exercise/Record
        [HttpPost]
        public ActionResult Record(string startDate, string endDate)
        {
            string fUserID = User.Identity.Name;

            // 如果沒有提供日期，則使用當前日期作為預設值
            if (string.IsNullOrEmpty(startDate))
                startDate = DateTime.Now.ToString("yyyyMMdd");
            if (string.IsNullOrEmpty(endDate))
                endDate = DateTime.Now.ToString("yyyyMMdd");

            var records = db.tExercise
                .Where(m => m.fEX_UserID == fUserID &&
                            string.Compare(m.fEX_DATE, startDate) >= 0 &&
                            string.Compare(m.fEX_DATE, endDate) <= 0)
                .ToList();

            return View(records);
        }


        // POST: Exercise/Create
        [HttpPost]
        public ActionResult Create(tExercise pExercise)
        {
            // 前端發送一個請求到伺服器(例如 Ajax)，這些數據綁訂到 tExercise pExercise 時 ModelState.IsValid 會檢查是否符合 tExercise 模型的驗證規則
            // ModelState 是一個字典，儲存模型綁定期間所有屬性的驗證結果(例如 [Required]、[EmailAddress])
            // IsValid 會檢查是否所有屬性都通過驗證，有任何失敗，將會回傳 false
            if (!ModelState.IsValid)
            {

                // .Values 提取 ModelState 中所有 value
                // 使用 LINQ，SelectMany 一次回傳多個結果
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors)
                                                     .Select(e => e.ErrorMessage)
                                                     .ToList();

                // 列印所有的錯誤到Debug窗口
                foreach (var errorMessage in errorMessages)
                {
                    Debug.WriteLine(errorMessage);
                }

                // 回傳所有錯誤
                return Json(new { success = false, errorMessage = string.Join("\n", errorMessages) });
            }

            try
            {
                pExercise.fEX_UserID = User.Identity.Name;

                // 使用 Entity Framework 將 pExercise 物件加到 tExercise 資料表的本地暫存中
                db.tExercise.Add(pExercise);
                // 將所有本地暫存的更改保存到資料庫
                db.SaveChanges();

                // 返回成功回應
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                // 返回錯誤回應
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        // POST: Exercise/Edit
        [HttpPost]
        public ActionResult Edit(tExercise pExercise)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors)
                                                     .Select(e => e.ErrorMessage)
                                                     .ToList();

                foreach (var errorMessage in errorMessages)
                {
                    Debug.WriteLine(errorMessage);
                }

                return Json(new { success = false, errorMessage = string.Join("\n", errorMessages) });
            }

            try
            {

                var exerciseToUpdate = db.tExercise.Find(pExercise.fID); // 假設你的tExercise有一個Id屬性來做為主鍵

                if (exerciseToUpdate == null)
                {
                    return Json(new { success = false, errorMessage = "找不到對應的紀錄!" });
                }

                // 更新資料
                exerciseToUpdate.fEX_DATE = pExercise.fEX_DATE;
                exerciseToUpdate.fEX_TYPE = pExercise.fEX_TYPE;
                exerciseToUpdate.fEX_MUSCLE = pExercise.fEX_MUSCLE;
                exerciseToUpdate.fEX_METHOD = pExercise.fEX_METHOD;
                exerciseToUpdate.fEX_TIMES = pExercise.fEX_TIMES;
                exerciseToUpdate.fEX_KG = pExercise.fEX_KG;
                exerciseToUpdate.fEX_SETS = pExercise.fEX_SETS;

                db.SaveChanges();

                return Json(new { success = true});
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }


        // POST: Exercise/Delete
        public ActionResult Delete(tExercise pExercise) // 假設您使用 tExercise 的主鍵來進行刪除操作
        {

            try
            {
                // 根據主鍵找到要刪除的物件
                var exerciseToDelete = db.tExercise.Find(pExercise.fID);

                if (exerciseToDelete == null)
                {
                    return Json(new { success = false, errorMessage = "找不到對應的紀錄!" });
                }

                // 從 DbSet 中刪除該物件
                db.tExercise.Remove(exerciseToDelete);

                // 儲存變更
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }


        public ActionResult FilterByDate(string startDate, string endDate)
        {
            var filteredData = db.tExercise
                .Where(x => string.Compare(x.fEX_DATE, startDate) >= 0 
                         && string.Compare(x.fEX_DATE, endDate) <= 0)
                .ToList();

            return PartialView("_ExerciseTablePartial", filteredData); // 假設你有一個叫 _ExerciseTablePartial 的Partial View
        }


    }
}
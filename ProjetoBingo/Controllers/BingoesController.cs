using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjetoBingo.Entity;
using System.Data.SqlClient;

namespace ProjetoBingo.Controllers
{
    public class BingoesController : Controller
    {
        private Database1Entities db = new Database1Entities();

        // GET: Bingoes
        public ActionResult Index()
        {
            return View(db.Bingo.ToList());
        }

        // GET: Bingoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bingo bingo = db.Bingo.Find(id);
            if (bingo == null)
            {
                return HttpNotFound();
            }
            return View(bingo);
        }

        // GET: Bingoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Bingoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdBingo,DataHora,Descricao,Status,DscCancelamenrto,DataCancelamento")] Bingo bingo)
        {
            bingo.Status = "A";
            if (ModelState.IsValid)
            {
                db.Bingo.Add(bingo);
                db.SaveChanges();

                CriarCartelas(bingo.IdBingo);

                return RedirectToAction("Edit", new { id = bingo.IdBingo });
            }

            return View(bingo);
        }

        private void CriarCartelas(int IdBingo)
        {
            try
            {
                string sqlQuery = "PRC_GerarCartelas {0}";

                using (Database1Entities dbContext = new Database1Entities())
                {
                    var retorno = dbContext.Database.ExecuteSqlCommand(sqlQuery, IdBingo);
                }
            }
            catch (Exception ex)
            {
                //handle, Log or throw exception 
            }
        }

        // GET: Bingoes/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.ListaStatus = ListaStatus();
            ViewBag.ListaPremios = db.Premio.ToList();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bingo bingo = db.Bingo.Find(id);
            if (bingo == null)
            {
                return HttpNotFound();
            }
            return View(bingo);
        }

        // POST: Bingoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdBingo,DataHora,Descricao,Status,DscCancelamenrto,DataCancelamento")] Bingo bingo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bingo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bingo);
        }

        // GET: Bingoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bingo bingo = db.Bingo.Find(id);
            if (bingo == null)
            {
                return HttpNotFound();
            }
            return View(bingo);
        }

        //// POST: Bingoes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Bingo bingo = db.Bingo.Find(id);
        //    db.Bingo.Remove(bingo);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        // POST: Bingoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete([Bind(Include = "IdBingo,DataHora,Descricao,Status,DscCancelamenrto,DataCancelamento")] Bingo bingo)
        {
            bingo.Status = "C";
            if (ModelState.IsValid)
            {
                db.Entry(bingo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bingo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private List<SelectListItem> ListaStatus()
        {
            List<SelectListItem> States = new List<SelectListItem>()
            {
                new SelectListItem() {Text="Ativo", Value="A"},
                new SelectListItem() {Text="Finalizado", Value="F"},
                new SelectListItem() {Text="Cancelado", Value="C"}
            };

            return States;
        }

        [HttpPost]
        public ActionResult GravarPremio(int idBingo, int idPremio, int? idBingoPremio)
        {
            try
            {
                BingoPremio bingoPremio = new BingoPremio();
                bingoPremio.IdBingo = idBingo;
                bingoPremio.IdPremio = idPremio;

                if (idBingoPremio == null)
                {
                    db.BingoPremio.Add(bingoPremio);
                    db.SaveChanges();
                    return Json(bingoPremio.Id, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    bingoPremio.Id = idBingoPremio.Value;
                    db.BingoPremio.Attach(bingoPremio);
                    db.BingoPremio.Remove(bingoPremio);
                    db.SaveChanges();
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Sorteio(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Sorterio(id);

            return RedirectToAction("Index");
        }

        private void Sorterio(int? IdBingo)
        {
            try
            {
                string sqlQuery = "PRC_Sorteio {0}";

                using (Database1Entities dbContext = new Database1Entities())
                {
                    var retorno = dbContext.Database.ExecuteSqlCommand(sqlQuery, IdBingo);
                }
            }
            catch (Exception ex)
            {
                //handle, Log or throw exception 
            }
        }

    }
}

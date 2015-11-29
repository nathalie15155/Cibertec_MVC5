﻿using NorthwindLab03.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NorthwindLab03.Controllers
{
    public class CategoriesController : Controller
    {

        private Models.NorthwindEntities Contexto =
            new Models.NorthwindEntities();

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.Contexto.Dispose(); 
        }
        private StringBuilder Trace = new StringBuilder(); 
        // GET: Categories
        private void makeTrace(string message)
        {
            Trace.AppendLine(message);            
        }


        //[OutputCache(Duration=60,
        //     VaryByHeader="", VaryByParam="", 
        //     Location= System.Web.UI.OutputCacheLocation.Client )]
        public ActionResult Index()
        {
            DateTime.UtcNow

            //System.Threading.Thread.CurrentThread.CurrentUICulture
            //System.Threading.Thread.CurrentThread.CurrentCulture
            //System.Threading.Thread.CurrentPrincipal.Identity

            ViewBag.Title = Resources.Titles.Index + " " + DateTime.Now.ToShortDateString() ;

            this.Contexto.Database.Log += makeTrace; 
                        

            var query = from c in this.Contexto.Categories
                        select c;

            var result = query.ToList();

            this.Contexto.Database.Log -= makeTrace;

            ViewBag.TraceMessage = Trace.ToString(); 
            return View(result);
        }
        [HttpPost]
        [OutputCache(CacheProfile="tinyCache",
            VaryByParam = "filter")]
        public ActionResult Index(string filter)
        {
            this.Contexto.Database.Log += makeTrace; 

            var query = (from c in this.Contexto.Categories
                        where c.CategoryName.Contains(filter)
                        select c).ToList();

            this.Contexto.Database.Log -= makeTrace;

            this.ViewBag.Filter = filter;
            ViewBag.TraceMessage = Trace.ToString(); 
            return View(query);

        }
        public ActionResult Details(int id)
        {
            var query = (from c in this.Contexto.Categories
                        where c.CategoryID == id
                        select c).FirstOrDefault();
            if (query == null)
                return this.HttpNotFound();
            return this.View(query); 
        }

        private void OnSaveContext()
        {
            this.Contexto.SaveChanges(); 

            string index = Url.Action("Index");
            this.Response.RemoveOutputCacheItem(index);

        }

        public ActionResult Create()
        {
            return this.View();
        }
        [HttpPost]
        public ActionResult Create(
            [Bind(Include="CategoryName,Description")]Categories Model)
        {
            
            if (!this.ModelState.IsValid)
            {
                return this.View(Model);
            }

            if (Model.CategoryName == Model.Description )
            {
                //this.ModelState.Clear();
                this.ModelState.AddModelError("", "Category must not equal description");
                return this.View(Model);
            }
            try
            {
                this.Contexto.Categories.Add(Model);
                OnSaveContext();     
                return this.RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError("", ex);
                return this.View(Model);
            }
            
        }

        public ActionResult Delete(int id)
        {
            var  category =  this.Contexto.Categories.Find(id);
            if (category == null)
                return this.HttpNotFound();

            return this.View(category);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult PostDelete(int id)
        {
            var category = this.Contexto.Categories.Find(id);
            if (category != null)
            {
                try
                {
                    this.Contexto.Categories.Remove(category);
                    this.Contexto.SaveChanges(); 
                }
                catch (Exception ex)
                {
                    this.ModelState.AddModelError("", ex);
                    return this.View(category);                    
                }               
            }
            return this.RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var category = this.Contexto.Categories.Find(id);
            if (category == null)
            {
                return this.HttpNotFound(); 
            }
            return this.View(category); 
        }
        [HttpPost]        
        public ActionResult  Edit(int id, Categories Model)
        {
            var category = this.Contexto.Categories.Find(id);
            if( category == null)
                return this.HttpNotFound(); 

            if(!this.ModelState.IsValid)
            {
                return this.View(Model);
            }                       
            
            if (!this.TryUpdateModel(category, 
                new string[]{ "CategoryName", "Description" }))
            {
                return this.View(Model);
            }
            try
            {
                this.Contexto.SaveChanges();
                return this.RedirectToAction("Index");
            }
            catch (Exception ex)
            {                
                this.ModelState.AddModelError("", ex);
                return this.View(Model);           
            }
        }

    }
}
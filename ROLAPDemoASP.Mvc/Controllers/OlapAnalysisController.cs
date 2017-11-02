using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using RadarSoft.RadarCube.Web.Mvc;

namespace Controllers
{
    public class OlapAnalysisController : Controller
    {
        public SamplesModel SampleModel { get; set; }

        public ViewResult Index()
        {
            SampleModel = new RSamplesModel { Sample = Samples.GridSample };
            ViewBag.Title = SampleModel.SamplesTitles[Samples.None];
            return View(SampleModel);
        }

        [AjaxOnly]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult CallbackHandler()
        {
            SampleModel = new RSamplesModel();
            return Json(SampleModel.DoCallback(), JsonRequestBehavior.DenyGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public BinaryStreamResult ExportHandler()
        {
            SampleModel = new RSamplesModel();
            return SampleModel.DoExport();
        }

        public ViewResult ChangeSkin(string skin)
        {
            SampleModel = new RSamplesModel();
            SampleModel.Skin = skin;
            ViewBag.Title = SampleModel.Title;
            return View("Index", SampleModel);
        }

        public ViewResult GridSample(FormCollection gridSamplesArgs)
        {
            SampleModel = new RSamplesModel(gridSamplesArgs) { Sample = Samples.GridSample };
            ViewBag.Title = SampleModel.Title;
            return View("Index", SampleModel);
        }

        public ViewResult SimpleSales()
        {
            SampleModel = new RSamplesModel { Sample = Samples.SimpleSales };
            ViewBag.Title = SampleModel.Title;
            return View("Index", SampleModel);
        }

        public ViewResult QuantitySales()
        {
            SampleModel = new RSamplesModel { Sample = Samples.QuantitySales };
            ViewBag.Title = SampleModel.Title;
            return View("Index", SampleModel);
        }

        public ViewResult SalesByCategories()
        {
            SampleModel = new RSamplesModel { Sample = Samples.SalesByCategories };
            ViewBag.Title = SampleModel.Title;
            return View("Index", SampleModel);
        }

        public ViewResult Compare()
        {
            SampleModel = new RSamplesModel { Sample = Samples.Compare };
            ViewBag.Title = SampleModel.Title;
            return View("Index", SampleModel);
        }

        public ViewResult Shapes()
        {
            SampleModel = new RSamplesModel { Sample = Samples.Shapes };
            ViewBag.Title = SampleModel.Title;
            return View("Index", SampleModel);
        }

        public ViewResult Density()
        {
            SampleModel = new RSamplesModel { Sample = Samples.Density };
            ViewBag.Title = SampleModel.Title;
            return View("Index", SampleModel);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult PaintTrend()
        {
            return new ImageStreamResult(); 
        }
    }
}

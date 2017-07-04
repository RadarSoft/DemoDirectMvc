using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RadarSoft.RadarCube.Web.Mvc;
using SamplesFactory;

namespace Models
{
    public class SamplesModel
    {
        public SamplesModel(FormCollection gridSamplesArgs = null)
        {
            Init();

            if (gridSamplesArgs == null)
                return;

            foreach (var gridSamplesArg in gridSamplesArgs)
            {
                switch (gridSamplesArg.ToString())
                {
                    case "ForeColorChecked":
                        ForeColorChecked = gridSamplesArgs.GetValue("ForeColorChecked").AttemptedValue == "true,false";
                        break;
                    case "BackColorChecked":
                        BackColorChecked = gridSamplesArgs.GetValue("BackColorChecked").AttemptedValue == "true,false";
                        break;
                    case "ImagesChecked":
                        ImagesChecked = gridSamplesArgs.GetValue("ImagesChecked").AttemptedValue == "true,false";
                        break;
                    case "PaintChecked":
                        PaintChecked = gridSamplesArgs.GetValue("PaintChecked").AttemptedValue == "true,false";
                        break;
                    case "ControlsChecked":
                        ControlsChecked = gridSamplesArgs.GetValue("ControlsChecked").AttemptedValue == "true,false";
                        break;
                    case "ThresholdChecked":
                        ThresholdChecked = gridSamplesArgs.GetValue("ThresholdChecked").AttemptedValue == "true,false";
                        break;
                    case "ThresholdValue":
                        ThresholdValue = gridSamplesArgs.GetValue("ThresholdValue").AttemptedValue;
                        break;
                    case "ddSkinSelector":
                        if (gridSamplesArgs.GetValue("ddSkinSelector").AttemptedValue != "")
                            Skin = gridSamplesArgs.GetValue("ddSkinSelector").AttemptedValue;
                        break;
                }
            }
        }

        private void Init()
        {
            FillSamplesTitles();
            FillSamplesCreators();
        }

        public Dictionary<Samples, string> SamplesTitles = new Dictionary<Samples, string>();

        public Dictionary<Samples, SampleCreator> SamplesCreators = new Dictionary<Samples, SampleCreator>();
        
        private Samples? _Sample;
        public Samples Sample
        {
            get
            {
                if (_Sample != null)
                    return (Samples)_Sample;

                var sample = Samples.None;

                if (Context.Session["Sample"] == null)
                {
                    _Sample = Samples.None;
                    return sample;
                }

                if (Samples.TryParse(Context.Session["Sample"].ToString(), out sample))
                {
                    _Sample = sample;
                    return sample;
                }

                _Sample = sample;
                return sample;
            }
            set
            {
                Context.Session["Sample"] = value.ToString();
                _Sample = value;
            }
        }

        protected HttpContextBase _Context;
        public virtual HttpContextBase Context
        {
            get { return _Context ?? (_Context = new HttpContextWrapper(HttpContext.Current)); }
        }

        public string Skin
        {
            get
            {
                return (Context.Session["Skin"] != null) ? Context.Session["Skin"].ToString() : "Base";
            }
            set
            {
                Context.Session["Skin"] = value;
            }
        }

        public bool ForeColorChecked
        {
            get
            {
                return (Context.Session["ForeColorChecked"] != null);
            }
            set
            {
                if (value)
                    Context.Session["ForeColorChecked"] = true;
                else
                    Context.Session.Remove("ForeColorChecked");
            }
        }

        public bool BackColorChecked
        {
            get
            {
                return (Context.Session["BackColorChecked"] != null);
            }
            set
            {
                if (value)
                    Context.Session["BackColorChecked"] = true;
                else
                    Context.Session.Remove("BackColorChecked");
            }
        }

        public bool ImagesChecked
        {
            get
            {
                return (Context.Session["ImagesChecked"] != null);
            }
            set
            {
                if (value)
                    Context.Session["ImagesChecked"] = true;
                else
                    Context.Session.Remove("ImagesChecked");
            }
        }

        public bool PaintChecked
        {
            get
            {
                return (Context.Session["PaintChecked"] != null);
            }
            set
            {
                if (value)
                    Context.Session["PaintChecked"] = true;
                else
                    Context.Session.Remove("PaintChecked");
            }
        }

        public bool ControlsChecked
        {
            get
            {
                return (Context.Session["ControlsChecked"] != null);
            }
            set
            {
                if (value)
                    Context.Session["ControlsChecked"] = true;
                else
                    Context.Session.Remove("ControlsChecked");
            }
        }

        public bool ThresholdChecked
        {
            get
            {
                return (Context.Session["ThresholdChecked"] != null);
            }
            set
            {
                if (value)
                    Context.Session["ThresholdChecked"] = true;
                else
                    Context.Session.Remove("ThresholdChecked");
            }
        }

        public string ThresholdValue
        {
            get
            {
                return (Context.Session["ThresholdValue"] != null) ? Context.Session["ThresholdValue"].ToString() : "10000";
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    Context.Session["ThresholdValue"] = value;
                else
                    Context.Session.Remove("ThresholdValue");
            }
        }

        public object Title
        {
            get { return SamplesTitles[Sample]; }
        }

        public string Legend { get; set; }       
        public string Description { get; set; }       

        protected const string _ChartTitleFormat = "Chart features | {0}";

        protected virtual void FillSamplesTitles()
        {
        }

        protected virtual void FillSamplesCreators()
        {
        }

        public SampleCreator GetSampleCreator()
        {
            return SamplesCreators[Sample];
        }

        public virtual object DoCallback()
        {
            var directSampleCreator = SamplesCreators[Samples.None];
            var directSample = directSampleCreator.CreateSample(this);
            return directSample.OlapAnalysis.DoCallback();
        }

        public virtual BinaryStreamResult DoExport()
        {
            var directSampleCreator = SamplesCreators[Samples.None];
            var directSample = directSampleCreator.CreateSample(this);
            return directSample.OlapAnalysis.DoExport();
        }

        public virtual HtmlString Render()
        {
            var sampleCreator = GetSampleCreator();
            var sample = sampleCreator.CreateSample(this);
            return sample.OlapAnalysis.Render();
        }
    }

    public enum Samples
    {
        None,
        GridSample,
        SimpleSales,
        QuantitySales,
        SalesByCategories,
        Compare,
        Shapes,
        Density
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Models;
using RadarSoft.RadarCube.Common;
using RadarSoft.RadarCube.Web;
using RadarSoft.RadarCube.Web.Analysis;
using RadarSoft.RadarCube.Web.Mvc;

namespace SamplesFactory
{
    public abstract class Sample
    {
        public MvcOlapAnalysis OlapAnalysis { get; set; }
        public SamplesModel Model { get; set; }

        protected Sample(SamplesModel samplesModel)
        {
            Model = samplesModel;
        }

        public abstract void DoActive();
        public abstract void CreateOlapAnalysis();
        public abstract void InitSample();
    }

    public abstract class SampleCreator
    {
        public abstract Sample CreateSample(SamplesModel sampleModel);
    }
}
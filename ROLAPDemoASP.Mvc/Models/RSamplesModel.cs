using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RadarSoft.RadarCube.Web.Mvc;
using SamplesFactory;

namespace Models
{
    public class RSamplesModel : SamplesModel
    {
        public RSamplesModel(FormCollection gridSamplesArgs = null):base(gridSamplesArgs)
        {
            Description = "<p>If you are looking for a flexible, easily deploy and feature-rich solution for OLAP analysis in your ASP.NET MVC application, " +
                          "then RadarCube controls for ASP.NET MVC is what you need.</p>" +
                        "<p>The examples demonstrate capabilities of Relational OLAP analysis using Grid and Chart RadarCube functionality of Direct Edition.</p>";
            Legend = "RadarCube for ASP.NET MVC (Direct Edition)";
        }

        protected override void FillSamplesTitles()
        {
            SamplesTitles.Add(Samples.None, "Samples of Relational OLAP analysis");
            SamplesTitles.Add(Samples.GridSample, "Grid features");
            SamplesTitles.Add(Samples.SimpleSales, string.Format(_ChartTitleFormat, "Mounthly sales"));
            SamplesTitles.Add(Samples.Compare, string.Format(_ChartTitleFormat, "Sales Count vs Sales Count Net"));
            SamplesTitles.Add(Samples.Density, string.Format(_ChartTitleFormat, "Density of distrubution"));
            SamplesTitles.Add(Samples.QuantitySales, string.Format(_ChartTitleFormat, "Cross Sales Count vs Warehouse Sales"));
            SamplesTitles.Add(Samples.SalesByCategories, string.Format(_ChartTitleFormat, "Sales Count by cities"));
            SamplesTitles.Add(Samples.Shapes, string.Format(_ChartTitleFormat, "Shapes"));
        }

        protected override void FillSamplesCreators()
        {
            SamplesCreators.Add(Samples.None, new RSampleCreator());
            SamplesCreators.Add(Samples.GridSample, new GridSampleCreator());
            SamplesCreators.Add(Samples.SimpleSales, new SimpleSalesCreator());
            SamplesCreators.Add(Samples.Compare, new CompareCreator());
            SamplesCreators.Add(Samples.Density, new DensityCreator());
            SamplesCreators.Add(Samples.QuantitySales, new QuantitySalesCreator());
            SamplesCreators.Add(Samples.SalesByCategories, new SalesByCategoriesCreator());
            SamplesCreators.Add(Samples.Shapes, new ShapesCreator());
        }
    }
}
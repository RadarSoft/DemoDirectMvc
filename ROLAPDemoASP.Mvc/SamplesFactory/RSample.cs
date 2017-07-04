using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Models;
using OLAPDemoASP.Code;
using RadarSoft.RadarCube.Common;
using RadarSoft.RadarCube.Web;
using RadarSoft.RadarCube.Web.Analysis;
using RadarSoft.RadarCube.Web.Mvc;

namespace SamplesFactory
{
    public class RSample : Sample
    {
        public RSample(SamplesModel samplesModel) : base(samplesModel)
        {
        }

        public override void CreateOlapAnalysis()
        {
            OlapAnalysis = new MvcOlapAnalysis("MvcOlapAnalysis1");

            OlapAnalysis.InitOlap += OlapAnalysisInit;
            

            var cube = new MvcRCube
            {
                DataSet = new Northwind(),
                ID = OlapAnalysis.ID + "_Cube"
            };

            OlapAnalysis.Cube = cube;
            cube.OnCalculateField += cube_OnCalculateField;
        }

        private void OlapAnalysisInit(object sender, EventArgs eventArgs)
        {
            InitSample();
        }

        internal void cube_OnCalculateField(object sender, TCalculateFieldArgs e)
        {
            if (e.ThisTable("Order Details"))
            {
                e.Row["Sales"] = Convert.ToDouble(e.Row["Quantity"]) * Convert.ToDouble(e.Row["UnitPrice"]) * (1 - Convert.ToDouble(e.Row["Discount"]));
            }
            if (e.ThisTable("Employees"))
            {
                e.Row["Employee Name"] = e.Row["FirstName"] + " " + e.Row["LastName"];
            }
        }

        public override void InitSample()
        {
            OlapAnalysis.CallbackController = "OlapAnalysis";
            OlapAnalysis.CallbackAction = "CallbackHandler";
            OlapAnalysis.ExportController = "OlapAnalysis";
            OlapAnalysis.ExportAction = "ExportHandler";
            OlapAnalysis.MaxTextLength = 17;
            OlapAnalysis.Height = new Unit("500px");
        }

        public override void DoActive()
        {
            if (OlapAnalysis.Cube.Active)
                OlapAnalysis.ClearAxesLayout();

            if (OlapAnalysis.Cube.Active == false)
            {
                InitCubeStructure();

                ((MvcRCube)OlapAnalysis.Cube).FactTableName = "Order Details";

                OlapAnalysis.Cube.Active = true;
            }
        }

        public virtual void InitCubeStructure()
        {
            var cube = OlapAnalysis.Cube as TOLAPCube;
            var d = cube.DataSet as Northwind;
            if (d == null)
                throw new ApplicationException("The cube's DataSet property must be assigned before setting up the structure");
            // Create dimensions and hierarchies
            cube.AddHierarchy("Shippers", d.Shippers, "CompanyName", "", "Shippers");
            // Make 3 hierarchies in the "Products" dimension: "Products", "Categories", and "Suppliers"
            TCubeHierarchy H1 = cube.AddHierarchy("Products", d.Products, "ProductName", "", "Products");
            TCubeHierarchy H2 = cube.AddHierarchy("Products", d.Categories, "CategoryName", "", "Categories");
            TCubeHierarchy H3 = cube.AddHierarchy("Products", d.Suppliers, "CompanyName", "", "Suppliers");
            // Make two composite (multilevel) hierarchies 
            cube.MakeUpCompositeHierarchy("Products", "Products by categories", new TCubeHierarchy[] { H2, H1 });
            cube.MakeUpCompositeHierarchy("Products", "Products by suppliers", new string[] { "Suppliers", "Products" });
            // Add BI time hierarchies: "Year", "Quarter", "Month"...
            cube.AddBIHierarchy("Time", d.Orders, "Year", "OrderDate", TBIMembersType.ltTimeYear);
            cube.AddBIHierarchy("Time", d.Orders, "Quarter", "OrderDate", TBIMembersType.ltTimeQuarter);
            cube.AddBIHierarchy("Time", d.Orders, "Month", "OrderDate", TBIMembersType.ltTimeMonthLong);
            // ... and combine them into a single "Date" hierarchy
            cube.MakeUpCompositeHierarchy("Time", "Date", new string[] { "Year", "Quarter", "Month" });

            // The two lines add the calculated hierarchy "Employee Name" into the "Employees" dimension:
            // The "Employee Name" column must be calculated in the TOLAPCube1.OnCalculateField even handler
            cube.AddCalculatedColumn(d.Employees, "Employee Name", typeof(String));
            cube.AddHierarchy("Employees", d.Employees, "Employee Name", "ReportsTo", "Employees");
            // just the same thing might have been done with a single line of code:
            // cube.AddCalculatedHierarchy("Employees", d.Employees, typeof(string), "Employee Name");

            cube.AddHierarchy("Customers", d.Customers, "CompanyName", "", "Customers");
            // Add two measures: "Quantity" and "Sales"
            cube.AddMeasure(d.Order_Details, "Quantity");
            // The "Sales" column must be calculated in the TOLAPCube1.OnCalculateField even handler
            cube.AddCalculatedMeasure(d.Order_Details, typeof(double), "Sales");
        }

        internal void InitColorModification()
        {
            var h = OlapAnalysis.Dimensions.FindHierarchyByDisplayName("Products by categories");
            var m = OlapAnalysis.Measures.FindByDisplayName("Sales");

            if (Model.ForeColorChecked && h != null)
                OlapAnalysis.PivotingFirst(h, TLayoutArea.laColorFore);
            else if (h != null && h.Initialized) OlapAnalysis.PivotingOut(h, TLayoutArea.laColorFore);

            if (Model.BackColorChecked && m != null)
                OlapAnalysis.Pivoting(m, TLayoutArea.laColor);
            else if (m != null) OlapAnalysis.PivotingOut(m, TLayoutArea.laColor);
        }

        internal void SetModificationPanelsVisibility()
        {
            if (!Model.ForeColorChecked && !Model.BackColorChecked)
            {
                OlapAnalysis.ShowLegends = false;
                OlapAnalysis.ShowModificationAreas = false;
            }
            else
            {
                OlapAnalysis.ShowLegends = true;
                OlapAnalysis.ShowModificationAreas = true;
            }
        }
    }

    public class GridSample : RSample
    {
        public GridSample(SamplesModel samplesModel) : base(samplesModel)
        {
        }

        public override void CreateOlapAnalysis()
        {
            base.CreateOlapAnalysis();
            OlapAnalysis.OnRenderCell += OlapAnalysisOnOnRenderCell;
        }

        public override void InitSample()
        {
            base.InitSample();
            OlapAnalysis.AnalysisType = AnalysisType.Grid;
            OlapAnalysis.StructureTreeWidth = new Unit(300, UnitType.Pixel);

            DoActive();

            THierarchy H = OlapAnalysis.Dimensions.FindHierarchyByDisplayName("Products by categories");
            H.TotalAppearance = TTotalAppearance.taInvisible;
            H.ShowEmptyLines = true;
            OlapAnalysis.PivotingLast(H, TLayoutArea.laRow);
            H = OlapAnalysis.Dimensions.FindHierarchyByDisplayName("Date");
            H.ShowEmptyLines = true;
            OlapAnalysis.PivotingLast(H, TLayoutArea.laColumn);

            TMeasure measure = OlapAnalysis.Measures.FindByDisplayName("Sales");
            if (measure != null)
                OlapAnalysis.Pivoting(measure, TLayoutArea.laRow, null);

            measure = OlapAnalysis.Measures.FindByDisplayName("Quantity");
            if (measure != null)
                OlapAnalysis.Pivoting(measure, TLayoutArea.laRow, null);

            InitColorModification();

            SetModificationPanelsVisibility();
        }

        double _maxValue = Double.MinValue;
        double _minValue = Double.MaxValue;

        private bool SetMaxValue()
        {
            if (_maxValue != Double.MinValue) return true;
            if (_minValue == _maxValue) return false;
            for (int i = OlapAnalysis.CellSet.FixedRows; i < OlapAnalysis.CellSet.RowCount; i++)
            {
                IDataCell d = OlapAnalysis.CellSet[OlapAnalysis.CellSet.FixedColumns, i] as IDataCell;
                if ((d == null) || (d.Data == null) || (d.IsTotalHorizontal)) continue;
                try
                {
                    double v = Convert.ToDouble(d.Data);
                    if (v > _maxValue) _maxValue = v;
                    if (v < _minValue) _minValue = v;
                }
                catch
                {; }
            }
            return ((_maxValue != Double.MinValue) && (_minValue != _maxValue));
        }

        internal void OlapAnalysisOnOnRenderCell(object sender, RenderCellEventArgs e)
        {
            if (OlapAnalysis.AnalysisType == AnalysisType.Chart)
                return;

            if (e.Cell.CellType == TCellType.ctMember)
            {
                if (Model.ImagesChecked)
                {
                    var mc = e.Cell as IMemberCell;
                    if ((mc.Member != null) && (mc.Member.Level.DisplayName == "Categories"))
                    {
                        string s = e.Text;
                        if (s.Contains("/"))
                            s = s.Substring(0, s.IndexOf('/'));
                        e.Text = "<img src=\"../../Content/images/Example/" + s + ".png\">" + e.Text;
                    }

                }

                if (Model.ControlsChecked)
                    e.Text = WriteContextMenuButton(e.Text);
            }
            if (e.Cell.CellType == TCellType.ctLevel)
            {
                if (Model.ControlsChecked)
                    e.Text = WriteContextMenuButton(e.Text);
            }
            if (e.Cell.CellType == TCellType.ctData)
            {
                IDataCell dc = e.Cell as IDataCell;
                if ((Model.PaintChecked) && (dc.StartColumn == OlapAnalysis.CellSet.FixedColumns))
                {
                    if ((!dc.IsTotalHorizontal) && (SetMaxValue()))
                    {
                        try
                        {
                            Double v = Convert.ToDouble(dc.Data);
                            double hue = (v - _minValue) / (_maxValue - _minValue) * 85;
                            Color c = FromHSB(hue, 255, 255);
                            e.Text = "<table width=\"100%\"><tr><td align=\"left\" valign=\"middle\"><img src=\"/" + OlapAnalysis.CallbackController + "/PaintTrend/?color=" +
                                ColorTranslator.ToHtml(c).Substring(1) + "\"></td><td align=\"right\" valign=\"middle\">"
                                + e.Text + "</td></tr></table>";
                        }
                        catch
                        {; }
                    }
                }

                if ((dc.Address == null) || (dc.Address.MeasureMode == null)) return;
                if (dc.Address.MeasureMode.Mode != TMeasureShowModeType.smNormal) return;

                if (Model.ThresholdChecked)
                {
                    try
                    {
                        Double d = Convert.ToDouble(Model.ThresholdValue);
                        Double v = Convert.ToDouble(dc.Data);
                        if (v < d) e.CellStyle.BackColor = Color.Coral;
                    }
                    catch
                    {; }
                }

                if (Model.ControlsChecked)
                    e.Text = WriteContextMenuButton(e.Text);
            }
        }

        private string WriteContextMenuButton(string name)
        {
            var btn = new Button();
            btn.CssClass = "btn btn-default";
            btn.Text = name;
            btn.UseSubmitBehavior = false;
            btn.Attributes.Add("onclick", "RadarSoft.$('#" + OlapAnalysis.ClientID + "').data('grid').createPopup(event);");
            var sw = new StringWriter();
            var w = new Html32TextWriter(sw);
            btn.RenderControl(w);
            return sw.ToString();
        }

        private Color FromHSB(double H, double S, double B)
        {
            double r = B;
            double g = B;
            double b = B;
            if (S != 0)
            {
                double max = B;
                double dif = B * S / 255f;
                double min = B - dif;

                double h = H * 360f / 255f;

                if (h < 60f)
                {
                    r = max;
                    g = h * dif / 60f + min;
                    b = min;
                }
                else if (h < 120f)
                {
                    r = -(h - 120f) * dif / 60f + min;
                    g = max;
                    b = min;
                }
                else if (h < 180f)
                {
                    r = min;
                    g = max;
                    b = (h - 120f) * dif / 60f + min;
                }
                else if (h < 240f)
                {
                    r = min;
                    g = -(h - 240f) * dif / 60f + min;
                    b = max;
                }
                else if (h < 300f)
                {
                    r = (h - 240f) * dif / 60f + min;
                    g = min;
                    b = max;
                }
                else if (h <= 360f)
                {
                    r = max;
                    g = min;
                    b = -(h - 360f) * dif / 60 + min;
                }
                else
                {
                    r = 0;
                    g = 0;
                    b = 0;
                }
            }

            return Color.FromArgb
                (
                    255,
                    (int)Math.Round(Math.Min(Math.Max(r, 0), 255)),
                    (int)Math.Round(Math.Min(Math.Max(g, 0), 255)),
                    (int)Math.Round(Math.Min(Math.Max(b, 0), 255))
                    );
        }
    }

    public class ChartSample : RSample
    {
        protected ChartSample(SamplesModel samplesModel)
            : base(samplesModel)
        {
        }

        public override void InitSample()
        {
            OlapAnalysis.AnalysisType = AnalysisType.Chart;
            OlapAnalysis.ShowModificationAreas = true;
            OlapAnalysis.ShowLegends = true;
            OlapAnalysis.StructureTreeWidth = new Unit(255, UnitType.Pixel);
            base.InitSample();
            DoActive();
        }
    }

    public class SimpleSales : ChartSample
    {
        public SimpleSales(SamplesModel samplesModel) : base(samplesModel)
        {
        }

        public override void InitSample()
        {
            base.InitSample();

            THierarchy H = OlapAnalysis.Dimensions.FindHierarchyByDisplayName("Date");
            if (H != null) OlapAnalysis.PivotingLast(H, TLayoutArea.laColumn);
            H.Levels[2].Visible = true;

            //+ Search for a measure named "Sales"...
            TMeasure M = OlapAnalysis.Measures.FindByDisplayName("Sales");

            //+ ...and make it visible
            if (M != null) OlapAnalysis.Pivoting(M, TLayoutArea.laRow);

            M.DefineChartMeasureType(SeriesType.Spline, true);

            H = OlapAnalysis.Dimensions.FindHierarchyByDisplayName("Shippers");
            if (H != null) OlapAnalysis.PivotingLast(H, TLayoutArea.laColor);
        }
    }

    public class QuantitySales : ChartSample
    {
        public QuantitySales(SamplesModel samplesModel)
            : base(samplesModel)
        {
        }

        public override void InitSample()
        {
            base.InitSample();
            //+ Search for a measure named "Sales"...
            TMeasure M = OlapAnalysis.Measures.FindByDisplayName("Sales");
            //+ ...and make it visible
            if (M != null) OlapAnalysis.Pivoting(M, TLayoutArea.laRow);

            TMeasure M1 = OlapAnalysis.Measures.FindByDisplayName("Quantity");
            //+ ...and make it visible
            if (M1 != null) OlapAnalysis.Pivoting(M1, TLayoutArea.laColumn);

            THierarchy H = OlapAnalysis.Dimensions.FindHierarchyByDisplayName("Shippers");
            if (H != null) OlapAnalysis.PivotingLast(H, TLayoutArea.laSize);

            H = OlapAnalysis.Dimensions.FindHierarchyByDisplayName("Customers");
            if (H != null) OlapAnalysis.PivotingLast(H, TLayoutArea.laColor);

            OlapAnalysis.SetScale(1.1, 2.1);
        }
    }

    public class SalesByCategories : ChartSample
    {
        public SalesByCategories(SamplesModel samplesModel)
            : base(samplesModel)
        {
        }

        public override void InitSample()
        {
            base.InitSample();
            //+ Search for a measure named "Sales"...
            TMeasure M = OlapAnalysis.Measures.FindByDisplayName("Sales");
            //+ ...and make it visible
            if (M != null)
            {
                OlapAnalysis.Pivoting(M, TLayoutArea.laRow);
                OlapAnalysis.Pivoting(M, TLayoutArea.laColor);
                M.DefineChartMeasureType(SeriesType.Bar);
            }

            THierarchy H = OlapAnalysis.Dimensions.FindHierarchyByDisplayName("Shippers");
            if (H != null) OlapAnalysis.PivotingLast(H, TLayoutArea.laColumn);

            H = OlapAnalysis.Dimensions.FindHierarchyByDisplayName("Categories");
            if (H != null) OlapAnalysis.PivotingLast(H, TLayoutArea.laColumn);
        }
    }

    public class Compare : ChartSample
    {
        public Compare(SamplesModel samplesModel)
            : base(samplesModel)
        {
        }

        public override void InitSample()
        {
            base.InitSample();
            THierarchy H = OlapAnalysis.Dimensions.FindHierarchyByDisplayName("Date");
            if (H != null) OlapAnalysis.PivotingLast(H, TLayoutArea.laColumn);
            H.Levels[2].Visible = true;

            //+ Search for a measure named "Sales"...
            TMeasure M = OlapAnalysis.Measures.FindByDisplayName("Sales");
            //+ ...and make it visible
            if (M != null)
            {
                OlapAnalysis.Pivoting(M, TLayoutArea.laRow);
                M.DefineChartMeasureType(SeriesType.StackedArea);
            }

            H = OlapAnalysis.Dimensions.FindHierarchyByDisplayName("Shippers");
            if (H != null) OlapAnalysis.PivotingLast(H, TLayoutArea.laColor);

            //+ Search for a measure named "Sales"...
            M = OlapAnalysis.Measures.FindByDisplayName("Quantity");
            //+ ...and make it visible
            if (M != null)
            {
                OlapAnalysis.Pivoting(M, TLayoutArea.laRow);
                M.DefineChartMeasureType(SeriesType.Column);
            }
        }
    }

    public class Shapes : ChartSample
    {
        public Shapes(SamplesModel samplesModel)
            : base(samplesModel)
        {
        }

        public override void InitSample()
        {
            base.InitSample();
            THierarchy H = OlapAnalysis.Dimensions.FindHierarchyByDisplayName("Date");
            if (H != null) OlapAnalysis.PivotingLast(H, TLayoutArea.laColumn);

            //+ Search for a measure named "Sales"...
            TMeasure M = OlapAnalysis.Measures.FindByDisplayName("Sales");
            //+ ...and make it visible
            if (M != null) OlapAnalysis.Pivoting(M, TLayoutArea.laRow);

            //+ Search for a measure named "Quantity"...
            M = OlapAnalysis.Measures.FindByDisplayName("Quantity");
            //+ ...and make it visible
            if (M != null) OlapAnalysis.Pivoting(M, TLayoutArea.laColumn);

            M.DefineChartMeasureType(SeriesType.Scatter, true);

            H = OlapAnalysis.Dimensions.FindHierarchyByDisplayName("Categories");
            if (H != null) OlapAnalysis.PivotingLast(H, TLayoutArea.laColor);

            H = OlapAnalysis.Dimensions.FindHierarchyByDisplayName("Quarter");
            if (H != null) OlapAnalysis.PivotingLast(H, TLayoutArea.laShape);
        }
    }

    public class Density : ChartSample
    {
        public Density(SamplesModel samplesModel)
            : base(samplesModel)
        {
        }

        public override void InitSample()
        {
            base.InitSample();
            THierarchy H = OlapAnalysis.Dimensions.FindHierarchyByDisplayName("Year");
            if (H != null) OlapAnalysis.PivotingLast(H, TLayoutArea.laColumn);

            //+ Search for a measure named "Sales"...
            TMeasure M = OlapAnalysis.Measures.FindByDisplayName("Sales");
            //+ ...and make it visible
            if (M != null)
                OlapAnalysis.Pivoting(M, TLayoutArea.laRow);

            H = OlapAnalysis.Dimensions.FindHierarchyByDisplayName("Shippers");
            if (H != null) OlapAnalysis.PivotingLast(H, TLayoutArea.laColumn);

            H = OlapAnalysis.Dimensions.FindHierarchyByDisplayName("Customers");
            if (H != null) OlapAnalysis.PivotingLast(H, TLayoutArea.laDetails);
        }
    }

    public class RSampleCreator : SampleCreator
    {
        public override Sample CreateSample(SamplesModel sampleModel)
        {
            var sample = new RSample(sampleModel);
            sample.CreateOlapAnalysis();
            return sample;
        }
    }

    public class GridSampleCreator : SampleCreator
    {
        public override Sample CreateSample(SamplesModel sampleModel)
        {
            var sample = new GridSample(sampleModel);
            sample.CreateOlapAnalysis();
            return sample;
        }
    }

    public class SimpleSalesCreator : SampleCreator
    {
        public override Sample CreateSample(SamplesModel sampleModel)
        {
            var sample = new SimpleSales(sampleModel);
            sample.CreateOlapAnalysis();
            return sample;
        }
    }

    public class QuantitySalesCreator : SampleCreator
    {
        public override Sample CreateSample(SamplesModel sampleModel)
        {
            var sample = new QuantitySales(sampleModel);
            sample.CreateOlapAnalysis();
            return sample;
        }
    }

    public class SalesByCategoriesCreator : SampleCreator
    {
        public override Sample CreateSample(SamplesModel sampleModel)
        {
            var sample = new SalesByCategories(sampleModel);
            sample.CreateOlapAnalysis();
            return sample;
        }
    }

    public class CompareCreator : SampleCreator
    {
        public override Sample CreateSample(SamplesModel sampleModel)
        {
            var sample = new Compare(sampleModel);
            sample.CreateOlapAnalysis();
            return sample;
        }
    }

    public class ShapesCreator : SampleCreator
    {
        public override Sample CreateSample(SamplesModel sampleModel)
        {
            var sample = new Shapes(sampleModel);
            sample.CreateOlapAnalysis();
            return sample;
        }
    }

    public class DensityCreator : SampleCreator
    {
        public override Sample CreateSample(SamplesModel sampleModel)
        {
            var sample = new Density(sampleModel);
            sample.CreateOlapAnalysis();
            return sample;
        }
    }
}
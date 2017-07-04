using System;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Models;

namespace Helpers
{
    public static class HtmlHelperExtension
    {
        public static HtmlString OlapSamples(this HtmlHelper helper, SamplesModel model)
        {
            return model.Render(); 
        }
    }
}

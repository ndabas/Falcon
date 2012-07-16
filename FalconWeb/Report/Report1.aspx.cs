using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FalconWeb.Reports
{
    public partial class Report1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string reportMethod = Request.QueryString["report"];
            if (!String.IsNullOrEmpty(reportMethod))
            {
                ReportDataSource.SelectMethod = reportMethod + "Report";
            }
        }

        protected void GoButton_Click(object sender, EventArgs e)
        {
            ReportViewer1.LocalReport.Refresh();
        }
    }
}
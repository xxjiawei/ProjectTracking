using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XProjectWPF.Model
{
    public class MQuery
    {
        public DateFilterTypes DateFilterType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BillType { get; set; }
        public string BillNo { get; set; }
        public string FllowMan { get; set; }
        public string ProjectName { get; set; }
        public string CompanyName { get; set; }
        public string[] Type { get; set; }
    }
}

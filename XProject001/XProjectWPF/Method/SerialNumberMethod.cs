using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XProjectWPF.Model;

namespace XProjectWPF.Method
{
    /// <summary>
    /// 流水号生成方法
    /// </summary>
    public class SerialNumberMethod
    {
        public ProjectTrackingEntities m_Entities = new ProjectTrackingEntities();


        public string GetMaxQNumber()
        {
            string startNum = "Q" + DateTime.Today.ToString("yyMM");


            var temp = from p in m_Entities.PT_B_Quotation
                       where p.Quotation_No.StartsWith(startNum)
                       select p;

            foreach (PT_B_Quotation model in temp)
            {
                if (model == null)
                {

                }
            }

            return null;
        }


    }
}

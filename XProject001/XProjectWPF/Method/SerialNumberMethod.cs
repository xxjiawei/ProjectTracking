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

        /// <summary>
        /// 获取新流水号值
        /// </summary>
        /// <returns></returns>
        public string GetMaxQNumber()
        {
            string maxQNumber = string.Empty;
            string startNum = "Q" + DateTime.Today.ToString("yyMM");
            var temp = from p in m_Entities.PT_B_Quotation
                       orderby p.Quotation_No descending
                       where p.Quotation_No.StartsWith(startNum)
                       select p;
            foreach (PT_B_Quotation model in temp)
            {
                string num = model.Quotation_No.Substring(model.Quotation_No.Length - 3);
                maxQNumber = (int.Parse(num) + 1).ToString().PadLeft(3, '0');
                break;
            }
            if (string.IsNullOrEmpty(maxQNumber))
                maxQNumber = "001";

            return startNum + maxQNumber;
        }

        /// <summary>
        /// 获取项目单流水号值
        /// </summary>
        /// <param name="type">前缀</param>
        /// <returns>返回获取到的流水号值</returns>
        public string GetMaxPNumber(string type)
        {
            string maxQNumber = string.Empty;
            string startNum = type + DateTime.Today.ToString("yyMM");
            var temp = from p in m_Entities.PT_B_Project
                       orderby p.Project_No descending
                       where p.Project_No.StartsWith(startNum)
                       select p;
            foreach (PT_B_Project model in temp)
            {
                string num = model.Project_No.Substring(model.Project_No.Length - 3);
                maxQNumber = (int.Parse(num) + 1).ToString().PadLeft(3, '0');
                break;
            }
            if (string.IsNullOrEmpty(maxQNumber))
                maxQNumber = "001";

            return startNum + maxQNumber;
        }


    }
}

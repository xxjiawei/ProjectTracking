using RJ.Common.DBUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XProjectWPF.Model
{
    /// <summary>
    /// 表示单证日期过滤控件的参数维护类
    /// </summary>
    public class MDateFilterParam
    {
        #region 字段

        /// <summary>
        /// 格式化的时间格式
        /// </summary>
        public static readonly String DATE_FORMAT = "yyyy.MM.dd";

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置显示列的唯一主键
        /// </summary>
        public string UUID { get; set; }
        /// <summary>
        /// 获取或设置模块编号
        /// </summary>
        public string ModuleId { get; set; }
        /// <summary>
        /// 获取或设置是否启用自动记忆功能
        /// </summary>
        public bool IsRemember { get; set; }
        /// <summary>
        /// 获取或设置单证日期过滤方式
        /// </summary>
        public DateFilterTypes DateFilterType { get; set; }
        /// <summary>
        /// 获取或设置自定义日期范围的起始日期
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 获取或设置自定义日期范围的结束日期
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 获取或设置用户的唯一标识符(机器码或用户编号)
        /// </summary>
        public string UserSign { get; set; }

        #endregion

        #region 公开方法

        /// <summary>
        /// 获取当前参数格式化的查询语句
        /// </summary>
        /// <param name="dateField">日期数据库字段</param>
        /// <returns>返回格式化的查询语句</returns>
        public string GetFilterSql(string dateField)
        {
            DatabaseTypes myType = DatabaseTypes.SqlServer;
            string strSql = String.Empty;

            switch (this.DateFilterType)
            {
                case DateFilterTypes.Today:
                    strSql = DateTimeScript.Instance.GetFilterSqlByDay(myType, dateField);
                    break;
                case DateFilterTypes.Week:
                    strSql = DateTimeScript.Instance.GetFilterSqlByWeek(myType, dateField);
                    break;
                case DateFilterTypes.Month:
                    strSql = DateTimeScript.Instance.GetFilterSqlByMonth(myType, dateField);
                    break;
                case DateFilterTypes.Custom:
                    strSql = DateTimeScript.Instance.GetFilterSqlByCustom(myType, dateField, this.StartDate, this.EndDate);
                    break;
            }

            return strSql;
        }
        #endregion
    }
}

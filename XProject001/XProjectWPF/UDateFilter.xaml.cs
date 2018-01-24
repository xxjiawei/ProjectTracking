using RJ.XStyle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XProjectWPF.Model;

namespace XProjectWPF
{
    /// <summary>
    /// UDateFilter.xaml 的交互逻辑
    /// </summary>
    public partial class UDateFilter : UserControl
    {
        public UDateFilter()
        {
            InitializeComponent();
            m_CurrentParam = new MDateFilterParam();
        }


        /// <summary>
        /// 表示当前日期过滤控件使用的配置参数
        /// </summary>
        private MDateFilterParam m_CurrentParam;

        #region 公开事件

        /// <summary>
        /// 在时间过滤控件参数变化时触发的事件
        /// </summary>
        public event DateFilterValueChanged ValueChanged;

        #endregion

        #region 公开方法

     
        /// <summary>
        /// 表示获取当前日期过滤控件的参数
        /// </summary>
        /// <returns></returns>
        public MDateFilterParam GetFilterParam()
        {
            return m_CurrentParam;
        }

        #endregion


        #region 界面事件

        /// <summary>
        /// 自定义范围起始日期改变事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_dtp_StartDate_ValueChanged(object sender, EventArgs e)
        {
            if (m_CurrentParam.StartDate.Equals(t_dtp_StartDate.Value)) return;

            m_CurrentParam.StartDate = t_dtp_StartDate.Value;

            if (ValueChanged != null)
                ValueChanged(m_CurrentParam);
        }
        /// <summary>
        /// 自定义范围结束日期改变事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_dtp_EndDate_ValueChanged(object sender, EventArgs e)
        {
            if (m_CurrentParam.EndDate.Equals(t_dtp_EndDate.Value)) return;

            m_CurrentParam.EndDate = t_dtp_EndDate.Value;

            if (ValueChanged != null)
                ValueChanged(m_CurrentParam);
        }
        /// <summary>
        /// 当前日期范围单选按钮选中状态改变时调用的方法
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void OnDateFilterCheckedChanged(object sender, EventArgs e)
        {
            XRadioButton myControl = sender as XRadioButton;
            if (myControl == null) return;
            if (myControl.IsChecked == false) return;

            string type = string.Empty;
            switch (myControl.Name)
            {
                case "t_rdo_Today":
                    type = "Today";
                    break;
                case "t_rdo_Week":
                    type = "Week";
                    break;
                case "t_rdo_Month":
                    type = "Month";
                    break;
                case "t_rdo_All":
                    type = "All";
                    break;
                case "t_rdo_Custom":
                    type = "Custom";
                    break;
            }

            DateFilterTypes myType = (DateFilterTypes)Enum.Parse(typeof(DateFilterTypes), type, true);
            if (myType == DateFilterTypes.Custom)
            {
                t_dtp_StartDate.IsEnabled = true;
                t_dtp_EndDate.IsEnabled = true;
            }
            else
            {
                t_dtp_StartDate.IsEnabled = false;
                t_dtp_EndDate.IsEnabled = false;
            }

            m_CurrentParam.DateFilterType = myType;

            if (ValueChanged != null)
                ValueChanged(m_CurrentParam);
        }
        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化非记忆模式情况下的参数
        /// </summary>
        /// <param name="myParams">日期过滤控件的参数维护类</param>
        private void InitParamsOfNoRemember(MDateFilterParam myParams)
        {
            m_CurrentParam.DateFilterType = myParams.DateFilterType;

            switch (m_CurrentParam.DateFilterType)
            {
                case DateFilterTypes.Today:  t_rdo_Today.IsChecked = true; break;
                case DateFilterTypes.Week: t_rdo_Week.IsChecked = true; break;
                case DateFilterTypes.Month:  t_rdo_Month.IsChecked = true; break;
                case DateFilterTypes.All:  t_rdo_All.IsChecked = true; break;
                case DateFilterTypes.Custom: t_rdo_Custom.IsChecked = true; break;
            }
            m_CurrentParam.StartDate = DateTime.Now;
            m_CurrentParam.EndDate = DateTime.Now;
            t_dtp_StartDate.Value = DateTime.Now;
            t_dtp_EndDate.Value = DateTime.Now;
        }
     

        #endregion
    }
}

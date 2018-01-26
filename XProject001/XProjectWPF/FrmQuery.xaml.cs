using RJ.XStyle;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;
using XProjectWPF.Model;

namespace XProjectWPF
{
    /// <summary>
    /// FrmQuery.xaml 的交互逻辑
    /// </summary>
    public partial class FrmQuery : XBaseForm
    {
        public FrmQuery()
        {
            InitializeComponent();
        }


        public MQuery QueryModel { get; set; }

        private void XBaseForm_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                BindDateSource();

                BindTypeSource();
            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }

        
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void t_cmb_Date_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (t_cmb_Date.SelectedValue.ToString() == "自定义范围")
                {
                    t_dtp_EndDate.IsReadOnly = false;
                    t_dtp_StartDate.IsReadOnly = false;
                }
                else
                {
                    t_dtp_EndDate.IsReadOnly = true;
                    t_dtp_StartDate.IsReadOnly = true;
                    t_dtp_StartDate.Value = DateTime.Parse("1900-01-01");
                    t_dtp_EndDate.Value = DateTime.Parse("1900-01-01");
                }
            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }

          
        }
        /// <summary>
        /// 复选框控制只能选折一个事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_chk_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var checkBoxes = new[] { t_chk_Save, t_chk_EMC, t_chk_Chemis };
                var current = (CheckBox)sender;
                if (current.IsChecked == false)
                {
                    current.IsChecked = true;
                    return;
                }
                foreach (var checkBox in checkBoxes)
                {
                    if (checkBox != current)
                    {
                        checkBox.IsChecked = !current.IsChecked;
                    }
                }
            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }
          
        }
        private void t_cmb_Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (t_cmb_Type.SelectedIndex == 0)
            {
                t_lbl_Type.Content = "报价类型";
                t_lbl_BillNo.Content = "报价单号";
                t_grp_Query.Header = "报价查询条件";
            }
            else
            {
                t_lbl_BillNo.Content = "项目单号";
                t_lbl_Type.Content = "项目类型";
                t_grp_Query.Header = "项目查询条件";
            }
        }

        private void BindTypeSource()
        {
            DataTable myTable = new DataTable();
            myTable.Columns.Add("Type", typeof(string));

            DataRow myRow = myTable.NewRow();
            myRow["Type"] = "报价单";
            myTable.Rows.Add(myRow);

            myRow = myTable.NewRow();
            myRow["Type"] = "项目单";
            myTable.Rows.Add(myRow);
            t_cmb_Type.DisplayMemberPath = "Type";
            t_cmb_Type.SelectedValuePath = "Type";
            t_cmb_Type.ItemsSource = myTable.DefaultView;

            t_cmb_Type.SelectedIndex = 0;
        }

        private void BindDateSource()
        {
            DataTable myTable = new DataTable();
            myTable.Columns.Add("Type", typeof(string));

            DataRow myRow = myTable.NewRow();
            myRow["Type"] = "当天单据";
            myTable.Rows.Add(myRow);

            myRow = myTable.NewRow();
            myRow["Type"] = "当周单据";
            myTable.Rows.Add(myRow);

            myRow = myTable.NewRow();
            myRow["Type"] = "当月单据";
            myTable.Rows.Add(myRow);

            myRow = myTable.NewRow();
            myRow["Type"] = "所有单据";
            myTable.Rows.Add(myRow);

            myRow = myTable.NewRow();
            myRow["Type"] = "自定义范围";
            myTable.Rows.Add(myRow);

            t_cmb_Date.DisplayMemberPath = "Type";
            t_cmb_Date.SelectedValuePath = "Type";
            t_cmb_Date.ItemsSource = myTable.DefaultView;

            t_cmb_Date.SelectedIndex = 3;
        }

        private void t_btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            t_cmb_Date.SelectedIndex = 3;
            t_cmb_Type.SelectedIndex = 0;
            t_txt_BillNo.Text = string.Empty;
            t_txt_FllowMan.Text = string.Empty;
            t_txt_ProjectName.Text = string.Empty;
            t_txt_CompanyName.Text = string.Empty;
            t_chk_Save.IsChecked = true;
            t_chk_EMC.IsChecked = false;
            t_chk_Chemis.IsChecked = false;
        }

        private void t_btn_Query_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MQuery myModel = new MQuery();

                switch (t_cmb_Date.SelectedValue.ToString())
                {
                    case "当天单据":
                        myModel.DateFilterType = DateFilterTypes.Today;
                        break;
                    case "当周单据":
                        myModel.DateFilterType = DateFilterTypes.Week;
                        break;
                    case "当月单据":
                        myModel.DateFilterType = DateFilterTypes.Month;
                        break;
                    case "所有单据":
                        myModel.DateFilterType = DateFilterTypes.All;
                        break;
                    case "自定义范围":
                        myModel.DateFilterType = DateFilterTypes.Custom;
                        break;
                }
                if (t_cmb_Type.SelectedIndex == 0)
                    myModel.BillType = "Q";
                else
                    myModel.BillType = "P";

                myModel.BillNo = t_txt_BillNo.Text.Trim();
                myModel.StartDate = t_dtp_StartDate.Value;
                myModel.EndDate = t_dtp_EndDate.Value;
                myModel.FllowMan = t_txt_FllowMan.Text;
                myModel.ProjectName = t_txt_ProjectName.Text;
                myModel.CompanyName = t_txt_CompanyName.Text;
                if (t_chk_Save.IsChecked == true)
                    myModel.Type = "安全";
                if (t_chk_EMC.IsChecked == true)
                    myModel.Type = "EMC";
                if (t_chk_Chemis.IsChecked == true)
                    myModel.Type = "化学";
                QueryModel = myModel;
                this.Close();
            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }

        
        }
        private void t_btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

using RJ.XStyle;
using RJ.XStyle.GridEx;
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
    /// FrmProject.xaml 的交互逻辑
    /// </summary>
    public partial class FrmProject : XBaseForm
    {
        public FrmProject()
        {
            InitializeComponent();
        }

        public bool IsNew { get; set; }

        public MQuotation22 QuotationModel { get; set; }


        private DataGridStyleConfig m_GirdStyleConfig;

        public DataGridStyleConfig GirdStyleConfig
        {
            get
            {
                if (m_GirdStyleConfig == null)
                {
                    m_GirdStyleConfig = new DataGridStyleConfig();

                    m_GirdStyleConfig.Add("已收金额", "已收金额", 80);
                    m_GirdStyleConfig.Add("支付时间", "支付时间", 80);
                    m_GirdStyleConfig.Add("收款人", "收款人", 80);
                    m_GirdStyleConfig.Add("是否开票", "是否开票", 60);
                    m_GirdStyleConfig.Add("发票金额", "发票金额", 80);
                    m_GirdStyleConfig.Add("发票号", "发票号", 100);
                    m_GirdStyleConfig.Add("发票时间", "发票时间", 80);
                    m_GirdStyleConfig.Add("备注信息", "备注信息", 260);
                }
                return m_GirdStyleConfig;
            }
        }


        private void XBaseForm_Loaded(object sender, RoutedEventArgs e)
        {
            if (QuotationModel == null) return;
            t_txt_QuotationNo.Text = QuotationModel.QuotationNo;
            t_txt_FollowMan.Text = QuotationModel.FollowMan;
            t_txt_CompanyName.Text = QuotationModel.CompanyName;
            t_txt_CompanyAddress.Text = QuotationModel.CompanyAddress;
            t_txt_ContactMan.Text = QuotationModel.ContactMan;
            t_txt_Email.Text = QuotationModel.Email;
            t_txt_ProjectName.Text = QuotationModel.ProjectName;
            t_txt_Tel.Text = QuotationModel.Tel;
            t_txt_Price.Text = QuotationModel.Price;
            t_dtp_QuotationDate.Value = QuotationModel.QuotationDate;
  
  

            //表格赋值
            DataTable myTable = new DataTable();
            myTable.Columns.Add("已收金额");
            myTable.Columns.Add("支付时间");
            myTable.Columns.Add("收款人");
            myTable.Columns.Add("是否开票");
            myTable.Columns.Add("发票金额");
            myTable.Columns.Add("发票号");
            myTable.Columns.Add("发票时间");
            myTable.Columns.Add("备注信息");

            if(IsNew)
            {
                myTable.Rows.Add("5000", "2018-1-1", "何显俊", "是", "5000", "I01608658", "2018-1-1", "客户预付5000元，约定1月中旬支付剩余钱款。");
                myTable.Rows.Add("45000", "2018-1-12", "何显俊", "是", "35000", "I01608622", "2018-1-12", "");

            
            }

            GirdStyleConfig.ItemsSource = myTable.DefaultView;
            t_dge_CustomBill.StyleConfig = GirdStyleConfig;
        }

        private void t_tsb_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void t_tsb_Save_Click(object sender, RoutedEventArgs e)
        {
            t_txt_ProjectNo.Text = "XM20180112001";
            MessageBox.Show("保存成功");
        }
    }
}

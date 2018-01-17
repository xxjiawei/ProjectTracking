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
using System.Windows.Shapes;
using XProjectWPF.Method;
using XProjectWPF.Model;

namespace XProjectWPF
{
    /// <summary>
    /// FrmQuotation.xaml 的交互逻辑
    /// </summary>
    public partial class FrmQuotation : XBaseForm
    {
        public FrmQuotation()
        {
            InitializeComponent();
        }

        public MQuotation22 QuotationModel { get; set; }

        /// <summary>
        /// 数据库操作类
        /// </summary>
        private ProjectTrackingEntities  m_Entities = new ProjectTrackingEntities();


        private SerialNumberMethod m_SerialNumberMethod = new SerialNumberMethod();

        /// <summary>
        /// 保存按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_tsb_Save_Click(object sender, RoutedEventArgs e)
        {
            //生成流水号
            string maxNo = m_SerialNumberMethod.GetMaxQNumber();

            if (maxNo != null)
            {

            }
            return;

            PT_B_Quotation myModel = new PT_B_Quotation()
            {
                Quotation_No = "Q1801002",
                Quotation_Date = t_dtp_QuotationDate.Value,
                Follow_Man = t_txt_FollowMan.Text,
                Product_Model = t_txt_ProductModel.Text,
                Project_Name = t_txt_ProjectName.Text,
                Price = double.Parse(t_txt_Price.Text),
                Is_Tax = t_chk_IsTax.IsChecked == true ? "1" : "0",
                Quotation_Type = t_rad_Safe.IsChecked == true ? "安全" : "化学",
                Company_Name = t_txt_CompanyName.Text,
                Company_Address = t_txt_CompanyAddress.Text,
                Contact_Man = t_txt_ContactMan.Text,
                Tel = t_txt_Tel.Text,
                Email = t_txt_Email.Text,
                Fax = t_txt_Fax.Text,
                Remark = t_txt_Remark.Text,
                Bill_Status = "1",
                Oper_Time = DateTime.Today
            };
            m_Entities.PT_B_Quotation.Add(myModel);
            m_Entities.SaveChanges();

            XMessageBox.Enter("保存成功", this);
        }

        private void XBaseForm_Loaded(object sender, RoutedEventArgs e)
        {
            t_dtp_QuotationDate.Value = DateTime.Today;

          

            if (QuotationModel == null) return;
            t_txt_QuotationNo.Text = QuotationModel.QuotationNo;
            t_dtp_QuotationDate.Value = QuotationModel.QuotationDate;
            t_txt_FollowMan.Text = QuotationModel.FollowMan;
            t_txt_ProjectName.Text = QuotationModel.ProjectName;
            t_txt_Price.Text = QuotationModel.Price;
            t_txt_CompanyName.Text = QuotationModel.CompanyName;
            t_txt_CompanyAddress.Text = QuotationModel.CompanyAddress;
            t_txt_Tel.Text = QuotationModel.Tel;
            t_txt_Email.Text = QuotationModel.Email;
            t_txt_ContactMan.Text = QuotationModel.ContactMan;
        }

        private void t_tsb_CreateProject_Click(object sender, RoutedEventArgs e)
        {
            if(t_txt_QuotationNo.Text == "新单")
            {
                XMessageBox.Enter("当前单据尚未保存，请保存后再操作！",this);
                return;
            }
            MQuotation22 quotationModel = new MQuotation22();
            quotationModel.QuotationNo = t_txt_QuotationNo.Text;
            quotationModel.QuotationDate = t_dtp_QuotationDate.Value;
            quotationModel.FollowMan = t_txt_FollowMan.Text;
            quotationModel.ProjectName = t_txt_ProjectName.Text;
            quotationModel.Price = t_txt_Price.Text;
            quotationModel.CompanyName = t_txt_CompanyName.Text;
            quotationModel.CompanyAddress = t_txt_CompanyAddress.Text;
            quotationModel.Tel = t_txt_Tel.Text;
            quotationModel.Email = t_txt_Email.Text;
            quotationModel.ContactMan = t_txt_ContactMan.Text;

            this.Visibility = Visibility.Hidden;
            FrmProject myForm = new FrmProject();
            myForm.QuotationModel = quotationModel;
            myForm.ShowDialog();
            this.Close();
        }

        private void t_tsb_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

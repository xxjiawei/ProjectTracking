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

        public MQuotation QuotationModel { get; set; }

        private void t_tsb_Save_Click(object sender, RoutedEventArgs e)
        {
            t_txt_QuotationNo.Text = "BJ20180112004";

            XMessageBox.Enter("保存成功", this);
        }

        private void XBaseForm_Loaded(object sender, RoutedEventArgs e)
        {
            t_txt_QuotationDate.Value = DateTime.Today;

            if (QuotationModel == null) return;
            t_txt_QuotationNo.Text = QuotationModel.QuotationNo;
            t_txt_QuotationDate.Value = QuotationModel.QuotationDate;
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
            MQuotation quotationModel = new MQuotation();
            quotationModel.QuotationNo = t_txt_QuotationNo.Text;
            quotationModel.QuotationDate = t_txt_QuotationDate.Value;
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

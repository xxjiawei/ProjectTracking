using RJ.XStyle;
using RJ.XStyle.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            RegisterEvent();

        }

        public MQuotation22 QuotationModel { get; set; }
        /// <summary>
        /// 标识是否已修改
        /// </summary>
        private bool m_IsModify = false;
        /// <summary>
        /// 标识是否已加载完成
        /// </summary>
        private bool m_IsLoad = true;

        /// <summary>
        /// 对象锁
        /// </summary>
        private Object thisLock = new Object();
        /// <summary>
        /// 获取或设置报价单对象
        /// </summary>
        public PT_B_Quotation PTBQuotation { get; set; }

        /// <summary>
        /// 数据库操作类
        /// </summary>
        private ProjectTrackingEntities  m_Entities = new ProjectTrackingEntities();
        /// <summary>
        /// 流水号生成规则类
        /// </summary>
        private SerialNumberMethod m_SerialNumberMethod = new SerialNumberMethod();

        /// <summary>
        /// 保存按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_tsb_Save_Click(object sender, RoutedEventArgs e)
        {
            lock (thisLock)
            {
                if (string.IsNullOrEmpty(PTBQuotation.Bill_Status))
                {
                    string newQuotationNo = m_SerialNumberMethod.GetMaxQNumber();
                    SaveModel(newQuotationNo);
                    m_Entities.PT_B_Quotation.Add(PTBQuotation);
                    t_txt_QuotationNo.Text = newQuotationNo;
                    t_tslStateText.Text = "已入库";
                }
                else
                    SaveModel(t_txt_QuotationNo.Text);
                m_Entities.SaveChanges();

            }
            XMessageBox.Enter("保存成功", this);
            m_IsModify = false;
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void XBaseForm_Loaded(object sender, RoutedEventArgs e)
        {
            if (PTBQuotation == null)
                CreateNewQuotationModel();
            LoadControlsValue();

            Thread myThread = new Thread(SetMenuEnabel);
            myThread.IsBackground = true;
            myThread.Start();
        }
        /// <summary>
        /// 设置菜单栏是否可用，解决双击表格过快直接点到按钮触发按钮事件问题
        /// </summary>
        private void SetMenuEnabel()
        {
            Thread.Sleep(500);
            Dispatcher.BeginInvoke(new Action(delegate
            {
                t_meu_Menu.IsEnabled = true;
            }));
        }
        /// <summary>
        /// 设置新报价单对象
        /// </summary> 
        private void CreateNewQuotationModel()
        {
            PTBQuotation = new PT_B_Quotation();
            PTBQuotation.Quotation_No = "新单";
            PTBQuotation.Quotation_Date = DateTime.Today;
        }
        /// <summary>
        /// 加载报价单对象值
        /// </summary>
        private void LoadControlsValue()
        {
            t_txt_QuotationNo.Text = PTBQuotation.Quotation_No;
            t_dtp_QuotationDate.Value = PTBQuotation.Quotation_Date;
            t_txt_FollowMan.Text = PTBQuotation.Follow_Man;
            t_txt_ProductModel.Text = PTBQuotation.Product_Model;
            t_txt_ProjectName.Text = PTBQuotation.Project_Name;
            t_txt_Price.Text = PTBQuotation.Price.ToString();
            t_chk_IsTax.IsChecked = PTBQuotation.Is_Tax == "1" ? true : false;
            t_rad_Safe.IsChecked = PTBQuotation.Quotation_Type != "化学" ? true : false;
            t_rad_Chemistry.IsChecked = PTBQuotation.Quotation_Type == "化学" ? true : false;
            t_txt_CycleTime.Text = PTBQuotation.Cycle_Time;
            t_txt_CompanyName.Text = PTBQuotation.Company_Name;
            t_txt_CompanyAddress.Text = PTBQuotation.Company_Address;
            t_txt_Tel.Text = PTBQuotation.Tel;
            t_txt_Email.Text = PTBQuotation.Email;
            t_txt_ContactMan.Text = PTBQuotation.Contact_Man;
            t_txt_Fax.Text = PTBQuotation.Fax;
            t_txt_Remark.Text = PTBQuotation.Remark;
            if (PTBQuotation.Bill_Status == "R")
                t_tslStateText.Text = "回收站";
            else if (PTBQuotation.Bill_Status == "A")
                t_tslStateText.Text = "归档";
            else if (PTBQuotation.Bill_Status == "1")
                t_tslStateText.Text = "已入库";
            else
                t_tslStateText.Text = "新建";
            m_IsLoad = false;
        }

        private void SaveModel(string newQuotationNo)
        {
            PTBQuotation.Quotation_No = newQuotationNo;
            PTBQuotation.Quotation_Date = t_dtp_QuotationDate.Value;
            PTBQuotation.Follow_Man = t_txt_FollowMan.Text;
            PTBQuotation.Product_Model = t_txt_ProductModel.Text;
            PTBQuotation.Project_Name = t_txt_ProjectName.Text;
            PTBQuotation.Price = string.IsNullOrEmpty(t_txt_Price.Text) ? 0 : double.Parse(t_txt_Price.Text);
            PTBQuotation.Is_Tax = t_chk_IsTax.IsChecked == true ? "1" : "0";
            PTBQuotation.Quotation_Type = t_rad_Safe.IsChecked == true ? "安全" : "化学";
            PTBQuotation.Cycle_Time = t_txt_CycleTime.Text;
            PTBQuotation.Company_Name = t_txt_CompanyName.Text;
            PTBQuotation.Company_Address = t_txt_CompanyAddress.Text;
            PTBQuotation.Contact_Man = t_txt_ContactMan.Text;
            PTBQuotation.Tel = t_txt_Tel.Text;
            PTBQuotation.Email = t_txt_Email.Text;
            PTBQuotation.Fax = t_txt_Fax.Text;
            PTBQuotation.Remark = t_txt_Remark.Text;
            PTBQuotation.Bill_Status = "1";
            PTBQuotation.Oper_Time = DateTime.Now;

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
        /// <summary>
        /// 关闭窗口事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_tsb_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 新建按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_tsb_New_Click(object sender, RoutedEventArgs e)
        {
            if (m_IsModify)
            {
                MessageResult myResult = XMessageBox.Ask("当前单据尚未保存，是否继续？", 370);
                if (myResult == MessageResult.Yes)
                {
                    //设置新报检单对象
                    CreateNewQuotationModel();
                    //对窗体进行赋值
                    LoadControlsValue();
                }
            }
            else
            {
                //设置新报检单对象
                CreateNewQuotationModel();
                //对窗体进行赋值
                LoadControlsValue();
            }
        }
        /// <summary>
        /// 事件注册
        /// </summary>
        private void RegisterEvent()
        {
            RegisterEvent(t_grd_Quotation.Children);
            RegisterEvent(t_grd_Company.Children);
        }

        /// <summary>
        /// 遍历界面中的所有控件
        /// </summary>
        /// <param name="uiControls"></param>
        private void RegisterEvent(UIElementCollection uiControls)
        {
            foreach (UIElement element in uiControls)
            {
                if (element is XTextBox)
                {
                    (element as XTextBox).TextChanged += myTextChanged;
                }
                else if (element is XDatePicker)
                {
                    (element as XDatePicker).ValueChanged += myValueChanged;
                }
                else if (element is XCheckBox)
                {
                    (element as XCheckBox).Checked += myChecked;
                }
                else if (element is Grid)
                {
                    this.RegisterEvent((element as Grid).Children);
                }
                else if (element is StackPanel)
                {
                    this.RegisterEvent((element as StackPanel).Children);
                }
            }
        }

        private void myChecked(object sender, RoutedEventArgs e)
        {
            if (!m_IsLoad)
                m_IsModify = true;
        }

        private void myValueChanged(object sender, RoutedEventArgs e)
        {
            if (!m_IsLoad)
                m_IsModify = true;
        }

        private void myTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!m_IsLoad)
                m_IsModify = true;
        }

        /// <summary>
        /// 复制按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_tsb_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (m_IsModify)
            {
                XMessageBox.Warning("当前单据尚未保存，请保存后再操作！", this);
                return;
            }
            PT_B_Quotation copyModel = new PT_B_Quotation();
            copyModel.Quotation_No = "新单";
            copyModel.Quotation_Date = PTBQuotation.Quotation_Date;
            copyModel.Follow_Man = PTBQuotation.Follow_Man;
            copyModel.Product_Model = PTBQuotation.Product_Model;
            copyModel.Project_Name = PTBQuotation.Project_Name;
            copyModel.Price = PTBQuotation.Price;
            copyModel.Is_Tax = PTBQuotation.Is_Tax;
            copyModel.Quotation_Type = PTBQuotation.Quotation_Type;
            copyModel.Cycle_Time = PTBQuotation.Cycle_Time;
            copyModel.Company_Name = PTBQuotation.Company_Name;
            copyModel.Company_Address = PTBQuotation.Company_Address;
            copyModel.Contact_Man = PTBQuotation.Contact_Man;
            copyModel.Tel = PTBQuotation.Tel;
            copyModel.Email = PTBQuotation.Email;
            copyModel.Fax = PTBQuotation.Fax;
            copyModel.Remark = PTBQuotation.Remark;
            copyModel.Oper_Time = DateTime.Now;

            PTBQuotation = copyModel;
            LoadControlsValue();
        }
    }
}

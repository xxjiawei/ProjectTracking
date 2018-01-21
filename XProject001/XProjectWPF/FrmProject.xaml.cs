using RJ.XStyle;
using RJ.XStyle.GridEx;
using RJ.XStyle.Model;
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
using XProjectWPF.Method;
using XProjectWPF.Model;

namespace XProjectWPF
{
    /// <summary>
    /// FrmProject.xaml 的交互逻辑
    /// </summary>
    public partial class FrmProject : XBaseForm
    {
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmProject()
        {
            InitializeComponent();
            RegisterEvent();
        }
        #endregion


        #region 字段

        /// <summary>
        /// 数据库操作类
        /// </summary>
        private ProjectTrackingEntities m_Entities = new ProjectTrackingEntities();
        /// <summary>
        /// 当前项目对象
        /// </summary>
        public PT_B_Project PTBProject { get; set; }
        /// <summary>
        /// 获取或设置是否客户表格行切换加载
        /// </summary>
        private bool m_IsCustomerGridLoad = false;
        /// <summary>
        /// 获取或设置是否客户表格行切换加载
        /// </summary>
        private bool m_IsAgencyGridLoad = false;
        /// <summary>
        /// 获取或设置是否客户表格行切换加载
        /// </summary>
        private bool m_IsLabGridLoad = false;
        /// <summary>
        /// 获取或设置是否客户表格行切换加载
        /// </summary>
        private bool m_IsOtherGridLoad = false;
        /// <summary>
        /// 获取或设置当前客户资金往来信息集合
        /// </summary>
        private List<PT_B_Project_Customer> m_CustomerList = new List<PT_B_Project_Customer>();
        /// <summary>
        /// 获取或设置当前机构资金往来信息集合
        /// </summary>
        private List<PT_B_Project_Agency> m_AgencyList = new List<PT_B_Project_Agency>();
        /// <summary>
        /// 获取或设置当前外包资金往来信息集合
        /// </summary>
        private List<PT_B_Project_Lab> m_LabList = new List<PT_B_Project_Lab>();
        /// <summary>
        /// 获取或设置当前其他资金往来信息集合
        /// </summary>
        private List<PT_B_Project_Other> m_OtherList = new List<PT_B_Project_Other>();
        /// <summary>
        /// 流水号生成规则类
        /// </summary>
        private SerialNumberMethod m_SerialNumberMethod = new SerialNumberMethod();
        /// <summary>
        /// 标识是否已修改
        /// </summary>
        private bool m_IsModify = false;
        /// <summary>
        /// 标识是否已加载完成
        /// </summary>
        private bool m_IsLoad = true;
        #endregion

        #region 界面事件


        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender">事件参数</param>
        /// <param name="e">事件对象</param>
        private void XBaseForm_Loaded(object sender, RoutedEventArgs e)
        {
            //加载控件值
            LoadData();
            //计算已收客户资金总和
            SumPaymentReceivable();
            //计算已付机构资金总和
            SumAgencyAccountsPrepaid();
            //计算已付外包资金总和
            SumLabAccountsPrepaid();
            //计算已付其他资金总和
            SumOtherAccountsPrepaid();
            m_IsLoad = false;
        }
        /// <summary>
        /// 当前报价值改变事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_txt_Price_TextChanged(object sender, TextChangedEventArgs e)
        {
            t_txt_AccountReceivable.Text = t_txt_Price.Text;

            //ValidateIsAllAgency();
            //ValidateIsAllLab();
            //ValidateIsOtherLab();
        }
        /// <summary>
        /// 复选框控制只能选折一个事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_chk_Save_Click(object sender, RoutedEventArgs e)
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
        private void t_tsb_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 保存按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_tsb_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveMethod();
            XMessageBox.Enter("保存成功", this);
            m_IsModify = false;
        }
        /// <summary>
        /// 保存调用方法
        /// </summary>
        private void SaveMethod()
        {
            //保存基本信息
            if (string.IsNullOrEmpty(PTBProject.Bill_Status))
            {
                string type = string.Empty;

                if (t_chk_Save.IsChecked == true)
                    type = "S";
                if (t_chk_Chemis.IsChecked == true)
                    type = "E";
                if (t_chk_EMC.IsChecked == true)
                    type = "C";

                string newProjectNo = m_SerialNumberMethod.GetMaxPNumber(type);
                SaveBaseModel(newProjectNo);
                m_Entities.PT_B_Project.Add(PTBProject);
                t_txt_ProjectNo.Text = newProjectNo;
                t_tslStateText.Text = "已入库";
            }
            else
            {
                SaveBaseModel(t_txt_ProjectNo.Text);
                m_Entities.Entry(PTBProject).State = System.Data.Entity.EntityState.Modified;
            }
            m_Entities.SaveChanges();
            //保存表格信息
            SaveCustomer();
            SaveAgency();
            SaveLab();
            SaveOther();
        }
        /// <summary>
        /// 保存基本信息
        /// </summary>
        /// <param name="newProjectNo">项目号</param>
        private void SaveBaseModel(string newProjectNo)
        {
            PTBProject.Project_No = newProjectNo;
            PTBProject.Quotation_No = t_txt_QuotationNo.Text;
            PTBProject.Follow_Man = t_txt_FollowMan.Text;

            if (t_chk_Save.IsChecked == true)
                PTBProject.Project_Type = "安全";
            if (t_chk_Chemis.IsChecked == true)
                PTBProject.Project_Type = "化学";
            if (t_chk_EMC.IsChecked == true)
                PTBProject.Project_Type = "EMC";

            PTBProject.Company_Name = t_txt_CompanyName.Text;
            PTBProject.Contact_Man = t_txt_ContactMan.Text;

            PTBProject.Tel = t_txt_Tel.Text;
            PTBProject.Company_Address = t_txt_CompanyAddress.Text;
            PTBProject.Fax = t_txt_Fax.Text;
            PTBProject.Email = t_txt_Email.Text;
            PTBProject.Project_Name = t_txt_ProjectName.Text;
            PTBProject.Product_Model = t_txt_ProductModel.Text;
            PTBProject.Cycle_Time = t_txt_CycleTime.Text;
            PTBProject.Price = t_txt_Price.Text;

            PTBProject.Is_Tax = t_chk_IsTax.IsChecked == true ? "是" : "否";
            PTBProject.Quotation_Date = t_dtp_QuotationDate.Value;
            PTBProject.Remark = t_txt_Remark.Text;
            //应收客户账款
            PTBProject.Account_Receivable = t_txt_AccountReceivable.Text;
            PTBProject.Payment_Receivable = t_txt_PaymentReceivable.Text;
            PTBProject.Un_Account_Receivable = t_txt_UnAccountReceivable.Text;
            PTBProject.Is_All_Customer = t_chk_IsAllCustomer.IsChecked == true ? "是" : "否";
            //应付机构账款
            PTBProject.Agency_Account_Payable = t_txt_AgencyAccountPayable.Text;
            PTBProject.Agency_Accounts_Prepaid = t_txt_AgencyAccountsPrepaid.Text;
            PTBProject.Un_Agency_Account_Payable = t_txt_UnAgencyAccountPayable.Text;
            PTBProject.Is_All_Agency = t_chk_IsAllAgency.IsChecked == true ? "是" : "否";
            //应付外包账款
            PTBProject.Lab_Account_Payable = t_txt_LabAccountPayable.Text;
            PTBProject.Lab_Accounts_Prepaid = t_txt_LabAccountsPrepaid.Text;
            PTBProject.Un_Lab_Account_Payable = t_txt_UnLabAccountPayable.Text;
            PTBProject.Is_All_Lab = t_chk_IsAllLab.IsChecked == true ? "是" : "否";
            //其他账款信息
            PTBProject.Other_Account = t_txt_OtherAccount.Text;
            PTBProject.Other_Pad_Account = t_txt_OtherPadAccount.Text;
            PTBProject.Un_Other_Account = t_txt_UnOtherAccount.Text;
            PTBProject.Is_All_Other = t_chk_IsAllOther.IsChecked == true ? "是" : "否";
            //利润与垫付信息
            PTBProject.Profits = t_txt_Profits.Text;
            PTBProject.Now_Profits = t_txt_NowProfits.Text;
            PTBProject.Pads_Money = t_txt_PadsMoney.Text;
            PTBProject.Is_Pads = t_chk_IsPads.IsChecked == true ? "是" : "否";

            PTBProject.Bill_Status = "1";
            PTBProject.Oper_Time = DateTime.Now;
        }
        /// <summary>
        /// 保存客户流程
        /// </summary>
        private void SaveCustomer()
        {
            var customer = from p in m_Entities.PT_B_Project_Customer
                           where p.Project_No == PTBProject.Project_No
                           orderby p.Seq_No
                           select p;
            foreach (PT_B_Project_Customer myModel in customer)
            {
                m_Entities.PT_B_Project_Customer.Remove(myModel);
            }
            m_Entities.SaveChanges();
            int seqNo = 1;
            foreach (PT_B_Project_Customer myModel in m_CustomerList)
            {
                if (string.IsNullOrEmpty(myModel.Customer_Pays_Id))
                    myModel.Customer_Pays_Id = Guid.NewGuid().ToString("N");
                myModel.Project_No = PTBProject.Project_No;
                myModel.Seq_No = seqNo;
                seqNo++;
                m_Entities.PT_B_Project_Customer.Add(myModel);
            }
            m_Entities.SaveChanges();
        }
        /// <summary>
        /// 保存机构流程
        /// </summary>
        private void SaveAgency()
        {
            var agency = from p in m_Entities.PT_B_Project_Agency
                         where p.Project_No == PTBProject.Project_No
                           orderby p.Seq_No
                           select p;
            foreach (PT_B_Project_Agency myModel in agency)
            {
                m_Entities.PT_B_Project_Agency.Remove(myModel);
            }
            m_Entities.SaveChanges();
            int seqNo = 1;
            foreach (PT_B_Project_Agency myModel in m_AgencyList)
            {
                if (string.IsNullOrEmpty(myModel.Agency_Pays_Id))
                    myModel.Agency_Pays_Id = Guid.NewGuid().ToString("N");
                myModel.Project_No = PTBProject.Project_No;
                myModel.Seq_No = seqNo;
                seqNo++;
                m_Entities.PT_B_Project_Agency.Add(myModel);
            }
            m_Entities.SaveChanges();
        }
        /// <summary>
        /// 保存外包流程
        /// </summary>
        private void SaveLab()
        {
            var lab = from p in m_Entities.PT_B_Project_Lab
                           where p.Project_No == PTBProject.Project_No
                           orderby p.Seq_No
                           select p;
            foreach (PT_B_Project_Lab myModel in lab)
            {
                m_Entities.PT_B_Project_Lab.Remove(myModel);
            }
            m_Entities.SaveChanges();
            int seqNo = 1;
            foreach (PT_B_Project_Lab myModel in m_LabList)
            {
                if (string.IsNullOrEmpty(myModel.Lab_Pays_Id))
                    myModel.Lab_Pays_Id = Guid.NewGuid().ToString("N");
                myModel.Project_No = PTBProject.Project_No;
                myModel.Seq_No = seqNo;
                seqNo++;
                m_Entities.PT_B_Project_Lab.Add(myModel);
            }
            m_Entities.SaveChanges();
        }
        /// <summary>
        /// 保存其他资金流程
        /// </summary>
        private void SaveOther()
        {
            var other = from p in m_Entities.PT_B_Project_Other
                           where p.Project_No == PTBProject.Project_No
                           orderby p.Seq_No
                           select p;
            foreach (PT_B_Project_Other myModel in other)
            {
                m_Entities.PT_B_Project_Other.Remove(myModel);
            }
            m_Entities.SaveChanges();
            int seqNo = 1;
            foreach (PT_B_Project_Other myModel in m_OtherList)
            {
                if (string.IsNullOrEmpty(myModel.Other_Pays_Id))
                    myModel.Other_Pays_Id = Guid.NewGuid().ToString("N");
                myModel.Project_No = PTBProject.Project_No;
                myModel.Seq_No = seqNo;
                seqNo++;
                m_Entities.PT_B_Project_Other.Add(myModel);
            }
            m_Entities.SaveChanges();
        }


        #region 添加与删除按钮事件

        /// <summary>
        /// 客户信息添加按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_btn_CustomerAdd_Click(object sender, RoutedEventArgs e)
        {
            if (t_dge_CustomBill.Items.Count == 0)
                SetCustomerReadOnly(false);

            //添加新对象
            PT_B_Project_Customer newModel = new PT_B_Project_Customer();
            newModel.Customer_Date = DateTime.Today;
            newModel.Is_Customer_Inv = "否";
            m_CustomerList.Add(newModel);
            t_dge_CustomBill.Items.Refresh();
            t_dge_CustomBill.SelectedIndex = m_CustomerList.Count - 1;
            t_dge_CustomBill.Focus();
        }
        /// <summary>
        /// 客户信息删除按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_btn_CustomerDelete_Click(object sender, RoutedEventArgs e)
        {
            if (t_dge_CustomBill.Items.Count < 1)
            {
                XMessageBox.Warning("请选择需要删除的记录行!", this);
                return;
            }

            PT_B_Project_Customer myModel = t_dge_CustomBill.SelectedItem as PT_B_Project_Customer;
            if (myModel == null) return;

            int myIndex = t_dge_CustomBill.SelectedIndex;
            MessageResult myResult = XMessageBox.Ask("确定要删除当前数据吗？", this);
            if (myResult == MessageResult.No) return;

            m_CustomerList.Remove(myModel);

            t_dge_CustomBill.Items.Refresh();
            t_dge_CustomBill_SelectionChanged(null, null);

            if (t_dge_CustomBill.Items.Count == 0)
            {
                SetCustomerReadOnly(true);
            }

            if (myIndex == 0)
                myIndex++;
            t_dge_CustomBill.SelectedIndex = myIndex - 1;
            t_dge_CustomBill.Focus();

            //计算已收客户资金总和
            SumPaymentReceivable();
            //校验开票
            ValidateIsAllCustomer();
        }
        /// <summary>
        /// 机构信息添加按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_btn_AgencyAdd_Click(object sender, RoutedEventArgs e)
        {
            if (t_dge_Agency.Items.Count == 0)
                SetAgencyReadOnly(false);

            //添加新对象
            PT_B_Project_Agency newModel = new PT_B_Project_Agency();
            newModel.Agency_Date = DateTime.Today;
            newModel.Is_Agency_Inv = "否";
            m_AgencyList.Add(newModel);
            t_dge_Agency.Items.Refresh();
            t_dge_Agency.SelectedIndex = m_AgencyList.Count - 1;
            t_dge_Agency.Focus();
        }
        /// <summary>
        /// 机构信息删除按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_btn_AgencyDelete_Click(object sender, RoutedEventArgs e)
        {
            if (t_dge_Agency.Items.Count < 1)
            {
                XMessageBox.Warning("请选择需要删除的记录行!", this);
                return;
            }

            PT_B_Project_Agency myModel = t_dge_Agency.SelectedItem as PT_B_Project_Agency;
            if (myModel == null) return;

            int myIndex = t_dge_Agency.SelectedIndex;
            MessageResult myResult = XMessageBox.Ask("确定要删除当前数据吗？", this);
            if (myResult == MessageResult.No) return;

            m_AgencyList.Remove(myModel);

            t_dge_Agency.Items.Refresh();
            t_dge_Agency_SelectionChanged(null, null);

            if (t_dge_Agency.Items.Count == 0)
            {
                SetAgencyReadOnly(true);
            }

            if (myIndex == 0)
                myIndex++;
            t_dge_Agency.SelectedIndex = myIndex - 1;
            t_dge_Agency.Focus();
            //计算已付机构资金总和
            SumAgencyAccountsPrepaid();
            //校验开票
            ValidateIsAllAgency();
        }
        /// <summary>
        /// 外包信息添加按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_btn_LabAdd_Click(object sender, RoutedEventArgs e)
        {
            if (t_dge_Lab.Items.Count == 0)
                SetLabReadOnly(false);

            //添加新对象
            PT_B_Project_Lab newModel = new PT_B_Project_Lab();
            newModel.Lab_Date = DateTime.Today;
            newModel.Is_Lab_Inv = "否";
            m_LabList.Add(newModel);
            t_dge_Lab.Items.Refresh();
            t_dge_Lab.SelectedIndex = m_LabList.Count - 1;
            t_dge_Lab.Focus();
        }
        /// <summary>
        /// 外包信息删除按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_btn_LabDelete_Click(object sender, RoutedEventArgs e)
        {
            if (t_dge_Lab.Items.Count < 1)
            {
                XMessageBox.Warning("请选择需要删除的记录行!", this);
                return;
            }

            PT_B_Project_Lab myModel = t_dge_Lab.SelectedItem as PT_B_Project_Lab;
            if (myModel == null) return;

            int myIndex = t_dge_Lab.SelectedIndex;
            MessageResult myResult = XMessageBox.Ask("确定要删除当前数据吗？", this);
            if (myResult == MessageResult.No) return;

            m_LabList.Remove(myModel);

            t_dge_Lab.Items.Refresh();
            t_dge_Lab_SelectionChanged(null, null);

            if (t_dge_Lab.Items.Count == 0)
            {
                SetLabReadOnly(true);
            }

            if (myIndex == 0)
                myIndex++;
            t_dge_Lab.SelectedIndex = myIndex - 1;
            t_dge_Lab.Focus();
            //计算已付外包资金总和
            SumLabAccountsPrepaid();
            //校验开票
            ValidateIsAllLab();
        }
        /// <summary>
        /// 其他信息添加按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_btn_OtherAdd_Click(object sender, RoutedEventArgs e)
        {
            if (t_dge_Other.Items.Count == 0)
                SetOtherReadOnly(false);

            //添加新对象
            PT_B_Project_Other newModel = new PT_B_Project_Other();
            newModel.Other_Date = DateTime.Today;
            newModel.Is_Other_Inv = "否";
            m_OtherList.Add(newModel);
            t_dge_Other.Items.Refresh();
            t_dge_Other.SelectedIndex = m_OtherList.Count - 1;
            t_dge_Other.Focus();
        }
        /// <summary>
        /// 其他信息删除按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_btn_OtherDelete_Click(object sender, RoutedEventArgs e)
        {
            if (t_dge_Other.Items.Count < 1)
            {
                XMessageBox.Warning("请选择需要删除的记录行!", this);
                return;
            }

            PT_B_Project_Other myModel = t_dge_Other.SelectedItem as PT_B_Project_Other;
            if (myModel == null) return;

            int myIndex = t_dge_Other.SelectedIndex;
            MessageResult myResult = XMessageBox.Ask("确定要删除当前数据吗？", this);
            if (myResult == MessageResult.No) return;

            m_OtherList.Remove(myModel);

            t_dge_Other.Items.Refresh();
            t_dge_Other_SelectionChanged(null, null);

            if (t_dge_Other.Items.Count == 0)
            {
                SetOtherReadOnly(true);
            }

            if (myIndex == 0)
                myIndex++;
            t_dge_Other.SelectedIndex = myIndex - 1;
            t_dge_Other.Focus();
            //计算已付其他资金总和
            SumOtherAccountsPrepaid();
            //校验开票
            ValidateIsOtherLab();
        }

        #endregion

        #region 表格行切换事件

        /// <summary>
        /// 客户表格行切换事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_dge_CustomBill_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e == null) return;

            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                m_IsCustomerGridLoad = true;
                //对新行控件进行赋值
                PT_B_Project_Customer myModel = e.AddedItems[0] as PT_B_Project_Customer;
                BindCustomerModelToControl(myModel);
                m_IsCustomerGridLoad = false;
            }
            t_dge_CustomBill.Items.Refresh();
        }
        /// <summary>
        /// 机构表格行切换事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_dge_Agency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e == null) return;

            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                m_IsAgencyGridLoad = true;
                //对新行控件进行赋值
                PT_B_Project_Agency myModel = e.AddedItems[0] as PT_B_Project_Agency;
                BindAgencyModelToControl(myModel);
                m_IsAgencyGridLoad = false;
            }
            t_dge_Agency.Items.Refresh();
        }
        /// <summary>
        /// 外包表格行切换事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_dge_Lab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e == null) return;

            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                m_IsLabGridLoad = true;
                //对新行控件进行赋值
                PT_B_Project_Lab myModel = e.AddedItems[0] as PT_B_Project_Lab;
                BindLabModelToControl(myModel);
                m_IsLabGridLoad = false;
            }
            t_dge_CustomBill.Items.Refresh();
        }
        /// <summary>
        /// 其他表格行切换事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_dge_Other_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e == null) return;

            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                m_IsOtherGridLoad = true;
                //对新行控件进行赋值
                PT_B_Project_Other myModel = e.AddedItems[0] as PT_B_Project_Other;
                BindOtherModelToControl(myModel);
                m_IsOtherGridLoad = false;
            }
            t_dge_CustomBill.Items.Refresh();
        }

        #endregion

        #region 资金值变更事件

        /// <summary>
        /// 客户资金值变更事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_txt_AccountReceivable_TextChanged(object sender, TextChangedEventArgs e)
        {
            //计算待收钱款
            double accountReceivable = string.IsNullOrEmpty(t_txt_AccountReceivable.Text) ? 0 : double.Parse(t_txt_AccountReceivable.Text);
            double paymentReceivable = string.IsNullOrEmpty(t_txt_PaymentReceivable.Text) ? 0 : double.Parse(t_txt_PaymentReceivable.Text);
            double unAccountReceivable = accountReceivable - paymentReceivable;
            t_txt_UnAccountReceivable.Text = unAccountReceivable.ToString() == "0" ? string.Empty : unAccountReceivable.ToString();

            //计算总利润
            double agencyAccountPayable = string.IsNullOrEmpty(t_txt_AgencyAccountPayable.Text) ? 0 : double.Parse(t_txt_AgencyAccountPayable.Text);
            double labAccountPayable = string.IsNullOrEmpty(t_txt_LabAccountPayable.Text) ? 0 : double.Parse(t_txt_LabAccountPayable.Text);
            double otherAccount = string.IsNullOrEmpty(t_txt_OtherAccount.Text) ? 0 : double.Parse(t_txt_OtherAccount.Text);
            double profits = accountReceivable - agencyAccountPayable - labAccountPayable - otherAccount;
            t_txt_Profits.Text = profits.ToString() == "0" ? string.Empty : profits.ToString();

            //校验是否全开票
            ValidateIsAllCustomer();
        }
        /// <summary>
        /// 机构资金值变更事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_txt_AgencyAccountPayable_TextChanged(object sender, TextChangedEventArgs e)
        {
            //计算待付钱款
            double agencyAccountPayable = string.IsNullOrEmpty(t_txt_AgencyAccountPayable.Text) ? 0 : double.Parse(t_txt_AgencyAccountPayable.Text);
            double agencyAccountsPrepaid = string.IsNullOrEmpty(t_txt_AgencyAccountsPrepaid.Text) ? 0 : double.Parse(t_txt_AgencyAccountsPrepaid.Text);
            double unAgencyAccountPayable = agencyAccountPayable - agencyAccountsPrepaid;
            t_txt_UnAgencyAccountPayable.Text = unAgencyAccountPayable.ToString() == "0" ? string.Empty : unAgencyAccountPayable.ToString();

            //计算利润
            double accountReceivable = string.IsNullOrEmpty(t_txt_AccountReceivable.Text) ? 0 : double.Parse(t_txt_AccountReceivable.Text);
            double labAccountPayable = string.IsNullOrEmpty(t_txt_LabAccountPayable.Text) ? 0 : double.Parse(t_txt_LabAccountPayable.Text);
            double otherAccount = string.IsNullOrEmpty(t_txt_OtherAccount.Text) ? 0 : double.Parse(t_txt_OtherAccount.Text);
            double profits = accountReceivable - agencyAccountPayable - labAccountPayable - otherAccount;
            t_txt_Profits.Text = profits.ToString() == "0" ? string.Empty : profits.ToString();
        }
        /// <summary>
        /// 外包资金值变更事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_txt_LabAccountPayable_TextChanged(object sender, TextChangedEventArgs e)
        {
            //计算待付钱款
            double labAccountPayable = string.IsNullOrEmpty(t_txt_LabAccountPayable.Text) ? 0 : double.Parse(t_txt_LabAccountPayable.Text);
            double labAccountsPrepaid = string.IsNullOrEmpty(t_txt_LabAccountsPrepaid.Text) ? 0 : double.Parse(t_txt_LabAccountsPrepaid.Text);
            double unLabAccountPayable = labAccountPayable - labAccountsPrepaid;
            t_txt_UnLabAccountPayable.Text = unLabAccountPayable.ToString() == "0" ? string.Empty : unLabAccountPayable.ToString();

            //计算利润
            double accountReceivable = string.IsNullOrEmpty(t_txt_AccountReceivable.Text) ? 0 : double.Parse(t_txt_AccountReceivable.Text);
            double agencyAccountPayable = string.IsNullOrEmpty(t_txt_AgencyAccountPayable.Text) ? 0 : double.Parse(t_txt_AgencyAccountPayable.Text);
            double otherAccount = string.IsNullOrEmpty(t_txt_OtherAccount.Text) ? 0 : double.Parse(t_txt_OtherAccount.Text);
            double profits = accountReceivable - agencyAccountPayable - labAccountPayable - otherAccount;
            t_txt_Profits.Text = profits.ToString() == "0" ? string.Empty : profits.ToString();
        }
        /// <summary>
        /// 其他资金值变更事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_txt_OtherAccount_TextChanged(object sender, TextChangedEventArgs e)
        {
            //计算利润
            double accountReceivable = string.IsNullOrEmpty(t_txt_AccountReceivable.Text) ? 0 : double.Parse(t_txt_AccountReceivable.Text);
            double agencyAccountPayable = string.IsNullOrEmpty(t_txt_AgencyAccountPayable.Text) ? 0 : double.Parse(t_txt_AgencyAccountPayable.Text);
            double labAccountPayable = string.IsNullOrEmpty(t_txt_LabAccountPayable.Text) ? 0 : double.Parse(t_txt_LabAccountPayable.Text);
            double otherAccount = string.IsNullOrEmpty(t_txt_OtherAccount.Text) ? 0 : double.Parse(t_txt_OtherAccount.Text);
            double profits = accountReceivable - agencyAccountPayable - labAccountPayable - otherAccount;
            t_txt_Profits.Text = profits.ToString() == "0" ? string.Empty : profits.ToString();
        }

        #endregion

        #region  已付账款值变更事件

        /// <summary>
        /// 已收客户账款值变更事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_txt_PaymentReceivable_TextChanged(object sender, TextChangedEventArgs e)
        {
            //计算待收钱款
            double accountReceivable = string.IsNullOrEmpty(t_txt_AccountReceivable.Text) ? 0 : double.Parse(t_txt_AccountReceivable.Text);
            double paymentReceivable = string.IsNullOrEmpty(t_txt_PaymentReceivable.Text) ? 0 : double.Parse(t_txt_PaymentReceivable.Text);
            double unAccountReceivable = accountReceivable - paymentReceivable;
            t_txt_UnAccountReceivable.Text = unAccountReceivable.ToString() == "0" ? string.Empty : unAccountReceivable.ToString();

            //计算当前利润
            double agencyAccountsPrepaid = string.IsNullOrEmpty(t_txt_AgencyAccountsPrepaid.Text) ? 0 : double.Parse(t_txt_AgencyAccountsPrepaid.Text);
            double labAccountsPrepaid = string.IsNullOrEmpty(t_txt_LabAccountsPrepaid.Text) ? 0 : double.Parse(t_txt_LabAccountsPrepaid.Text);
            double otherPadAccoun = string.IsNullOrEmpty(t_txt_OtherPadAccount.Text) ? 0 : double.Parse(t_txt_OtherPadAccount.Text);
            double nowProfits = paymentReceivable - agencyAccountsPrepaid - labAccountsPrepaid - otherPadAccoun;
            t_txt_NowProfits.Text = nowProfits.ToString() == "0" ? string.Empty : nowProfits.ToString();

            //计算垫付金额
            if (nowProfits < 0)
            {
                t_txt_PadsMoney.Text = (-1 * nowProfits).ToString();
                t_chk_IsPads.IsChecked = true;
            }
            else
            {
                t_txt_PadsMoney.Text = string.Empty;
                t_chk_IsPads.IsChecked = false;
            }
        }
        /// <summary>
        /// 已付机构账款值变更事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_txt_AgencyAccountsPrepaid_TextChanged(object sender, TextChangedEventArgs e)
        {
            //计算待付钱款
            double AgencyAccountPayable = string.IsNullOrEmpty(t_txt_AgencyAccountPayable.Text) ? 0 : double.Parse(t_txt_AgencyAccountPayable.Text);
            double agencyAccountsPrepaid = string.IsNullOrEmpty(t_txt_AgencyAccountsPrepaid.Text) ? 0 : double.Parse(t_txt_AgencyAccountsPrepaid.Text);
            double unAgencyAccountPayable = AgencyAccountPayable - agencyAccountsPrepaid;
            t_txt_UnAgencyAccountPayable.Text = unAgencyAccountPayable.ToString() == "0" ? string.Empty : unAgencyAccountPayable.ToString();

            //计算当前利润
            double paymentReceivable = string.IsNullOrEmpty(t_txt_PaymentReceivable.Text) ? 0 : double.Parse(t_txt_PaymentReceivable.Text);
            double labAccountsPrepaid = string.IsNullOrEmpty(t_txt_LabAccountsPrepaid.Text) ? 0 : double.Parse(t_txt_LabAccountsPrepaid.Text);
            double otherPadAccoun = string.IsNullOrEmpty(t_txt_OtherPadAccount.Text) ? 0 : double.Parse(t_txt_OtherPadAccount.Text);
            double nowProfits = paymentReceivable - agencyAccountsPrepaid - labAccountsPrepaid - otherPadAccoun;
            t_txt_NowProfits.Text = nowProfits.ToString() == "0" ? string.Empty : nowProfits.ToString();

            //计算垫付金额
            if (nowProfits < 0)
            {
                t_txt_PadsMoney.Text = (-1 * nowProfits).ToString();
                t_chk_IsPads.IsChecked = true;
            }
            else
            {
                t_txt_PadsMoney.Text = string.Empty;
                t_chk_IsPads.IsChecked = false;
            }
        }
        /// <summary>
        /// 已付外包账款值变更事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_txt_LabAccountsPrepaid_TextChanged(object sender, TextChangedEventArgs e)
        {
            //计算待付钱款
            double labAccountPayable = string.IsNullOrEmpty(t_txt_LabAccountPayable.Text) ? 0 : double.Parse(t_txt_LabAccountPayable.Text);
            double labAccountsPrepaid = string.IsNullOrEmpty(t_txt_LabAccountsPrepaid.Text) ? 0 : double.Parse(t_txt_LabAccountsPrepaid.Text);
            double unLabAccountPayable = labAccountPayable - labAccountsPrepaid;
            t_txt_UnLabAccountPayable.Text = unLabAccountPayable.ToString() == "0" ? string.Empty : unLabAccountPayable.ToString();

            //计算当前利润
            double paymentReceivable = string.IsNullOrEmpty(t_txt_PaymentReceivable.Text) ? 0 : double.Parse(t_txt_PaymentReceivable.Text);
            double agencyAccountsPrepaid = string.IsNullOrEmpty(t_txt_AgencyAccountsPrepaid.Text) ? 0 : double.Parse(t_txt_AgencyAccountsPrepaid.Text);
            double otherPadAccoun = string.IsNullOrEmpty(t_txt_OtherPadAccount.Text) ? 0 : double.Parse(t_txt_OtherPadAccount.Text);
            double nowProfits = paymentReceivable - agencyAccountsPrepaid - labAccountsPrepaid - otherPadAccoun;
            t_txt_NowProfits.Text = nowProfits.ToString() == "0" ? string.Empty : nowProfits.ToString();

            //计算垫付金额
            if (nowProfits < 0)
            {
                t_txt_PadsMoney.Text = (-1 * nowProfits).ToString();
                t_chk_IsPads.IsChecked = true;
            }
            else
            {
                t_txt_PadsMoney.Text = string.Empty;
                t_chk_IsPads.IsChecked = false;
            }
        }
        /// <summary>
        /// 已付其他账款值变更事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_txt_OtherPadAccount_TextChanged(object sender, TextChangedEventArgs e)
        {
            //计算待付钱款
            double otherAccount = string.IsNullOrEmpty(t_txt_OtherAccount.Text) ? 0 : double.Parse(t_txt_OtherAccount.Text);
            double otherPadAccount = string.IsNullOrEmpty(t_txt_OtherPadAccount.Text) ? 0 : double.Parse(t_txt_OtherPadAccount.Text);
            double unOtherAccount = otherAccount - otherPadAccount;
            t_txt_UnOtherAccount.Text = unOtherAccount.ToString() == "0" ? string.Empty : unOtherAccount.ToString();

            //计算当前利润
            double paymentReceivable = string.IsNullOrEmpty(t_txt_PaymentReceivable.Text) ? 0 : double.Parse(t_txt_PaymentReceivable.Text);
            double agencyAccountsPrepaid = string.IsNullOrEmpty(t_txt_AgencyAccountsPrepaid.Text) ? 0 : double.Parse(t_txt_AgencyAccountsPrepaid.Text);
            double otherPadAccoun = string.IsNullOrEmpty(t_txt_OtherPadAccount.Text) ? 0 : double.Parse(t_txt_OtherPadAccount.Text);
            double labAccountsPrepaid = string.IsNullOrEmpty(t_txt_LabAccountsPrepaid.Text) ? 0 : double.Parse(t_txt_LabAccountsPrepaid.Text);
            double nowProfits = paymentReceivable - agencyAccountsPrepaid - labAccountsPrepaid - otherPadAccoun;
            t_txt_NowProfits.Text = nowProfits.ToString() == "0" ? string.Empty : nowProfits.ToString();

            //计算垫付金额
            if (nowProfits < 0)
            {
                t_txt_PadsMoney.Text = (-1 * nowProfits).ToString();
                t_chk_IsPads.IsChecked = true;
            }
            else
            {
                t_txt_PadsMoney.Text = string.Empty;
                t_chk_IsPads.IsChecked = false;
            }
        }

        #endregion

        #endregion

        #region 私有方法_数据加载

        /// <summary>
        /// 加载数据至界面
        /// </summary>
        private void LoadData()
        {
            if (PTBProject == null) return;

            //对界面控件进行赋值
            if (string.IsNullOrEmpty(PTBProject.Project_No))
            {
                PTBProject.Project_No = "新单";
                t_tslStateText.Text = "新建";
            }
            t_txt_ProjectNo.Text = PTBProject.Project_No;
            t_txt_QuotationNo.Text = PTBProject.Quotation_No;
            t_txt_FollowMan.Text = PTBProject.Follow_Man;
            if (PTBProject.Project_Type == "安全")
                t_chk_Save.IsChecked = true;
            else if (PTBProject.Project_Type == "化学")
                t_chk_Chemis.IsChecked = true;
            t_txt_CompanyName.Text = PTBProject.Company_Name;
            t_txt_ContactMan.Text = PTBProject.Contact_Man;
            t_txt_Tel.Text = PTBProject.Tel;
            t_txt_CompanyAddress.Text = PTBProject.Company_Address;
            t_txt_Fax.Text = PTBProject.Fax;
            t_txt_Email.Text = PTBProject.Email;
            t_txt_ProjectName.Text = PTBProject.Project_Name;
            t_txt_ProductModel.Text = PTBProject.Product_Model;
            t_txt_CycleTime.Text = PTBProject.Cycle_Time;
            t_txt_Price.Text = PTBProject.Price;
            t_chk_IsTax.IsChecked = PTBProject.Is_Tax == "是" ? true : false;
            t_dtp_QuotationDate.Value = PTBProject.Quotation_Date;
            t_txt_Remark.Text = PTBProject.Remark;
            //应收客户账款
            t_txt_AccountReceivable.Text = PTBProject.Account_Receivable;
            t_txt_PaymentReceivable.Text = PTBProject.Payment_Receivable;
            t_txt_UnAccountReceivable.Text = PTBProject.Un_Account_Receivable;
            t_chk_IsAllCustomer.IsChecked = PTBProject.Is_All_Customer == "是" ? true : false;
            //应付机构账款
            t_txt_AgencyAccountPayable.Text = PTBProject.Agency_Account_Payable;
            t_txt_AgencyAccountsPrepaid.Text = PTBProject.Agency_Accounts_Prepaid;
            t_txt_UnAgencyAccountPayable.Text = PTBProject.Un_Agency_Account_Payable;
            t_chk_IsAllAgency.IsChecked = PTBProject.Is_All_Agency == "是" ? true : false;
            //应付外包账款
            t_txt_LabAccountPayable.Text = PTBProject.Lab_Account_Payable;
            t_txt_LabAccountsPrepaid.Text = PTBProject.Lab_Accounts_Prepaid;
            t_txt_UnLabAccountPayable.Text = PTBProject.Un_Lab_Account_Payable;
            t_chk_IsAllLab.IsChecked = PTBProject.Is_All_Lab == "是" ? true : false;
            //其他账款信息
            t_txt_OtherAccount.Text = PTBProject.Other_Account;
            t_txt_OtherPadAccount.Text = PTBProject.Other_Pad_Account ;
            t_txt_UnOtherAccount.Text= PTBProject.Un_Other_Account ;
            t_chk_IsAllOther.IsChecked = PTBProject.Is_All_Other == "是" ? true : false;
            //利润与垫付信息
            t_txt_Profits.Text = PTBProject.Profits;
            t_txt_NowProfits.Text = PTBProject.Now_Profits;
            t_txt_PadsMoney.Text = PTBProject.Pads_Money;
            t_chk_IsPads.IsChecked = PTBProject.Is_Pads == "是" ? true : false;
            //客户资金往来
            var customer = from p in m_Entities.PT_B_Project_Customer
                           where p.Project_No == PTBProject.Project_No
                           orderby p.Seq_No
                           select p;

            m_CustomerList = customer.ToList();
            t_dge_CustomBill.ItemsSource = m_CustomerList;
            if (m_CustomerList != null && m_CustomerList.Count > 0)
            {
                t_dge_CustomBill.SelectedIndex = 0;
                t_dge_CustomBill.Focus();
            }
            else
                SetCustomerReadOnly(true);
            //机构资金往来
            var agency = from p in m_Entities.PT_B_Project_Agency
                         where p.Project_No == PTBProject.Project_No
                         orderby p.Seq_No
                         select p;
            m_AgencyList = agency.ToList();
            t_dge_Agency.ItemsSource = m_AgencyList;
            if (m_AgencyList != null && m_AgencyList.Count > 0)
            {
                t_dge_Agency.SelectedIndex = 0;
                t_dge_Agency.Focus();
            }
            else
                SetAgencyReadOnly(true);
            //外包资金往来
            var lab = from p in m_Entities.PT_B_Project_Lab
                      where p.Project_No == PTBProject.Project_No
                      orderby p.Seq_No
                      select p;

            m_LabList = lab.ToList();
            t_dge_Lab.ItemsSource = m_LabList;
            if (m_LabList != null && m_LabList.Count > 0)
            {
                t_dge_Lab.SelectedIndex = 0;
                t_dge_Lab.Focus();
            }
            else
                SetLabReadOnly(true);

            //其他资金往来
            var other = from p in m_Entities.PT_B_Project_Other
                        where p.Project_No == PTBProject.Project_No
                        orderby p.Seq_No
                        select p;
            m_OtherList = other.ToList();
            t_dge_Other.ItemsSource = m_OtherList;
            if (m_OtherList != null && m_OtherList.Count > 0)
            {
                t_dge_Other.SelectedIndex = 0;
                t_dge_Other.Focus();
            }
            else
                SetOtherReadOnly(true);
        }

        #endregion

        #region 私有方法_设置控件只读性

        /// <summary>
        /// 设置客户控件只读性
        /// </summary>
        /// <param name="isReadOnly">标识值</param>
        private void SetCustomerReadOnly(bool isReadOnly)
        {
            t_txt_CustomerMoney.IsReadOnly = isReadOnly;
            t_txt_Customer.IsReadOnly = isReadOnly;
            t_txt_CustomerRemark.IsReadOnly = isReadOnly;
            t_dtp_CustomerDate.IsReadOnly = isReadOnly;
            t_chk_IsCustomerInv.IsReadOnly = isReadOnly;
            t_chk_IsCustomerInv.IsChecked = false;

            if (isReadOnly)
            {
                t_txt_CustomerMoney.Text = String.Empty;
                t_txt_Customer.Text = String.Empty;
                t_txt_CustomerRemark.Text = String.Empty;
                t_dtp_CustomerDate.Value = DateTime.Parse("1900-01-01");
                t_chk_IsCustomerInv.IsChecked = false;
                t_txt_CustomerInvPrice.Text = String.Empty;
                t_txt_CustomerInvNo.Text = String.Empty;
                t_dtp_CustomerInvDate.Value = DateTime.Parse("1900-01-01");
            }
        }
        /// <summary>
        /// 设置机构控件只读性
        /// </summary>
        /// <param name="isReadOnly">标识值</param>
        private void SetAgencyReadOnly(bool isReadOnly)
        {
            t_txt_AgencyMoney.IsReadOnly = isReadOnly;
            t_txt_Agency.IsReadOnly = isReadOnly;
            t_txt_AgencyRemark.IsReadOnly = isReadOnly;
            t_dtp_AgencyDate.IsReadOnly = isReadOnly;
            t_chk_IsAgencyInv.IsReadOnly = isReadOnly;
            t_chk_IsAgencyInv.IsChecked = false;
            if (isReadOnly)
            {
                t_txt_AgencyMoney.Text = String.Empty;
                t_txt_Agency.Text = String.Empty;
                t_txt_AgencyRemark.Text = String.Empty;
                t_dtp_AgencyDate.Value = DateTime.Parse("1900-01-01");
                t_chk_IsAgencyInv.IsChecked = false;
                t_txt_AgencyInvPrice.Text = String.Empty;
                t_txt_AgencyInvNo.Text = String.Empty;
                t_dtp_AgencyInvDate.Value = DateTime.Parse("1900-01-01");
            }
        }
        /// <summary>
        /// 设置外包控件只读性
        /// </summary>
        /// <param name="isReadOnly">标识值</param>
        private void SetLabReadOnly(bool isReadOnly)
        {
            t_txt_LabMoney.IsReadOnly = isReadOnly;
            t_txt_Lab.IsReadOnly = isReadOnly;
            t_txt_LabRemark.IsReadOnly = isReadOnly;
            t_dtp_LabDate.IsReadOnly = isReadOnly;
            t_chk_IsLabInv.IsReadOnly = isReadOnly;
            t_chk_IsLabInv.IsChecked = false;

            if (isReadOnly)
            {
                t_txt_LabMoney.Text = String.Empty;
                t_txt_Lab.Text = String.Empty;
                t_txt_LabRemark.Text = String.Empty;
                t_dtp_LabDate.Value = DateTime.Parse("1900-01-01");
                t_chk_IsLabInv.IsChecked = false;
                t_txt_LabInvPrice.Text = String.Empty;
                t_txt_LabInvNo.Text = String.Empty;
                t_dtp_LabInvDate.Value = DateTime.Parse("1900-01-01");
            }
        }
        /// <summary>
        /// 设置其他控件只读性
        /// </summary>
        /// <param name="isReadOnly">标识值</param>
        private void SetOtherReadOnly(bool isReadOnly)
        {
            t_txt_OtherMoney.IsReadOnly = isReadOnly;
            t_txt_Other.IsReadOnly = isReadOnly;
            t_txt_OtherRemark.IsReadOnly = isReadOnly;
            t_dtp_OtherDate.IsReadOnly = isReadOnly;
            t_chk_IsOtherInv.IsReadOnly = isReadOnly;
            t_chk_IsOtherInv.IsChecked = false;
            if (isReadOnly)
            {
                t_txt_OtherMoney.Text = String.Empty;
                t_txt_Other.Text = String.Empty;
                t_txt_OtherRemark.Text = String.Empty;
                t_dtp_OtherDate.Value = DateTime.Parse("1900-01-01");
                t_chk_IsOtherInv.IsChecked = false;
                t_txt_OtherInvPrice.Text = String.Empty;
                t_txt_OtherInvNo.Text = String.Empty;
                t_dtp_OtherInvDate.Value = DateTime.Parse("1900-01-01");
            }
        }

        #endregion

        #region 私有方法_设计算资金总和

        /// <summary>
        /// 计算已收客户资金总和
        /// </summary>
        private void SumPaymentReceivable()
        {
            double sumMoney = 0;
            foreach (PT_B_Project_Customer model in t_dge_CustomBill.Items)
            {
                string customerMoney = model.Customer_Money;
                if (!string.IsNullOrEmpty(customerMoney))
                    sumMoney += double.Parse(customerMoney);
            }
            t_txt_PaymentReceivable.Text = sumMoney.ToString() == "0" ? string.Empty : sumMoney.ToString();
        }
        /// <summary>
        /// 计算已付机构资金总和
        /// </summary>
        private void SumAgencyAccountsPrepaid()
        {
            double sumMoney = 0;
            foreach (PT_B_Project_Agency model in t_dge_Agency.Items)
            {
                string agencyMoney = model.Agency_Money;
                if (!string.IsNullOrEmpty(agencyMoney))
                    sumMoney += double.Parse(agencyMoney);
            }
            t_txt_AgencyAccountsPrepaid.Text = sumMoney.ToString() == "0" ? string.Empty : sumMoney.ToString();
        }
        /// <summary>
        /// 计算已付外包资金总和
        /// </summary>
        private void SumLabAccountsPrepaid()
        {
            double sumMoney = 0;
            foreach (PT_B_Project_Lab model in t_dge_Lab.Items)
            {
                string labMoney = model.Lab_Money;
                if (!string.IsNullOrEmpty(labMoney))
                    sumMoney += double.Parse(labMoney);
            }
            t_txt_LabAccountsPrepaid.Text = sumMoney.ToString() == "0" ? string.Empty : sumMoney.ToString();
        }
        /// <summary>
        /// 计算已付其他资金总和
        /// </summary>
        private void SumOtherAccountsPrepaid()
        {
            double sumMoney = 0;
            foreach (PT_B_Project_Other model in t_dge_Other.Items)
            {
                string otherMoney = model.Other_Money;
                if (!string.IsNullOrEmpty(otherMoney))
                    sumMoney += double.Parse(otherMoney);
            }
            t_txt_OtherPadAccount.Text = sumMoney.ToString() == "0" ? string.Empty : sumMoney.ToString();
        }
        #endregion

        #region 私有方法_设控件绑定行对象值

        /// <summary>
        /// 控件客户绑定行对象值
        /// </summary>
        /// <param name="iModel">绑定实体</param>
        private void BindCustomerModelToControl(PT_B_Project_Customer myModel)
        {
            if (myModel == null)
            {
                t_txt_CustomerMoney.Text = String.Empty;
                t_txt_Customer.Text = String.Empty;
                t_txt_CustomerRemark.Text = String.Empty;
                t_dtp_CustomerDate.Value = DateTime.Parse("1900-01-01");
                t_chk_IsCustomerInv.IsChecked = false;
                t_txt_CustomerInvPrice.Text = String.Empty;
                t_txt_CustomerInvNo.Text = String.Empty;
                t_dtp_CustomerInvDate.Value = DateTime.Parse("1900-01-01");
            }
            else
            {
                t_txt_CustomerMoney.Text = myModel.Customer_Money == null ? string.Empty : myModel.Customer_Money.ToString();
                t_txt_Customer.Text = myModel.Customer;
                t_txt_CustomerRemark.Text = myModel.Customer_Remark;
                t_dtp_CustomerDate.Value = myModel.Customer_Date;
                t_chk_IsCustomerInv.IsChecked = myModel.Is_Customer_Inv == "是" ? true : false;
                t_txt_CustomerInvPrice.Text = myModel.Customer_Inv_Price == null ? string.Empty : myModel.Customer_Inv_Price.ToString();
                t_txt_CustomerInvNo.Text = myModel.Customer_Inv_No;
                if (myModel.Customer_Inv_Date == null)
                    t_dtp_CustomerInvDate.Value = DateTime.Parse("1900-01-01");
                else
                    t_dtp_CustomerInvDate.Value = DateTime.Parse(myModel.Customer_Inv_Date.ToString());
            }
        }
        /// <summary>
        /// 控件机构绑定行对象值
        /// </summary>
        /// <param name="iModel">绑定实体</param>
        private void BindAgencyModelToControl(PT_B_Project_Agency myModel)
        {
            if (myModel == null)
            {
                t_txt_AgencyMoney.Text = String.Empty;
                t_txt_Agency.Text = String.Empty;
                t_txt_AgencyRemark.Text = String.Empty;
                t_dtp_AgencyDate.Value = DateTime.Parse("1900-01-01");
                t_chk_IsAgencyInv.IsChecked = false;
                t_txt_AgencyInvPrice.Text = String.Empty;
                t_txt_AgencyInvNo.Text = String.Empty;
                t_dtp_AgencyInvDate.Value = DateTime.Parse("1900-01-01");
            }
            else
            {
                t_txt_AgencyMoney.Text = myModel.Agency_Money == null ? string.Empty : myModel.Agency_Money.ToString();
                t_txt_Agency.Text = myModel.Agency;
                t_txt_AgencyRemark.Text = myModel.Agency_Remark;
                t_dtp_AgencyDate.Value = myModel.Agency_Date;
                t_chk_IsAgencyInv.IsChecked = myModel.Is_Agency_Inv == "是" ? true : false;
                t_txt_AgencyInvPrice.Text = myModel.Agency_Inv_Price == null ? string.Empty : myModel.Agency_Inv_Price.ToString();
                t_txt_AgencyInvNo.Text = myModel.Agency_Inv_No;
                if (myModel.Agency_Inv_Date == null)
                    t_dtp_AgencyInvDate.Value = DateTime.Parse("1900-01-01");
                else
                    t_dtp_AgencyInvDate.Value = DateTime.Parse(myModel.Agency_Inv_Date.ToString());
            }
        }
        /// <summary>
        /// 控件外包绑定行对象值
        /// </summary>
        /// <param name="iModel">绑定实体</param>
        private void BindLabModelToControl(PT_B_Project_Lab myModel)
        {
            if (myModel == null)
            {
                t_txt_LabMoney.Text = String.Empty;
                t_txt_Lab.Text = String.Empty;
                t_txt_LabRemark.Text = String.Empty;
                t_dtp_LabDate.Value = DateTime.Parse("1900-01-01");
                t_chk_IsLabInv.IsChecked = false;
                t_txt_LabInvPrice.Text = String.Empty;
                t_txt_LabInvNo.Text = String.Empty;
                t_dtp_LabInvDate.Value = DateTime.Parse("1900-01-01");
            }
            else
            {
                t_txt_LabMoney.Text = myModel.Lab_Money == null ? string.Empty : myModel.Lab_Money.ToString();
                t_txt_Lab.Text = myModel.Lab;
                t_txt_LabRemark.Text = myModel.Lab_Remark;
                t_dtp_LabDate.Value = myModel.Lab_Date;
                t_chk_IsLabInv.IsChecked = myModel.Is_Lab_Inv == "是" ? true : false;
                t_txt_LabInvPrice.Text = myModel.Lab_Inv_Price == null ? string.Empty : myModel.Lab_Inv_Price.ToString();
                t_txt_LabInvNo.Text = myModel.Lab_Inv_No;
                if (myModel.Lab_Inv_Date == null)
                    t_dtp_LabInvDate.Value = DateTime.Parse("1900-01-01");
                else
                    t_dtp_LabInvDate.Value = DateTime.Parse(myModel.Lab_Inv_Date.ToString());
            }
        }
        /// <summary>
        /// 控件其他绑定行对象值
        /// </summary>
        /// <param name="iModel">绑定实体</param>
        private void BindOtherModelToControl(PT_B_Project_Other myModel)
        {
            if (myModel == null)
            {
                t_txt_OtherMoney.Text = String.Empty;
                t_txt_Other.Text = String.Empty;
                t_txt_OtherRemark.Text = String.Empty;
                t_dtp_OtherDate.Value = DateTime.Parse("1900-01-01");
                t_chk_IsOtherInv.IsChecked = false;
                t_txt_OtherInvPrice.Text = String.Empty;
                t_txt_OtherInvNo.Text = String.Empty;
                t_dtp_OtherInvDate.Value = DateTime.Parse("1900-01-01");
            }
            else
            {
                t_txt_OtherMoney.Text = myModel.Other_Money == null ? string.Empty : myModel.Other_Money.ToString();
                t_txt_Other.Text = myModel.Other;
                t_txt_OtherRemark.Text = myModel.Other_Remark;
                t_dtp_OtherDate.Value = myModel.Other_Date;
                t_chk_IsOtherInv.IsChecked = myModel.Is_Other_Inv == "是" ? true : false;
                t_txt_OtherInvPrice.Text = myModel.Other_Inv_Price == null ? string.Empty : myModel.Other_Inv_Price.ToString();
                t_txt_OtherInvNo.Text = myModel.Other_Inv_No;
                if (myModel.Other_Inv_Date == null)
                    t_dtp_OtherInvDate.Value = DateTime.Parse("1900-01-01");
                else
                    t_dtp_OtherInvDate.Value = DateTime.Parse(myModel.Other_Inv_Date.ToString());
            }
        }

        #endregion

        #region 私有方法_校验是否全开票

        /// <summary>
        /// 校验客户是否全开票
        /// </summary>
        private void ValidateIsAllCustomer()
        {
            double sumInvPrice = 0;
            foreach (PT_B_Project_Customer model in t_dge_CustomBill.Items)
            {
                if (model.Is_Customer_Inv == "否") continue;

                string customerInvPrice = model.Customer_Inv_Price;
                if (!string.IsNullOrEmpty(customerInvPrice))
                    sumInvPrice += double.Parse(customerInvPrice);
            }
            double accountReceivable = string.IsNullOrEmpty(t_txt_AccountReceivable.Text) ? 0 : double.Parse(t_txt_AccountReceivable.Text);
            if (accountReceivable == sumInvPrice)
                t_chk_IsAllCustomer.IsChecked = true;
            else
                t_chk_IsAllCustomer.IsChecked = false;
        }
        /// <summary>
        /// 校验机构是否全开票
        /// </summary>
        private void ValidateIsAllAgency()
        {
            double sumInvPrice = 0;
            foreach (PT_B_Project_Agency model in t_dge_Agency.Items)
            {
                if (model.Is_Agency_Inv == "否") continue;

                string AgencyInvPrice = model.Agency_Inv_Price;
                if (!string.IsNullOrEmpty(AgencyInvPrice))
                    sumInvPrice += double.Parse(AgencyInvPrice);
            }
            double agencyAccountPayable = string.IsNullOrEmpty(t_txt_AgencyAccountPayable.Text) ? 0 : double.Parse(t_txt_AgencyAccountPayable.Text);
            if (agencyAccountPayable == sumInvPrice)
                t_chk_IsAllAgency.IsChecked = true;
            else
                t_chk_IsAllAgency.IsChecked = false;
        }
        /// <summary>
        /// 校验外包是否全开票
        /// </summary>
        private void ValidateIsAllLab()
        {
            double sumInvPrice = 0;
            foreach (PT_B_Project_Lab model in t_dge_Lab.Items)
            {
                if (model.Is_Lab_Inv == "否") continue;

                string LabInvPrice = model.Lab_Inv_Price;
                if (!string.IsNullOrEmpty(LabInvPrice))
                    sumInvPrice += double.Parse(LabInvPrice);
            }
            double labAccountPayable = string.IsNullOrEmpty(t_txt_LabAccountPayable.Text) ? 0 : double.Parse(t_txt_LabAccountPayable.Text);
            if (labAccountPayable == sumInvPrice)
                t_chk_IsAllLab.IsChecked = true;
            else
                t_chk_IsAllLab.IsChecked = false;
        }
        /// <summary>
        /// 校验其他是否全开票
        /// </summary>
        private void ValidateIsOtherLab()
        {
            double sumInvPrice = 0;
            foreach (PT_B_Project_Other model in t_dge_Other.Items)
            {
                if (model.Is_Other_Inv == "否") continue;

                string otherInvPrice = model.Other_Inv_Price;
                if (!string.IsNullOrEmpty(otherInvPrice))
                    sumInvPrice += double.Parse(otherInvPrice);
            }
            double otherAccount = string.IsNullOrEmpty(t_txt_OtherAccount.Text) ? 0 : double.Parse(t_txt_OtherAccount.Text);
            if (otherAccount == sumInvPrice)
                t_chk_IsAllOther.IsChecked = true;
            else
                t_chk_IsAllOther.IsChecked = false;
        }

        #endregion

        #region 私有方法_值变更事件

        /// <summary>
        /// 客户值变更事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void Control_TextChanged(object sender, RoutedEventArgs e)
        {
            Control myControl = sender as Control;
            if (myControl == null || (m_IsCustomerGridLoad && myControl.Name != "t_chk_IsCustomerInv")) return;

            PT_B_Project_Customer myModel = t_dge_CustomBill.SelectedItem as PT_B_Project_Customer;
            if (myModel == null) return;
            switch (myControl.Name)
            {
                case "t_txt_CustomerMoney":
                    myModel.Customer_Money = t_txt_CustomerMoney.Text;
                    SumPaymentReceivable();
                    break;
                case "t_txt_Customer":
                    myModel.Customer = t_txt_Customer.Text;
                    break;
                case "t_txt_CustomerRemark":
                    myModel.Customer_Remark = t_txt_CustomerRemark.Text;
                    break;
                case "t_dtp_CustomerDate":
                    myModel.Customer_Date = t_dtp_CustomerDate.Value;
                    break;
                case "t_chk_IsCustomerInv":
                    myModel.Is_Customer_Inv = t_chk_IsCustomerInv.IsChecked == true ? "是" : "否";
                    if (t_chk_IsCustomerInv.IsChecked == true)
                    {
                        t_txt_CustomerInvPrice.IsReadOnly = false;
                        t_txt_CustomerInvNo.IsReadOnly = false;
                        t_dtp_CustomerInvDate.IsReadOnly = false;
                    }
                    else
                    {
                        t_txt_CustomerInvPrice.Text = string.Empty;
                        t_txt_CustomerInvNo.Text = string.Empty;
                        t_dtp_CustomerInvDate.Text = string.Empty;
                        t_txt_CustomerInvPrice.IsReadOnly = true;
                        t_txt_CustomerInvNo.IsReadOnly = true;
                        t_dtp_CustomerInvDate.IsReadOnly = true;
                    }
                    break;
                case "t_txt_CustomerInvPrice":
                    myModel.Customer_Inv_Price = t_txt_CustomerInvPrice.Text;
                    ValidateIsAllCustomer();
                    break;
                case "t_txt_CustomerInvNo":
                    myModel.Customer_Inv_No = t_txt_CustomerInvNo.Text;
                    break;
                case "t_dtp_CustomerInvDate":
                    if (t_dtp_CustomerInvDate.Value == DateTime.Parse("1900-01-01"))
                        myModel.Customer_Inv_Date = null;
                    else
                        myModel.Customer_Inv_Date = t_dtp_CustomerInvDate.Value;
                    break;
            }
            t_dge_CustomBill.Items.Refresh();
        }
        /// <summary>
        /// 机构值变更事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void Agency_Control_TextChanged(object sender, RoutedEventArgs e)
        {
            Control myControl = sender as Control;
            if (myControl == null || (m_IsAgencyGridLoad && myControl.Name != "t_chk_IsAgencyInv")) return;

            PT_B_Project_Agency myModel = t_dge_Agency.SelectedItem as PT_B_Project_Agency;
            if (myModel == null) return;
            switch (myControl.Name)
            {
                case "t_txt_AgencyMoney":
                    myModel.Agency_Money = t_txt_AgencyMoney.Text;
                    SumAgencyAccountsPrepaid();
                    break;
                case "t_txt_Agency":
                    myModel.Agency = t_txt_Agency.Text;
                    break;
                case "t_txt_AgencyRemark":
                    myModel.Agency_Remark = t_txt_AgencyRemark.Text;
                    break;
                case "t_dtp_AgencyDate":
                    myModel.Agency_Date = t_dtp_AgencyDate.Value;
                    break;
                case "t_chk_IsAgencyInv":
                    myModel.Is_Agency_Inv = t_chk_IsAgencyInv.IsChecked == true ? "是" : "否";
                    if (t_chk_IsAgencyInv.IsChecked == true)
                    {
                        t_txt_AgencyInvPrice.IsReadOnly = false;
                        t_txt_AgencyInvNo.IsReadOnly = false;
                        t_dtp_AgencyInvDate.IsReadOnly = false;
                    }
                    else
                    {
                        t_txt_AgencyInvPrice.Text = string.Empty;
                        t_txt_AgencyInvNo.Text = string.Empty;
                        t_dtp_AgencyInvDate.Text = string.Empty;
                        t_txt_AgencyInvPrice.IsReadOnly = true;
                        t_txt_AgencyInvNo.IsReadOnly = true;
                        t_dtp_AgencyInvDate.IsReadOnly = true;
                    }
                    break;
                case "t_txt_AgencyInvPrice":
                    myModel.Agency_Inv_Price = t_txt_AgencyInvPrice.Text;
                    ValidateIsAllAgency();
                    break;
                case "t_txt_AgencyInvNo":
                    myModel.Agency_Inv_No = t_txt_AgencyInvNo.Text;
                    break;
                case "t_dtp_AgencyInvDate":
                    if (t_dtp_AgencyInvDate.Value == DateTime.Parse("1900-01-01"))
                        myModel.Agency_Inv_Date = null;
                    else
                        myModel.Agency_Inv_Date = t_dtp_AgencyInvDate.Value;
                    break;
            }
            t_dge_Agency.Items.Refresh();
        }
        /// <summary>
        /// 外包值变更事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void Lab_Control_TextChanged(object sender, RoutedEventArgs e)
        {
            Control myControl = sender as Control;
            if (myControl == null || (m_IsLabGridLoad && myControl.Name != "t_chk_IsLabInv")) return;

            PT_B_Project_Lab myModel = t_dge_Lab.SelectedItem as PT_B_Project_Lab;
            if (myModel == null) return;
            switch (myControl.Name)
            {
                case "t_txt_LabMoney":
                    myModel.Lab_Money = t_txt_LabMoney.Text;
                    SumLabAccountsPrepaid();
                    break;
                case "t_txt_Lab":
                    myModel.Lab = t_txt_Lab.Text;
                    break;
                case "t_txt_LabRemark":
                    myModel.Lab_Remark = t_txt_LabRemark.Text;
                    break;
                case "t_dtp_LabDate":
                    myModel.Lab_Date = t_dtp_LabDate.Value;
                    break;
                case "t_chk_IsLabInv":
                    myModel.Is_Lab_Inv = t_chk_IsLabInv.IsChecked == true ? "是" : "否";
                    if (t_chk_IsLabInv.IsChecked == true)
                    {
                        t_txt_LabInvPrice.IsReadOnly = false;
                        t_txt_LabInvNo.IsReadOnly = false;
                        t_dtp_LabInvDate.IsReadOnly = false;
                    }
                    else
                    {
                        t_txt_LabInvPrice.Text = string.Empty;
                        t_txt_LabInvNo.Text = string.Empty;
                        t_dtp_LabInvDate.Text = string.Empty;
                        t_txt_LabInvPrice.IsReadOnly = true;
                        t_txt_LabInvNo.IsReadOnly = true;
                        t_dtp_LabInvDate.IsReadOnly = true;
                    }
                    break;
                case "t_txt_LabInvPrice":
                    myModel.Lab_Inv_Price = t_txt_LabInvPrice.Text;
                    ValidateIsAllLab();
                    break;
                case "t_txt_LabInvNo":
                    myModel.Lab_Inv_No = t_txt_LabInvNo.Text;
                    break;
                case "t_dtp_LabInvDate":
                    if (t_dtp_LabInvDate.Value == DateTime.Parse("1900-01-01"))
                        myModel.Lab_Inv_Date = null;
                    else
                        myModel.Lab_Inv_Date = t_dtp_LabInvDate.Value;
                    break;
            }
            t_dge_Lab.Items.Refresh();
        }
        /// <summary>
        /// 其他值变更事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void Other_Control_TextChanged(object sender, RoutedEventArgs e)
        {
            Control myControl = sender as Control;
            if (myControl == null || (m_IsOtherGridLoad && myControl.Name != "t_chk_IsOtherInv")) return;

            PT_B_Project_Other myModel = t_dge_Other.SelectedItem as PT_B_Project_Other;
            if (myModel == null) return;
            switch (myControl.Name)
            {
                case "t_txt_OtherMoney":
                    myModel.Other_Money = t_txt_OtherMoney.Text;
                    SumOtherAccountsPrepaid();
                    break;
                case "t_txt_Other":
                    myModel.Other = t_txt_Other.Text;
                    break;
                case "t_txt_OtherRemark":
                    myModel.Other_Remark = t_txt_OtherRemark.Text;
                    break;
                case "t_dtp_OtherDate":
                    myModel.Other_Date = t_dtp_OtherDate.Value;
                    break;
                case "t_chk_IsOtherInv":
                    myModel.Is_Other_Inv = t_chk_IsOtherInv.IsChecked == true ? "是" : "否";
                    if (t_chk_IsOtherInv.IsChecked == true)
                    {
                        t_txt_OtherInvPrice.IsReadOnly = false;
                        t_txt_OtherInvNo.IsReadOnly = false;
                        t_dtp_OtherInvDate.IsReadOnly = false;
                    }
                    else
                    {
                        t_txt_OtherInvPrice.Text = string.Empty;
                        t_txt_OtherInvNo.Text = string.Empty;
                        t_dtp_OtherInvDate.Text = string.Empty;
                        t_txt_OtherInvPrice.IsReadOnly = true;
                        t_txt_OtherInvNo.IsReadOnly = true;
                        t_dtp_OtherInvDate.IsReadOnly = true;
                    }
                    break;
                case "t_txt_OtherInvPrice":
                    myModel.Other_Inv_Price = t_txt_OtherInvPrice.Text;
                    ValidateIsOtherLab();
                    break;
                case "t_txt_OtherInvNo":
                    myModel.Other_Inv_No = t_txt_OtherInvNo.Text;
                    break;
                case "t_dtp_OtherInvDate":
                    if (t_dtp_OtherInvDate.Value == DateTime.Parse("1900-01-01"))
                        myModel.Other_Inv_Date = null;
                    else
                        myModel.Other_Inv_Date = t_dtp_OtherInvDate.Value;
                    break;
            }
            t_dge_Other.Items.Refresh();
        }

        #endregion

        /// <summary>
        /// 客户信息保存按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_btn_CustomerSave_Click(object sender, RoutedEventArgs e)
        {
            //客户资金往来
            var customer = from p in m_Entities.PT_B_Project_Customer
                           where p.Project_No == PTBProject.Project_No
                           orderby p.Seq_No
                           select p;


            foreach (PT_B_Project_Customer myModel in customer)
            {
                m_Entities.PT_B_Project_Customer.Remove(myModel);
            }

            m_Entities.SaveChanges();

            int seqNo = 1;
            foreach (PT_B_Project_Customer myModel in m_CustomerList)
            {
                if (string.IsNullOrEmpty(myModel.Customer_Pays_Id))
                    myModel.Customer_Pays_Id = Guid.NewGuid().ToString("N");
                myModel.Project_No = PTBProject.Project_No;
                myModel.Seq_No = seqNo;
                seqNo++;
                m_Entities.PT_B_Project_Customer.Add(myModel);
            }
            m_Entities.SaveChanges();

            XMessageBox.Enter("保存成功", this);


            //GirdStyleConfig.ItemsSource = m_CustomerList;
            //t_dge_CustomBill.StyleConfig = GirdStyleConfig;

            //t_dge_CustomBill.ItemsSource = m_CustomerList;

            //if (m_CustomerList != null && m_CustomerList.Count > 0)
            //{
            //    t_dge_CustomBill.SelectedIndex = 0;
            //    t_dge_CustomBill.Focus();
            //}
            //else
            //{
            //    t_btn_CustomerAdd_Click(null, null);
            //}
            ////机构资金往来
            //var agency = from p in m_Entities.PT_B_Project_Agency
            //             where p.Project_No == PTBProject.Project_No
            //             orderby p.Seq_No
            //             select p;
            //AgencyGirdStyleConfig.ItemsSource = agency.ToList();
            //t_dge_Agency.StyleConfig = AgencyGirdStyleConfig;
            ////外包资金往来
            //var lab = from p in m_Entities.PT_B_Project_Customer
            //          where p.Project_No == PTBProject.Project_No
            //          orderby p.Seq_No
            //          select p;
            //LabGirdStyleConfig.ItemsSource = lab.ToList();
            //t_dge_Lab.StyleConfig = LabGirdStyleConfig;
            ////其他资金往来
            //var other = from p in m_Entities.PT_B_Project_Customer
            //            where p.Project_No == PTBProject.Project_No
            //            orderby p.Seq_No
            //            select p;
            //OtherGirdStyleConfig.ItemsSource = other.ToList();
            //t_dge_Other.StyleConfig = OtherGirdStyleConfig;
        }
        /// <summary>
        /// 事件注册
        /// </summary>
        private void RegisterEvent()
        {
            RegisterEvent(t_grp_BaseInfo.Children);
            RegisterEvent(t_grp_Money.Children);
            RegisterEvent(t_grp_Customer.Children);
            RegisterEvent(t_grp_Agency.Children);
            RegisterEvent(t_grp_Lab.Children);
            RegisterEvent(t_grp_Other.Children);
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
            if (!m_IsLoad && !m_IsLabGridLoad)
                m_IsModify = true;
        }

        private void myValueChanged(object sender, RoutedEventArgs e)
        {
            if (!m_IsLoad && !m_IsLabGridLoad)
                m_IsModify = true;
        }

        private void myTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!m_IsLoad && !m_IsLabGridLoad)
                m_IsModify = true;
        }

        private void XBaseForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (m_IsModify)
            {
                MessageResult myResult = XMessageBox.Select("当前单据已修改，是否保存？", this);
                if (myResult == MessageResult.Yes)
                {
                    SaveMethod();
                }
                else if (myResult == MessageResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
        /// <summary>
        /// 查看原始报价单
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_tsb_Back_Click(object sender, RoutedEventArgs e)
        {
            if (m_IsModify)
            {
                MessageResult myResult = XMessageBox.Ask("当前单据已修改，是否保存？", this);
                if (myResult == MessageResult.Yes)
                {
                    SaveMethod();
                    m_IsModify = false;
                }
            }
            var temp = from p in m_Entities.PT_B_Quotation
                       where p.Quotation_No == PTBProject.Quotation_No
                       select p;

            foreach (PT_B_Quotation model in temp)
            {
                this.Visibility = Visibility.Hidden;
                FrmQuotation myForm = new FrmQuotation();
                myForm.PTBQuotation = model;
                myForm.ShowDialog();
                this.Close();
            }

        }
    }
}

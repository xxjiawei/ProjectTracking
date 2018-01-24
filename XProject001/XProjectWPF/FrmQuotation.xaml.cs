using Microsoft.Win32;
using RJ.XStyle;
using RJ.XStyle.Model;
using System;
using System.Collections.Generic;
using System.IO;
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
        private ProjectTrackingEntities m_Entities = new ProjectTrackingEntities();
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
            SaveMethod();
            XMessageBox.Enter("保存成功", this);
            m_IsModify = false;
        }
        /// <summary>
        /// 保存方法
        /// </summary>
        private void SaveMethod()
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
            {
                SaveModel(t_txt_QuotationNo.Text);
                m_Entities.Entry(PTBQuotation).State = System.Data.Entity.EntityState.Modified;
            }
            m_Entities.SaveChanges();
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
            t_dtp_QuotationDate.Value = PTBQuotation.Quotation_Date == null ? DateTime.Parse("1900-01-01") : DateTime.Parse(PTBQuotation.Quotation_Date.ToString());
            t_txt_FollowMan.Text = PTBQuotation.Follow_Man;
            t_txt_ProductModel.Text = PTBQuotation.Product_Model;
            t_txt_ProjectName.Text = PTBQuotation.Project_Name;
            t_txt_Price.Text = PTBQuotation.Price;
            t_chk_IsTax.IsChecked = PTBQuotation.Is_Tax == "是" ? true : false;
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

            //加载关联项目单表格
            var customer = from p in m_Entities.PT_B_Project
                           where p.Quotation_No == PTBQuotation.Quotation_No
                           orderby p.Project_No
                           select p;

            t_dge_Project.ItemsSource = customer.ToList();
            if (customer.ToList() != null && customer.ToList().Count > 0)
            {
                t_dge_Project.SelectedIndex = 0;
                t_dge_Project.Focus();
            }

            m_IsLoad = false;
        }

        private void SaveModel(string newQuotationNo)
        {
            PTBQuotation.Quotation_No = newQuotationNo;
            PTBQuotation.Quotation_Date = t_dtp_QuotationDate.Value;
            PTBQuotation.Follow_Man = t_txt_FollowMan.Text;
            PTBQuotation.Product_Model = t_txt_ProductModel.Text;
            PTBQuotation.Project_Name = t_txt_ProjectName.Text;
            PTBQuotation.Price = t_txt_Price.Text;
            PTBQuotation.Is_Tax = t_chk_IsTax.IsChecked == true ? "是" : "否";
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
            if (m_IsModify || t_txt_QuotationNo.Text == "新单")
            {
                XMessageBox.Enter("当前单据尚未保存，请保存后再操作！", this);
                return;
            }
            m_IsModify = false;
            this.Visibility = Visibility.Hidden;

            PT_B_Project myModel = new PT_B_Project();
            myModel.Quotation_No = PTBQuotation.Quotation_No;
            myModel.Quotation_Date = PTBQuotation.Quotation_Date;
            myModel.Follow_Man = PTBQuotation.Follow_Man;
            myModel.Product_Model = PTBQuotation.Product_Model;
            myModel.Project_Name = PTBQuotation.Project_Name;
            myModel.Price = PTBQuotation.Price;
            myModel.Is_Tax = PTBQuotation.Is_Tax;
            myModel.Project_Type = PTBQuotation.Quotation_Type;
            myModel.Cycle_Time = PTBQuotation.Cycle_Time;
            myModel.Company_Name = PTBQuotation.Company_Name;
            myModel.Company_Address = PTBQuotation.Company_Address;
            myModel.Contact_Man = PTBQuotation.Contact_Man;
            myModel.Tel = PTBQuotation.Tel;
            myModel.Email = PTBQuotation.Email;
            myModel.Fax = PTBQuotation.Fax;
            myModel.Remark = PTBQuotation.Remark;
            myModel.Oper_Time = DateTime.Now;
            myModel.Account_Receivable = PTBQuotation.Price;
            myModel.Profits = PTBQuotation.Price;
            FrmProject myForm = new FrmProject();
            myForm.PTBProject = myModel;
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
                MessageResult myResult = XMessageBox.Ask("当前单据尚未保存，是否继续？", this);
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
        /// <summary>
        /// 窗体关闭前事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
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

        private void t_dge_Project_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            PT_B_Project myModel = (PT_B_Project)t_dge_Project.SelectedItem;
            FrmProject myForm = new FrmProject();
            myForm.PTBProject = myModel;
            myForm.ShowDialog();
            this.Close();
        }

        private void t_tsb_ExportWord_Click(object sender, RoutedEventArgs e)
        {

            object oMissing = System.Reflection.Missing.Value;
            //创建一个Word应用程序实例
            Microsoft.Office.Interop.Word._Application oWord = new Microsoft.Office.Interop.Word.Application();
            //设置为不可见
            oWord.Visible = false;
            //模板文件地址，这里假设在X盘根目录
            object oTemplate = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Template\\Template.doc";
            //以模板为基础生成文档
            Microsoft.Office.Interop.Word._Document oDoc = oWord.Documents.Add(ref oTemplate, ref oMissing, ref oMissing, ref oMissing);
            //声明书签数组
            object[] oBookMark = new object[4];
            //赋值书签名
            oBookMark[0] = "Company_Name";
            oBookMark[1] = "Tel";
            oBookMark[2] = "Email";
            oBookMark[3] = "Fax";
            //赋值任意数据到书签的位置
            oDoc.Bookmarks.get_Item(ref oBookMark[0]).Range.Text = PTBQuotation.Company_Name;
            oDoc.Bookmarks.get_Item(ref oBookMark[1]).Range.Text = PTBQuotation.Tel;
            oDoc.Bookmarks.get_Item(ref oBookMark[2]).Range.Text = PTBQuotation.Email;
            oDoc.Bookmarks.get_Item(ref oBookMark[3]).Range.Text = PTBQuotation.Fax;
            //弹出保存文件对话框，保存生成的Word
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Word Document(*.doc)|*.doc";
            sfd.DefaultExt = "Word Document(*.doc)|*.doc";
            if (sfd.ShowDialog() == true)
            {
                object filename = sfd.FileName;

                oDoc.SaveAs(ref filename, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing);
                oDoc.Close(ref oMissing, ref oMissing, ref oMissing);
                //关闭word
                oWord.Quit(ref oMissing, ref oMissing, ref oMissing);

                System.Diagnostics.Process.Start(sfd.FileName);
            }

        }
    }
}

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

//EF更新数据时，不要去更新主键，会报错System.Data.Entity.Infrastructure.DbUpdateConcurrencyException: Store update, insert, or delete statement affected an unexpected number of rows (0). Entities may have been modified or deleted since entities were loaded. 


namespace XProjectWPF
{
    /// <summary>
    /// FrmQuotation.xaml 的交互逻辑
    /// </summary>
    public partial class FrmQuotation : XBaseForm
    {
        #region 构造函数

        public FrmQuotation()
        {
            InitializeComponent();
            RegisterEvent();
        }

        #endregion

        #region 字段

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
        /// 标识是否手动生成单号
        /// </summary>
        private bool m_IsHand = false;

        #endregion

        #region 界面事件

        /// <summary>
        /// 保存按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_tsb_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isSuccess = SaveMethod();
                if (isSuccess)
                {
                    XMessageBox.Enter("保存成功", this);
                    m_IsModify = false;
                }
            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }
       
        }
        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void XBaseForm_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PTBQuotation == null)
                    CreateNewQuotationModel();
                LoadControlsValue();

                Thread myThread = new Thread(SetMenuEnabel);
                myThread.IsBackground = true;
                myThread.Start();
            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }

        }
        /// <summary>
        /// 生成项目单
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_tsb_CreateProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (m_IsModify || t_txt_QuotationNo.Text == "新单")
                {
                    XMessageBox.Enter("当前单据尚未保存，请保存后再操作！", this);
                    return;
                }
                m_IsModify = false;
                this.Visibility = Visibility.Hidden;

                PT_B_Project myModel = new PT_B_Project();
                myModel.Project_Id = Guid.NewGuid().ToString("N");
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
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }

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
            try
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
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }

        }
        /// <summary>
        /// 复制按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_tsb_Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (m_IsModify || PTBQuotation.Quotation_No == "新单")
                {
                    XMessageBox.Warning("当前单据尚未保存，请保存后再操作！", this);
                    return;
                }
                PT_B_Quotation copyModel = new PT_B_Quotation();
                copyModel.Quotation_Id = Guid.NewGuid().ToString("N");
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
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }

        }
        /// <summary>
        /// 窗体关闭前事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void XBaseForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }

        }
        /// <summary>
        /// 项目单窗口双击事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_dge_Project_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (t_dge_Project.Items.Count == 0)
                    return;

                this.Visibility = Visibility.Hidden;
                PT_B_Project myModel = (PT_B_Project)t_dge_Project.SelectedItem;
                FrmProject myForm = new FrmProject();
                myForm.PTBProject = myModel;
                myForm.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }
        }
        /// <summary>
        /// 导出word功能
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
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
        /// <summary>
        /// 单号手动录入功能
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_tsb_Change_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBlock myBlock = t_tsb_Change.Header as TextBlock;

                if (myBlock.Text == "单号手动录入")
                {
                    //手动生成单号处理
                    myBlock.Text = "单号自动生成";
                    m_IsHand = true;
                    t_txt_QuotationNo.IsReadOnly = false;
                }
                else
                {
                    myBlock.Text = "单号手动录入";
                    m_IsHand = false;
                    t_txt_QuotationNo.IsReadOnly = true;
                    t_txt_QuotationNo.Text = PTBQuotation.Quotation_No;
                }
            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 保存方法
        /// </summary>
        private bool SaveMethod()
        {
            //新单模式
            if (string.IsNullOrEmpty(PTBQuotation.Bill_Status))
            {
                string newQuotationNo = string.Empty;
                //手动录入单号情况
                if (m_IsHand)
                {
                    //校验是否新单
                    if (t_txt_QuotationNo.Text.Trim() == "新单" || string.IsNullOrEmpty(t_txt_QuotationNo.Text.Trim()))
                    {
                        XMessageBox.Warning("报价单号不能为空或包含【新单】字符！");
                        t_txt_QuotationNo.Focus();
                        return false;
                    }
                    //校验是否重号
                    var temp = from p in m_Entities.PT_B_Quotation
                               where p.Quotation_No == t_txt_QuotationNo.Text.Trim()
                               select p;
                    if (temp.ToList().Count > 0)
                    {
                        string mes = string.Format("报价单号：{0} 已存在，请修改！", t_txt_QuotationNo.Text.Trim());
                        XMessageBox.Warning(mes);
                        t_txt_QuotationNo.Focus();
                        return false;
                    }
                    newQuotationNo = t_txt_QuotationNo.Text.Trim();
                }
                else
                    //单号自动生成
                    newQuotationNo = m_SerialNumberMethod.GetMaxQNumber();

                PTBQuotation.Quotation_Id = Guid.NewGuid().ToString("N");
                SaveModel(newQuotationNo);
                m_Entities.PT_B_Quotation.Add(PTBQuotation);
                t_txt_QuotationNo.Text = newQuotationNo;
                t_tslStateText.Text = "已入库";
            }
            else
            {
                //打开单证，手动录入单号情况
                if (m_IsHand)
                {
                    //校验是否新单
                    if (t_txt_QuotationNo.Text.Trim() == "新单" || string.IsNullOrEmpty(t_txt_QuotationNo.Text.Trim()))
                    {
                        XMessageBox.Warning("报价单号不能为空或包含【新单】字符！");
                        t_txt_QuotationNo.Focus();
                        return false;
                    }
                    //当报价单号有修改时
                    if (t_txt_QuotationNo.Text.Trim() != PTBQuotation.Quotation_No)
                    {
                        //读取数据库判断是否有重号
                        ProjectTrackingEntities qEntities = new ProjectTrackingEntities();
                        var temp = from p in qEntities.PT_B_Quotation
                                   where p.Quotation_No == t_txt_QuotationNo.Text.Trim()
                                   select p;
                        if (temp.ToList().Count > 0)
                        {
                            string mes = string.Format("报价单号：{0} 已存在，请修改！", t_txt_QuotationNo.Text.Trim());
                            XMessageBox.Warning(mes);
                            t_txt_QuotationNo.Focus();
                            return false;
                        }
                        ProjectTrackingEntities pEntities = new ProjectTrackingEntities();
                        //读取原报价单所关联的项目单
                        var project = from p in pEntities.PT_B_Project
                                      where p.Quotation_No == PTBQuotation.Quotation_No
                                      orderby p.Project_No
                                      select p;
                        //修改项目单的关联报价单号
                        foreach (PT_B_Project projectModel in project)
                        {
                            projectModel.Quotation_No = t_txt_QuotationNo.Text.Trim();
                            pEntities.Entry(projectModel).State = System.Data.Entity.EntityState.Modified;
                        }
                        if (project.ToList().Count > 0)
                        {
                            pEntities.SaveChanges();
                        }
                    }
                }
                SaveModel(t_txt_QuotationNo.Text);
                m_Entities.Entry(PTBQuotation).State = System.Data.Entity.EntityState.Modified;
            }
            m_Entities.SaveChanges();
            return true;
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
        /// <summary>
        /// 保存对象值
        /// </summary>
        /// <param name="newQuotationNo">报价单号</param>
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
                else if(element is XRadioButton)
                {
                    (element as XRadioButton).Checked += myChecked;
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
        /// <summary>
        /// 复选框触发事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void myChecked(object sender, RoutedEventArgs e)
        {
            if (!m_IsLoad)
                m_IsModify = true;
        }
        /// <summary>
        /// 值变更事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void myValueChanged(object sender, RoutedEventArgs e)
        {
            if (!m_IsLoad)
                m_IsModify = true;
        }
        /// <summary>
        /// 值变更事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void myTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!m_IsLoad)
                m_IsModify = true;
        }

        #endregion
    }
}

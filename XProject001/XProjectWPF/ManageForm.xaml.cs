using Microsoft.Win32;
using RJ.Common.DBUtility;
using RJ.XStyle;
using RJ.XStyle.GridEx;
using RJ.XStyle.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
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
    /// ManageForm.xaml 的交互逻辑
    /// </summary>
    public partial class ManageForm : XBaseForm
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public ManageForm()
        {
            InitializeComponent();
        }

        #endregion

        #region 字段

        /// <summary>
        /// 数据库操作类
        /// </summary>
        private ProjectTrackingEntities m_Entities = new ProjectTrackingEntities();
        /// <summary>
        /// 报价单表格风格配置
        /// </summary>
        private DataGridStyleConfig m_GirdStyleConfig;
        /// <summary>
        /// 报价单表格风格配置
        /// </summary>
        public DataGridStyleConfig GirdStyleConfig
        {
            get
            {
                if (m_GirdStyleConfig == null)
                {
                    m_GirdStyleConfig = new DataGridStyleConfig();

                    m_GirdStyleConfig.Add("Quotation_No", "报价单号", 90);
                    m_GirdStyleConfig.Add("Company_Name", "公司名称", 140);
                    m_GirdStyleConfig.Add("Project_Name", "测试项目", 180);
                    m_GirdStyleConfig.Add("Price", "当前报价", 90);
                    m_GirdStyleConfig.Add("Is_Tax", "含税", 40);
                    m_GirdStyleConfig.Add("Quotation_Type", "报价类型", 80);
                    m_GirdStyleConfig.AddDateTime("Quotation_Date", "报价日期", 100);
                    m_GirdStyleConfig.Add("Product_Model", "产品/型号", 80);
                    m_GirdStyleConfig.Add("Cycle_Time", "认证周期", 80);
                    m_GirdStyleConfig.Add("Follow_Man", "业务跟进人", 80);
                    m_GirdStyleConfig.Add("Contact_Man", "联系人", 80);
                    m_GirdStyleConfig.Add("Tel", "联系电话", 100);
                    m_GirdStyleConfig.Add("Email", "电子邮箱", 100);
                    m_GirdStyleConfig.Add("Fax", "传真", 100);
                    m_GirdStyleConfig.Add("Remark", "备注", 150);
                }
                return m_GirdStyleConfig;
            }
        }
        /// <summary>
        /// 项目单表格风格配置
        /// </summary>
        private DataGridStyleConfig m_ProjectGirdStyleConfig;
        /// <summary>
        /// 项目单表格风格配置
        /// </summary>
        public DataGridStyleConfig ProjectGirdStyleConfig
        {
            get
            {
                if (m_ProjectGirdStyleConfig == null)
                {
                    m_ProjectGirdStyleConfig = new DataGridStyleConfig();

                    m_ProjectGirdStyleConfig.Add("Project_No", "项目单号", 90);
                    m_ProjectGirdStyleConfig.Add("Quotation_No", "关联报价单号", 90);
                    m_ProjectGirdStyleConfig.Add("Company_Name", "公司名称", 140);
                    m_ProjectGirdStyleConfig.Add("Project_Name", "测试项目", 180);
                    m_ProjectGirdStyleConfig.Add("Price", "当前报价", 90);
                    m_ProjectGirdStyleConfig.Add("Profits", "测试总利润", 90);
                    m_ProjectGirdStyleConfig.Add("Is_Pads", "垫付", 40);
                    m_ProjectGirdStyleConfig.Add("Project_Type", "项目类型", 80);
                    m_ProjectGirdStyleConfig.Add("Account_Receivable", "应收客户账款", 90);
                    m_ProjectGirdStyleConfig.Add("Agency_Account_Payable", "应付机构账款", 90);
                    m_ProjectGirdStyleConfig.Add("Lab_Account_Payable", "应付外包账款", 90);
                    m_ProjectGirdStyleConfig.Add("Other_Account", "其他费用", 90);
                    m_ProjectGirdStyleConfig.AddDateTime("Quotation_Date", "报价日期", 80);
                    m_ProjectGirdStyleConfig.Add("Follow_Man", "业务跟进人", 70);
                    m_ProjectGirdStyleConfig.Add("Contact_Man", "联系人", 60);
                    m_ProjectGirdStyleConfig.Add("Tel", "联系电话", 90);
                    m_ProjectGirdStyleConfig.Add("Email", "电子邮箱", 100);
                    m_ProjectGirdStyleConfig.Add("Fax", "传真", 100);
                    m_ProjectGirdStyleConfig.Add("Remark", "备注", 150);
                }
                return m_ProjectGirdStyleConfig;
            }
        }
        /// <summary>
        /// 报价单树节点数量字典
        /// </summary>
        private Dictionary<string, int> m_TreeNodeDictionary;
        /// <summary>
        /// 报价单树节点数量字典
        /// </summary>
        public Dictionary<string, int> TreeNodeDictionary
        {
            get
            {
                if (m_TreeNodeDictionary == null)
                {
                    m_TreeNodeDictionary = new Dictionary<string, int>();
                    m_TreeNodeDictionary.Add("Q", 0);
                    m_TreeNodeDictionary.Add("1", 0);
                    m_TreeNodeDictionary.Add("R", 0);
                    m_TreeNodeDictionary.Add("A", 0);
                }
                return m_TreeNodeDictionary;
            }
        }
        /// <summary>
        /// 项目单树节点数量字典
        /// </summary>
        private Dictionary<string, int> m_TreeNodeProjectDic;
        /// <summary>
        /// 项目单树节点数量字典
        /// </summary>
        public Dictionary<string, int> TreeNodeProjectDic
        {
            get
            {
                if (m_TreeNodeProjectDic == null)
                {
                    m_TreeNodeProjectDic = new Dictionary<string, int>();
                    m_TreeNodeProjectDic.Add("P", 0);
                    m_TreeNodeProjectDic.Add("1", 0);
                    m_TreeNodeProjectDic.Add("R", 0);
                    m_TreeNodeProjectDic.Add("A", 0);
                }
                return m_TreeNodeProjectDic;
            }
        }
        /// <summary>
        /// 时间选中类型
        /// </summary>
        private DateFilterTypes m_CurrentTypes = DateFilterTypes.All;
        /// <summary>
        /// 查询对象
        /// </summary>
        private MQuery m_MQuery;

        #endregion

        #region 界面事件

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void XBaseForm_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //绑定树节点
                BindTreeNode();
                //选中首节点
                t_tvw_Module.Focus();
                TreeViewItem myItem = t_tvw_Module.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
                myItem.IsSelected = true;

            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }
           
        }
        /// <summary>
        /// 树形图选择切换
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_tvw_Module_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                LoadBusinessData();
            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }
        }
        /// <summary>
        /// 新建报价单窗口
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_btn_New_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FrmQuotation myForm = new FrmQuotation();
                myForm.ShowDialog();
                m_Entities.Dispose();
                m_Entities = new ProjectTrackingEntities();
                RefreshTreeNode();
            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }
      
        }
        /// <summary>
        /// 打开窗口按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_btn_Open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (t_pgg_Bill.SelectedItem == null)
                {
                    XMessageBox.Warning("未选中一条单据！", this);
                    return;
                }
                t_pgg_Bill_MouseDoubleClick(null, null);

            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }
       
        }
        /// <summary>
        /// 表格行鼠标双击事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_pgg_Bill_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (t_pgg_Bill.SelectedItem == null) return;
                XTreeNode myItem = t_tvw_Module.SelectedItem as XTreeNode;
                string myTag = string.Empty;
                string parentTag = string.Empty;
                if (myItem == null && m_MQuery != null)
                {
                    myTag = m_MQuery.BillType;
                }
                else
                {
                    myTag = myItem.Tag.ToString();
                    if (myItem.Parent != null)
                        parentTag = myItem.Parent.Tag.ToString();
                }
                if (myTag == "Q" || parentTag == "Q")
                {
                    PT_B_Quotation myModel = (PT_B_Quotation)t_pgg_Bill.SelectedItem;
                    FrmQuotation myForm = new FrmQuotation();
                    myForm.PTBQuotation = myModel;
                    myForm.ShowDialog();
                    m_Entities.Dispose();
                    m_Entities = new ProjectTrackingEntities();
                    RefreshTreeNode();
                }
                else
                {
                    PT_B_Project myModel = (PT_B_Project)t_pgg_Bill.SelectedItem;
                    FrmProject myForm = new FrmProject();
                    myForm.PTBProject = myModel;
                    myForm.ShowDialog();
                    m_Entities.Dispose();
                    m_Entities = new ProjectTrackingEntities();
                    RefreshTreeNode();
                }
            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }
        }
        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_tsm_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 删除按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (t_pgg_Bill.SelectedItem == null)
                {
                    XMessageBox.Warning("未选中一条单据！", this);
                    return;
                }
                XTreeNode myItem = t_tvw_Module.SelectedItem as XTreeNode;
                string myTag = string.Empty;
                string parentTag = string.Empty;
                if (myItem == null && m_MQuery != null)
                {
                    myTag = m_MQuery.BillType;
                }
                else
                {
                    myTag = myItem.Tag.ToString();
                    if (myItem.Parent != null)
                        parentTag = myItem.Parent.Tag.ToString();
                }
                if (myTag == "Q" || parentTag == "Q")
                {
                    PT_B_Quotation myModel = (PT_B_Quotation)t_pgg_Bill.SelectedItem;
                    if (myModel.Bill_Status != "R")
                    {
                        MessageResult myResult = XMessageBox.Ask("确认删除当前选中的单据？", this);

                        if (myResult == MessageResult.Yes)
                        {
                            myModel.Bill_Status = "R";
                            m_Entities.SaveChanges();
                            RefreshTreeNode();
                        }
                    }
                    else
                    {
                        MessageResult myResult = XMessageBox.Ask("确认彻底删除当前选中的单据？", this);
                        if (myResult == MessageResult.Yes)
                        {
                            m_Entities.PT_B_Quotation.Remove(myModel);
                            m_Entities.SaveChanges();
                            RefreshTreeNode();
                        }
                    }
                }
                else
                {
                    PT_B_Project myModel = (PT_B_Project)t_pgg_Bill.SelectedItem;
                    if (myModel.Bill_Status != "R")
                    {
                        MessageResult myResult = XMessageBox.Ask("确认删除当前选中的单据？", this);

                        if (myResult == MessageResult.Yes)
                        {
                            myModel.Bill_Status = "R";
                            m_Entities.SaveChanges();
                            RefreshTreeNode();
                        }
                    }
                    else
                    {
                        MessageResult myResult = XMessageBox.Ask("确认彻底删除当前选中的单据？", this);
                        if (myResult == MessageResult.Yes)
                        {
                            m_Entities.PT_B_Project.Remove(myModel);
                            m_Entities.SaveChanges();
                            RefreshTreeNode();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }
        }
        /// <summary>
        /// 查询按钮点击事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_tsb_Query_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FrmQuery myForm = new FrmQuery();
                myForm.ShowDialog();
                m_MQuery = myForm.QueryModel;

                QueryMethod();
            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }

        }
        /// <summary>
        /// 开始时间值改变事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_dtp_StartDate_ValueChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                BindTreeNode();

            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }

        }
        /// <summary>
        /// 结束时间值改变事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_dtp_EndDate_ValueChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                BindTreeNode();

            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }
        }
        /// <summary>
        /// 打开按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_tsb_Open_Click(object sender, RoutedEventArgs e)
        {
            t_btn_Open_Click(null, null);
        }
        /// <summary>
        /// 删除按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void t_tsb_Delete_Click(object sender, RoutedEventArgs e)
        {
            t_btn_Delete_Click(null, null);
        }
        /// <summary>
        /// 右键点击前事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_pgg_Bill_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ArrayList statusList = new ArrayList();
                statusList.Add("已入库");
                statusList.Add("回收站");
                statusList.Add("归档");
                XTreeNode myItem = t_tvw_Module.SelectedItem as XTreeNode;
                string myTag = string.Empty;
                string parentTag = string.Empty;
                if (myItem == null && m_MQuery != null)
                {
                    myTag = m_MQuery.BillType;
                }
                else
                {
                    myTag = myItem.Tag.ToString();
                    if (myItem.Parent != null)
                        parentTag = myItem.Parent.Tag.ToString();
                }
                string billStatus = string.Empty;
                if (myTag == "Q" || parentTag == "Q")
                {
                    PT_B_Quotation myModel = (PT_B_Quotation)t_pgg_Bill.SelectedItem;
                    statusList.Remove(SetBillStatus(myModel.Bill_Status));
                    billStatus = "Q";
                }
                else
                {
                    PT_B_Project myModel = (PT_B_Project)t_pgg_Bill.SelectedItem;
                    statusList.Remove(SetBillStatus(myModel.Bill_Status));
                    billStatus = "P";
                }
                t_tsb_ChangeState.Items.Clear();
                foreach (string status in statusList)
                {
                    MenuItem item = new MenuItem();
                    item.Header = status;
                    item.Tag = billStatus;
                    item.Click += Item_Click;
                    t_tsb_ChangeState.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }
        }
        /// <summary>
        ///  改变状态方法
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void Item_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem myItem = sender as MenuItem;

                if (myItem.Tag.ToString() == "Q")
                {
                    PT_B_Quotation myModel = (PT_B_Quotation)t_pgg_Bill.SelectedItem;
                    string mes = string.Format("确定将此单据状态修改为【{0}】", myItem.Header.ToString());
                    MessageResult myResult = XMessageBox.Ask(mes, this);
                    if (myResult == MessageResult.Yes)
                    {
                        myModel.Bill_Status = SetBillStatus(myItem.Header.ToString());
                        m_Entities.SaveChanges();
                        RefreshTreeNode();
                    }
                }
                else
                {
                    PT_B_Project myModel = (PT_B_Project)t_pgg_Bill.SelectedItem;
                    string mes = string.Format("确定将此单据状态修改为【{0}】", myItem.Header.ToString());
                    MessageResult myResult = XMessageBox.Ask(mes, this);
                    if (myResult == MessageResult.Yes)
                    {
                        myModel.Bill_Status = SetBillStatus(myItem.Header.ToString());
                        m_Entities.SaveChanges();
                        RefreshTreeNode();
                    }
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
        /// 绑定树节点方法
        /// </summary>
        private void BindTreeNode()
        {
            //计算报价单各状态节点数量
            CalculateQuotationCount();

            List<XTreeNode> myCollect = new List<XTreeNode>();
            XTreeNode myModel = new XTreeNode();
            myModel.Text = "报价单" + "[" + TreeNodeDictionary["Q"] + "]";
            myModel.NormalImage = "/Resources/CloseFloder.png";
            myModel.ExpandedImage = "/Resources/OpenFloder.png";
            myModel.Tag = "Q";
            myModel.Nodes = new List<XTreeNode>();
            myCollect.Add(myModel);

            XTreeNode mySub = new XTreeNode();
            mySub.Text = "已入库" + "[" + TreeNodeDictionary["1"] + "]";
            mySub.NormalImage = "/Resources/Info.png";
            mySub.ExpandedImage = "/Resources/Info.png";
            mySub.Tag = "1";
            myModel.Nodes.Add(mySub);
            XTreeNode mySub2 = new XTreeNode();
            mySub2.Text = "回收站" + "[" + TreeNodeDictionary["R"] + "]";
            mySub2.NormalImage = "/Resources/Info.png";
            mySub2.ExpandedImage = "/Resources/Info.png";
            mySub2.Tag = "R";
            myModel.Nodes.Add(mySub2);
            XTreeNode mySub3 = new XTreeNode();
            mySub3.Text = "归档" + "[" + TreeNodeDictionary["A"] + "]";
            mySub3.NormalImage = "/Resources/Info.png";
            mySub3.ExpandedImage = "/Resources/Info.png";
            mySub3.Tag = "A";
            myModel.Nodes.Add(mySub3);

            //计算项目单各状态节点数量
            CalculateProjectCount();

            myModel = new XTreeNode();
            myModel.Text = "项目单" + "[" + TreeNodeProjectDic["P"] + "]";
            myModel.NormalImage = "/Resources/CloseFloder.png";
            myModel.ExpandedImage = "/Resources/OpenFloder.png";
            myModel.Tag = "P";
            myModel.Nodes = new List<XTreeNode>();
            myCollect.Add(myModel);

            mySub = new XTreeNode();
            mySub.Text = "已入库" + "[" + TreeNodeProjectDic["1"] + "]";
            mySub.NormalImage = "/Resources/Info.png";
            mySub.ExpandedImage = "/Resources/Info.png";
            mySub.Tag = "1";
            myModel.Nodes.Add(mySub);
            mySub2 = new XTreeNode();
            mySub2.Text = "回收站" + "[" + TreeNodeProjectDic["R"] + "]";
            mySub2.NormalImage = "/Resources/Info.png";
            mySub2.ExpandedImage = "/Resources/Info.png";
            mySub2.Tag = "R";
            myModel.Nodes.Add(mySub2);
            mySub3 = new XTreeNode();
            mySub3.Text = "归档" + "[" + TreeNodeProjectDic["A"] + "]";
            mySub3.NormalImage = "/Resources/Info.png";
            mySub3.ExpandedImage = "/Resources/Info.png";
            mySub3.Tag = "A";
            myModel.Nodes.Add(mySub3);

            t_tvw_Module.BindTreeView(myCollect);
            SetNodeExpandedState(t_tvw_Module, true);
        }
        /// <summary>
        /// 计算报价单各状态数量
        /// </summary>
        private void CalculateQuotationCount()
        {
            //重置字典值为0
            TreeNodeDictionary["Q"] = 0;
            TreeNodeDictionary["1"] = 0;
            TreeNodeDictionary["R"] = 0;
            TreeNodeDictionary["A"] = 0;

            DateTime date = new DateTime();
            if (m_CurrentTypes != DateFilterTypes.Custom)
            {
                date = GetFilterDate();
                var temp = from p in m_Entities.PT_B_Quotation
                           where p.Quotation_Date > date
                           group p by p.Bill_Status;
                int sum = 0;
                foreach (var group in temp)
                {
                    string billStatus = group.Key;
                    int count = group.ToList().Count;
                    sum += count;
                    TreeNodeDictionary[billStatus] = count;
                }
                TreeNodeDictionary["Q"] = sum;
            }
            else
            {
                var temp = from p in m_Entities.PT_B_Quotation
                           where p.Quotation_Date >= t_dtp_StartDate.Value && p.Quotation_Date <= t_dtp_EndDate.Value
                           group p by p.Bill_Status;
                int sum = 0;
                foreach (var group in temp)
                {
                    string billStatus = group.Key;
                    int count = group.ToList().Count;
                    sum += count;
                    TreeNodeDictionary[billStatus] = count;
                }
                TreeNodeDictionary["Q"] = sum;
            }
        }
        /// <summary>
        /// 计算项目单各状态数量
        /// </summary>
        private void CalculateProjectCount()
        {
            //重置字典值为0
            TreeNodeProjectDic["P"] = 0;
            TreeNodeProjectDic["1"] = 0;
            TreeNodeProjectDic["R"] = 0;
            TreeNodeProjectDic["A"] = 0;

            DateTime date = new DateTime();
            if (m_CurrentTypes != DateFilterTypes.Custom)
            {
                date = GetFilterDate();
                var temp = from p in m_Entities.PT_B_Project
                           where p.Quotation_Date > date
                           group p by p.Bill_Status;
                int sum = 0;
                foreach (var group in temp)
                {
                    string billStatus = group.Key;
                    int count = group.ToList().Count;
                    sum += count;
                    TreeNodeProjectDic[billStatus] = count;
                }
                TreeNodeProjectDic["P"] = sum;
            }
            else
            {
                var temp = from p in m_Entities.PT_B_Project
                           where p.Quotation_Date >= t_dtp_StartDate.Value && p.Quotation_Date<=t_dtp_EndDate.Value
                           group p by p.Bill_Status;
                int sum = 0;
                foreach (var group in temp)
                {
                    string billStatus = group.Key;
                    int count = group.ToList().Count;
                    sum += count;
                    TreeNodeProjectDic[billStatus] = count;
                }
                TreeNodeProjectDic["P"] = sum;
            }
        }
        /// <summary>
        /// 加载业务数据
        /// </summary>
        private void LoadBusinessData()
        {
            XTreeNode myItem = t_tvw_Module.SelectedItem as XTreeNode;
            if (myItem == null) return;

            string billStatus = myItem.Tag.ToString();

            DateTime date = new DateTime();
            if (myItem.Nodes.Count == 0)
            {
                XTreeNode myParent = myItem.Parent as XTreeNode;
                string parentStatus = myParent.Tag.ToString();
                if (parentStatus == "Q")
                {
                    if (m_CurrentTypes != DateFilterTypes.Custom)
                    {
                        date = GetFilterDate();
                        var temp = from p in m_Entities.PT_B_Quotation
                                   where p.Bill_Status == billStatus && p.Quotation_Date > date
                                   orderby p.Quotation_No
                                   select p;
                        GirdStyleConfig.ItemsSource = temp.ToList();
                    }
                    else
                    {
                        var temp = from p in m_Entities.PT_B_Quotation
                                   where p.Bill_Status == billStatus && p.Quotation_Date >= t_dtp_StartDate.Value && p.Quotation_Date <= t_dtp_EndDate.Value
                                   orderby p.Quotation_No
                                   select p;
                        GirdStyleConfig.ItemsSource = temp.ToList();

                    }
                    t_pgg_Bill.SelectedIndex = -1;
                    t_pgg_Bill.StyleConfig = GirdStyleConfig;
                    t_pgg_Bill.SelectedIndex = 0;
                }
                else if (parentStatus == "P")
                {
                    if (m_CurrentTypes != DateFilterTypes.Custom)
                    {
                        date = GetFilterDate();
                        var temp = from p in m_Entities.PT_B_Project
                                   where p.Bill_Status == billStatus && p.Quotation_Date > date
                                   orderby p.Project_No
                                   select p;
                        ProjectGirdStyleConfig.ItemsSource = temp.ToList();
                    }
                    else
                    {
                        var temp = from p in m_Entities.PT_B_Project
                                   where p.Bill_Status == billStatus && p.Quotation_Date >= t_dtp_StartDate.Value && p.Quotation_Date <= t_dtp_EndDate.Value
                                   orderby p.Project_No
                                   select p;
                        ProjectGirdStyleConfig.ItemsSource = temp.ToList();
                    }

                    t_pgg_Bill.SelectedIndex = -1;
                    t_pgg_Bill.StyleConfig = ProjectGirdStyleConfig;
                    t_pgg_Bill.SelectedIndex = 0;
                }
            }
            else
            {
                if (billStatus == "Q")
                {
                    if (m_CurrentTypes != DateFilterTypes.Custom)
                    {
                        date = GetFilterDate();
                        var temp = from p in m_Entities.PT_B_Quotation
                                   where p.Quotation_Date > date
                                   orderby p.Quotation_No
                                   select p;
                        GirdStyleConfig.ItemsSource = temp.ToList();
                    }
                    else
                    {
                        var temp = from p in m_Entities.PT_B_Quotation
                                   where p.Quotation_Date >= t_dtp_StartDate.Value && p.Quotation_Date <= t_dtp_EndDate.Value
                                   orderby p.Quotation_No
                                   select p;
                        GirdStyleConfig.ItemsSource = temp.ToList();
                    }
                    t_pgg_Bill.SelectedIndex = -1;
                    t_pgg_Bill.StyleConfig = GirdStyleConfig;
                    t_pgg_Bill.SelectedIndex = 0;
                }
                else if (billStatus == "P")
                {
                    if (m_CurrentTypes != DateFilterTypes.Custom)
                    {
                        date = GetFilterDate();
                        var temp = from p in m_Entities.PT_B_Project
                                   where p.Quotation_Date > date
                                   orderby p.Quotation_No
                                   select p;
                        ProjectGirdStyleConfig.ItemsSource = temp.ToList();
                    }
                    else
                    {
                        var temp = from p in m_Entities.PT_B_Project
                                   where p.Quotation_Date >= t_dtp_StartDate.Value && p.Quotation_Date <= t_dtp_EndDate.Value
                                   orderby p.Quotation_No
                                   select p;
                        ProjectGirdStyleConfig.ItemsSource = temp.ToList();
                    }
                    t_pgg_Bill.SelectedIndex = -1;
                    t_pgg_Bill.StyleConfig = ProjectGirdStyleConfig;
                    t_pgg_Bill.SelectedIndex = 0;
                }
            }
        }
        /// <summary>
        /// 设置选中的树节点
        /// </summary>
        /// <param name="myNodes">树节点</param>
        /// <param name="tag">原始tag值</param>
        /// <param name="parentTag">父节点的tag值</param>
        private void SetSelected(List<XTreeNode> myNodes, string tag, string parentTag)
        {
            foreach (XTreeNode myNode in myNodes)
            {
                if (myNode.Tag.ToString() == tag)
                {
                    if (myNode.Nodes.Count != 0)
                        myNode.IsSelected = true;
                    else if (myNode.Parent.Tag.ToString() == parentTag)
                        myNode.IsSelected = true;
                }
                if (myNode.Nodes.Count != 0)
                {
                    SetSelected(myNode.Nodes, tag, parentTag);
                }
            }
        }
        /// <summary>
        /// 刷新树节点并重设选中节点
        /// </summary>
        private void RefreshTreeNode()
        {
            XTreeNode myItem = t_tvw_Module.SelectedItem as XTreeNode;
            string myTag = string.Empty;
            string parentTag = string.Empty;
            if (myItem == null && m_MQuery != null)
            {
                myTag = m_MQuery.BillType;
            }
            else
            {
                myTag = myItem.Tag.ToString();
                if (myItem.Parent != null)
                    parentTag = myItem.Parent.Tag.ToString();
            }
            BindTreeNode();
            SetSelected(t_tvw_Module.Nodes, myTag, parentTag);
            if (myItem == null && m_MQuery != null)
                QueryMethod();
        }
        /// <summary>
        /// 设置树是否展开或收缩
        /// </summary>
        /// <param name="control">TreeView控件</param>
        /// <param name="expandNode">true:展开 false:收缩</param>
        private void SetNodeExpandedState(ItemsControl control, bool expandNode)
        {
            try
            {
                if (control != null)
                {
                    foreach (object item in control.Items)
                    {
                        TreeViewItem treeItem = control.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;

                        if (treeItem != null && treeItem.HasItems)
                        {
                            treeItem.IsExpanded = expandNode;
                            if (treeItem.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                            {
                                treeItem.UpdateLayout();
                            }

                            SetNodeExpandedState(treeItem as ItemsControl, expandNode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 查询方法
        /// </summary>
        private void QueryMethod()
        {
            if (m_MQuery == null) return;
            if (m_MQuery.BillType == "Q")
            {
                var query = m_Entities.PT_B_Quotation.AsQueryable();
                if (m_MQuery.DateFilterType != DateFilterTypes.Custom)
                {
                    DateTime date = GetFilterDate();
                    query = query.Where(c => c.Quotation_Date > date);
                }
                else
                    query = query.Where(c => c.Quotation_Date >= m_MQuery.StartDate && c.Quotation_Date <= m_MQuery.EndDate);

                if (!string.IsNullOrEmpty(m_MQuery.BillNo))
                {
                    query = query.Where(c => c.Quotation_No.Contains(m_MQuery.BillNo));
                }
                if (!string.IsNullOrEmpty(m_MQuery.FllowMan))
                {
                    query = query.Where(c => c.Follow_Man.Contains(m_MQuery.FllowMan));
                }
                if (!string.IsNullOrEmpty(m_MQuery.ProjectName))
                {
                    query = query.Where(c => c.Project_Name.Contains(m_MQuery.ProjectName));
                }
                if (!string.IsNullOrEmpty(m_MQuery.CompanyName))
                {
                    query = query.Where(c => c.Company_Name.Contains(m_MQuery.CompanyName));
                }
                if (m_MQuery.Type.Length > 0)
                {
                    query = query.Where(c => m_MQuery.Type.Contains(c.Quotation_Type));
                }
                query = query.OrderBy(c => c.Quotation_No);
                GirdStyleConfig.ItemsSource = query.ToList();
                t_pgg_Bill.SelectedIndex = -1;
                t_pgg_Bill.StyleConfig = GirdStyleConfig;
                t_pgg_Bill.SelectedIndex = 0;
            }
            else if (m_MQuery.BillType == "P")
            {
                var query = m_Entities.PT_B_Project.AsQueryable();
                if (m_MQuery.DateFilterType != DateFilterTypes.Custom)
                {
                    DateTime date = GetFilterDate();
                    query = query.Where(c => c.Quotation_Date > date);
                }
                else
                    query = query.Where(c => c.Quotation_Date >= m_MQuery.StartDate && c.Quotation_Date <= m_MQuery.EndDate);

                if (!string.IsNullOrEmpty(m_MQuery.BillNo))
                {
                    query = query.Where(c => c.Project_No.Contains(m_MQuery.BillNo));
                }
                if (!string.IsNullOrEmpty(m_MQuery.FllowMan))
                {
                    query = query.Where(c => c.Follow_Man.Contains(m_MQuery.FllowMan));
                }
                if (!string.IsNullOrEmpty(m_MQuery.ProjectName))
                {
                    query = query.Where(c => c.Project_Name.Contains(m_MQuery.ProjectName));
                }
                if (!string.IsNullOrEmpty(m_MQuery.CompanyName))
                {
                    query = query.Where(c => c.Company_Name.Contains(m_MQuery.CompanyName));
                }
                if (m_MQuery.Type.Length > 0)
                {
                    query = query.Where(c => m_MQuery.Type.Contains(c.Project_Type));
                }
                query = query.OrderBy(c => c.Project_No);
                ProjectGirdStyleConfig.ItemsSource = query.ToList();
                t_pgg_Bill.SelectedIndex = -1;
                t_pgg_Bill.StyleConfig = ProjectGirdStyleConfig;
                t_pgg_Bill.SelectedIndex = 0;
            }
            foreach (XTreeNode myNode in t_tvw_Module.Nodes)
            {
                myNode.IsSelected = false;
            }
        }
        /// <summary>
        /// 获取当前参数格式化的查询语句
        /// </summary>
        /// <param name="dateField">日期数据库字段</param>
        /// <returns>返回格式化的查询语句</returns>
        private DateTime GetFilterDate()
        {
            DateTime dt = new DateTime();
            switch (m_CurrentTypes)
            {
                case DateFilterTypes.Today:
                    dt = DateTime.Today.AddDays(-1);
                    break;
                case DateFilterTypes.Week:
                    dt = DateTime.Today.AddDays(-Convert.ToInt32(DateTime.Now.Date.DayOfWeek));
                    break;
                case DateFilterTypes.Month:
                    dt = DateTime.Today.AddDays(-Convert.ToInt32(DateTime.Now.Date.Day));
                    break;
            }
            return dt;
        }
        /// <summary>
        /// 当前日期范围单选按钮选中状态改变时调用的方法
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void OnDateFilterCheckedChanged(object sender, RoutedEventArgs e)
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

            m_CurrentTypes = (DateFilterTypes)Enum.Parse(typeof(DateFilterTypes), type, true);
            if (m_CurrentTypes == DateFilterTypes.Custom)
            {
                t_dtp_StartDate.IsEnabled = true;
                t_dtp_EndDate.IsEnabled = true;
            }
            else
            {
                t_dtp_StartDate.IsEnabled = false;
                t_dtp_EndDate.IsEnabled = false;
            }
            XTreeNode myItem = t_tvw_Module.SelectedItem as XTreeNode;
            string myTag = string.Empty;
            string parentTag = string.Empty;
            BindTreeNode();
            if (myItem == null && m_MQuery != null)
            {
                myTag = m_MQuery.BillType;
                SetSelected(t_tvw_Module.Nodes, myTag, parentTag);
            }
            else if (myItem != null)
            {
                myTag = myItem.Tag.ToString();
                if (myItem.Parent != null)
                    parentTag = myItem.Parent.Tag.ToString();
                SetSelected(t_tvw_Module.Nodes, myTag, parentTag);
            }

        }
        /// <summary>
        /// 状态值转换
        /// </summary>
        /// <param name="billStatus">状态</param>
        /// <returns>返回转换的状态值</returns>
        private string SetBillStatus(string billStatus)
        {
            string myStatus = string.Empty;
            switch (billStatus)
            {
                case "1":
                    myStatus = "已入库";
                    break;
                case "R":
                    myStatus = "回收站";
                    break;
                case "A":
                    myStatus = "归档";
                    break;
                case "已入库":
                    myStatus = "1";
                    break;
                case "回收站":
                    myStatus = "R";
                    break;
                case "归档":
                    myStatus = "A";
                    break;
            }
            return myStatus;
        }

        #endregion
    }
}

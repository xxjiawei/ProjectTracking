using Microsoft.Win32;
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
        private DataGridStyleConfig m_GirdStyleConfig;
        public DataGridStyleConfig GirdStyleConfig
        {
            get
            {
                if (m_GirdStyleConfig == null)
                {
                    m_GirdStyleConfig = new DataGridStyleConfig();

                    m_GirdStyleConfig.Add("Quotation_No", "报价单号", 100);
                    m_GirdStyleConfig.Add("Company_Name", "公司名称", 120);
                    m_GirdStyleConfig.Add("Project_Name", "测试项目", 120);
                    m_GirdStyleConfig.Add("Price", "当前报价", 63);
                    m_GirdStyleConfig.Add("Follow_Man", "业务跟进人", 70);
                    m_GirdStyleConfig.Add("Contact_Man", "联系人", 60);
                    m_GirdStyleConfig.Add("Tel", "联系电话", 90);
                    m_GirdStyleConfig.Add("Quotation_Date", "报价日期", 80);
                }
                return m_GirdStyleConfig;
            }
        }
        private DataGridStyleConfig m_ProjectGirdStyleConfig;
        public DataGridStyleConfig ProjectGirdStyleConfig
        {
            get
            {
                if (m_ProjectGirdStyleConfig == null)
                {
                    m_ProjectGirdStyleConfig = new DataGridStyleConfig();

                    m_ProjectGirdStyleConfig.Add("Project_No", "项目单号", 100);
                    m_ProjectGirdStyleConfig.Add("Quotation_No", "报价单号", 100);
                    m_ProjectGirdStyleConfig.Add("Company_Name", "公司名称", 120);
                    m_ProjectGirdStyleConfig.Add("Project_Name", "测试项目", 120);
                    m_ProjectGirdStyleConfig.Add("Price", "当前报价", 63);
                    m_ProjectGirdStyleConfig.Add("Follow_Man", "业务跟进人", 70);
                    m_ProjectGirdStyleConfig.Add("Contact_Man", "联系人", 60);
                    m_ProjectGirdStyleConfig.Add("Tel", "联系电话", 90);
                    m_ProjectGirdStyleConfig.Add("Quotation_Date", "报价日期", 80);
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

        #endregion

        #region 界面事件

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void XBaseForm_Loaded(object sender, RoutedEventArgs e)
        {
            //绑定树节点
            BindTreeNode();
            //选中首节点
            t_tvw_Module.Focus();
            TreeViewItem myItem = t_tvw_Module.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
            myItem.IsSelected = true;
        }
        /// <summary>
        /// 树形图选择切换
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_tvw_Module_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            LoadBusinessData();
        }
        /// <summary>
        /// 新建报价单窗口
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_btn_New_Click(object sender, RoutedEventArgs e)
        {
            FrmQuotation myForm = new FrmQuotation();
            myForm.ShowDialog();

            RefreshTreeNode();
        }
        /// <summary>
        /// 打开窗口按钮事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_btn_Open_Click(object sender, RoutedEventArgs e)
        {
            if (t_pgg_Bill.SelectedItem == null)
            {
                XMessageBox.Warning("未选中一条单据！", this);
                return;
            }
            t_pgg_Bill_MouseDoubleClick(null, null);
        }
        /// <summary>
        /// 表格行鼠标双击事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_pgg_Bill_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            XTreeNode myItem = t_tvw_Module.SelectedItem as XTreeNode;
            string myTag = myItem.Tag.ToString();
            string parentTag = string.Empty;
            if (myItem.Parent != null)
                parentTag = myItem.Parent.Tag.ToString();

            if (myTag == "Q" || parentTag == "Q")
            {
                PT_B_Quotation myModel = (PT_B_Quotation)t_pgg_Bill.SelectedItem;
                FrmQuotation myForm = new FrmQuotation();
                myForm.PTBQuotation = myModel;
                myForm.ShowDialog();
                RefreshTreeNode();
            }
            else
            {
                PT_B_Project myModel = (PT_B_Project)t_pgg_Bill.SelectedItem;
                FrmProject myForm = new FrmProject();
                myForm.PTBProject = myModel;
                myForm.ShowDialog();
                RefreshTreeNode();
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
            if (t_pgg_Bill.SelectedItem == null)
            {
                XMessageBox.Warning("未选中一条单据！", this);
                return;
            }
            XTreeNode myItem = t_tvw_Module.SelectedItem as XTreeNode;
            string myTag = myItem.Tag.ToString();
            string parentTag = string.Empty;
            if (myItem.Parent != null)
                parentTag = myItem.Parent.Tag.ToString();
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

            var temp = from p in m_Entities.PT_B_Quotation
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

            var temp = from p in m_Entities.PT_B_Project
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
        /// <summary>
        /// 加载业务数据
        /// </summary>
        private void LoadBusinessData()
        {
            XTreeNode myItem = t_tvw_Module.SelectedItem as XTreeNode;
            if (myItem == null) return;

            string billStatus = myItem.Tag.ToString();

            if (myItem.Nodes.Count == 0)
            {
                XTreeNode myParent = myItem.Parent as XTreeNode;
                string parentStatus = myParent.Tag.ToString();
                if (parentStatus == "Q")
                {
                    var temp = from p in m_Entities.PT_B_Quotation
                               where p.Bill_Status == billStatus
                               select p;

                    t_pgg_Bill.SelectedIndex = -1;
                    GirdStyleConfig.ItemsSource = temp.ToList();
                    t_pgg_Bill.StyleConfig = GirdStyleConfig;
                    t_pgg_Bill.SelectedIndex = 0;
                }
                else if (parentStatus == "P")
                {
                    var temp = from p in m_Entities.PT_B_Project
                               where p.Bill_Status == billStatus
                               select p;

                    t_pgg_Bill.SelectedIndex = -1;
                    ProjectGirdStyleConfig.ItemsSource = temp.ToList();
                    t_pgg_Bill.StyleConfig = ProjectGirdStyleConfig;
                    t_pgg_Bill.SelectedIndex = 0;
                }
            }
            else
            {
                if (billStatus == "Q")
                {
                    var temp = from p in m_Entities.PT_B_Quotation
                               orderby p.Quotation_No
                               select p;

                    t_pgg_Bill.SelectedIndex = -1;
                    GirdStyleConfig.ItemsSource = temp.ToList();
                    t_pgg_Bill.StyleConfig = GirdStyleConfig;
                    t_pgg_Bill.SelectedIndex = 0;
                }
                else if (billStatus == "P")
                {
                    var temp = from p in m_Entities.PT_B_Project
                               orderby p.Quotation_No
                               select p;

                    t_pgg_Bill.SelectedIndex = -1;
                    ProjectGirdStyleConfig.ItemsSource = temp.ToList();
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
            string myTag = myItem.Tag.ToString();
            string parentTag = string.Empty;
            if (myItem.Parent != null)
                parentTag = myItem.Parent.Tag.ToString();
            BindTreeNode();
            SetSelected(t_tvw_Module.Nodes, myTag, parentTag);
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
        #endregion

        private void t_btn_Export_Click(object sender, RoutedEventArgs e)
        {
            //SqlConnection conn = new SqlConnection("server=.; uid=sa; pwd=sa123456");
            //if (conn.State != ConnectionState.Open)
            //{
            //    conn.Open();
            //}
            //string sql = "CREATE DATABASE ProjectTracking1 on primary" + "(name=ProjectTracking1,filename='F:\\ProjectTracking1.mdf',size=3, maxsize=5,filegrowth=10%)";
            //sql += "log on" + "(name=ProjectTracking1_log,filename='F:\\ProjectTracking1_log.ldf',size=3,filegrowth=1)";
            //SqlCommand cmd = new SqlCommand(sql, conn);

            //cmd.ExecuteNonQuery();

            //excutesqlfile("sa", "sa123456", "ProjectTracking1", @"C:\Users\40326\Documents\Visual Studio 2017\XProject001\ProjectTracking.git\XProject001\XProjectWPF\DataBase\");

           // ExecuteSqlFile("server=.; uid=sa; pwd=sa123456", @"C:\Users\40326\Documents\Visual Studio 2017\XProject001\ProjectTracking.git\XProject001\XProjectWPF\DataBase\数据库脚本.sql");
        }

        /// <summary>
        /// 导入sql脚本
        /// </summary>
        /// <param name="sqlConnString">连接数据库字符串</param>
        /// <param name="varFileName">脚本路径</param>
        /// <returns></returns>
        private static bool ExecuteSqlFile(string sqlConnString, string varFileName)
        {
            if (!File.Exists(varFileName))
            {
                return false;
            }
            StreamReader rs = new StreamReader(varFileName, System.Text.Encoding.Default);
            ArrayList alSql = new ArrayList();
            string commandText = "";
            string varLine = "";
            while (rs.Peek() > -1)
            {
                varLine = rs.ReadLine();
                if (varLine == "")
                {
                    continue;
                }
                if (varLine != "GO")
                {
                    commandText += varLine;
                    commandText += "\r\n";
                }
                else
                {
                    commandText += "";
                }
            }
            alSql.Add(commandText);
            rs.Close();
            try
            {
                ExecuteCommand(sqlConnString, alSql);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static void ExecuteCommand(string sqlConnString, ArrayList varSqlList)
        {
            using (SqlConnection conn = new SqlConnection(sqlConnString))
            {
                conn.Open();
                //Don't use Transaction, because some commands cannot execute in one Transaction.
                //SqlTransaction varTrans = conn.BeginTransaction();
                SqlCommand command = new SqlCommand();
                command.Connection = conn;
                //command.Transaction = varTrans;
                try
                {
                    foreach (string varcommandText in varSqlList)
                    {
                        command.CommandText = varcommandText;
                        command.ExecuteNonQuery();
                    }
                    //varTrans.Commit();
                }
                catch (Exception ex)
                {
                    //varTrans.Rollback();
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}

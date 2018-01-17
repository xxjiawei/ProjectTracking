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
    /// ManageForm.xaml 的交互逻辑
    /// </summary>
    public partial class ManageForm : XBaseForm
    {
        public ManageForm()
        {
            InitializeComponent();
        }


        private DataGridStyleConfig m_GirdStyleConfig;

        public DataGridStyleConfig GirdStyleConfig
        {
            get
            {
                if (m_GirdStyleConfig == null)
                {
                    m_GirdStyleConfig = new DataGridStyleConfig();

                    m_GirdStyleConfig.Add("报价单号", "报价单号",100);
                    m_GirdStyleConfig.Add("公司名称", "公司名称", 120);
                    m_GirdStyleConfig.Add("测试项目", "测试项目", 120);
                    m_GirdStyleConfig.Add("当前报价", "当前报价", 63);
                    m_GirdStyleConfig.Add("业务跟进人", "业务跟进人", 70);
                    m_GirdStyleConfig.Add("联系人", "联系人", 60);
                    m_GirdStyleConfig.Add("联系电话", "联系电话", 90);
                    m_GirdStyleConfig.Add("报价日期", "报价日期", 80);
                }
                return m_GirdStyleConfig;
            }
        }
        //private IEnumerable GetItemsSource()
        //{
        //        DataTable myTable = new DataTable();
        //        myTable.Columns.Add("ID", typeof(string));
        //        myTable.Columns.Add("Name", typeof(string));
        //        myTable.Columns.Add("Age", typeof(int));
        //        myTable.Columns.Add("IsMale", typeof(bool));
        //        myTable.Columns.Add("Sex", typeof(int));
        //        myTable.Columns.Add("Code", typeof(int));

        //        for (int index = 1; index < 20; index++)
        //        {
        //            DataRow dr = myTable.NewRow();
        //            dr["ID"] = index.ToString();
        //            dr["Name"] = "Name" + index.ToString();
        //            dr["Age"] = index.ToString();
        //            dr["Code"] = index.ToString();

        //            if (index % 2 != 0)
        //            {
        //                dr["IsMale"] = true;
        //                dr["Sex"] = 1;
        //            }
        //            else
        //            {
        //                dr["IsMale"] = false;
        //                dr["Sex"] = 0;
        //            }


        //            myTable.Rows.Add(dr);
        //        }

        //        return myTable.DefaultView;
        //}
        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void XBaseForm_Loaded(object sender, RoutedEventArgs e)
        {
            //绑定树节点
            BindTreeNode();
            //加载表格
            //InitGridColumn();

            //选中首节点
            t_tvw_Module.Focus();
            TreeViewItem myItem = t_tvw_Module.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
            myItem.IsSelected = true;

            //表格赋值
            DataTable myTable = new DataTable();
            myTable.Columns.Add("报价单号");
            myTable.Columns.Add("公司名称");
            myTable.Columns.Add("测试项目");
            myTable.Columns.Add("当前报价");
            myTable.Columns.Add("业务跟进人");
            myTable.Columns.Add("联系人");
            myTable.Columns.Add("联系电话");
            myTable.Columns.Add("报价日期");

            myTable.Rows.Add("BJ20180112001","广东科技有限公司","光电技术项目","50000","何显俊","王德顺","2825250","2018-1-1");
            myTable.Rows.Add("BJ20180112002", "中山测试公司", "测量项目", "30000", "何显俊", "鹿晗", "13599956555", "2018-1-12");
            myTable.Rows.Add("BJ20180112003", "中山移动分公司", "通讯技术项目", "20000", "王二丫", "潘玮柏", "13764877545", "2018-1-12");
            GirdStyleConfig.ItemsSource = myTable.DefaultView;
            t_pgg_Bill.StyleConfig = GirdStyleConfig;

        }
        /// <summary>
        /// 绑定树节点方法
        /// </summary>
        private void BindTreeNode()
        {
            TreeViewItem myNode = CreateTreeNode("报价单", "/Resources/CloseFloder.png");
            TreeViewItem mySubNode = CreateTreeNode("报价单跟踪", "/Resources/Info.png");
            myNode.Items.Add(mySubNode);
            mySubNode = CreateTreeNode("回收站", "/Resources/Info.png");
            myNode.Items.Add(mySubNode);
            mySubNode = CreateTreeNode("归档", "/Resources/Info.png");
            myNode.Items.Add(mySubNode);
            t_tvw_Module.Items.Add(myNode);
            myNode = CreateTreeNode("项目单", "/Resources/CloseFloder.png");
            mySubNode = CreateTreeNode("项目单跟踪", "/Resources/Info.png");
            myNode.Items.Add(mySubNode);
            mySubNode = CreateTreeNode("回收站", "/Resources/Info.png");
            myNode.Items.Add(mySubNode);
            mySubNode = CreateTreeNode("归档", "/Resources/Info.png");
            myNode.Items.Add(mySubNode);
            t_tvw_Module.Items.Add(myNode);
            SetNodeExpandedState(t_tvw_Module, true);
        }
        /// <summary>
        /// 加载表格
        /// </summary>
        private void InitGridColumn()
        {
            DataGridTextColumn myColumn = new DataGridTextColumn();
            myColumn.Header = "报价单号";
            myColumn.Width = 80;
            myColumn.Binding = new Binding("报价单号");
            t_pgg_Bill.Columns.Add(myColumn);
            myColumn = new DataGridTextColumn();
            myColumn.Header = "公司名称";
            myColumn.Width = 80;
            myColumn.Binding = new Binding("公司名称");
            t_pgg_Bill.Columns.Add(myColumn);
            myColumn = new DataGridTextColumn();
            myColumn.Header = "联系人";
            myColumn.Width = 80;
            myColumn.Binding = new Binding("联系人");
            t_pgg_Bill.Columns.Add(myColumn);
            myColumn = new DataGridTextColumn();
            myColumn.Header = "联系电话";
            myColumn.Width = 80;
            myColumn.Binding = new Binding("联系电话");
            t_pgg_Bill.Columns.Add(myColumn);
            myColumn = new DataGridTextColumn();
            myColumn.Header = "测试项目";
            myColumn.Width = 80;
            myColumn.Binding = new Binding("测试项目");
            t_pgg_Bill.Columns.Add(myColumn);
            myColumn = new DataGridTextColumn();
            myColumn.Header = "报价金额";
            myColumn.Width = 80;
            myColumn.Binding = new Binding("报价金额");
            t_pgg_Bill.Columns.Add(myColumn);
        }
        /// <summary>
        /// 表示创建一个节点
        /// </summary>
        /// <param name="text"></param>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        private TreeViewItem CreateTreeNode(string text, string imageUrl)
        {
            StackPanel myPanel = new StackPanel();
            myPanel.Height = 25;
            myPanel.Orientation = Orientation.Horizontal;

            Image myImage = new Image();
            myImage.Source = new BitmapImage(new Uri(imageUrl, UriKind.Relative));
            myImage.Width = 15;
            myImage.Height = 15;

            TextBlock myText = new TextBlock();
            myText.VerticalAlignment = VerticalAlignment.Center;
            myText.Margin = new Thickness(5, 0, 0, 0);
            myText.Text = text + "[0]";
            myText.Tag = text + "[{0}]";

            if( text == "项目单" || text == "项目单跟踪")
                myText.Text = text + "[2]";

            if(text == "报价单跟踪" || text == "报价单")
                myText.Text = text + "[3]";
            myPanel.Children.Add(myImage);
            myPanel.Children.Add(myText);

            TreeViewItem myNode = new TreeViewItem();
            myNode.Header = myPanel;
            myNode.AllowDrop = true;

            return myNode;
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
        /// 新建报价单窗口
        /// </summary>
        /// <param name="sender">事件对象</param>
        /// <param name="e">事件参数</param>
        private void t_btn_New_Click(object sender, RoutedEventArgs e)
        {
            FrmQuotation myForm = new FrmQuotation();
            myForm.ShowDialog();
        }

        private void t_btn_Open_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem myItem = t_tvw_Module.SelectedItem as TreeViewItem;

            TextBlock myText = (myItem.Header as StackPanel).Children[1] as TextBlock;


            if (myText.Text == "项目单[2]" || myText.Text == "项目单跟踪[2]")
            {
                FrmProject myForm = new FrmProject();
                MQuotation22 quotationModel = new MQuotation22();
                quotationModel.QuotationNo = "BJ20180112001";
                quotationModel.QuotationDate = DateTime.Parse("2018-1-1");
                quotationModel.FollowMan = "何显俊";
                quotationModel.ProjectName = "光电技术项目";
                quotationModel.Price = "50000";
                quotationModel.CompanyName = "广东科技有限公司";
                quotationModel.CompanyAddress = "中山街305号";
                quotationModel.Tel = "2825250";
                quotationModel.Email = "4022222@126.com";
                quotationModel.ContactMan = "王德顺";
                myForm.QuotationModel = quotationModel;
                myForm.IsNew = true;
                myForm.ShowDialog();
            }
            if (myText.Text == "报价单[3]" || myText.Text == "报价单跟踪[3]")
            {
                FrmQuotation myForm = new FrmQuotation();
                MQuotation22 quotationModel = new MQuotation22();
                quotationModel.QuotationNo = "BJ20180112001";
                quotationModel.QuotationDate = DateTime.Parse("2018-1-1");
                quotationModel.FollowMan = "何显俊";
                quotationModel.ProjectName = "光电技术项目";
                quotationModel.Price = "50000";
                quotationModel.CompanyName = "广东科技有限公司";
                quotationModel.CompanyAddress = "中山街305号";
                quotationModel.Tel = "2825250";
                quotationModel.Email = "4022222@126.com";
                quotationModel.ContactMan = "王德顺";
                myForm.QuotationModel = quotationModel;
                myForm.ShowDialog();
            }
        }

        private void t_pgg_Bill_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid datagrid = sender as DataGrid;
            Point aP = e.GetPosition(datagrid);
            IInputElement obj = datagrid.InputHitTest(aP);
            DependencyObject target = obj as DependencyObject;

            while (target != null)
            {
                if (target is DataGridRow)
                {
                    TreeViewItem myItem = t_tvw_Module.SelectedItem as TreeViewItem;

                    TextBlock myText = (myItem.Header as StackPanel).Children[1] as TextBlock;

                  
                    if (myText.Text == "项目单[2]" || myText.Text == "项目单跟踪[2]")
                    {
                        FrmProject myForm = new FrmProject();
                        MQuotation22 quotationModel = new MQuotation22();
                        quotationModel.QuotationNo = "BJ20180112001";
                        quotationModel.QuotationDate = DateTime.Parse("2018-1-1");
                        quotationModel.FollowMan = "何显俊";
                        quotationModel.ProjectName = "光电技术项目";
                        quotationModel.Price = "50000";
                        quotationModel.CompanyName = "广东科技有限公司";
                        quotationModel.CompanyAddress = "中山街305号";
                        quotationModel.Tel = "2825250";
                        quotationModel.Email = "4022222@126.com";
                        quotationModel.ContactMan = "王德顺";
                        myForm.QuotationModel = quotationModel;
                        myForm.IsNew = true;
                        myForm.ShowDialog();
                        break;
                    }
                    if (myText.Text == "报价单[3]" || myText.Text == "报价单跟踪[3]")
                    {
                        FrmQuotation myForm = new FrmQuotation();
                        MQuotation22 quotationModel = new MQuotation22();
                        quotationModel.QuotationNo = "BJ20180112001";
                        quotationModel.QuotationDate = DateTime.Parse("2018-1-1");
                        quotationModel.FollowMan = "何显俊";
                        quotationModel.ProjectName = "光电技术项目";
                        quotationModel.Price = "50000";
                        quotationModel.CompanyName = "广东科技有限公司";
                        quotationModel.CompanyAddress = "中山街305号";
                        quotationModel.Tel = "2825250";
                        quotationModel.Email = "4022222@126.com";
                        quotationModel.ContactMan = "王德顺";
                        myForm.QuotationModel = quotationModel;
                        myForm.ShowDialog();
                        break;
                    }
                    else
                        break;
              
                }
                target = VisualTreeHelper.GetParent(target);
            }
        }

        private void t_tsm_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

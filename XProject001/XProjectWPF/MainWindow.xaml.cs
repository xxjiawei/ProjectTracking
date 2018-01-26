using RJ.XStyle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XProjectWPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow :  XBaseForm 
    {
        public MainWindow()
        {
            InitializeComponent();
           
        }

        private void t_btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void t_btn_Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Visibility = Visibility.Hidden;
                ManageForm myForm = new ManageForm();
                //Window1 myForm = new Window1();
                myForm.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }
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

        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        /// <param name="_strDBName">数据库名称</param>
        /// <returns></returns>
        private static bool CreateDatabase(string connStr, string _strDBName)
        {
            bool bSuccess = false;
            try
            {
                using (SqlConnection conMaster = new SqlConnection(connStr))
                {
                    conMaster.Open();
                    // Check if the Database has existed first
                    string strExist = @"select * from dbo.sysdatabases where name='" + _strDBName + @"'";
                    SqlCommand cmdExist = new SqlCommand(strExist, conMaster);
                    SqlDataReader readerExist = cmdExist.ExecuteReader();
                    bool bExist = readerExist.HasRows;
                    readerExist.Close();
                    if (bExist)
                    {
                        string strDel = @"drop database " + _strDBName;
                        SqlCommand cmdDel = new SqlCommand(strDel, conMaster);
                        cmdDel.ExecuteNonQuery();
                    }
                    // Create the database now;     
                    string strDatabase = "Create Database [" + _strDBName + "]";
                    SqlCommand cmdCreate = new SqlCommand(strDatabase, conMaster);
                    cmdCreate.ExecuteNonQuery();
                    conMaster.Close();
                }
                bSuccess = true;
            }
            catch (Exception e)
            {
                throw e;
            }
            return bSuccess;
        }

        private void t_btn_Close_Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CreateDatabase("server=.\\SQLEXPRESS; uid=sa; pwd=sa123456", "ProjectTracking");

                ExecuteSqlFile("server=.\\SQLEXPRESS; uid=sa; pwd=sa123456", @"C:\Users\40326\Documents\Visual Studio 2017\XProject001\ProjectTracking.git\XProject001\XProjectWPF\DataBase\数据库脚本.sql");

                XMessageBox.Enter("创建成功！",this);
            }
            catch (Exception ex)
            {
                XMessageBox.Exception(ex);
            }
        }
    }
}

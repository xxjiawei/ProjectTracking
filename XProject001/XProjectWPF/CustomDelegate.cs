using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XProjectWPF.Model;

namespace XProjectWPF
{
    /// <summary>
    /// 表示日期过滤控件的参数改变时调用的方法
    /// </summary>
    /// <param name="sender">表示日期过滤控件的参数</param>
    public delegate void DateFilterValueChanged(MDateFilterParam sender);
}

   M S B A M L   `   `   `  ����D  @RJ.XStyle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null8*clr-namespace:RJ.XStyle;assembly=RJ.XStyle	RJ.XStyle  5      F BXProjectWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:XProjectWPFXProjectWPF 5   	      XProjectWPF.ManageForm   5      5xstyle*clr-namespace:RJ.XStyle;assembly=RJ.XStyle  R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:XProjectWPF     5   	   -   5          � Title$   ��5
   	   $	��720��6   $
��1024��6    � WindowStartupLocation$ CenterScreen=�6,    � 
ResizeMode$ NoResize=�6Q   .��5      � .+�5   
   o� -   5         o�Name$
 menu��5      $��30��6   $
��1016��6)   $	��Top=�66   $	��Top=�6K   $	��Top=�6j   $
��Left=�6�   ���	#FFDDEAFA$��2��6�   .@�5      m� -   5        $��t_tsb_Query��5   (   $��3,0,0,0q�6   ��5      �� 5       �� TextAlignment$
 Left=�6    $��-5,0,0,0q�65   $��Center=�6H   $��查询单据��6c    m� Icon 5      �� 5      $2��,/XProjectWPF;component/Resources/gntb_20.png��6   $��16��6R   $��16��6]   $��0,0,0,0q�6i   5      m� 5      -   5        $��	t_btn_New��5      $��3,0,0,0q�6(   ��5      �� 5      $
 Left=�6    $��-5,0,0,0q�65   $��Center=�6H   $��新建报价单据��6c    5      �� 5      $.��(/XProjectWPF;component/Resources/New.png��6   $��16��6N   $��16��6Y   5      m� 5      -   5        $��
t_btn_Open��5      $��3,0,0,0q�6)   ��5      �� 5      $
 Left=�6    $��-5,0,0,0q�65   $��Center=�6H   $��打开单据��6c    5!      �� 5"      $/��)/XProjectWPF;component/Resources/Open.png��6   $��16��6O   $��16��6Z   5$      m� 5&      -   5        $��t_btn_Delete��5&      $��3,0,0,0q�6+   ��5'      �� 5(      $
 Left=�6    $��-5,0,0,0q�65   $��Center=�6H   $��删除单据��6c    5*      �� 5+      $1��+/XProjectWPF;component/Resources/Delete.png��6   $��16��6Q   $��16��6\   5-      m� 5.      -   5        $��t_tsm_Close��5.   (   $��3,0,0,0q�6   ��5/      �� 50      $
 Left=�6    $��-5,0,0,0q�65   $��Center=�6H   $��关闭��6c    52      �� 53      $/��)/XProjectWPF;component/Resources/Quit.png��6   $��16��6O   $��16��6Z   55      5G         RJ.XStyle.XTreeView  5H   
   -   5          Name$ t_tvw_Module��5H      $
��Left=�61   $��10,67,0,160q�6L   $	��240��6a   
�������6�   �� 5I   
   $��单据类型��6   $
��Left=�6   $��	10,37,0,0q�6:   $	��Top=�6M    J� RenderTransformOrigin$ 
-0.125,0.4&�6e   $��25��6�      RJ.XStyle.XDataGrid  5J   
   -	   5          Name$ 
t_pgg_Bill��5J      $��Stretch=�6/   $��262,67,10,12q�6M   $��Stretch=�6c   &   System.Windows.Controls.DataGrid	   	RowHeight$	 24Q�6   
   
IsReadOnly
 .    ColumnHeaderHeight$ 32Q�5K   %      AutoGenerateColumns .     DisplayRowNum .  � ContextMenu 5L      �� 5M      -
   5         ��Name$ SettingMenu��5M       �� 	Placement$ 
MousePoint=�63   $	��150��6J    �� 	StaysOpen .   5N      �� 5O      .@�5Q      m� -   5         m�Name$ 
t_tsb_Open��5Q      $��打开单据��63    5R      �� 5S      $/��)/XProjectWPF;component/Resources/Open.png��6$   $��16��6W   $��16��6b   5U      m� 5V      -   5        $ t_tsb_Delete��5V      $��删除单据��65    5W      �� 5X      $1��+/XProjectWPF;component/Resources/Delete.png��6$   $��16��6Y   $��16��6d   5Z      m� 5[      -   5        $ t_tsb_ChangeState��5[      $��改变状态��6:    5\      �� 5]      $5��//XProjectWPF;component/Resources/SetVisible.png��6$   $��16��6]   $��16��6h   5_      5`      5b         RJ.XStyle.XGroupBox  5c   
   $��单据日期范围设置��6   $
��Left=�6-   $	��140��6H   $��10,0,0,0q�6U   $��Bottom=�6g   $	��240��6�      GroupBoxDrawType$ TopLine=�6�   ���#F0F5FA.��5d      � .+�5e         RJ.XStyle.XDatePicker  -   5          Name$ t_dtp_StartDate��5e   %   {�. $
��Left=�6O   $��
15,105,0,0q�6j   $	��Top=�6~   $��90��6�   $ 0.578,0.708&�6�   6�      RJ.XStyle.XLabel  5f      $	��至��6    $��116,109,110,0q�6,   $	��Top=�6C   $ 2.318,-0.24&�6[     5g      -   5        $ t_dtp_EndDate��5g   %   {�. $
��Left=�6M   $��140,105,0,0q�6h   $	��Top=�6}   $��90��6�   6�      RJ.XStyle.XRadioButton  5h      -   5          Name$ t_rdo_Today��5h   &   $��当天单证��6;   $
��Left=�6J   $��	15,20,0,0q�6e   $	��Top=�6x   
 � Tag$ Today��6�     5i      -   5        $ 
t_rdo_Week��5i   &   $��当周单证��6:   $
��Left=�6I   $��
140,20,0,0q�6d   $	��Top=�6x   $ 0.224,1.929&�6�   $
 Week��6    5j      -   5        $ t_rdo_Month��5j   &   $��当月单证��6;   $
��Left=�6J   $��	15,48,0,0q�6e   $	��Top=�6x   $ 0.925,1.214&�6�   $ Month��6    5k      -   5        $ 	t_rdo_All��5k   &   $��所有单证��69   $
��Left=�6H   $��
140,48,0,0q�6c   $	��Top=�6w    d� 	IsChecked$
 TrueY�6�   $ 0.507,-0.143&�6�   $	 All��6    5l      -   5        $ t_rdo_Custom��5l   &   $��自定义范围��6<   $
��Left=�6L   $��	15,76,0,0q�6g   $	��Top=�6z   $ Custom��6�   5m      5n      5o      5p      
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinEditors;

namespace YR.ERP.Base.Model
{
    /// <summary>
    /// 做為進階查詢之間傳遞訊息的物件
    /// </summary>
    public class AdvanceQueryInfo
    {
        public AdvanceQueryInfo()
        {
            ConditionList = new List<ConditionInfo>();
        }

        public List<ConditionInfo> ConditionList { get; set; }          //載入畫面上的需求資料,由ADMI620取得
        public string Key;                                              //傳入主檔的來源查詢資料ado01
        public List<SqlParameter> RelationParams;                       // master detial 的 relation 參數 for detail使用

        public string StrWhereAppend;
        public List<SqlParameter> sqlParmsList;

        public DialogResult Result { get; set; }                        //傳回OK 或 Cacel

        //設定UI控制項
        public UltraGroupBox UGroupBox { get; set; }
        public RadioButton RadioAnd { get; set; }
        public RadioButton RadioOr { get; set; }
        public UltraCombo UComboColumn { get; set; }
        public UltraCombo UComboCondition { get; set; }
        public UltraTextEditor UTextValue { get; set; }
        public UltraGrid UGrid { get; set; }
    }
}

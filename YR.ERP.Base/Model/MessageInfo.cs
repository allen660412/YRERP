using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Windows.Forms;

namespace YR.ERP.Base.Model
{
    /// <summary>
    /// 做為Pick挑選視窗之間傳遞息的物件
    /// </summary>
    public class MessageInfo
    {
        public MessageInfo()
        {
            DataRowList = new List<System.Data.DataRow>();
            IntMaxRow = 1;
            //IsAutoQuery = false;
        }

        public System.Windows.Forms.Form FrmSource { get; set; }    //來源的表單
        public int IntMaxRow { get; set; }                          //可取回的最多列數
        public Boolean? IsAutoQuery { get; set; }                   //進入視窗時是否要自動查詢 使用PICK時,如為null 交由azp08決定
        public string StrWhereAppend { get; set; }                  //查詢預設條件
        public string StrOrderAppend { get; set; }                  //增加查詢排序
        public string StrMultiColumn { get; set; }                  //將多筆資料以 |區隔的作用欄位,目前尚未用到
        public string StrMultiRtn { get; set; }                     //回傳勾選後的字串,格式 ex: Y01|Y02|Y03....
        public List<System.Data.DataRow> DataRowList { get; set; }  //DataRow List
        public List<SqlParameter> ParamSearchList{ get; set; }      //傳入搜尋陣列
        public DialogResult Result { get; set; }                    //傳回OK 或 Cacel
    }
}

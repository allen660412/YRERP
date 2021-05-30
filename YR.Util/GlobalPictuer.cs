using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YR.Util
{
    public class GlobalPictuer
    {
        #region Property
        private static System.Windows.Forms.ImageList _imgLists;

        public static readonly string TOOLBAR_ACTION = "action_32";
        public static readonly string TOOLBAR_CANCEL_CONFIRM = "cancel_confirm_32";
        public static readonly string TOOLBAR_CANCEL = "cancel_32";
        public static readonly string TOOLBAR_CONFIRM = "confirm_32";
        public static readonly string TOOLBAR_GIVEUP = "give_up_32";
        public static readonly string TOOLBAR_INSERT = "insert_32";
        public static readonly string TOOLBAR_INSERT_DETAIL = "insert_detail_32";
        public static readonly string TOOLBAR_INVALID = "invalid_32";
        public static readonly string TOOLBAR_UPDATE = "update_32";
        public static readonly string TOOLBAR_COPY = "copy_32";
        public static readonly string TOOLBAR_DEL = "del_32";
        public static readonly string TOOLBAR_DEL_DETAIL = "del_detail_32";
        public static readonly string TOOLBAR_ERASER = "eraser_32";
        public static readonly string TOOLBAR_EXCEL = "excel_32";
        public static readonly string TOOLBAR_EXIT = "exit_32";
        public static readonly string TOOLBAR_NAVGIATOR = "form_navgiator_32";
        public static readonly string TOOLBAR_QUERY = "query_32";
        public static readonly string TOOLBAR_QUERY_ADVANCE = "query_advance_32";
        public static readonly string TOOLBAR_HOME = "home_32";
        public static readonly string TOOLBAR_OK = "ok_32";
        public static readonly string TOOLBAR_REPORT = "report_32";
        public static readonly string TOOLBAR_REPORT_CHART = "report_chart_32";
        public static readonly string TOOLBAR_REPORT_LIST = "report_list_32";
        public static readonly string TOOLBAR_REPORT_RECEIPT = "report_receipt_32";
        public static readonly string TOOLBAR_SAVE = "save_32";
        public static readonly string TOOLBAR_SELECT_ALL = "select_all_32";
        public static readonly string TOOLBAR_SELECT_NONE = "select_none_32";
        public static readonly string TOOLBAR_END = "end_32";
        public static readonly string TOOLBAR_FIRST = "first_32";
        public static readonly string TOOLBAR_NEXT = "next_32";
        public static readonly string TOOLBAR_PREVIOUS = "previous_32";

        public static readonly string TOOLBAR_SHIPPING = "shipped_32";
        public static readonly string TOOLBAR_PRINTER = "printer_32";
        public static readonly string TOOLBAR_TRANSH_CAN = "trash_can_32";

        public static readonly string DOC_CLOSED = "doc_closed_32";
        public static readonly string DOC_CONFIRM = "doc_confirm_32";
        public static readonly string DOC_OPEN = "doc_open_32";
        public static readonly string DOC_INVALID = "doc_invalid_32";

        public static readonly string NAVIGATOR_QUERY = "magnifier_128";
        public static readonly string NAVIGATOR_BATCH = "batch_128";
        public static readonly string NAVIGATOR_REPORT = "report_128";
        public static readonly string NAVIGATOR_PARAMETER = "parameter_128";
        public static readonly string NAVIGATOR_MENU = "menu_128";
        public static readonly string NAVIGATOR_RECEIPT = "receipt_128";
        public static readonly string NAVIGATOR_FILING = "documnet_128";

        public static readonly string MODULE_BAS = "module_bas_32";
        public static readonly string MODULE_INV = "module_inv_32";
        public static readonly string MODULE_PUR = "module_pur_32";
        public static readonly string MODULE_STP = "module_stp_32";
        public static readonly string MODULE_ADM = "module_adm_32";
        public static readonly string MODULE_MAN = "module_man_32";
        public static readonly string MODULE_GLA = "module_gla_32";
        public static readonly string MODULE_CAR = "module_car_32";
        public static readonly string MODULE_TAX = "module_tax_32";

        public static readonly string MENU_TREE_FOLDER = "folder_16";
        public static readonly string MENU_TREE_FOLDER_ACTIVE = "folder_active_16";
        public static readonly string MENU_TREE_FORM = "form_16";
        public static readonly string MENU_TREE_FORM_ACTIVE = "form_active_16";


        #endregion

        #region LoadToolBarImage() 32*32
        public static ImageList LoadToolBarImage()
        {
            string imagePath = "", imageName = "", imageFullName = ""; ;
            try
            {
                _imgLists = new ImageList();
                _imgLists.ColorDepth = ColorDepth.Depth32Bit;
                _imgLists.ImageSize = new System.Drawing.Size(32, 32);
                imagePath = System.Windows.Forms.Application.StartupPath + "\\pictures\\";

                imageName = TOOLBAR_ACTION;
                imageFullName = imagePath + imageName + ".png";
                //ui 在設計環境時如不加入此行會有問題,勿移除
                if (!System.IO.File.Exists(imageFullName))
                    return null;
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_CANCEL_CONFIRM;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_CANCEL;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_CONFIRM;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_INSERT;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_INSERT_DETAIL;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_INVALID;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_UPDATE;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_COPY;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_DEL;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_DEL_DETAIL;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_ERASER;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_EXCEL;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_EXIT;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = "form_active_32";   //????好像沒用到
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_GIVEUP;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_NAVGIATOR;   
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_QUERY;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_QUERY_ADVANCE;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));


                imageName = TOOLBAR_HOME;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_OK;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = "menu_active_32";       //?????
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = "pdf_32";               //????
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_REPORT;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_REPORT_CHART;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_REPORT_LIST;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_REPORT_RECEIPT;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_SAVE;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_SELECT_ALL;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_SELECT_NONE;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = "wizard_32";            //????
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = "word_32";              //?????
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_END;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_FIRST;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_NEXT;
                imageFullName = imagePath + imageName + ".png";
                 _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_PREVIOUS;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = "pick_32";                  //要切開
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_PRINTER;                  
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_SHIPPING;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = TOOLBAR_TRANSH_CAN;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                return _imgLists;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        } 
        #endregion

        #region LoadProgramListImage() 16*16
        public static ImageList LoadProgramListImage()
        {
            string imagePath = "", imageName = "", imageFullName = ""; ;
            try
            {
                _imgLists = new ImageList();
                _imgLists.ColorDepth = ColorDepth.Depth32Bit;
                _imgLists.ImageSize = new System.Drawing.Size(16, 16);
                imagePath = System.Windows.Forms.Application.StartupPath + "\\pictures\\";

                imageName = MENU_TREE_FOLDER;
                imageFullName = imagePath + imageName + ".png";
                //ui 在設計環境時如不加入此行會有問題,勿移除
                if (!System.IO.File.Exists(imageFullName))
                    return null;
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = MENU_TREE_FOLDER_ACTIVE;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = MENU_TREE_FORM;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = MENU_TREE_FORM_ACTIVE;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                return _imgLists;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
        #endregion

        #region LoadDocImage--取得確認,取消確認..等圖案
        public static ImageList LoadDocImage()
        {
            string imagePath = "", imageName = "", imageFullName = ""; ;
            try
            {
                _imgLists = new ImageList();
                _imgLists.ColorDepth = ColorDepth.Depth32Bit;
                _imgLists.ImageSize = new System.Drawing.Size(32, 32);
                imagePath = System.Windows.Forms.Application.StartupPath + "\\pictures\\";

                imageName = DOC_CLOSED;
                imageFullName = imagePath + imageName + ".png";
                //ui 在設計環境時如不加入此行會有問題,勿移除
                if (!System.IO.File.Exists(imageFullName))
                    return null;
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = DOC_CONFIRM;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = DOC_OPEN;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = DOC_INVALID;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));
                return _imgLists;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        } 
        #endregion

        #region LoadNavigatorImage()  導覽視窗專用
        public static ImageList LoadNavigatorImage()
        {
            string imagePath = "", imageName = "", imageFullName = ""; ;
            try
            {
                _imgLists = new ImageList();
                _imgLists.ColorDepth = ColorDepth.Depth32Bit;
                _imgLists.ImageSize = new System.Drawing.Size(80, 80);
                imagePath = System.Windows.Forms.Application.StartupPath + "\\pictures\\";

                imageName = NAVIGATOR_BATCH;
                imageFullName = imagePath + imageName + ".png";
                //ui 在設計環境時如不加入此行會有問題,勿移除
                if (!System.IO.File.Exists(imageFullName))
                    return null;
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = NAVIGATOR_FILING;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = NAVIGATOR_MENU;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = NAVIGATOR_PARAMETER;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = NAVIGATOR_QUERY;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = NAVIGATOR_RECEIPT;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = NAVIGATOR_REPORT;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));
                return _imgLists;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        } 
        #endregion

        #region LoadModuleImage()  
        public static ImageList LoadModuleImage()
        {
            string imagePath = "", imageName = "", imageFullName = ""; ;
            try
            {
                _imgLists = new ImageList();
                _imgLists.ColorDepth = ColorDepth.Depth32Bit;
                _imgLists.ImageSize = new System.Drawing.Size(32, 32);
                imagePath = System.Windows.Forms.Application.StartupPath + "\\pictures\\";

                imageName = MODULE_BAS;
                imageFullName = imagePath + imageName + ".png";
                //ui 在設計環境時如不加入此行會有問題,勿移除
                if (!System.IO.File.Exists(imageFullName))
                    return null;
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = MODULE_INV;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = MODULE_PUR;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = MODULE_STP;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = MODULE_ADM;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = MODULE_MAN;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = MODULE_GLA;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = MODULE_CAR;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = MODULE_TAX;
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));
                return _imgLists;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region LoadUsedImage() 載入二手圖案
        public static ImageList LoadUsedImage()
        {
            string imagePath = "", imageName = "", imageFullName = ""; ;
            try
            {
                _imgLists = new ImageList();
                _imgLists.ColorDepth = ColorDepth.Depth32Bit;
                _imgLists.ImageSize = new System.Drawing.Size(256, 175);
                imagePath = System.Windows.Forms.Application.StartupPath + "\\pictures\\";

                imageName = MODULE_BAS;
                imageFullName = imagePath + imageName + ".png";
                //ui 在設計環境時如不加入此行會有問題,勿移除
                if (!System.IO.File.Exists(imageFullName))
                    return null;
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));

                imageName = "old_goods";
                imageFullName = imagePath + imageName + ".png";
                _imgLists.Images.Add(imageName, System.Drawing.Image.FromFile(imageFullName));
              
                return _imgLists;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region GetBytesFromImage 取得圖像
        public static byte[] GetBytesFromImage(Image img)
        {
            if (img == null) return null;
            byte[] result;
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                Bitmap old = new Bitmap(img);
                //Bitmap new_bit = new Bitmap(old, Convert.ToInt32(5.52 * 38), Convert.ToInt32(4.86 * 38));
                Bitmap new_bit = new Bitmap(old);
                old.Dispose();

                new_bit.Save(stream, img.RawFormat);
                result = stream.GetBuffer();
            }
            return result;
        }
        #endregion
    }
}

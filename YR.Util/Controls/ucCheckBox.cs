using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Drawing;
using System.Windows.Forms;


namespace YR.Util.Controls
{
    /// <summary>
    /// Null-able/bindable checkbox.
    /// </summary>
    [DefaultProperty("CheckValue"), DefaultEvent("CheckValueChanged"), ToolboxBitmap(typeof(CheckBox))]
    public partial class UcCheckBox : System.Windows.Forms.CheckBox
    {
        /// <summary>
        /// The current value of the checkbox.
        /// </summary>
        private object _checkValue = "N";

        // private object _nullValue = Convert.DBNull;
        private object _nullValue = "N";
        private object _trueValue = "Y";
        private object _falseValue = "N";
        private bool _emptyStringIsDBNull = true;

        #region "Helper Methods"
        /// <summary>
        /// 檢查傳入的值是否符合checkbox 的內含值(nullvalue/falsevalue/truevalue)
        /// </summary>
        /// <param name="checkValue">傳入檢查值</param>
        /// <returns></returns>
        private bool IsValidCheckValue(object checkValue)
        {
            bool retValue = (checkValue.Equals(this.NullValue) ||
                checkValue.Equals(this._falseValue) ||
                checkValue.Equals(this._trueValue));

            return retValue;
        }

        internal protected virtual CheckState FromCheckValue(object checkValue)
        {
            CheckState retValue = CheckState.Unchecked;

            if (checkValue.Equals(this.TrueValue))
                retValue = CheckState.Checked;
            else if (checkValue.Equals(this.FalseValue))
                retValue = CheckState.Unchecked;
            else if (checkValue.Equals(this.NullValue))
                retValue = CheckState.Indeterminate;

            return retValue;
        }

        internal protected virtual object FromCheckState(CheckState checkState)
        {
            object retValue = this.FalseValue;

            if (checkState.Equals(CheckState.Checked))
                retValue = this.TrueValue;
            else if (checkState.Equals(CheckState.Indeterminate))
                retValue = this.NullValue;

            return retValue;
        }

        #endregion

        #region "Overrides Stuff"
        /// <summary>
        /// Override to change CheckValue when CheckState changed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCheckStateChanged(EventArgs e)
        {
            // only set new CheckValue if different
            object checkValue = this.FromCheckState(this.CheckState);
            if (this.CheckValue != checkValue)
                this.CheckValue = checkValue;

            base.OnCheckStateChanged(e);
        }
        #endregion

        #region "Properties"


        //[DefaultValue(true), Description("True for CheckValue to automatically convert empty string to NullValue.")]
        /// <summary>
        /// 設定true時,若傳入null或空白會自動以NullValue代替
        /// </summary>
        [DefaultValue(true), Description("設定true時,若傳入null或空白會自動以NullValue代替")]
        public bool EmptyStringIsNull
        {
            get { return this._emptyStringIsDBNull; }
            set { this._emptyStringIsDBNull = value; }
        }
        [DefaultValue("N"), Bindable(true), RefreshProperties(RefreshProperties.All), TypeConverter(typeof(StringConverter))]
        public object CheckValue
        {
            get { return this._checkValue; }
            set
            {
                // null is DBNull
                if (value == null) value = DBNull.Value;

                // DBNull and empty string are translated to this.NullValue
                if ((value.Equals(DBNull.Value))
                    || (this.EmptyStringIsNull && (value.ToString().Length <= 0))) value = this.NullValue;

                // throw exception if value is not valid
                // 檢查傳入值不符合內含值(nullvalue/falsevalue/truevalue)時,傳出例外
                // 這邊改寫成 若不是其中一項時,以 falsevalue代替,以利後續狀態
                //if (!this.IsValidCheckValue(value))
                //    throw new ArgumentOutOfRangeException("CheckValue", value, "NCheckBox CheckValue is invalid.");
                if (!this.IsValidCheckValue(value))
                    value = _falseValue;

                if (!this._checkValue.Equals(value))
                {
                    this._checkValue = value;
                    this.OnCheckValueChanged(EventArgs.Empty);

                    // Cause a checkstate change
                    CheckState newState = this.FromCheckValue(value);
                    if (this.CheckState != newState)
                        this.CheckState = newState;
                }
            }
        }

        /// <summary>
        /// 設定false value,未勾選時的內含值
        /// </summary>
        [DefaultValue("N"), TypeConverter(typeof(StringConverter)), Description("設定false value,未勾選時的內含值")]
        public object FalseValue
        {
            get { return this._falseValue; }
            set
            {
                if (value != null && !this._falseValue.Equals(value))
                {
                    this._falseValue = value;
                    this.OnFalseValueChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// 設定null時,要轉換的值
        /// </summary>
        [TypeConverter(typeof(StringConverter)), Description("設定null時,要轉換的值")]
        public object NullValue
        {
            get { return this._nullValue; }
            set
            {
                if (!this._nullValue.Equals(value))
                {
                    this._nullValue = (value == null) ? DBNull.Value : value;
                    this.OnFalseValueChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// 設定勾選時的內含值
        /// </summary>
        [DefaultValue("Y"), TypeConverter(typeof(StringConverter)), Description("設定勾選時的內含值")]
        public object TrueValue
        {
            get { return this._trueValue; }
            set
            {
                if (value != null && !this._trueValue.Equals(value))
                {
                    this._trueValue = value;
                    this.OnTrueValueChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// New to support AllowNull notification.
        /// </summary>
        [DefaultValue(false), Description("設定元件是否要有三種狀態")]
        public new bool ThreeState
        {
            get { return base.ThreeState; }
            set
            {
                if (value != base.ThreeState)
                {
                    base.ThreeState = value;
                    this.OnAllowNullChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Allow three state CheckBox.
        /// </summary>
        [DefaultValue(false)]
        public virtual bool AllowNull
        {
            get { return this.ThreeState; }
            set { this.ThreeState = value; }
        }
        #endregion

        #region "Events"
        public event EventHandler CheckValueChanged;
        protected virtual void OnCheckValueChanged(EventArgs e)
        {
            if (this.CheckValueChanged != null)
                this.CheckValueChanged(this, e);
        }

        public event EventHandler AllowNullChanged;
        protected virtual void OnAllowNullChanged(EventArgs e)
        {
            if (AllowNullChanged != null)
                AllowNullChanged(this, e);
        }

        public event EventHandler FalseValueChanged;
        protected virtual void OnFalseValueChanged(EventArgs e)
        {
            if (FalseValueChanged != null)
                FalseValueChanged(this, e);
        }

        public event EventHandler TrueValueChanged;
        protected virtual void OnTrueValueChanged(EventArgs e)
        {
            if (TrueValueChanged != null)
                TrueValueChanged(this, e);
        }
        #endregion

    }
}

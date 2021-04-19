using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR.Util
{
    public class Result
    {
        /// <summary>
        /// 預設回傳錯誤
        /// </summary>
        public Result()
            : this(false)
        {
        }

        public Result(bool success)
        {
            ID = Guid.NewGuid();
            Success = success;
        }

        public Guid ID
        {
            get;
            private set;
        }

        public bool Success { get; set; }

        public string Key1 { get; set; }
        public string Key2 { get; set; }
        public string Message { get; set; }

        public Exception Exception { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using YR.DAL;

namespace YR.ERP.DAL
{
    public class ERP_MSSQLDAL : YR.DAL.MSSQLDAL, YR.ERP.DAL.IERP_DAL, YR.ERP.DAL.IERP_PU_DAL
    {        
    }
}

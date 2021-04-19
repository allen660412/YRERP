using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace YR.ERP
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            string lsPath = "";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false); ;
            lsPath = System.Windows.Forms.Application.StartupPath + "\\StyleLibrary\\IG.isl";
            //lsPath = System.Windows.Forms.Application.StartupPath + "\\StyleLibrary\\Office2010Blue.isl";            
            Infragistics.Win.AppStyling.StyleManager.Load(lsPath);
            //Application.Run(new Forms.FrmMenu());
            Stimulsoft.Report.StiConfig.LoadLocalization(@"Localization\zh-CHT.xml");
            Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHlLc13jY+oNsBqJ3H1iVz3ZYDW/DEFeb2LEPIIkc/0onIZmw4" +
                    "1yslgwz5FFIFAo+TQqg4MxsA1BOrb0Uwl3e7BgM9u/ojkySmLwL08Kr1IJlnDYaFT66yVqFEr3FcF1CvSdh0Vk1zJr" +
                    "JIz3437OK07lhweahHDSSbZZlbAIZmKaQAjMoKpsJNAgjd0NiVmJiMPYPA4LUrrP9tmTYmwMg0EZmSGgxomumHiHBd" +
                    "kFnNpHxXrryMBJoq1ORwsnKP4uLJ5p1TyDqZgTt0orZ1crUgrVOxBuVa4x+nFoWqZmuYoNU9oHlNjVksSQeqHDvlWS" +
                    "xErAMAZ50jswAkwZK49n+XKNazYA4kwDaykqzAPBucw84sDJ0BFiiAP2yVa6UponQYHK2M38NeZ3c9MJGV5wVigAQd" +
                    "9bzkyi43xFPhpJmHxFQEILWZd8rsFhhD0fvjsnWLKBxcyamegcVfwYd4rGil5RnzAZp8FVkA6H/T3OWmnzBmozRHe6" +
                    "j+rkVxa2wni8TZeZVGlVGF8ovwFeETDKMKjQ";

            Application.Run(new Forms.FrmMain());
        }

    }
}

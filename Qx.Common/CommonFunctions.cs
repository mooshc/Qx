using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Qx.Common
{
    public static class CommonFunctions
    {
        public static readonly string ImageNativePath = AppDomain.CurrentDomain.BaseDirectory + "Pics/";

        public static readonly string DialogsNativePath = AppDomain.CurrentDomain.BaseDirectory + "Dialogs/";

        public static readonly string GraphicsNativePath = AppDomain.CurrentDomain.BaseDirectory + "Grphics/"; 

        public static BitmapImage GetBmpImageByFileName(string FileName, bool isDialog = false)
        {
            return new BitmapImage(new Uri((isDialog ? DialogsNativePath : ImageNativePath) + FileName));
        }

        public static Language HebLang;
    }
}

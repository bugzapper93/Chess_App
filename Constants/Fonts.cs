using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Drawing;

namespace Chess_App
{
    public static class Fonts
    {
        private static PrivateFontCollection font_collection = new PrivateFontCollection();
        static Fonts()
        {
            Load_Font();
        }
        private static void Load_Font()
        {
            string font_name = "Chess_App.Resources.pixeldroidMenuRegular.ttf";

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(font_name))
            {
                System.IntPtr data = Marshal.AllocCoTaskMem((int)stream.Length);
                byte[] font_data = new byte[stream.Length];
                stream.Read(font_data, 0, (int)stream.Length);
                Marshal.Copy(font_data, 0, data, (int)stream.Length);
                font_collection.AddMemoryFont(data, (int)stream.Length);
                Marshal.FreeCoTaskMem(data);
            }
        }
        public static Font Get_Font(float size, FontStyle style = FontStyle.Regular)
        {
            return new Font(font_collection.Families[0], size, style);
        }
    }
}

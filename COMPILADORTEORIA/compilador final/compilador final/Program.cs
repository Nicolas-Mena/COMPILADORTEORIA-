using System;
using System.Windows.Forms;

namespace CompiladorFinal
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); // ← Aquí debe coincidir el nombre exactamente
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BubblePony.Integers;
using BubblePony.Alloc;
namespace Quad64
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        unsafe static void Main(string[] args)
        {
			byte* kilobyte_memory = stackalloc byte[1024];
			Globals._kilobyte = kilobyte_memory;
			if (new Fixed16_16 { Whole = 1 }.Value != Fixed16_16.One) throw new System.InvalidOperationException();
			Allocation sixtyFourMegabytes = new Allocation(67108864u);
			Globals._alloc64 = sixtyFourMegabytes;
			Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm((args.Length != 0)?
				System.IO.Path.GetFullPath(string.Join(" ", args)) : null));
			GC.KeepAlive(sixtyFourMegabytes);
        }
    }
}

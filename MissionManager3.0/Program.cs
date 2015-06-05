using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SocketTutorial.FormsServer
{
    
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Server server = new Server();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(server); //altered from new Server()
        }
    }
}

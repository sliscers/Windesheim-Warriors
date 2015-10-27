using System.Windows.Forms;

namespace WindesHeim_Game
{
    static class Program
    {
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GameWindow());
        } 
    }
}

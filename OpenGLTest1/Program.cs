using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;


namespace OpenGLTest1
{
    class Program
    {
        static void Main(string[] args)
        {
            // The 'using' idiom guarantees proper resource cleanup.
            // We request 30 UpdateFrame events per second, and unlimited
            // RenderFrame events (as fast as the computer can handle).
            using (WizardWarsGameWindow game = new WizardWarsGameWindow())
            {
                game.Run(30.0);
            }
        }
    }
}

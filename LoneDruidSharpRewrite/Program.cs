using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoneDruidSharpRewrite
{
    class Program
    {
        #region Static Fields

        private static Bootstrap bootstrap;

        #endregion

        static void Main(string[] args)
        {
            bootstrap = new Bootstrap();
            bootstrap.SubscribeEvents();
        }
    }
}

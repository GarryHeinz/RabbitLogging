using System;
using System.Collections.Generic;
using System.Text;
using Base.logging;
namespace Base
{
    public class Demo
    {

        public static void Main(string[] args)
        {
            Logger l = new Logger(SystemName:"Demo");
            ServerLogger sl = new ServerLogger();
            sl.init();
            l.init();
            l.log("Hallo");

        }

        
    }
}

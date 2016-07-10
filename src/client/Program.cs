using System;
using System.Reflection;

namespace Sky72C
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 4)
            {
                DateTime _require = Convert.ToDateTime(args[0]);

                int _from = Convert.ToInt32(args[1]);
                int _till = Convert.ToInt32(args[2]);
                int _interval = Convert.ToInt32(args[3]);

                sky72 _sky = new sky72(_require, _from, _till, _interval, args[4]);
                _sky.StartService();
            }
            else
            {
                string _name = Assembly.GetEntryAssembly().GetName().Name;
                Console.WriteLine(String.Format("arg rrror: {0} requireDay fromTime tillTime interval cellphone", _name));
                Console.WriteLine(String.Format("ex) {0} 2009-12-31 16 18 5 01199250593;0102223333", _name));
            }
        }
    }
}

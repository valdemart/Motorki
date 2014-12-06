using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace event_processing
{
    class Program
    {
        public delegate void SomeEvent(CancellableEventArgs cea);
        public static event SomeEvent myEvent;

        public static void eventhandler1(CancellableEventArgs cea)
        {
            Console.WriteLine("eventhandler1");
            cea.CancelEvent = true;
        }

        public static void eventhandler2(CancellableEventArgs cea)
        {
            Console.WriteLine("eventhandler2");
        }

        //funkcja nadzorujaca wywolywanie handlerow eventu
        public static void RiseEvent()
        {
            Delegate[] inv_list = myEvent.GetInvocationList();
            if (inv_list.Length == 0)
                return;
            for (int i = 0; i < inv_list.Length; i++)
            {
                CancellableEventArgs cea = new CancellableEventArgs();
                inv_list[i].Method.Invoke(null /*this*/, new object[] { cea });
                if (cea.CancelEvent)
                    return;
            }
        }

        static void Main(string[] args)
        {
            //assign handlers to event
            myEvent += eventhandler1;
            myEvent += eventhandler2;

            //rise event in usual way
            Console.WriteLine("normal rise");
            CancellableEventArgs cea = new CancellableEventArgs();
            myEvent(cea);

            //rise event through RiseEvent method
            Console.WriteLine("controlled rise");
            RiseEvent();

            Console.ReadKey(true);
        }
    }

    /// <summary>
    /// moja klasa parametrow eventu, zawierajaca informacje o tym czy nalezy przerwac event. klasa poniewaz trzeba przekazywania przez referencje
    /// </summary>
    public class CancellableEventArgs
    {
        public bool CancelEvent { get; set; }

        public CancellableEventArgs()
        {
            CancelEvent = false;
        }
    }
}

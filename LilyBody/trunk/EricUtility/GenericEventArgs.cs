using System;
using System.Collections.Generic;
using System.Text;

namespace EricUtility
{
    /// <summary>
    /// EventArgs avec un param d'un type quelconque (Key)
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class KeyEventArgs<TKey> : EventArgs
    {
        private TKey m_Key;
        public TKey Key { get { return m_Key; } }

        public KeyEventArgs(TKey key)
        {
            m_Key = key;
        }
    }

    /// <summary>
    /// EventArgs avec un param d'un type quelconque (Key) et un autre param d'un type quelconque (Value)
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class KeyValueEventArgs<TKey, TValue> : EventArgs
    {
        private TKey m_Key;
        private TValue m_Value;
        public TKey Key { get { return m_Key; } }
        public TValue Value { get { return m_Value; } }

        public KeyValueEventArgs(TKey key, TValue value)
        {
            m_Key = key;
            m_Value = value;
        }
    }

    /// <summary>
    /// EventArgs avec un param d'un type quelconque (Key) et d'autres params d'un type quelconque (Values)
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class KeyValuesEventArgs<TKey, TValue> : EventArgs
    {
        private TKey m_Key;
        private TValue[] m_Values;
        public TKey Key { get { return m_Key; } }
        public TValue[] Values { get { return m_Values; } }

        public KeyValuesEventArgs(TKey key, params TValue[] values)
        {
            m_Key = key;
            m_Values = values;
        }
    }
}
    /*
     * SIMULATION D'UTILISATION
     *
    class ClassSimulation
    {
        public event EventHandler<KeyEventArgs<string>> MessagePopped;
        public event EventHandler<KeyValueEventArgs<Control, bool>> VisibilityChanged;
        public event EventHandler<KeyValuesEventArgs<StreamWriter, int>> SumRequested;

        private StreamWriter fi;
        private Control co;

        public ClassSimulation()
        {
            fi = new StreamWriter("test.txt");
            Control co = new Button();
        }

        public void SimuleChangeVisibility()
        {
            if (VisibilityChanged != null)
                VisibilityChanged(this, new KeyValueEventArgs<Control, bool>(co, co.Visible));
        }
        public void SimuleMessageArrived(string s)
        {
            if (MessagePopped != null)
                MessagePopped(this, new KeyEventArgs<string>(s));
        }
        public void SimuleAsk2Nbs(int a, int b)
        {
            if (SumRequested != null)
                SumRequested(this, new KeyValuesEventArgs<StreamWriter, int>(fi, a, b));
        }
        public void SimuleAsk8Nbs(int a, int b, int c, int d, int e, int f, int g, int h)
        {
            if (SumRequested != null)
                SumRequested(this, new KeyValuesEventArgs<StreamWriter, int>(fi, a, b, c, d, e, f, g, h));
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ClassSimulation c = new ClassSimulation();
            c.MessagePopped += new EventHandler<KeyEventArgs<string>>(c_MessagePopped);
            c.SumRequested += new EventHandler<KeyValuesEventArgs<StreamWriter, int>>(c_SumRequested);
            c.VisibilityChanged += new EventHandler<KeyValueEventArgs<Control, bool>>(c_VisibilityChanged);

            //Simulation
            c.SimuleAsk2Nbs(2, 7);
            c.SimuleChangeVisibility();
            c.SimuleMessageArrived("MessageArrived");
            c.SimuleChangeVisibility();
            c.SimuleAsk8Nbs(1, 2, 3, 4, 5, 6, 7, 8);
        }

        static void c_VisibilityChanged(object sender, KeyValueEventArgs<Control, bool> e)
        {
            Console.WriteLine("Visibility of {0} changed to {1}", e.Key, e.Value);
        }

        static void c_SumRequested(object sender, KeyValuesEventArgs<StreamWriter, int> e)
        {
            string res = String.Format("La somme de tous les nombres donne {0}", e.Values.Sum());
            Console.WriteLine(res);
            e.Key.WriteLine(res);
        }

        static void c_MessagePopped(object sender, KeyEventArgs<string> e)
        {
            Console.WriteLine(e.Key);
        }
    }
}*/

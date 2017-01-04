using System;
using System.Collections.Generic;
using System.Text;

namespace EricUtility
{
    public class FlagGroup<T> where T : struct, IComparable, IFormattable, IConvertible
    {
        public FlagGroup()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
        }
        private T flag = (T)Enum.ToObject(typeof(T),0);

        public bool Allow(T f)
        {
            return (Convert.ToInt32(flag) & Convert.ToInt32(f)) != 0;
        }

        public void SetFlag(T f, bool value)
        {
            int val = Convert.ToInt32(flag);
            if (value)
                val |= Convert.ToInt32(f);
            else
                val &= ~Convert.ToInt32(f);
            flag = (T)Enum.ToObject(typeof(T), val);
        }

        public T Flag { get { return flag; } }
    }
}

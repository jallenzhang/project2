using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebNotifications.Permission
{
    public delegate bool EqualsComparer<T>(T x, T y);
    /// <summary>
    /// 过滤重复数据
    /// </summary>
    public class Comparint<T> : IEqualityComparer<T>
    {
        private EqualsComparer<T> ec;
        public Comparint() { }
        public Comparint(EqualsComparer<T> e)
        {
            this.ec = e;
        }
        #region IEqualityComparer<BudgetBE> 成员

        public bool Equals(T x, T y)
        {
            if (null != this.ec)
                return this.ec(x, y);
            else
                return false;
        }

        public int GetHashCode(T obj)
        {
            return obj.ToString().GetHashCode();
        }

        #endregion
    }
}
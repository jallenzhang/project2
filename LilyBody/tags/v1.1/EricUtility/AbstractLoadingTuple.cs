using System;
using System.Collections.Generic;
using System.Text;
using EricUtility;

namespace EricUtility
{
    public abstract class AbstractLoadingTuple
    {
        protected bool m_Loaded = false;
        private bool m_Loading = false;
        public bool Loaded { get { return m_Loaded; } }

        protected abstract void OnLoad();

        public void ForceLoad()
        {
            NeedToReload();
            LoadIfNeeded();
        }

        public void NeedToReload()
        {
            if( !m_Loading )
                m_Loaded = false;
        }

        public void LoadIfNeeded()
        {
            if (!m_Loaded && !m_Loading)
            {
                m_Loading = true;
                OnLoad();
                m_Loaded = true;
                m_Loading = false;
            }
        }
    }
}

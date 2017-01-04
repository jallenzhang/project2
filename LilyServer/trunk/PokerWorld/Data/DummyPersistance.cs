using System;
using System.Collections.Generic;
using System.Text;

namespace PokerWorld.Data
{
    public class DummyPersistance : IDataPersistance
    {
        private Dictionary<string, UserInfo> usersByUsername = new Dictionary<string, UserInfo>();
        private Dictionary<string, UserInfo> usersByDisplayname = new Dictionary<string, UserInfo>();

        #region IDataPersistance Members

        public bool IsUsernameExist(string username)
        {
            return usersByUsername.ContainsKey(username.ToLower());
        }

        public bool IsDisplayNameExist(string displayName)
        {
            return usersByDisplayname.ContainsKey(displayName.ToLower());
        }

        public void Register(UserInfo u)
        {
            usersByUsername.Add(u.Username.ToLower(), u);
            usersByDisplayname.Add(u.DisplayName.ToLower(), u);
        }

        public UserInfo Get(string username)
        {
            if (!IsUsernameExist(username))
                return null;
            return usersByUsername[username.ToLower()];
        }

        public UserInfo Authenticate(string username, string password)
        {
            UserInfo u = Get(username);
            if (u != null && u.Password == password)
                return u;
            return null;
        }

        public void Update(UserInfo u)
        {
            usersByUsername[u.Username.ToLower()] = u;
            usersByDisplayname[u.DisplayName.ToLower()] = u;
        }

        public void Delete(UserInfo u)
        {
            usersByUsername.Remove(u.Username.ToLower());
            usersByDisplayname.Remove(u.DisplayName.ToLower());
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PokerWorld.Data
{
    public interface IDataPersistance
    {
        bool IsUsernameExist(string username);
        bool IsDisplayNameExist(string displayName);
        void Register(UserInfo u);
        UserInfo Get(string username);
        UserInfo Authenticate(string username, string password);
        void Update(UserInfo u);
        void Delete(UserInfo u);
    }
}

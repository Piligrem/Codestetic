using System;
using Codestetic.Web.Core.Domain.Users;

namespace Codestetic.Web.Services.Users
{
    public interface ISessionService
    {
        Session AddNewSession();
        Session AddNewSession(User user, string ip);
        int GetCountSession();
        void CloseSession(User user);
        bool CheckAndCloseOldSession(Session session, bool closeSession = true);
        Session GetSessionById(long id);
        Session GetSessionByIP(string ip);
        Session GetSessionByUser(Codestetic.Web.Core.Domain.Users.User user);
    }
}

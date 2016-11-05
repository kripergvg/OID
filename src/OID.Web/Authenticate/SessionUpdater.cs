using OID.Core;

namespace OID.Web.Authenticate
{
    public class SessionUpdater : ISessionUpdater
    {
        private readonly IUserManager _userManager;

        public SessionUpdater(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public void Update(string sessionId)
        {
            var user = _userManager.GetUser();
            user.SessionId = sessionId;
            _userManager.SetUser(user);
        }
    }
}

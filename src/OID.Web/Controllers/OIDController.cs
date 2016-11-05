using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OID.Web.Core.AlertMessage;
using Constants = OID.Web.Core.Constants;

namespace OID.Web.Controllers
{
    public abstract class OIDController : Controller
    {
        public void ShowMessage(Message message)
        {
            TempData[Constants.ALERT_MESSAGE] = JsonConvert.SerializeObject(message);
        }

        public void ShowError(string message)
        {
            ShowMessage(new Message(message, MessageType.Error));
        }

        public void ShowWarning(string message)
        {
            ShowMessage(new Message(message, MessageType.Warning));
        }

        public void ShowSuccess(string message)
        {
            ShowMessage(new Message(message, MessageType.Success));
        }

        public void ShowInfo(string message)
        {
            ShowMessage(new Message(message, MessageType.Info));
        }

        public IActionResult RedirectToIndex()
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}

using NSSM.Core.Models;
using NSSM2.Core;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace NSSM2.App.Controllers
{
    public class BaseController : Controller
    {
        internal NSContext db = new NSContext("NSCONTEXTCONNSTRING");

        protected NSContext GetNSContext()
        {
            return new NSContext("NSCONTEXTCONNSTRING");
        }

        protected WindowsIdentity GetApplicationUser() => WindowsIdentity.GetCurrent();

        protected Member GetCurrentMemberContext()
        {
            var userPrincipal = UserPrincipal.Current;
            using (var db = GetNSContext())
            {
                var currentMember = db.Members.FirstOrDefault(x => x.ADContextKey == userPrincipal.Guid);
                if (currentMember == null)
                {
                    currentMember.FirstName = userPrincipal.Name;
                    currentMember.Email = userPrincipal.EmailAddress;
                    currentMember.ADContextKey = userPrincipal.Guid;
                    db.Members.Add(currentMember);

                    if (db.SaveChanges() <= 0)
                    {
                        throw new Exception("couldn't create member context");
                    }
                }

                return currentMember;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
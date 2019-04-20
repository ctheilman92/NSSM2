using NSSM2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NSSM2.Controllers
{
    public class BaseController : Controller
    {
        protected NSContext GetNSContext()
        {
            return new NSContext("NSCONTEXTCONNSTRING");
        }
    }
}
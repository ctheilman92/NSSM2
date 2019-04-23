using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSSM2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace NSSM2.Tests.Controllers
{


    [TestClass]
    public class TestBase
    {
        public static string _ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["NSCONTEXTCONNSTRING"].ConnectionString;
            }
        }

        private NSContext GetNSContext()
        {
            var nsContext = new NSContext(_ConnectionString);
            nsContext.Database.Log = DBLogger.Log;
            return nsContext;
        }
    }

    public static class DBLogger
    {
        public static void Log(string message)
        {
            Debug.WriteLine("EF OUTPUT: {0} ", message);
        }
    }

    public class NSCredendtials
    {
        private readonly string _domain;
        private readonly string _password;
        private readonly string _username;

        public string Domain { get { return _domain; } }
        public string Username { get { return _username; } }
        public string Password { get { return _password; } }

        public NSCredendtials(string domain, string username, string password)
        {
            _domain = domain;
            _username = username;
            _password = password;
        }

    }

    internal static class Advapi32
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool DuplicateToken(IntPtr ExistingTokenHandle, int SECURITY_IMPERSONATION_LEVEL,
                                                 out IntPtr DuplicateTokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword,
                                            int dwLogonType, int dwLogonProvider, out IntPtr phToken);
    }

    internal static class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);
    }

    public class NSImpersonation : IDisposable
    {
        private readonly IntPtr _dupeTokenHandle = new IntPtr(0);
        private readonly IntPtr _tokenHandle = new IntPtr(0);
        private WindowsImpersonationContext _impersonatedUser;


        public NSImpersonation(NSCredendtials credentials)
        {
            const int logon32ProviderDefault = 0;
            const int logon32LogonInteractive = 2;
            const int securityImpersonation = 2;

            _tokenHandle = IntPtr.Zero;
            _dupeTokenHandle = IntPtr.Zero;

            if (!Advapi32.LogonUser(credentials.Username, credentials.Domain, credentials.Password,
                                    logon32LogonInteractive, logon32ProviderDefault, out _tokenHandle))
            {
                var win32ErrorNumber = Marshal.GetLastWin32Error();

                // REVIEW: maybe ImpersonationException should inherit from win32exception
                throw new ImpersonationException(win32ErrorNumber, new Win32Exception(win32ErrorNumber).Message,
                                                 credentials.Username, credentials.Domain);
            }

            if (!Advapi32.DuplicateToken(_tokenHandle, securityImpersonation, out _dupeTokenHandle))
            {
                var win32ErrorNumber = Marshal.GetLastWin32Error();

                Kernel32.CloseHandle(_tokenHandle);
                throw new ImpersonationException(win32ErrorNumber, "Unable to duplicate token!", credentials.Username,
                                                 credentials.Domain);
            }

            var newId = new WindowsIdentity(_dupeTokenHandle);
            _impersonatedUser = newId.Impersonate();
        }

        public void Dispose()
        {
            if (_impersonatedUser != null)
            {
                _impersonatedUser.Undo();
                _impersonatedUser = null;

                if (_tokenHandle != IntPtr.Zero)
                    Kernel32.CloseHandle(_tokenHandle);

                if (_dupeTokenHandle != IntPtr.Zero)
                    Kernel32.CloseHandle(_dupeTokenHandle);
            }
        }
    }

}

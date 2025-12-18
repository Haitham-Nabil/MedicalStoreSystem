using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedicalStoreSystem.Models;

namespace MedicalStoreSystem.Helpers
{
    public static class CurrentSession
    {
        public static User LoggedInUser { get; set; }

        public static bool IsLoggedIn
        {
            get { return LoggedInUser != null; }
        }

        public static bool IsAdmin
        {
            get { return IsLoggedIn && LoggedInUser.Role == "Admin"; }
        }

        public static bool IsCashier
        {
            get { return IsLoggedIn && LoggedInUser.Role == "Cashier"; }
        }

        public static bool IsStoreKeeper
        {
            get { return IsLoggedIn && LoggedInUser.Role == "StoreKeeper"; }
        }

        public static bool IsAccountant
        {
            get { return IsLoggedIn && LoggedInUser.Role == "Accountant"; }
        }

        public static void Logout()
        {
            LoggedInUser = null;
        }
    }
}


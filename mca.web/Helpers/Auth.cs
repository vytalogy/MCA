using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mca.web.Helpers
{
    /// <summary>
    /// Authorization Key of Login User
    /// </summary>
    [Serializable]
    public class Auth
    {

        public static bool isAuthorize
        {
            get
            {
                if (Active && isLogin)
                    return true;
                else
                    return false;
            }
        }

        public static bool isLogin
        {
            get
            {
                return HttpContext.Current.Session["isLogin"].ConvertToBool();
            }
        }

        public static int UserID
        {
            get
            {
                return HttpContext.Current.Session["UserID"].ConvertToInt();
            }
        }

        public static string Email
        {
            get
            {
                return HttpContext.Current.Session["Email"].ConvertToString();
            }
        }      

        public static string RoleName
        {
            get
            {
                return HttpContext.Current.Session["RoleName"].ConvertToString();
            }
        }

        public static bool Active
        {
            get
            {
                return HttpContext.Current.Session["Active"].ConvertToBool();
            }
        }

        public static string FirstName
        {
            get
            {
                return HttpContext.Current.Session["FirstName"].ConvertToString();
            }
        }


        public static string LastName
        {
            get
            {
                return HttpContext.Current.Session["LastName"].ConvertToString();
            }
        }
    }
}

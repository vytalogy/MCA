using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mca.model
{
    public class User
    {
        public int id { set; get; }
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public string UserName { set; get; }
        //public String Email { set; get; }
        public string Password { set; get; }
        //public String SecretQuestion { set; get; }
        //public String SecretAnswer { set; get; }
        public DateTime? LastLogin { set; get; }
        public int NoOfLogin { set; get; }
        public int CreatedBy { set; get; }
        public DateTime CreatedOn { set; get; }
        public Boolean Active { set; get; }
        public string RoleName { set; get; }
        public int RoleID { set; get; }
    }
}

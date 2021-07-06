using Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DTO.User
{
    public class UsersDTO
    {
        public List<UserInfo> Users { get; set; }
        public string Logo { get; set; }
    }
}

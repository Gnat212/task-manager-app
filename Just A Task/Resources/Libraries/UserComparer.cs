using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerRemakeApp.Data;

namespace TaskManagerRemakeApp.Resources.Libraries
{
    public class UserComparer : IComparer<User>
    {
        public int Compare(User x, User y) => string.Compare(x.Username, y.Username);
    }
}

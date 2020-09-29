using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_Game_Editing_Tool_WinForm
{
    public class ProvinceEventArgs : EventArgs
    {
        public ProvinceEventArgs(IReadOnlyList<Province> provinces) : base()
        {
            this.provinces = provinces;
        }

        public IReadOnlyList<Province> provinces;
    }
}

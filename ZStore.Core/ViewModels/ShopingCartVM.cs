using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZStore.Core.ViewModels
{
    public class ShopingCartVM
    {
        public IEnumerable<ShopingCart> ShopingCartList { get; set; }

        public double OrderTotal { get; set; }
    }
}

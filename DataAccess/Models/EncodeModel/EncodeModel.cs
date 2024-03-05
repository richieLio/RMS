using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.EncodeModel
{
    public class CreateHashPasswordModel
    {
        public byte[] Salt {  get; set; }
        public byte[] HashedPassword { get; set; }
    }
}

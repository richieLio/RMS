using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.BillModel
{
    public class BillResModel
    {
        public Guid Id { get; set; }

        public decimal? TotalPrice { get; set; }

        public DateTime? Month { get; set; }

        public bool? IsPaid { get; set; }

        public DateTime? PaymentDate { get; set; }

        public Guid? CreateBy { get; set; }

        public Guid? RoomId { get; set; }
        public string RoomName { get; set; }
    }
}

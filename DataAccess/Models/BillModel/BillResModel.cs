using DataAccess.Entities;
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
        public string HouseName { get; set; }
    }
    public class BillDetailsResModel
    {
        public Guid Id { get; set; }

        public decimal? TotalPrice { get; set; }

        public DateTime? Month { get; set; }

        public bool? IsPaid { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string RoomName { get; set; }
        public string HouseName { get; set; }
        public List<BillServiceDetails> Services { get; set; }

    }
    public class BillServiceDetails
    {
        public string ServiceName { get; set; }
        public decimal? Quantity { get; set; }
    }

}

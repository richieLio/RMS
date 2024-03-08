using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.ContractModel
{
    public class ContractResModel
    {
        public Guid Id { get; set; }

        public Guid? OwnerId { get; set; }

        public Guid? CustomerId { get; set; }

        public Guid? RoomId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? ImagesUrl { get; set; }

        public string? FileUrl { get; set; }

        public string? Status { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.ServiceModel
{
    public class ServiceReqModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public decimal? Price { get; set; }
    }

    public class ServiceCreateReqModel
    {
        public string? Name { get; set; }

        public decimal? Price { get; set; }
    }
    public class ServiceUpdateReqModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public decimal? Price { get; set; }
    }



}

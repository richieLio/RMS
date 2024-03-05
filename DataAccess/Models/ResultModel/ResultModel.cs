using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ResultModel
{
    public class ResultModel
    {   
        public bool IsSuccess { get; set; }
        public int Code { get; set; }
        public object? Data { get; set; }
        public object? ResponseFailed { get; set; }
        public string? Message { get; set; }
    }
    public class PaginationModel<T>
    {
        public int Page { get; set; }
        public int TotalPage { get; set; }
        public int TotalRecords { get; set; }
        public List<T> ListData { get; set; }
    }

}

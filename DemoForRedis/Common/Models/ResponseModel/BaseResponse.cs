using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoForRedis.Common.ResponseModel
{
    public class BaseResponse
    {
        public int StatusCode { get; set; }
        public required string Message { get; set; }
        public object? Data { get; set; }
    }
}

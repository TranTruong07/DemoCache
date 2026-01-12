using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoForRedis.Common.ResponseModel
{
    public class Response<T> : BaseResponse
    {
        public new T? Data { get; set; }
    }
}

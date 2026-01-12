using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoForRedis
{
   
    public class MongoConfig
    {
        public required string ConnectionString { get; set; }
        public required string DatabaseName { get; set; }
    }
}

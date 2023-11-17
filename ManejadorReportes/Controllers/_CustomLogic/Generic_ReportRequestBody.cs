using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManejadorReportes.Controllers._CustomLogic
{
    public class Generic_ReportRequestBody
    {
        public string KeyReport { get; set; }
        public string KeyDbConector { get; set; }
        public Dictionary<string, object> SqlParams { get; set; }
        public Dictionary<string, object> CrystalParams { get; set; }
    }
}

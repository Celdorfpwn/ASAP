using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class AsapTask
    {
        public string JiraId { get; set; }

        public string CommitId { get; set; }

        public string FishEyeId { get; set; }

        public bool ReviewFinished { get; set; }
    }
}

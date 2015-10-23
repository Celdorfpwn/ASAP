using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JenkinsService
{
    public class Enums
    {
        /// <summary>
        /// Jenkins build status Enum
        /// </summary>
        public enum BuildState
        {
            None,
            Building,
            Failed,
            Success
        }
    }
}

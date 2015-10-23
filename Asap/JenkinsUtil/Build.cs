using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JenkinsService
{
    /// <summary>
    /// Jenkins build
    /// </summary>
    public class Build
    {
        /// <summary>
        /// Build number
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Iteration where the Issue resides
        /// </summary>
        public string Iteration { get; set; }

        /// <summary>
        /// Build timestamp
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// Build state
        /// </summary>
        public JenkinsService.Enums.BuildState State { get; set; }


        /// <summary>
        /// Commit number used for this build
        /// </summary>
        public string CommitNo { get; set; }


        /// <summary>
        /// Branch used for this build
        /// </summary>
        public string Branch { get; set; }
        
        
        //public List<ItemProxy> Items { get; set; }
    }
}

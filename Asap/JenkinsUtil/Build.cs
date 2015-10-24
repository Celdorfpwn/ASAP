using Newtonsoft.Json;
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
        public BuildState State { get; set; }


        /// <summary>
        /// Commit number used for this build
        /// </summary>
        public string CommitNo { get; set; }


        /// <summary>
        /// Branch used for this build
        /// </summary>
        public string Branch { get; set; }


        //public List<ItemProxy> Items { get; set; }

        #region Constructor

        /// <summary>
        /// The constructor
        /// </summary>
        public Build()
        {
            State = BuildState.None;
        }

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="jsonData"></param>
        public Build(string jsonData)
        {
            JkBuild jkBuild = JsonConvert.DeserializeObject<JkBuild>(jsonData);

            this.Number = jkBuild.number;
            this.Date = Utils.ConvertTimestampToDateTime(jkBuild.timestamp);

            State = Utils.StringToBuildResultConverter(jkBuild.result);
        }

        #endregion

    }
}

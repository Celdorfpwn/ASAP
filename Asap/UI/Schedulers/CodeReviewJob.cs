using BL;
using ModelsDI;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiPikant.UI.Schedulers
{ 
    public class CodeReviewJob : IJob
    {
        private CodeReviewModel _codeReviewModel { get; set; }

        public CodeReviewJob()
        {
            _codeReviewModel = ModelsDependencyInjection.Resolve<CodeReviewModel>();
        }

        public void Execute(IJobExecutionContext context)
        {
            _codeReviewModel.Run();
        }
    }
}

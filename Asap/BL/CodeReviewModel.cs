using CodeReview;
using Entities;
using IssuesTracking;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class CodeReviewModel
    {
        private IIssuesTracking _issuesTracking { get; set; }

        private ICodeReview _codeReview { get; set; }

        private IRepositoryFactory _repositoryFactory { get; set; }

        private const int MAXREVIEWS = 2;

        public CodeReviewModel(IIssuesTracking issuesTracking, ICodeReview codeReview, IRepositoryFactory repositoryFactory)
        {
            _issuesTracking = issuesTracking;
            _codeReview = codeReview;
            _repositoryFactory = repositoryFactory;
        }

        public void Run()
        {
            var issues = _issuesTracking.GetIssues().Issues;
            IEnumerable<AsapTask> asapTasks = GetAsapTasks();

            foreach (var asapTask in asapTasks)
            {
                if (String.IsNullOrEmpty(asapTask.FishEyeId))
                {
                    Task.Run(() => TryCreateReview(asapTask, issues.FirstOrDefault(issue => issue.Key == asapTask.JiraId)));
                }
                else if(!asapTask.ReviewFinished)
                {
                    Task.Run(() => UpdateIfReviewComplete(asapTask));
                }
            }

        }

        private void UpdateIfReviewComplete(AsapTask asapTask)
        {
            try
            {
                if (_codeReview.CountReviewers(asapTask.FishEyeId) >= MAXREVIEWS)
                {
                    asapTask.ReviewFinished = true;
                    UpdateTask(asapTask);
                }
            }
            catch(Exception e)
            {
                
            }
        }

        private void TryCreateReview(AsapTask asapTask, Issue issue)
        {
            try
            {
                var review = _codeReview.CreateReview(asapTask.CommitId, issue.Key + " " + issue.Field.Summary, issue.URL, issue.Field.Summary, issue.Key);
                if (review != null)
                {
                    asapTask.FishEyeId = review.PermaId.Id;

                    UpdateTask(asapTask);
                }
            }
            catch (Exception e)
            {

            }
        }

        private void UpdateTask(AsapTask asapTask)
        {
            using (var repository = _repositoryFactory.NewRepository)
            {
                repository.Edit<AsapTask>(asapTask);
                repository.Save();
            }
        }

        private IEnumerable<AsapTask> GetAsapTasks()
        {
            using (var repository = _repositoryFactory.NewRepository)
            {
                return repository.All<AsapTask>().ToList();
            }
        }
    }
}

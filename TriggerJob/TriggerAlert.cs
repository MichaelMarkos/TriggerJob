using Microsoft.EntityFrameworkCore;
using Quartz;
using TriggerJob.Models;

namespace TriggerJob
{
    public class TriggerAlert : IJob
    {
        //private readonly IUnitOfWork _unitOfWork;
        //public TriggerAlert(IUnitOfWork unitOfWork)
        //{
        //    _unitOfWork = unitOfWork;
        //}

        private readonly ApplicationDbContext _context;

        public TriggerAlert(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;

            var checkQuiesList = await _context.Quiz.Where(q => q.EndDate > now && q.EndDate <= now.AddHours(24)).ToListAsync();
            if (checkQuiesList.Count() > 0) // check quiz is soon for student or teatcher (to send notifications)
            {
                foreach (var item in checkQuiesList)
                {
                    _context.Notification.Add(new Notification { body = "Hurry Up your quiz" + item.Name + " will expire zone", CreationDate = DateTime.Now });
                    _context.SaveChanges();
                }
            }

            // return Task.CompletedTask;
        }
    }
}

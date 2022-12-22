using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnrealReplayServer.Data;
using UnrealReplayServer.Databases.Models;

namespace UnrealReplayServer.CronJobs
{
    public class ReplayCleanupJob : IJob
    {
        private readonly UnrealReplayServerContext _context;

        public ReplayCleanupJob(UnrealReplayServerContext context)
        {
            _context = context;
        }

        Task IJob.Execute(IJobExecutionContext context)
        {
            System.Diagnostics.Debug.WriteLine("Running replay cleanup");
            var values = _context.Session.ToList();
            var oldItems = new List<Session>();
            foreach (var entry in values)
            {
                DateTimeOffset cutOff = DateTimeOffset.UtcNow.AddDays(-30);
                if (DateTimeOffset.Compare(entry.CreationDate, cutOff) < 0)
                    oldItems.Add(entry);
            }

            _context.Session.RemoveRange(oldItems);
            _context.SaveChanges();
            return Task.CompletedTask;
        }
    }
}

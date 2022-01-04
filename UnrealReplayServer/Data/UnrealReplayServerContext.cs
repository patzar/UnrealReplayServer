using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UnrealReplayServer.Databases.Models;

namespace UnrealReplayServer.Data
{
    public class UnrealReplayServerContext : DbContext
    {
        public UnrealReplayServerContext (DbContextOptions<UnrealReplayServerContext> options)
            : base(options)
        {
        }

        public DbSet<UnrealReplayServer.Databases.Models.SessionFile> SessionFile { get; set; }

        public DbSet<UnrealReplayServer.Databases.Models.Session> Session { get; set; }
    }
}

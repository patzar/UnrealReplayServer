/*
The MIT License (MIT)
Copyright (c) 2021 Henning Thoele
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnrealReplayServer.Data;
using UnrealReplayServer.Databases.Models;

namespace UnrealReplayServer.Databases
{
    public class SessionDatabase : ISessionDatabase
    {
        private readonly UnrealReplayServerContext _context;

        public SessionDatabase(UnrealReplayServerContext context)
        {
            _context = context;
        }

        public async Task<string> CreateSession(string setSessionName, string setAppVersion, string setNetVersion, int? setChangelist,
            string setPlatformFriendlyName)
        {
            Session newSession = new Session()
            {
                AppVersion = setAppVersion,
                NetVersion = setNetVersion,
                PlatformFriendlyName = setPlatformFriendlyName,
                Changelist = setChangelist != null ? setChangelist.Value : 0,
                SessionName = setSessionName,
                IsLive = true
            };
            await _context.Session.AddAsync(newSession);
            await _context.SaveChangesAsync();
            return newSession.SessionName;
        }

        public async Task<Session> GetSessionByName(string sessionName)
        {
            var session = await _context.Session.FindAsync(sessionName);
            return session;
        }

        public async Task<SessionFile> GetSessionHeader(string sessionName)
        {
            var session = await _context.Session.FindAsync(sessionName);
            if (session == null)
            {
                return null;
            }
            return session.HeaderFile;
        }

        public async Task<SessionFile> GetSessionChunk(string sessionName, int chunkIndex)
        {
            var session = await _context.Session.FindAsync(sessionName);
            if (session == null)
            {
                return null;
            }
            if (chunkIndex >= 0 && chunkIndex < session.SessionFiles.Count)
            {
                return session.SessionFiles.Where(s => s.ChunkIndex == chunkIndex).First();
            }
            return null;
        }

        public async Task<bool> SetUsers(string sessionName, string[] users)
        {
            return true;
        }

        public async Task<bool> SetHeader(string sessionName, SessionFile sessionFile, int streamChunkIndex, int totalDemoTimeMs)
        {
            var session = await _context.Session.FindAsync(sessionName);
            if (session == null)
            {
                LogError($"Session {sessionName} not found");
                return false;
            }
 
            session.HeaderFile = sessionFile;
            session.TotalDemoTimeMs = totalDemoTimeMs;
            _context.Session.Update(session);
            await _context.SaveChangesAsync();

            Log($"[HEADER] Stats for {sessionName}: TotalDemoTimeMs={session.TotalDemoTimeMs}");

            return true;
        }

        public async Task<bool> AddChunk(string sessionName, SessionFile sessionFile, int totalDemoTimeMs, int totalChunks, int totalBytes)
        {
            var session = await _context.Session.FindAsync(sessionName);
            if (session == null)
            {
                LogError($"Session {sessionName} not found");
                return false;
            }

            session.TotalDemoTimeMs = totalDemoTimeMs;
            session.TotalChunks = totalChunks;
            session.TotalUploadedBytes = totalBytes;
            session.SessionFiles.Add(sessionFile);
            _context.Session.Update(session);
            await _context.SaveChangesAsync();

            Log($"[CHUNK] Stats for {sessionName}: TotalDemoTimeMs={session.TotalDemoTimeMs}, TotalChunks={session.TotalChunks}, " +
                $"TotalUploadedBytes={session.TotalUploadedBytes}");

            return true;
        }

        public async Task StopSession(string sessionName, int totalDemoTimeMs, int totalChunks, int totalBytes)
        {
            var session = await _context.Session.FindAsync(sessionName);
            if (session == null)
            {
                LogError($"Session {sessionName} not found");
                return;
            }
            session.IsLive = false;
            session.TotalDemoTimeMs = totalDemoTimeMs;
            session.TotalChunks = totalChunks;
            session.TotalUploadedBytes = totalBytes;
            _context.Session.Update(session);
            await _context.SaveChangesAsync();

            Log($"[END] Stats for {sessionName}: TotalDemoTimeMs={session.TotalDemoTimeMs}, TotalChunks={session.TotalChunks}, " +
                $"TotalUploadedBytes={session.TotalUploadedBytes}");
        }

        public async Task<Session[]> FindReplaysByGroup(string group, IEventDatabase eventDatabase)
        {
            return await Task.Run(async () =>
            {
                var sessionNames = await eventDatabase.FindSessionNamesByGroup(group);
                if (sessionNames == null ||
                    sessionNames.Length == 0)
                {
                    return Array.Empty<Session>();
                }

                List<Session> sessions = new List<Session>();

                for (int i = 0; i < sessionNames.Length; i++)
                {
                    var session = await _context.Session.FindAsync(sessionNames[i]);
                    if (session == null)
                    {
                        continue;
                    }
                    sessions.Add(session);
                }

                return sessions.ToArray();
            });
        }

        public async Task<Session[]> FindReplays(string app, int? cl, string version, string meta, string user, bool? recent)
        {
            _context.Database.EnsureCreated();

            return await Task.Run(() =>
            {
                List<Session> sessions = new List<Session>();
                var values = _context.Session.ToList();
                foreach (var entry in values)
                {
                    bool shouldAdd = true;
                    if (app != null)
                    {
                        shouldAdd &= entry.AppVersion == app;
                    }
                    if (cl != null)
                    {
                        shouldAdd &= entry.Changelist == cl;
                    }
                    if (version != null)
                    {
                        shouldAdd &= entry.NetVersion == version;
                    }
                    // Disable search by users for now.
                /*    if (user != null)
                    {
                        shouldAdd &= entry.Users.Contains(user);
                    } */

                    if (shouldAdd)
                    {
                        sessions.Add(entry);
                    }
                }

                return sessions.ToArray();
            });
        }

        public async Task CheckViewerInactivity()
        {
            await Task.Run(() =>
            {
                var sessions = _context.Session.ToList();
                for (int i = 0; i < sessions.Count; i++)
                {
                    sessions[i].CheckViewersTimeout();
                }
            });
        }

        private void Log(string line)
        {
            // Empty
        }

        private void LogError(string line)
        {
            // Empty
        }
    }
}

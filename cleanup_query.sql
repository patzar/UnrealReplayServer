ALTER TABLE [dbo].[Session] DROP CONSTRAINT [FK_Session_SessionFile_HeaderFileSessionFileID];
ALTER TABLE [dbo].[SessionFile] DROP CONSTRAINT [FK_SessionFile_Session_SessionName];


TRUNCATE TABLE [dbo].[Session];
TRUNCATE TABLE [dbo].[SessionFile];

ALTER TABLE [dbo].[Session] WITH NOCHECK
    ADD CONSTRAINT [FK_Session_SessionFile_HeaderFileSessionFileID] FOREIGN KEY ([HeaderFileSessionFileID]) REFERENCES [dbo].[SessionFile] ([SessionFileID]);

ALTER TABLE [dbo].[SessionFile]
    ADD CONSTRAINT [FK_SessionFile_Session_SessionName] FOREIGN KEY ([SessionName]) REFERENCES [dbo].[Session] ([SessionName]) ON DELETE CASCADE;
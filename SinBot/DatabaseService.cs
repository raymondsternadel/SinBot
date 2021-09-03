using SinBot.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinBot
{
    public static class DatabaseService
    {
        static SQLiteAsyncConnection db;
        static async Task Init()
        {
            if (db != null)
                return;

            db = new SQLiteAsyncConnection("E:\\SinBot\\SinBot\\SinbotDB.db");
        }

        #region INSERT

        public static async Task AddViewer(Viewer viewer)
        {
            await Init();

            await db.InsertAsync(viewer);
        }

        #endregion

        #region UPDATE

        #endregion

        #region GET

        #region Commands

        public static async Task<List<Command>> GetAllCommands()
        {
            await Init();

            return await db.Table<Command>().ToListAsync();
        }

        public static async Task<Command> GetRandomCommand()
        {
            await Init();

            var count = await db.Table<Command>().CountAsync();

            var rnd = ThreadSafeRandom.ThisThreadsRandom.Next(0, count);

            return await db.Table<Command>().ElementAtAsync(rnd);
        }

        public static async Task<Command> GetCommandByTriggerPhrase(string triggerPhrase)
        {
            await Init();

            return await db.Table<Command>().Where((x) => x.TriggerPhrase == triggerPhrase).FirstOrDefaultAsync();
        }

        public static async Task<Command> GetCommandByTriggerKey(string triggerKey)
        {
            await Init();

            return await db.Table<Command>().Where((x) => x.TriggerKey == triggerKey).FirstOrDefaultAsync();
        }

        #endregion

        #region Command Messages

        public static async Task<List<CommandMessage>> GetCommandMessagesByCommandID(int commandID)
        {
            await Init();

            return await db.Table<CommandMessage>().Where((x) => x.CommandID == commandID).ToListAsync();
        }

        #endregion

        #region Informational Messages

        public static async Task<InformationalMessage> GetRandomInformationalMessage()
        {
            await Init();

            var count = await db.Table<InformationalMessage>().CountAsync();

            var rnd = ThreadSafeRandom.ThisThreadsRandom.Next(0, count);

            return await db.Table<InformationalMessage>().ElementAtAsync(rnd);
        }

        #endregion

        #region BannedWords

        public static async Task<List<BannedWord>> GetAllBannedWords()
        {
            await Init();

            return await db.Table<BannedWord>().ToListAsync();
        }

        #endregion

        #endregion
    }
}

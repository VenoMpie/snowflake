﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using Snowflake.Platform;
using Snowflake.Game;
using Snowflake.MediaStore;

namespace Snowflake.Database
{
    public class GameDatabase
    {
        public string FileName{get; private set;}
        private SQLiteConnection DBConnection {get; set;}
        public GameDatabase(string fileName){
            this.FileName = fileName;

            if (!File.Exists(this.FileName))
            {
                this.CreateDatabase();
            }
            this.DBConnection = new SQLiteConnection("Data Source=" + this.FileName + ";Version=3;");
        }

        private void CreateDatabase()
        {
            SQLiteConnection.CreateFile(this.FileName);
            var dbConnection = new SQLiteConnection("Data Source="+this.FileName+";Version=3;");
            dbConnection.Open();
            var sqlCommand = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS games(
                                                                platform_id TEXT,
                                                                uuid TEXT,
                                                                filename TEXT,
                                                                name TEXT,
                                                                mediastorekey TEXT,
                                                                metadata TEXT,
                                                                settings TEXT
                                                                )", dbConnection);
            sqlCommand.ExecuteNonQuery();
            dbConnection.Close();
        }

        public void AddGame(GameInfo game){
            this.DBConnection.Open();
            using (var sqlCommand = new SQLiteCommand(@"INSERT INTO games VALUES(
                                          @platform_id,
                                          @uuid,
                                          @filename,
                                          @name,
                                          @mediastorekey,
                                          @metadata,
                                          @settings)", this.DBConnection))
            {
                sqlCommand.Parameters.AddWithValue("@platform_id", game.PlatformId);
                sqlCommand.Parameters.AddWithValue("@uuid", game.UUID);
                sqlCommand.Parameters.AddWithValue("@filename", game.FileName);
                sqlCommand.Parameters.AddWithValue("@name",  game.Name);
                sqlCommand.Parameters.AddWithValue("@mediastorekey", game.MediaStore.MediaStoreKey);
                sqlCommand.Parameters.AddWithValue("@metadata",  JsonConvert.SerializeObject(game.Metadata));
                sqlCommand.Parameters.AddWithValue("@settings", JsonConvert.SerializeObject(game.Settings));
                sqlCommand.ExecuteNonQuery();
            }
            this.DBConnection.Close();
        }

        public GameInfo GetGameByUUID(string uuid)
        {
            return GetGamesByColumn("uuid", uuid)[0];
        }

        public IList<GameInfo> GetGamesByPlatform(string platformId)
        {
            return GetGamesByColumn("platform_id", platformId);
        }
        public IList<GameInfo> GetGamesByName(string nameSearch)
        {
            return GetGamesByColumn("name", nameSearch);
        }
        private IList<GameInfo>GetGamesByColumn(string colName, string searchQuery){
            this.DBConnection.Open();
            using (var sqlCommand = new SQLiteCommand(@"SELECT * FROM `games` WHERE `%colName` == @searchQuery"
                , this.DBConnection))
            {
                sqlCommand.CommandText = sqlCommand.CommandText.Replace("%colName", colName); //Easier to read than string replacement.
                sqlCommand.Parameters.AddWithValue("@searchQuery", searchQuery);
                using (var reader = sqlCommand.ExecuteReader())
                {
                    var result = new DataTable();
                    result.Load(reader);
                    var gamesResults = (from DataRow row in result.Rows select GetGameFromDataRow(row)).ToList();
                    this.DBConnection.Close();
                    return gamesResults;
                }
            }
        }
        public IList<GameInfo> GetAllGames()
        {
            this.DBConnection.Open();
            using (var sqlCommand = new SQLiteCommand(@"SELECT * FROM `games`"
                , this.DBConnection))
            {
                using (var reader = sqlCommand.ExecuteReader())
                {
                    var result = new DataTable();
                    result.Load(reader);
                    var gamesResults = (from DataRow row in result.Rows select GetGameFromDataRow(row)).ToList();
                    this.DBConnection.Close();
                    return gamesResults;
                }
            }
        }
        private GameInfo GetGameFromDataRow(DataRow row)
        {
            var platformId = (string)row["platform_id"];
            var uuid = (string)row["uuid"];
            var fileName = (string)row["filename"];
            var name = (string)row["name"];
            var mediaStore = new FileMediaStore((string)row["mediastorekey"]);
            var metadata = JsonConvert.DeserializeObject<IDictionary<string, string>>((string)row["metadata"]);
            var settings = JsonConvert.DeserializeObject<IDictionary<string, dynamic>>((string)row["settings"]);

            return new GameInfo(platformId, name, mediaStore, metadata, uuid, fileName, settings);
        }

    }
}

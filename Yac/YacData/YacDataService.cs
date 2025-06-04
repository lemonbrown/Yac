using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using YacData.Models;


namespace YacData
{

    public class YacDataService
    {
        private readonly LiteDatabase db;

        public YacDataService(string dbPath = "yac.db")
        {
            db = new LiteDatabase(dbPath);
        }

        public void InsertPassingStat(GamePlayerPassingStats stat)
        {
            var col = db.GetCollection<GamePlayerPassingStats>("passing_stats");
            col.Insert(stat);
        }

        public void InsertTeam(Team team, bool overwrite = false)
        {
            var col = db.GetCollection<Team>("teams");

            var existingTeam = col.FindOne(n =>
                n.Name == team.Name &&
                n.ActiveStartDateTime == team.ActiveStartDateTime &&
                n.ActiveEndDateTime == team.ActiveEndDateTime);

            if (existingTeam != null && !overwrite)
                return;

            col.Insert(team);
        }      

        public void Dispose()
        {
            db.Dispose();
        }
    }

}

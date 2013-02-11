using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using PlugInWeb.PlugIns.DataApi.Model;

namespace PlugInWeb.PlugIns.DataApi.Controllers
{
    public class ElectionController : ApiController
    {
        public Election[] Get()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ElectionDb>());
            var db = new ElectionDb("Data Source=Data.sdf");

            var elections = db.Elections
                .Include(e => e.Votes.Select(v => v.Votes))
                .ToList();
            elections.ForEach(e => e.Votes.Sort(new TimestampComparer()));
            return elections.ToArray();
        }

        public void Post(int id, VoteSet votes)
        {
            var db = new ElectionDb("");
            var election = db.Elections.Include(e => e.Votes.Select(v => v.Votes)).Single(e => e.Id == id);
            votes.Id = 0;
            votes.Timestamp = DateTime.Now;
            election.Votes.Add(votes);
            db.SaveChanges();
        }
    }

    public class TimestampComparer : IComparer<VoteSet>
    {
        public int Compare(VoteSet x, VoteSet y)
        {
            return y.Timestamp.CompareTo(x.Timestamp);
        }
    }
}
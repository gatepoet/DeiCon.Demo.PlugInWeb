using System.Collections.Generic;
using System.Linq;

namespace PlugInWeb.PlugIns.DataApi.Model
{
    public class Election
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<VoteSet> Votes { get; set; }

        public VoteSet GetLastVote()
        {
            return Votes.OrderByDescending(s => s.Timestamp).FirstOrDefault();
        }
    }
}
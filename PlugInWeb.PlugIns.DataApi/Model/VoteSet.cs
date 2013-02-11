using System;
using System.Collections.Generic;

namespace PlugInWeb.PlugIns.DataApi.Model
{
    public class VoteSet
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public List<Vote> Votes { get; set; }
    }
}
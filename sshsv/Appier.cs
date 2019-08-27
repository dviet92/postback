using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appier
{
    public static class Appier
    {
        public static AppierCampaign appierCampaign = null;
    }
    public class AppierCampaign
    {
        public Paging paging { get; set; }
        public List<Datum> data { get; set; }
    }
    public class BlockPlacements
    {
        public List<string> blocked_placements { get; set; }
    }

    public class Cursors
    {
    }

    public class Paging
    {
        public Cursors cursors { get; set; }
    }

    public class Creative
    {
        public string type { get; set; }
        public string id { get; set; }
        public string media_url { get; set; }
    }

    public class Datum
    {
        public List<string> blocked_placements { get; set; }
        public string click_url { get; set; }
        public double daily_budget { get; set; }
        public double traffic_name_transparency { get; set; }
        public string store_url { get; set; }
        public List<object> allowed_placements { get; set; }
        public double bid { get; set; }
        public string kpi_goal_description { get; set; }
        public bool is_with_device_id { get; set; }
        public List<string> devices { get; set; }
        public List<Creative> creatives { get; set; }
        public List<string> countries { get; set; }
        public bool is_incentive { get; set; }
        public string min_os { get; set; }
        public int total_budget { get; set; }
        public List<object> cities { get; set; }
        public string os { get; set; }
        public string id { get; set; }
        public string name { get; set; }
    }


}

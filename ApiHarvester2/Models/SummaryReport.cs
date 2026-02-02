using System.Collections.Generic;

namespace ApiHarvester.Models
{
    public class SummaryReport
    {
        // Existing single-API fields (keep if you still use JSONPlaceholder flow)
        public List<UserPosts> topUsers { get; set; }
        public List<PostComments> mostCommentedPosts { get; set; }
        public TodoStats todos { get; set; }

        // New multi-API fields
        public SpacexSummary SpaceX { get; set; }
        public CoinGeckoSummary CoinGecko { get; set; }
        public OpenMeteoSummary OpenMeteo { get; set; }
    }

    public class UserPosts { public string name { get; set; } public int postCount { get; set; } }
    public class PostComments { public string title { get; set; } public int commentCount { get; set; } }
    public class TodoStats { public int completed { get; set; } public int pending { get; set; } }
}




//using System.Collections.Generic;

//namespace ApiHarvester.Models
//{
//    public class SummaryReport
//    { // Existing single-API fields (keep to avoid breaking older dashboard) public List<UserPosts> topUsers { get; set; } public List<PostComments> mostCommentedPosts { get; set; } public TodoStats todos { get; set; }

//        // New multi-API fields
//        public SpacexSummary SpaceX { get; set; }
//        public CoinGeckoSummary CoinGecko { get; set; }
//        public OpenMeteoSummary OpenMeteo { get; set; }
//    }

//    public class UserPosts { public string name { get; set; } public int postCount { get; set; } }
//    public class PostComments { public string title { get; set; } public int commentCount { get; set; } }
//    public class TodoStats { public int completed { get; set; } public int pending { get; set; } }
//}
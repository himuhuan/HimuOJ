using Himu.EntityFramework.Core.Entity.Components;
using Microsoft.AspNetCore.Identity;

namespace Himu.EntityFramework.Core.Entity
{
    /// <summary>
    /// The Entity of Himu's User
    /// </summary>
    public class HimuHomeUser : IdentityUser<long>
    {
        public string Avatar { get; set; } = null!;

        public string Background { get; set; } = null!;

        //> Migration@2024-03-14 Add_HimuHomeUser_RegisterLoginDate
        //> Add RegisterDate to HzimuHomeUser
        public DateOnly RegisterDate { get; set;}
        
        //> Migration@2024-03-14 Add_HimuHomeUser_RegisterLoginDate:
        //> Add LastLoginDate to HimuHomeUser
        public DateOnly LastLoginDate { get; set;}

        public virtual ICollection<HimuArticle>? Articles { get; set; }

        public virtual ICollection<HimuContest>? Contests { get; set; }

        public virtual ICollection<HimuProblem>? Problems { get; set; }

        public virtual ICollection<HimuCommit>? MyCommits { get; set; }

        /// <summary>
        /// Not include the contests that the user created.
        /// refer to <see cref="ContestCreator"/>
        /// </summary>
        public virtual ICollection<HimuContest>? AccessibleContests { get; set; }
    }
}
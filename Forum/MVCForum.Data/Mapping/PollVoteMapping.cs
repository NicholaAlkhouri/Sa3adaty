using System.Data.Entity.ModelConfiguration;
using MVCForum.Domain.DomainModel;

namespace MVCForum.Data.Mapping
{
    public class PollVoteMapping : EntityTypeConfiguration<PollVote>
    {
        public PollVoteMapping()
        {
            HasKey(x => x.Id);
            HasRequired(x => x.User).WithMany(t => t.PollVotes).Map(m => m.MapKey("UserId"));
            HasRequired(x => x.PollAnswer).WithMany(t => t.PollVotes).Map(m => m.MapKey("PollAnswer_Id"));
        }
    }
}

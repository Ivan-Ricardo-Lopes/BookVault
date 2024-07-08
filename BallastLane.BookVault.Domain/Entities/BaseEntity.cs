using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallastLane.BookVault.Domain.Entities
{
    public abstract class BaseEntity
    {
        public string CreatedBy { get; set; } = default!;
        public DateTimeOffset CreatedDateUtc { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTimeOffset? ModifiedDateUtc { get; set; }
        public string? DeletedBy { get; set; } 
        public DateTimeOffset? DeletedDateUtc { get; set; }
    }
}

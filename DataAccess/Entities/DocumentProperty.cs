using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class DocumentProperty
    {
        public int Id { get;     set; }
        public string? PropertyName { get;   set; }
        public string? PropertyValue { get; set; }

        public int DocumentId { get; set; }
        public virtual Document? Document { get; set; }
    }   
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Aggregation_DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbConfiguration
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbConfiguration()
        {
            this.tbActivityLoggers = new HashSet<tbActivityLogger>();
        }
    
        public int id { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public bool readOnly { get; set; }
        public bool immediate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbActivityLogger> tbActivityLoggers { get; set; }
    }
}

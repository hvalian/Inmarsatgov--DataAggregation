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
    
    public partial class tbLogError
    {
        public long id { get; set; }
        public long jobId { get; set; }
        public string methodName { get; set; }
        public string exceptionMessage { get; set; }
        public string innerException { get; set; }
        public Nullable<int> returnCode { get; set; }
        public long elapsedTime { get; set; }
        public Nullable<System.DateTime> timestamp { get; set; }
        public string stackTrace { get; set; }
        public string parameters { get; set; }
    
        public virtual tbJob tbJob { get; set; }
    }
}

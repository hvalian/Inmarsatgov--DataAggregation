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
    
    public partial class tbConfigurationByDate
    {
        public int id { get; set; }
        public System.DateTime aggregationDate { get; set; }
        public bool Refresh_Enabled { get; set; }
        public int Refresh_NumberOfDays { get; set; }
        public int Refresh_Interval { get; set; }
        public int Job_StartTimeDelay { get; set; }
        public System.DateTime createdDateTime { get; set; }
    }
}

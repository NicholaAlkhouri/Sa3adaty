//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sa3adaty.DAL.EntityModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Subscription
    {
        public int SubscriptionId { get; set; }
        public Nullable<int> UserId { get; set; }
        public int CampaignId { get; set; }
        public Nullable<System.DateTime> LastSend { get; set; }
        public bool Active { get; set; }
        public string Email { get; set; }
        public string UnsubscripeToken { get; set; }
    
        public virtual EmailCampagin EmailCampagin { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}

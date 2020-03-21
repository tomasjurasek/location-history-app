using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserIdentifier { get; set; }
        public string Token { get; set; }
        public string Phone { get; set; }
        public string VerifyCode { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }

    public enum Status
    {
        Undefined = 0,
        InProgress = 1,
        Done = 2,
        Failed = 3 
    }
}

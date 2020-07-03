using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ApiDemo.Contracts
{
    [DataContract]
    public class UserResponse
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember] 
        public string Name { get; set; }
        [DataMember] 
        public string PhoneNumber { get; set; }
        [DataMember] 
        public string EmailAddress { get; set; }
    }
}

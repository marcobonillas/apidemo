using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ApiDemo.Contracts
{
    [DataContract]
    public class UserRequest
    {
        [DataMember]
        [Required]
        public string FirstName { get; set; }
        [DataMember]
        public string MiddleName { get; set; }
        [DataMember]
        [Required]
        public string LastName { get; set; }
        [DataMember]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        [DataMember]
        public string EmailAddress { get; set; }
    }

}

using System.Runtime.Serialization;

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

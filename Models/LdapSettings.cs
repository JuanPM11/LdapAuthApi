namespace LdapAuthApi.Models
{
    public class LdapSettings
    {
        public string LdapServer { get; set; }        
        public int LdapPort { get; set; }            
        public string BaseDn { get; set; }           
        public string AdminDn { get; set; }          
        public string AdminPassword { get; set; }    
    }
}

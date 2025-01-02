using LdapAuthApi.Models;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using System;

public class LdapAuthenticationService
{
    private readonly LdapSettings _ldapSettings;

    public LdapAuthenticationService(IOptions<LdapSettings> ldapSettings)
    {
        _ldapSettings = ldapSettings.Value; 
    }

    public bool AuthenticateUser(string username, string password)
    {
        try
        {
            // Conexión al servidor LDAP
            using (var connection = new LdapConnection())
            {
                connection.Connect(_ldapSettings.LdapServer, _ldapSettings.LdapPort);
                connection.Bind(_ldapSettings.AdminDn, _ldapSettings.AdminPassword);

                // Ajusta el filtro de búsqueda según el atributo 
                var searchFilter = $"(&(cn={username}))"; 
                var searchResults = connection.Search(_ldapSettings.BaseDn, LdapConnection.ScopeSub, searchFilter, null, false);

                string userDn = null;

                while (searchResults.HasMore())
                {
                    var entry = searchResults.Next();
                    userDn = entry.Dn; 
                    break;
                }

                if (string.IsNullOrEmpty(userDn))
                {
                    Console.WriteLine("Usuario no encontrado en LDAP");
                    return false; 
                }

                connection.Bind(userDn, password); 

                if (connection.Bound)
                {
                    Console.WriteLine("Autenticación exitosa");
                    return true; 
                }
                else
                {
                    Console.WriteLine("Contraseña incorrecta");
                    return false; 
                }
            }
        }
        catch (LdapException ex)
        {
            Console.WriteLine($"Error de LDAP: {ex.Message}");
            return false; 
        }
    }
}

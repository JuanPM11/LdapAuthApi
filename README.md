# LdapAuthApi
# Configuracion inicial

Para crear el servidor LDAP se debe definir el siguiente archivo con extension .YAML el cual crea el servidor a partir de un administrador en PHP, el archivo se debe definir de la siguiente forma

services:
  openldap:
    image: osixia/openldap:latest
    container_name: openldap
    hostname: openldap
    ports:
      - "389:389"
      - "636:636"
    volumes:
      - ./data/certificates:/container/service/ldapd/assets/certs
      - ./data/slapd/database:/var/lib/ldap
      - ./data/slapd/config:/etc/ldap/slapd.d
    environment:
      - LDAP_ORGANIZATION=ramhlocal
      - LDAP_DOMAIN=ramhlocal.com
      - LDAP_ADMIN_USERNAME=admin
      - LDAP_ADMIN_PASSWORD=admin_pass
      - LDAP_CONFIG_PASSWORD=config_pass
      - LDAP_BASE_DN=dc=ramhlocal,dc=com
      - LDAP_TLS_CRT_FILENAME=server.crt
      - LDAP_TLS_KEY_FILENAME=server.key
      - LDAP_TLS_CA_CRT_FILENAME=ramhlocal.com.ca.crt
      - LDAP_READONLY_USER=false
      - LDAP_READONLY_USER_USERNAME=user-ro
      - LDAP_READONLY_USER_PASSWORD=ro_pass
    networks:
      - openldap

  phpldapadmin:
    image: osixia/phpldapadmin:latest
    container_name: phpldapadmin
    hostname: phpldapadmin
    ports:
      - "80:80"
    environment:
      - PHPLDAPADMIN_LDAP_HOSTS=openldap
      - PHPLDAPADMIN_HTTPS=false
    depends_on:
      - openldap
    networks:
      - openldap

networks:
  openldap:
    driver: bridge


Debe instalarse Docker.Desktop y estar ejecutandose para realizar la siguiente operación.

Una vez realizados los pasos anteriores, se ejecuta el siguiente comando para crear el servidor  

docker-compose -f ruta del archivo .yaml -p nombre del servidor up

Para validar el inicio de sesion del administrador del LDAP, se abre el navegador se ingresa a localhost y se ingresan las siguientes credenciales

usuario: cn=admin,dc=ramhlocal,dc=com
contraseña: admin_pass

# Configuración API
Se valida el inicio de sesión y se procede a realizar la prueba de autenticación con la api.

En el archivo appsettings.json se debe realizar las siguientes configuraciones

LdapSettings: se define el nombre del servidor, puerto, base, usuario y contraseña del administrador
JwtSettings: Se definen las configuraciones de la audiencia, el tiempo de expiracion del token
ConnectionStrings: Se define la cadena de conexión de la base de datos

A continuacion un ejemplo del JSON de configuracion de la api

{
  "LdapSettings": {
    "LdapServer": "localhost",
    "LdapPort": 389,
    "BaseDn": "dc=ramhlocal,dc=com",
    "AdminDn": "cn=admin,dc=ramhlocal,dc=com",
    "AdminPassword": "admin_pass",
    "UseSsl": false
  },
  "JwtSettings": {
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "TokenExpirationInHours": 1
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=your_db_name;User Id=your_user;Password=your_password;"
  }
}

# URL API

La url de consumo del servicio es la siguiente

api/auth/authenticate

y el json de consumo es el siguiente 

{
    "username": "user-ro",
    "password": "ro_pass"
}

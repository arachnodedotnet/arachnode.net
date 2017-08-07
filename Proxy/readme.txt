

               Mentalis.org Proxy
               ~~~~~~~~~~~~~~~~~~        (Tuesday, April 30, 2002)

  (1) How to build the source code  
  
       The source distribution comes with a file 'make.bat'.
       If you run this file, it will compile all the classes
       into an executable (Proxy.exe) and it will output an
       XML file that can be used to generate the class
       documentation (like the Proxy.chm file).


  (2) How to use it
  
       Here's a list of the commands and a small description
       of each command.
  
      help
        Shows a list of all available commands and a very short
        description of each command.
  
      uptime
        Shows the uptime of the proxy server in the following
        format: [d.]hh:mm:ss[.ff]
        where d is the number of days, hh the number of hours,
        mm the number of minutes, ss the number of seconds
        and ff the number of fractional seconds.

      version
        Shows the version of the proxy server.
        
      listusers
        Lists all the users in the user listing. These usernames
        can be used for SOCKS5 authentication.
      
      adduser  
        Adds a user to the user list.
        
      deluser
        Deletes a user from the user list.
        
      listlisteners
        Prints a list of all the active listeners and their IDs.
        You'll need this ID when you want to remove a specific
        listener.
        
      addlistener
        Creates a new listener. See (a) for more information about
        this.

      dellistener
        Deletes a listener from the listeners collection.
        To delete a listener, you'll need its ID (a 32-byte string).
        You can find this ID by executing the 'listlisteners' command.


  (3) License & Contact information

       All the code in this source code distribution is copyrighted
       by The KPD-Team, © 2002. Visit http://www.mentalis.org/ for
       the latest version of this project. E-Mail us at
       info@mentalis.org if you have comments or suggestions for us.

       Make sure you read the license before you use the code. This
       directory should contain a file license.txt which contains
       a copy of the source code license. The same license is also
       included in each class file and in the class documentation.


  (4) Final Notes

       The user/password authentication needs more work. It's already
       implemented in the base classes, but the Proxy class doesn't
       really use it.


  (a) AddListener Command

       There are currently four types of listeners you can start:
       an HTTP proxy, an FTP proxy, a SOCKS proxy and a PortMapper.



       HTTP proxy
         An HTTP proxy will typically relay traffic between web servers
         and web browsers. The full class name of the HTTP proxy
         is "Org.Mentalis.Proxy.Http.HttpListener" (without the quotes).
         The construction string of this object is "host:<ip>;int:<port>"
         where <ip> should be replaced by the IP address you want the
         server to listen on for incoming connections and <port> should
         be replaced by the port number you want to listen on for
         connections. For instance, if you want an HTTP proxy server on
         IP address 10.0.0.1 port 100, this would be the command sequence

>addlistener
Please enter the full class name of the Listener object you're trying to add:
 (ie. Org.Mentalis.Proxy.Http.HttpListener)
Org.Mentalis.Proxy.Http.HttpListener
Please enter the construction parameters:
host:10.0.0.1;int:100
HTTP service on 10.0.0.1:100 started.

         If you have your HTTP proxy running, you're almost done. Now, all
         you have to do is to make sure your web browser uses that proxy
         server.
         Internet Explorer users can go to Tools->Internet Options->
         Connections Tab->LAN Settings
         Check the 'Use a proxy server for your LAN' checkbox and make
         sure the address and port text boxes are set to the correct IP
         address and port number.



       FTP proxy
         An FTP proxy will typically relay traffic between FTP servers
         and FTP clients. The full class name of the FTP proxy
         is "Org.Mentalis.Proxy.Ftp.FtpListener" (without the quotes).
         The construction string of this object is "host:<ip>;int:<port>"
         where <ip> should be replaced by the IP address you want the
         server to listen on for incoming connections and <port> should
         be replaced by the port number you want to listen on for
         connections. For instance, if you want an FTP proxy server on
         IP address 10.0.0.1 port 21, this would be the command sequence

>addlistener
Please enter the full class name of the Listener object you're trying to add:
 (ie. Org.Mentalis.Proxy.Http.HttpListener)
Org.Mentalis.Proxy.Ftp.FtpListener
Please enter the construction parameters:
host:10.0.0.1;int:21
FTP service on 10.0.0.1:21 started.

         Search for the PROXY options in your favorite FTP client. It should
         support something like "OPEN command", "USER user@host:port"
         or "USER user@host port". Pick one, fill in the correct values for
         host and port, and you're ready. (There may also be some
         authentication fields; leave these blank.)



       SOCKS proxy
         A SOCKS proxy can relay data from and to any application using any
         subprotocol. The full class name of the SOCKS proxy
         is "Org.Mentalis.Proxy.Socks.SocksListener" (without the quotes).
         The construction string of this object is "host:<ip>;int<port>"
         -or- "host:<ip>;int<port>;authlist". You can use that last version
         if you want the users of the SOCKS5 server to be authenticated.
         Here's an example of how to create a SOCKS server listening
         on 10.0.0.1 port 1080

>addlistener
Please enter the full class name of the Listener object you're trying to add:
 (ie. Org.Mentalis.Proxy.Http.HttpListener)
Org.Mentalis.Proxy.Socks.SocksListener
Please enter the construction parameters:
host:10.0.0.1;int:1080
SOCKS service on 10.0.0.1:1080 started.         
         
         Since the configuration of a SOCKS proxy in your applications differ
         from application to application, I'm afraid you'll have to figure it
         out yourself.
         Note that the Socks5Listener class does not fully implement the SOCKS5
         protocol. It does not support UPD sockets.



       PORTMAP
         Portmap does exactely what its name says: it maps a port on a specific
         IP address to another port on another IP address. This can be useful
         for email and news applications. The full class name of the PortMap
         listener is "Org.Mentalis.Proxy.PortMap.PortMapListener" (without
         the quotes). The construction string of this object is
         "host:<l_ip>;int:<l_port>;host:<m_ip>;int:<m_port>"
         Where l_ip is the IP address to listen on, l_port is the port to listen
         on, m_ip is the IP address where all the traffic has to be mapped to
         and m_port is the port number where all the traffic has to be mapped to.

         For instance, let's say we want to map the port 119 on 10.0.0.1
         to the port 119 on msnews.microsoft.com. You'd have to do something
         like this:

>addlistener
Please enter the full class name of the Listener object you're trying to add:
 (ie. Org.Mentalis.Proxy.Http.HttpListener)
Org.Mentalis.Proxy.PortMap.PortMapListener
Please enter the construction parameters:
host:10.0.0.1;int:119;host:msnews.microsoft.com;int:119
PORTMAP service on 10.0.0.1:119 started.

         If you now connect to the IP address 10.0.0.1 with your favoutite news
         reader, you'll end up on msnews.microsoft.com.




// The idea behind this was a hacky way to make the PAC file communicate with the Proxy Switcher.
// In theory the switcher would write 6 entries in the hosts file: 
// "ProxyIP" and "ProxyPort#" where # goes from 0 to 4, the first one will contain the proxy's IP and the
// other 5 each will old a digit of the proxy port in their last segment, this is because you can't put
// arbitrary data in the hosts file.
// This PAC file would then make a DNS lookup on these entries, extract the values then tell the browser
// what proxy to use.

// I tested this idea first by manually editting the hosts file, but the problem was that it took a few
// seconds for the change to take effect due to DNS caching so I never continued on with that idea (it
// would've also forced me to run as admin, which sucks)
// ... but it's still quite awesome, so I left it here :D

// I was also thinking of trying to make ajax calls from this function, but I'm not sure which parts of
// the javascript library are available, and debugging this was tedious (they even disabled the alert
// function), so I dropped the whole thing and just decided to make an intermediate proxy (reverse proxy?)
// that does the switching.

// Here are example entries in the hosts file:
// 1.2.3.4 ProxyIP
// 1.0.0.0 ProxyPort0
// 1.0.0.8 ProxyPort1
// 1.0.0.0 ProxyPort2
// 1.0.0.8 ProxyPort3
// 1.0.0.0 ProxyPort4

function FindProxyForURL(url, host) {
    var ip = dnsResolve("ProxyIP");

    var port = "";
    for(var i = 0; i < 5; i++)
    	port += dnsResolve("ProxyPort" + i).substr(6); // eg: 1.0.0.2 - we want that "2"

    return "PROXY " + ip + ":" + port;
}
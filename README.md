# Proxy Dashboard #

## What is this? ##
This tool combines a bunch of features that makes finding a free proxy server that works and using it a simple and automated process.

First it scrapes a few "proxy list" websites for ip:host entries, then it checks if they actually work, then finally switching you to use that proxy. Then it asks you if you'd like to add the current proxy to an offline proxy list (a text file), or add it to an ignore list (so it gets skipped next time), or just switch to the next working proxy, or exit the program.

Currently this tool only gets US proxies, but I plan on extending it so you can specify the country. Also sometimes you'll get random non-US proxies, and I plan on adding one more validation phase that uses `ipinfo.io`'s JSON API.

## Motivation ##
I made this tool because I got tired of finding free proxies to access geo-blocked content. I believe that there are no country borders on the internet, so to me geo-blocking is a form of censorship.

*<sup>I also pay for Netflix the same price as every other person, so I expect to see the same content!</sup>*

## Architecture ##
The process of finding proxy lists happens through implementations of the `IProxyProvider` interface which just defines an `EnumerateIPs` method for now. I've made scrapers for some of the websites I found by googling "proxy list", and there is also a `TxtFile` provider which gets entries from a `proxy-list.txt` file. These providers run parallel.

After that is the process of validation, this happens through implementations of the `IProxyValidator` interface which defines an `IsValid(string)` method. There are two validators right now, the first checks if the proxy isn't in the `ignore-list.txt` file (the class also allows you to add entries to that list too), the second checks if the proxy actually works by doing a HEAD request to Google over SSL. These validation stages run in sequence and fail-fast, but multiple proxies get validated in parallel.

Finally there is the process of setting the proxy as the current proxy to be used by your browser, this happens through implementations of the `IProxySetter` interface. I first made one that changes Firefox's preferences (first attempt was prefs.js, second was user.js) but I couldn't find a way to force Firefox to reload it's preferences. Next attempt was one that creates a PAC file (Proxy Auto-Configuration file) but I also couldn't find an easy way to make Firefox reload the file, I found a nice add-on called [Reload PAC Button](https://addons.mozilla.org/en-US/firefox/addon/reload-pac-button/) but I wanted to make the switching fully automated, so decided to make an Intermediate Proxy. So basically this part is unused in the code, but somebody might find it useful so I left it.

The Intermediate Proxy is basically a proxy that forwards everything it gets to another proxy and back (you can think of it as a reverse-proxy for proxies :D). The idea is to set my browser's proxy to 127.0.0.1 and then when the program finds a validated proxy it tells the Intermediate Proxy to forward everything to that proxy, that way you only have to configure your browser once.

## Contribution ##
If you made any improvements to the code feel free to submit a pull request and I'll check it out, I'd be very happy if anybody is interested in this project :)

## License ##
This project is licensed under the Apache License, Version 2.0
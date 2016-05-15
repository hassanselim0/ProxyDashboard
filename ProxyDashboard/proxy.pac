// This is a default PAC file that does nothing
// It gets overwritten by ProxySetters.PacFile

function FindProxyForURL(url, host)
{
	return "DIRECT";
}
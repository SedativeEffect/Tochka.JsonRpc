# Overview

This is a set of packages to help make JSON RPC 2.0 APIs like you used to with ASP.Net Core (MVC/REST).
You only need one line in `Startup.cs`, and a different class for your controllers. 

## Key features

* Simple installation
* Zero configuration because of sane defaults. Clear options like customizable serialization and routing
* Server-side uses standard routing, binding and other pipeline built-ins without reinventing or breaking anything
* Tries to replicate ASP.Net Core experience: write controllers and actions like it's normal REST API
* Everything is extensible via DI so you can achieve any specific behavior
* Supports batches, notifications, array params and other JSON RPC 2.0 quirks while hiding them from user
* Supports returning non-json data if required, eg. to redirect browser or send binary file
* Client is intended to be helpful when diagnosing errors

## Limitations and things to consider

* Currently tested only with ASP.Net Core 2.2
* Does not support ASP.Net Core <= 2.1 (requires endpoint routing feature)
* Not tested with ASP.Net Core 3.x (but written with concern to be compatible with no or minimal changes)
* Supports only UTF-8 (because who does JSON serialization in different encodings?)

# Server

## Installation for ASP.Net Core 2.2

> nuget package
```
Tochka.JsonRpc.Server
```

Register it in Startup and set compatibility version

> Startup.cs
```cs
    public void ConfigureServices(IServiceCollection services)
        {
		    services.AddMvc()
                .AddJsonRpcServer()  // <-- add this
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);  // <-- this is required because 2.1 disables endpoint routing
        }
```

Write your controller as usual, but instead of inheriting from `Controller`, inherit `JsonRpcController`

> EchoController.cs
```cs
    public class EchoController : JsonRpcController
	{
	    public string ToLower(string value)
        {
            return value.ToLower();
        }
	}
```

Start your app and send POST
> HTTP request (part)
```HTTP

POST /api/jsonrpc HTTP/1.1
Content-Type: application/json

{
    "id": 1,
    "jsonrpc": "2.0",
    "method": "to_lower",
    "params": {
        "value": "TEST"
    }
}
```

> HTTP response (part)
```HTTP
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8

{
    "id": 1,
    "jsonrpc": "2.0",
    "result": "test"
}
```
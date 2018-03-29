ASP.NET Core Proxy
==================

This fork of ASP.NET Core Proxy enables dyanmic proxying of traffic based on path steps, where the first two steps represent the scheme and the host respectively.

For example the following request...
```
http://localhost/https/www.google.com/calendar
```
...proxies to...
```
https://www.google.com/calendar
```

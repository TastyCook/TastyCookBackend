# Overview
Infrastructure folder is a temporarily solution for
cloud integration until ArgoCD and AKS are in place.
For now, docker containers can be used to run backend part of the project.

# How to use scripts

> ⚠️ Please note that Docker Engine has to be installed: https://docs.docker.com/engine/install/

## Start containers

This script will create containers for all folders in the project:

```bash
cd infra
start-containers.sh
```

After loading finishes - you are good to go. Server's services will be available on http://localhost.
Ports are starting at 9000.
For example, you can test that containers are working with this curl command for recipes API:
```shell
curl http://localhost:9000/health
```

Output should look something like this:
```shell
StatusCode        : 200
StatusDescription : OK
Content           : Server is working
RawContent        : HTTP/1.1 200 OK
                    Transfer-Encoding: chunked
                    Content-Type: text/plain; charset=utf-8
                    Date: Fri, 14 Apr 2023 18:41:47 GMT
                    Server: Kestrel

                    Server is working
Forms             : {}
Headers           : {[Transfer-Encoding, chunked], [Content-Type, text/plain; charset=utf-8], [Date, Fri, 14 Apr 2023
                    18:41:47 GMT], [Server, Kestrel]}
Images            : {}
InputFields       : {}
Links             : {}
ParsedHtml        : mshtml.HTMLDocumentClass
RawContentLength  : 17
```

## Remove containers

Stop and remove them with `remove-containers.sh`:
```bash
remove-containers.sh
```

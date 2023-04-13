# Overview
Infrastructure folder is a temporarily solution for
cloud integration until ArgoCD and AKS are in place.
For now, docker containers can be used to run backend part of the project.

# How to use scripts

> ⚠️ Please note that Docker Engine has to be installed: https://docs.docker.com/engine/install/

This script will create containers for all folders in the project:

```bash
cd infra
start-containers.sh
```

After loading finishes - you are good to go. Server will be available on http://localhost:44383

Stop them with `stop-containers.sh`:
```bash
stop-containers.sh
```

# Tasty Cook## Overviewtodo: add description for the project## How to deploy### PrerequisitesYou need following tools to deploy Tasty Cook correctly:1. [Chocolatey](https://docs.chocolatey.org/en-us/)Chocolatey is a software management solution for Windows. Open your cmd with admin access and paste this command:```bash@"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -InputFormat None -ExecutionPolicy Bypass -Command "[System.Net.ServicePointManager]::SecurityProtocol = 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))" && SET "PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin"```2. [Docker](https://www.docker.com/)We use Docker to containerise our application and ship it to Kubernetes cluster. Install it with choco:```bashchoco install docker-desktop```This will install Docker desktop application and start docker daemon in the background which will operate our containers.3. [Kubernetes CLI](https://kubernetes.io/)Kubernetes CLI is needed to connect to remote AKS cluster.```bashchoco install kubernetes-cli```4. [Make](https://www.gnu.org/software/make/manual/make.html)We use Makefile to run tasks. ```bashchoco install make```### Start containersThis script will create containers for all folders in the project:```bashcd infrastart-containers.sh```After loading finishes - you are good to go. Server's services will be available on http://localhost.Ports are starting at 9000.For example, you can test that containers are working with this curl command for recipes API:```shellcurl http://localhost:9000/health```Output should look something like this:```shellStatusCode        : 200StatusDescription : OKContent           : Server is workingRawContent        : HTTP/1.1 200 OK                    Transfer-Encoding: chunked                    Content-Type: text/plain; charset=utf-8                    Date: Fri, 14 Apr 2023 18:41:47 GMT                    Server: Kestrel                    Server is workingForms             : {}Headers           : {[Transfer-Encoding, chunked], [Content-Type, text/plain; charset=utf-8], [Date, Fri, 14 Apr 2023                    18:41:47 GMT], [Server, Kestrel]}Images            : {}InputFields       : {}Links             : {}ParsedHtml        : mshtml.HTMLDocumentClassRawContentLength  : 17```### Remove containersStop and remove them with `remove-containers.sh`:```bashremove-containers.sh```
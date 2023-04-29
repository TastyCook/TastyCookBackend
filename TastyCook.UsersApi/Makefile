IMG ?= tastycook.azurecr.io/users-api
TAG ?= v0.1.0

.PHONY: docker-build
docker-build:
	cd .. && docker build -t ${IMG}:${TAG} -f TastyCook.UsersAPI/Dockerfile .

.PHONY: docker-push
docker-push:
	docker push ${IMG}:${TAG}
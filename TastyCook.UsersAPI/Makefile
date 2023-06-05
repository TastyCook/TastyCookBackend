IMG ?= tastycook.azurecr.io/users-api
TAG ?= 1686005986

.PHONY: docker-build
docker-build:
	cd .. && docker build -t ${IMG}:${TAG} -f TastyCook.UsersAPI/Dockerfile .

.PHONY: docker-push
docker-push:
	docker push ${IMG}:${TAG}

.PHONY: docker-start
docker-start:
	docker create -p "9002:80" --name "users-api-container" "${IMG}:${TAG}"
	docker start "users-api-container"

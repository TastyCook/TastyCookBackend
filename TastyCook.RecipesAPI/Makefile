IMG ?= tastycook.azurecr.io/recipes-api
TAG ?= v0.1.0

.PHONY: docker-build
docker-build:
	cd .. && docker build -t ${IMG}:${TAG} -f TastyCook.RecipesAPI/Dockerfile .

.PHONY: docker-push
docker-push:
	docker push ${IMG}:${TAG}
IMG ?= tastycook.azurecr.io/products-api
TAG ?= 74e8d0f

.PHONY: docker-build
docker-build:
	cd .. && docker build -t ${IMG}:${TAG} -f TastyCook.ProductsAPI/Dockerfile .

.PHONY: docker-push
docker-push:
	docker push ${IMG}:${TAG}

.PHONY: docker-start
docker-start:
	docker create -p "9003:80" --name "products-api-container" "${IMG}:${TAG}"
	docker start "products-api-container"

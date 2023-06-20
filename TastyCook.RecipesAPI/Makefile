IMG ?= tastycook.azurecr.io/recipes-api
TAG ?= cb364ff

.PHONY: docker-build
docker-build:
	cd .. && docker build -t ${IMG}:${TAG} -f TastyCook.RecipesAPI/Dockerfile .

.PHONY: docker-push
docker-push:
	docker push ${IMG}:${TAG}

.PHONY: docker-start
docker-start:
	docker create -p "9001:80" --name "recipes-api-container" "${IMG}:${TAG}"
	docker start "recipes-api-container"

.PHONY: test
test:
	if [ -d "../TastyCook.RecipesAPI.Tests" ]; then \
		dotnet test ../TastyCook.RecipesAPI.Tests; \
	else \
		echo "Test directory not found. Skipping tests."; \
	fi

IMG ?= tastycook.azurecr.io/users-api
TAG ?= 4755321

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

.PHONY: test
test:
	if [ -d "../TastyCook.UsersAPI.Tests" ]; then \
		dotnet test ../TastyCook.UsersAPI.Tests; \
	else \
		echo "Test directory not found. Skipping tests."; \
	fi

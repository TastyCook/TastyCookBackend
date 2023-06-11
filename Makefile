.PHONY: docker-build
docker-build:
	$(MAKE) -C TastyCook.RecipesAPI docker-build
	$(MAKE) -C TastyCook.UsersAPI docker-build
	$(MAKE) -C TastyCook.Products docker-build

.PHONY: docker-push
docker-push:
	$(MAKE) -C TastyCook.RecipesAPI docker-push
	$(MAKE) -C TastyCook.UsersAPI docker-push
	$(MAKE) -C TastyCook.Products docker-build

.PHONY: docker-start
docker-start:
	$(MAKE) -C TastyCook.RecipesAPI docker-start
	$(MAKE) -C TastyCook.UsersAPI docker-start
	$(MAKE) -C TastyCook.Products docker-build

.PHONY: test
test:
	$(MAKE) -C TastyCook.RecipesAPI test
	$(MAKE) -C TastyCook.UsersAPI test
	$(MAKE) -C TastyCook.ProductsAPI test

.PHONY: docker-build
docker-build:
	$(MAKE) -C TastyCook.RecipesAPI docker-build
	$(MAKE) -C TastyCook.UsersAPI docker-build
	$(MAKE) -C TastyCook.ProductsAPI docker-build

.PHONY: docker-push
docker-push:
	$(MAKE) -C TastyCook.RecipesAPI docker-push
	$(MAKE) -C TastyCook.UsersAPI docker-push
	$(MAKE) -C TastyCook.ProductsAPI docker-push

.PHONY: docker-start
docker-start:
	$(MAKE) -C TastyCook.RecipesAPI docker-start
	$(MAKE) -C TastyCook.UsersAPI docker-start
	$(MAKE) -C TastyCook.ProductsAPI docker-start

.PHONY: test
test:
	$(MAKE) -C TastyCook.RecipesAPI test
	$(MAKE) -C TastyCook.UsersAPI test
	$(MAKE) -C TastyCook.ProductsAPI test

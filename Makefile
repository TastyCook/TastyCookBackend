.PHONY: docker-build
docker-build:
	$(MAKE) -C TastyCook.RecipesAPI docker-build
	$(MAKE) -C TastyCook.UsersAPI docker-build

.PHONY: docker-push
docker-push:
	$(MAKE) -C TastyCook.RecipesAPI docker-push
	$(MAKE) -C TastyCook.UsersAPI docker-push

.PHONY: docker-start
docker-start:
	$(MAKE) -C TastyCook.RecipesAPI docker-start
	$(MAKE) -C TastyCook.UsersAPI docker-start

#!/bin/bash

# Get the list of container IDs with the "tasty-cook" prefix
container_ids=$(docker ps -a --filter "name=tasty-cook" --format "{{.ID}}")

# Loop through the container IDs and remove each one
for container_id in $container_ids; do
  echo "Removing container with ID: $container_id"
  docker rm -f "$container_id"
done

echo "All 'tasty-cook' containers have been removed."
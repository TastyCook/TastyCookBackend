#!/bin/bash

set -e

cd ../

portIndex=9000

# Recursively look for subdirectories with a Dockerfile and build their images
for dir in $(find . -type d -name "*"); do
  if [ -f "${dir}/Dockerfile" ]; then
    # Extract the directory name by removing the path and the "/Dockerfile" suffix
    dir_name=$(basename "${dir}")
    dir_name=${dir_name%/Dockerfile}

    # Convert the directory name from PascalCase to kebab-case
    name=$(echo "${dir_name}" | sed 's/\([a-z0-9]\)\([A-Z]\)/\1-\2/g' | tr '[:upper:]' '[:lower:]')

    echo "Building ${name} image"
    docker build -t "${name}" -f "${dir}/Dockerfile" "${dir}"
    docker create -p "${portIndex}:80" --name "${name}-container" "${name}"
    echo "Starting ${name}-container"
    docker start "${name}-container"

    portIndex+=1
  fi
done

echo "All backend services have been started"

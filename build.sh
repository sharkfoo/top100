#!/bin/bash

docker buildx build --platform linux/amd64,linux/arm64 -t sharkfoo/top100sync:latest -f Top100Sync/Dockerfile --push .

docker buildx build --platform linux/amd64,linux/arm64 -t sharkfoo/top100import:latest -f Top100Import/Dockerfile --push .

docker buildx build --platform linux/amd64,linux/arm64 -t sharkfoo/top100ui:latest -f Top100UI/Dockerfile --push .

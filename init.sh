#!/bin/bash

docker stack deploy  -c docker-compose-import.yml --with-registry-auth top100

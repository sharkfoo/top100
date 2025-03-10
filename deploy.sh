#!/bin/bash

docker stack deploy  -c docker-compose-ui.yml --with-registry-auth top100

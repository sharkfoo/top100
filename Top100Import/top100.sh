#!/bin/bash

cat 2019.txt \
	| sed -e "s/\t\"/\t/g" \
	| sed -e "s/\"\t/\t/g" \
	| sed -e "s/featuring\([^\t]*\)/(feat.\1)/g" \
	| awk -F '\t' '{print $2","$3",2019,"$1",0"}'

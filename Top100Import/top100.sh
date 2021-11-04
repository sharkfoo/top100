#!/bin/bash

cat 2020.txt \
	| sed -e "s/\t\"/\t/g" \
	| sed -e "s/\"\t/\t/g" \
	| sed -e "s/featuring\([^\t]*\)/(feat.\1)/g" \
	| awk -F '\t' '{print $2","$3",2020,"$1",0"}'

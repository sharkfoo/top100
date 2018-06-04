#!/bin/bash

cat 2017.txt \
	| sed -e "s/\t\"/\t/g" \
	| sed -e "s/\"\t/\t/g" \
	| sed -e "s/featuring\([^\t]*\)/(feat.\1)/g" \
	| awk -F '\t' '{print $2","$3",2017,"$1",0"}'

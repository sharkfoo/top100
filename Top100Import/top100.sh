#!/bin/bash

/bin/cat 2020.txt \
	| /bin/sed -e "s/\t\"/\t/g" \
	| /bin/sed -e "s/\"\t/\t/g" \
	| /bin/sed -e "s/featuring\([^\t]*\)/(feat.\1)/g" \
	| /bin/awk -F '\t' '{print $2","$3",2021,"$1",0"}'

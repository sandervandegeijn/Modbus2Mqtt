#!/bin/bash
cd "$(dirname "$0")"
git remote update
changes=`git status | grep "Your branch is up-to-date" | wc -l`
if [ $changes -eq 0 ]
then
	echo "Updating container"
	git pull
	cd Modbus2Mqtt
	docker rmi modbus2mqtt
	docker build  --file Dockerfile -t modbus2mqtt .
fi

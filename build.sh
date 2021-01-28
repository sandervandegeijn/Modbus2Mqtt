#!/bin/bash
cd "$(dirname "$0")"
git remote update
changes=`git status | grep "Your branch is up-to-date" | wc -l`
if [ $changes -eq 0 ]
then
	echo "Updating container"
	docker stop modbus2mqtt-9600

	#docker rm modbus2mqtt-9600
	#docker rmi modbus2mqtt
	git pull
	docker build  --file Dockerfile -t modbus2mqtt .
	#docker run -d --name neofotoweb --network custombridge --restart always neofotoweb
fi

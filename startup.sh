docker rm $(docker stop $(docker ps -a -q --filter ancestor=office-365-main --format="{{.ID}}"))
docker build . -t office-365-main
# uncomment this line when you want to your container to start
#docker run -dit --restart unless-stopped office-365-main

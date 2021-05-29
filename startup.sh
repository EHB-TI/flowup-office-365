docker rm $(docker stop $(docker ps -a -q --filter ancestor=office-365-main --format="{{.ID}}"))
docker build . -t office-365-main
docker run -d office-365-main
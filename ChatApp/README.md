docker compose down
docker-compose up -d --build
docker attach chatapp-client-1
docker attach chatapp-client2-1
host = host.docker.internal
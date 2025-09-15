# Prod Mod
```bash
docker compose up -d --build
```

# Dev Mod
```bash
# override ettiği için normal çalıştırdığımızda override ile dev modda, yani watch ile çalışır.
docker compose -f docker-compose.watch.yml up -d --build
```
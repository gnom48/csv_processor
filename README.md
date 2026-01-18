# CsvProcessor

### Запуск:
1. Создать `.env` в корне решения:
```
TIME_DB_USER=postgres
TIME_DB_PASS=tkPkU9RoqSN5hxtfK0
TIME_DB_PORT=5432
TIME_DB_HOST=timescale-db
```

2. Выполнить:
```sh
docker-compose up --build -d 
```

### Для запуска тестов:
1. Перейти в /Testing

2. Выполнить:
```sh
dotnet test
```
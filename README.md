# Tubes3_SiHashtag
Tugas pemenuhan TUBES STIMA




## Dump the database
Run the following command and change the necessary field
```
docker exec <CONTAINER ID> mariadb-dump -u root -p<password> tubes3_stima24 > dump.sql
```
for example
```
docker exec 4b4d7661eb39 mariadb-dump -u root -ppassword tubes3_stima24 > Assets/dump/tubes3_stima24_2.sql
```
the `-ppassword` is correct without any space

To get the Container Id, run the following statement.
```
docker ps
```

To use existing dump, run the following statement.
```
docker exec -i <container id> mariadb -u root -ppassword tubes3_stima24 < Assets/dump/tubes3_stima24_2.sql
```

```
docker exec -i 4b4d7661eb39 mariadb -u root -ppassword tubes3_stima24 < Assets/dump/tubes3_stima24.sql
```

If using MariaDB and the dump is MySQL, replace every `utf8mb4_0900_ai_ci` with `utf8mb4_general_ci`
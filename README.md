# Tubes3_SiHashtag
Tugas pemenuhan TUBES STIMA




## Dump the database
Run the following command and change the necessary field
```
docker exec <CONTAINER ID> mariadb-dump -u root -p<password> duhfun > dump.sql
```
for example
```
docker exec 31d374628ddb mariadb-dump -u root -ppassword finger > Assets/dump/tubes3_stima24_2.sql
```
the `-ppassword` is correct without any space

To get the Container Id, run the following statement.
```
docker ps
```

To use existing dump, run the following statement.
```
docker exec <CONTAINER ID> mariadb-dump -u root -ppassword finger < Assets/dump/tubes3_stima24_2.sql
```
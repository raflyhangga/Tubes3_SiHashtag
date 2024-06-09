# Tubes3_SiHashtag
Tugas pemenuhan TUBES STIMA

## Anggota Kelompok
- 13522014, Raden Rafly Hanggaraksa Budiarto 
- 13522084, Dhafin Fawwaz Ikramullah 
- 13522107, Rayendra Althaf Taraka Noor 

## Warning!
Please beware that some MySQL or MariaDB version is not compatible with this project. Tested MariaDB version is 11.3.2-MariaDB-1:11.3.2+maria~ubu2204. To make sure its compatible, please use docker and follow the steps below. Or if you want to test on your own, try running `dotnet run testascii` and see if its passed.

## Build the executable
Run the following command to build the executable.
```
dotnet publish -r win-x64
```
After the build is finished, you can run by double clicking `bin/Release/publish/Tubes3_SiHashtag.exe`


## How to run program
Build the docker image
```
docker-compose up --build
```

After you have build it, you can just run the following without needing to build it again. This will enable the database.
```
docker-compose up
```

Before starting the app, we need to restore the database with the existing dump. Open a new terminal. Run the following command and change the necessary field
```
docker exec -i <container id> mariadb -u root -ppassword tubes3_stima24 < Assets/dump/tubes3_stima24_2.sql
```
for example
```
docker exec -i 4b4d7661eb39 mariadb -u root -ppassword tubes3_stima24 < Assets/dump/tubes3_stima24.sql
```
The `-ppassword` is correct without any space.
If using MariaDB and the dump is MySQL, replace every `utf8mb4_0900_ai_ci` with `utf8mb4_general_ci`

To get the Container Id, run the following statement.
```
docker ps
```

Make sure to run the preprocess before starting the app
```
dotnet run preprocess
```

If you want to try the seeding, run the following command
```
dotnet run seed
```

If you dont want to use docker, it might be possible depending on your MariaDB/MySQL version. Run the following command to check whether its compatible or not.
```
dotnet run testascii
```

Then run the executable or just run the following command. Make sure .Net is installed.
```
dotnet run
```


If you want to dump the database, open a new terminal. Run the following command and change the necessary field
```
docker exec <CONTAINER ID> mariadb-dump -u root -p<password> tubes3_stima24 > dump.sql
```
for example
```
docker exec 4b4d7661eb39 mariadb-dump -u root -ppassword tubes3_stima24 > Assets/dump/tubes3_stima24.sql
```
using System;
using System.IO;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Diagnostics;

public class Seeder{
    public static void PreprocessSidikjari() 
    {
        Console.WriteLine("Before:");
        CheckDatabaseContent();


        Database.Initialize();

        Console.WriteLine("Preprocess started...");

        MySqlDataReader reader = Database.Execute("ALTER TABLE sidik_jari ADD COLUMN IF NOT EXISTS ascii TEXT"); 
        reader.Close();

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        List<SidikJari> sjList = SidikJari.GetAll();
        int counter = 0;
        Parallel.ForEach(sjList, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, sj => {
            sj.PopulateImageAscii();
            if(++counter % 1000 == 0) Console.WriteLine(counter+" images converted");
        });
        stopwatch.Stop();
        Console.WriteLine("Ascii conversion finished in "+stopwatch.ElapsedMilliseconds+" ms");


        stopwatch.Restart();
        counter = 0;
        foreach(SidikJari sj in sjList){
            Database.ExecuteNonQuery("INSERT INTO sidik_jari (berkas_citra, nama, ascii) VALUES (@berkas_citra, @nama, @ascii)", 
                ("@berkas_citra", sj.BerkasCitra),
                ("@nama", sj.Nama),
                ("@ascii", sj.Ascii)
            );
            if(++counter % 1000 == 0) Console.WriteLine(counter+" ascii inserted");
        }
        stopwatch.Stop();
        Console.WriteLine("Database update finished in "+stopwatch.ElapsedMilliseconds+" ms");
        
        Console.WriteLine("Preprocess Finished!");


        
        Console.WriteLine("After:");
        CheckDatabaseContent();
    }

    public static void CheckDatabaseContent(){
        List<SidikJari> list = SidikJari.GetAll();
        int max = Math.Min(3, list.Count);
        for(int i = 0; i < max; i++){
            Console.WriteLine("==== "+i+" ====");
            Console.WriteLine(list[i].Nama);
            Console.WriteLine(list[i].BerkasCitra);
            Console.WriteLine(list[i].Ascii);
        }
    }

    public static void StartSeeding(string imageFolderPath){
        Database.Initialize();
        Database.ExecuteNonQuery("ALTER TABLE sidik_jari ADD COLUMN IF NOT EXISTS ascii TEXT"); 
        Database.ExecuteNonQuery("TRUNCATE TABLE biodata");
        Database.ExecuteNonQuery("TRUNCATE TABLE sidik_jari");


        string[] filePathList = Directory.GetFiles(imageFolderPath);

        Console.WriteLine("Seeding started...");
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // Parallel version with max thread = amount of processor
        SidikJari[] sjList = new SidikJari[filePathList.Length];
        int maxThread = Environment.ProcessorCount;
        int unsafeCounter = 0;
        Parallel.For(0, filePathList.Length, new ParallelOptions { MaxDegreeOfParallelism = maxThread }, i => {
            string file = filePathList[i];
            string name = Path.GetFileNameWithoutExtension(file);
            SidikJari sj = new SidikJari(file, name);
            sjList[i] = sj;

            // below is not accurate because threading
            if(++unsafeCounter % 1000 == 0) Console.WriteLine(unsafeCounter+" images converted");
        });
        stopwatch.Stop();
        Console.WriteLine("Ascii conversion finished in "+stopwatch.ElapsedMilliseconds+" ms");



        stopwatch.Restart();
        long count = (long) Math.Pow(10,15);
        // Cannot bulk insert because of max_allowed_packet
        Reader rdr = new TextReader();
        Stack<string> allNames = getCopyOfAllNames(rdr);
        Stack<string> allAddress = getCopyOfAllAddress(rdr);
        string[] countries = rdr.getContentList("Assets/country.txt");
        unsafeCounter = 0;
        foreach(SidikJari sj in sjList){
            string name = (allNames.Count != 0)? allNames.Pop() : sj.Nama;
            string address = (allAddress.Count != 0)? allAddress.Pop() : "Twobagoes Esmail";
            string jenis_kelamin = (string)Random.Shared.Choice("Laki-laki", "Perempuan");
            string gol_darah = (string)Random.Shared.Choice("A+","A-","B+","B-","O+","O-","AB+","AB-");
            string agama = (string)Random.Shared.Choice("Islam","Kristen","Protestan","Hindu","Buddha");
            string status_menikah = (string)Random.Shared.Choice("Belum menikah", "Menikah");
            string pekerjaan = (string)Random.Shared.Choice("Tidak Bekerja", "Manager","Consultant","Software Engineer");
            string country = (string)Random.Shared.Choice(countries);

            Database.ExecuteNonQuery("INSERT INTO sidik_jari (berkas_citra, nama, ascii) VALUES (@berkas_citra, @nama, @ascii)", 
                ("@berkas_citra", sj.BerkasCitra),
                ("@nama", Random.Shared.NextDouble() >= 0.5 ? name : name.ToAlay()),
                ("@ascii", sj.Ascii)
            );

            Database.ExecuteNonQuery("INSERT INTO biodata (NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan) VALUES (@NIK, @nama, @tempat_lahir, @tanggal_lahir, @jenis_kelamin, @golongan_darah, @alamat, @agama, @status_perkawinan, @pekerjaan, @kewarganegaraan)", 
                ("@NIK", (count++).ToString()),
                ("@nama", name), 
                ("@tempat_lahir", country), 
                ("@tanggal_lahir", new DateOnly()), 
                ("@jenis_kelamin", jenis_kelamin), 
                ("@golongan_darah", gol_darah),
                ("@alamat", address),
                ("@agama", agama), 
                ("@status_perkawinan", status_menikah),
                ("@pekerjaan", pekerjaan), 
                ("@kewarganegaraan", country)
            );

            if(++unsafeCounter % 1000 == 0) Console.WriteLine(unsafeCounter+" images inserted");
        }
        stopwatch.Stop();
        Console.WriteLine("Insert database finished in "+stopwatch.ElapsedMilliseconds+" ms");
    }

    
    
    static Stack<string> getCopyOfAllNames(Reader rdr){
        Stack<string> copy = new();
        string[] temp = rdr.getContentList("Assets/nama.txt");
        foreach(string str in temp) {
            copy.Push(str);
        }
        return copy;
    }

    static Stack<string> getCopyOfAllAddress(Reader rdr){
        Stack<string> copy = new();
        string[] temp = rdr.getContentList("Assets/alamat.txt");
        foreach(string str in temp) {
            copy.Push(str);
        }
        return copy;
    }


    public static void TestAllAscii(){
        Database.Initialize();
        Database.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS test (value TEXT)");
        
        string value = "";
        for(int i = 0; i < 256; i++) value += (char)i;

        Database.ExecuteNonQuery("INSERT INTO test (value) VALUES(@value)", ("@value", value));

        MySqlDataReader reader = Database.Execute("SELECT * FROM test");
        reader.Read();
        string valToCheck = reader.GetString("value");
        reader.Close();
        if(valToCheck != value) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed. Please use another MariaDB/MySQL version or use docker.");
            Console.WriteLine("Value after database SELECT statement: " + valToCheck);
            Console.WriteLine("Hardcoded value: " + value);
            Console.WriteLine();
            
            for(int i = 0; i < value.Length; i++){
                if(value[i] != valToCheck[i]){
                    Console.WriteLine("Different value at index "+i);
                    Console.WriteLine(value[i] + ": " + (int)value[i]);
                    Console.WriteLine(valToCheck[i] + ": " + (int)valToCheck[i]);
                    break;
                }
            }
            Console.ResetColor();
        } else {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Passed, you can use this MariaDB/MySQL version.");
            Console.ResetColor();
        }

        Database.ExecuteNonQuery("DROP TABLE test");
    }
}
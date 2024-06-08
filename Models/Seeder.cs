using System;
using System.IO;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;

public class Seeder{

    public static void PreprocessSidikjari() 
    {
        Console.WriteLine("Before:");
        CheckDatabaseContent();


        Database.Initialize();

        Console.WriteLine("Preprocess started...");

        MySqlDataReader reader = Database.Execute("ALTER TABLE sidik_jari ADD COLUMN IF NOT EXISTS ascii TEXT"); 
        reader.Close();

        // Non parallel version
        // reader = Database.Execute("SELECT * FROM sidik_jari");
        // List<SidikJari> sjList = new List<SidikJari>();
        // while(reader.Read()){
        //     SidikJari sidikJari = new SidikJari(
        //         reader.GetString("berkas_citra"),
        //         reader.GetString("nama")
        //     );
        //     sjList.Add(sidikJari);
        // }
        // reader.Close();

        // foreach(Tuple<int,SidikJari> tuple in sjList){
        //     int id = tuple.Item1;
        //     SidikJari sj = tuple.Item2;
        //     Database.Execute("UPDATE sidik_jari SET ascii = '"+sj.Ascii+"' WHERE id = "+id);
        // }

        // Parallel version. Not allowed
        // List<SidikJari> sjList = SidikJari.GetAll();
        // Parallel.ForEach(sjList, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, sj => {
        //     MySqlCommand cmd = Database.ExecuteCommand("UPDATE sidik_jari SET ascii = '"+sj.ReadImageASCII()+"' WHERE nama = "+sj.Nama);
        //     cmd.ExecuteNonQuery();
        // });


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



        // Parallel version
        // SidikJari[] sjList = new SidikJari[filePathList.Length];
        // int unsafeCounter = 0;
        // Parallel.For(0, filePathList.Length, i => {
        //     string file = filePathList[i];
        //     string name = Path.GetFileNameWithoutExtension(file);
        //     SidikJari sj = new SidikJari(file, name);
        //     sjList[i] = sj;

        //     // below is not accurate because threading
        //     if(++unsafeCounter % 1000 == 0) Console.WriteLine(unsafeCounter+" images converted");
        // });
        // stopwatch.Stop();
        // Console.WriteLine("Seeding finished in "+stopwatch.ElapsedMilliseconds+" ms");



        // Non parallel version
        // List<SidikJari> sjList = new List<SidikJari>(filePathList.Length);
        // int unsafeCounter = 0;
        // foreach(string file in filePathList){
        //     string name = Path.GetFileNameWithoutExtension(file);
        //     SidikJari sj = new SidikJari(file, name);
        //     sjList.Add(sj);

        //     if(++unsafeCounter % 1000 == 0) Console.WriteLine(unsafeCounter+" images converted");         
        // }
        // stopwatch.Stop();
        // Console.WriteLine("Seeding finished in "+stopwatch.ElapsedMilliseconds+" ms");



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

        Stack<string> allNames = getCopyOfAllNames();
        unsafeCounter = 0;
        foreach(SidikJari sj in sjList){
            string name;
            if(allNames.Count != 0) name = allNames.Pop();
            else name = sj.Nama;

            Database.ExecuteNonQuery("INSERT INTO sidik_jari (berkas_citra, nama, ascii) VALUES (@berkas_citra, @nama, @ascii)", 
                ("@berkas_citra", sj.BerkasCitra),
                ("@nama", name),
                ("@ascii", sj.Ascii)
            );

            Database.ExecuteNonQuery("INSERT INTO biodata (NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan) VALUES (@NIK, @nama, @tempat_lahir, @tanggal_lahir, @jenis_kelamin, @golongan_darah, @alamat, @agama, @status_perkawinan, @pekerjaan, @kewarganegaraan)", 
                ("@NIK", (count++).ToString()),
                ("@nama", Random.Shared.NextDouble() >= 0.5 ? name : name.ToAlay()), 
                ("@tempat_lahir", "tempat_lahir"), 
                ("@tanggal_lahir", new DateOnly()), 
                ("@jenis_kelamin", Random.Shared.Choice("Laki-laki", "Perempuan")), 
                ("@golongan_darah", "A"),
                ("@alamat", "alamat"),
                ("@agama", "agama"), 
                ("@status_perkawinan", Random.Shared.Choice("Belum menikah", "Menikah")),
                ("@pekerjaan", "pekerjaan"), 
                ("@kewarganegaraan", "kewarganegaraan")
            );

            if(++unsafeCounter % 1000 == 0) Console.WriteLine(unsafeCounter+" images inserted");
        }


        // Manual version
        // MySqlConnection _connection = null;
        // foreach(SidikJari sj in sjList){
        //     MySqlCommand cmd = new MySqlCommand("INSERT INTO sidik_jari (berkas_citra, nama, ascii) VALUES (@berkas_citra, @nama, @ascii)", _connection);
        //     cmd.Parameters.AddWithValue("@berkas_citra", sj.BerkasCitra);
        //     cmd.Parameters.AddWithValue("@nama", sj.Nama);
        //     cmd.Parameters.AddWithValue("@ascii", sj.Ascii);
        //     reader = cmd.ExecuteReader();
        //     reader.Close();

        //     cmd = new MySqlCommand("INSERT INTO biodata (NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan) VALUES (@NIK, @nama, @tempat_lahir, @tanggal_lahir, @jenis_kelamin, @golongan_darah, @alamat, @agama, @status_perkawinan, @pekerjaan, @kewarganegaraan)", _connection);
        //     cmd.Parameters.AddWithValue("@NIK", (count++).ToString());
        //     cmd.Parameters.AddWithValue("@nama", sj.Nama);
        //     cmd.Parameters.AddWithValue("@tempat_lahir", "tempat_lahir");
        //     cmd.Parameters.AddWithValue("@tanggal_lahir", new DateOnly());
        //     cmd.Parameters.AddWithValue("@jenis_kelamin", Random.Shared.Choice("Laki-laki", "Perempuan"));
        //     cmd.Parameters.AddWithValue("@golongan_darah", "A");
        //     cmd.Parameters.AddWithValue("@alamat", "alamat");
        //     cmd.Parameters.AddWithValue("@agama", "agama");
        //     cmd.Parameters.AddWithValue("@status_perkawinan", Random.Shared.Choice("Belum menikah", "Menikah"));
        //     cmd.Parameters.AddWithValue("@pekerjaan", "pekerjaan");
        //     cmd.Parameters.AddWithValue("@kewarganegaraan", "kewarganegaraan");
        //     reader = cmd.ExecuteReader();
        //     reader.Close();
        // }



        // Bulk edit version. Not possible because Packets larger than max_allowed_packet
        // stopwatch.Restart();
        // long count = (long) Math.Pow(10,15);
        // var sidikJariQuery = new StringBuilder();
        // var biodataQuery = new StringBuilder();
        // var parameters = new List<MySqlParameter>();

        // int paramIndex = 0;

        // foreach(SidikJari sj in sjList)
        // {
        //     // Append to the sidik_jari query with parameters
        //     sidikJariQuery.AppendLine($"INSERT INTO sidik_jari (berkas_citra, nama, ascii) VALUES (@berkas_citra{paramIndex}, @nama{paramIndex}, @ascii{paramIndex});");
        //     parameters.AddRange(new[]
        //     {
        //         new MySqlParameter($"@berkas_citra{paramIndex}", sj.BerkasCitra),
        //         new MySqlParameter($"@nama{paramIndex}", sj.Nama),
        //         new MySqlParameter($"@ascii{paramIndex}", sj.Ascii)
        //     });

        //     // Append to the biodata query with parameters
        //     var NIK = (count++).ToString();
        //     var tempat_lahir = "tempat_lahir";
        //     var tanggal_lahir = DateTime.Now.ToString("yyyy-MM-dd"); // Assuming you want today's date for demo purposes
        //     var jenis_kelamin = new Random().Next(0, 2) == 0 ? "Laki-laki" : "Perempuan"; // Randomly choose gender
        //     var golongan_darah = "A";
        //     var alamat = "alamat";
        //     var agama = "agama";
        //     var status_perkawinan = new Random().Next(0, 2) == 0 ? "Belum menikah" : "Menikah"; // Randomly choose status
        //     var pekerjaan = "pekerjaan";
        //     var kewarganegaraan = "kewarganegaraan";

        //     biodataQuery.AppendLine($"INSERT INTO biodata (NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan) VALUES (@NIK{paramIndex}, @nama_b{paramIndex}, @tempat_lahir{paramIndex}, @tanggal_lahir{paramIndex}, @jenis_kelamin{paramIndex}, @golongan_darah{paramIndex}, @alamat{paramIndex}, @agama{paramIndex}, @status_perkawinan{paramIndex}, @pekerjaan{paramIndex}, @kewarganegaraan{paramIndex});");
        //     parameters.AddRange(new[]
        //     {
        //         new MySqlParameter($"@NIK{paramIndex}", NIK),
        //         new MySqlParameter($"@nama_b{paramIndex}", sj.Nama),
        //         new MySqlParameter($"@tempat_lahir{paramIndex}", tempat_lahir),
        //         new MySqlParameter($"@tanggal_lahir{paramIndex}", tanggal_lahir),
        //         new MySqlParameter($"@jenis_kelamin{paramIndex}", jenis_kelamin),
        //         new MySqlParameter($"@golongan_darah{paramIndex}", golongan_darah),
        //         new MySqlParameter($"@alamat{paramIndex}", alamat),
        //         new MySqlParameter($"@agama{paramIndex}", agama),
        //         new MySqlParameter($"@status_perkawinan{paramIndex}", status_perkawinan),
        //         new MySqlParameter($"@pekerjaan{paramIndex}", pekerjaan),
        //         new MySqlParameter($"@kewarganegaraan{paramIndex}", kewarganegaraan)
        //     });

        //     paramIndex++;
        // }

        // Database.Execute(sidikJariQuery.ToString(), parameters.ToArray());
        // Database.Execute(biodataQuery.ToString(), parameters.ToArray());

        stopwatch.Stop();
        Console.WriteLine("Insert database finished in "+stopwatch.ElapsedMilliseconds+" ms");
    }

    
    
    static Stack<string> getCopyOfAllNames(){
        Stack<string> copy = new Stack<string>();
        for(int i = 0; i < IndonesianNames.Names.Length; i++){
            copy.Push(IndonesianNames.Names[i]);
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
            Console.WriteLine("Kok beda banhhh");
            Console.WriteLine(valToCheck);
            Console.WriteLine(value);
            Console.WriteLine();
            
            for(int i = 0; i < value.Length; i++){
                if(value[i] != valToCheck[i]){
                    Console.WriteLine("beda di "+i);
                    Console.WriteLine(value[i] + ": " + (int)value[i]);
                    Console.WriteLine(valToCheck[i] + ": " + (int)valToCheck[i]);
                    break;
                }
            }
        } else {
            Console.WriteLine("yeay sama");
        }

        Database.ExecuteNonQuery("DROP TABLE test");
    }
}
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


        Console.WriteLine("Preprocess started...");
        Database.Initialize();
        MySqlDataReader reader = Database.Execute("SELECT * FROM sidik_jari");
        List<Tuple<int,SidikJari>> sjList = new List<Tuple<int,SidikJari>>();
        while(reader.Read()){
            SidikJari sidikJari = new SidikJari(
                reader.GetString("berkas_citra"),
                reader.GetString("nama")
            );
            int id = reader.GetInt32("id");
            sjList.Add(new Tuple<int,SidikJari>(id,sidikJari));
        }
        reader.Close();

        foreach(Tuple<int,SidikJari> tuple in sjList){
            int id = tuple.Item1;
            SidikJari sj = tuple.Item2;
            Database.Execute("UPDATE sidik_jari SET ascii = '"+sj.Ascii+"' WHERE id = "+id);
        }
        
        Console.WriteLine("Preprocess Finished!");

        
        Console.WriteLine("After:");
        CheckDatabaseContent();
    }

    public static void CheckDatabaseContent(){
        List<SidikJari> list = SidikJari.GetAll();
        int max = Math.Min(5, list.Count);
        for(int i = 0; i < max; i++){
            Console.WriteLine("==== "+i+" ====");
            Console.WriteLine(list[i].Nama);
            Console.WriteLine(list[i].BerkasCitra);
            Console.WriteLine(list[i].Ascii);
        }
    }

    public static void StartSeeding(string imageFolderPath){
        Database.Initialize();
        MySqlDataReader reader = Database.Execute("ALTER TABLE sidik_jari ADD COLUMN IF NOT EXISTS ascii TEXT"); 
        reader.Close();

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



        // Parallel version with max thread = 100
        SidikJari[] sjList = new SidikJari[filePathList.Length];
        int maxThread = 8000;
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
        foreach(SidikJari sj in sjList){
            reader = Database.Execute("INSERT INTO sidik_jari (berkas_citra, nama, ascii) VALUES (@berkas_citra, @nama, @ascii)", 
                ("@berkas_citra", sj.BerkasCitra),
                ("@nama", sj.Nama),
                ("@ascii", sj.Ascii)
            );
            reader.Close();

            reader = Database.Execute("INSERT INTO biodata (NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan) VALUES (@NIK, @nama, @tempat_lahir, @tanggal_lahir, @jenis_kelamin, @golongan_darah, @alamat, @agama, @status_perkawinan, @pekerjaan, @kewarganegaraan)", 
                ("@NIK", (count++).ToString()),
                ("@nama", sj.Nama), 
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
            reader.Close();
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
}
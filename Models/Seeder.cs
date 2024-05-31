using System;
using System.IO;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

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
        Console.WriteLine("Seeding started...");

        Database.Initialize();

        string[] filePathList = Directory.GetFiles(imageFolderPath);
        int progress = 0;
        List<SidikJari> sjList = new List<SidikJari>(filePathList.Length);
        foreach(string file in filePathList){
            string name = Path.GetFileNameWithoutExtension(file);
            SidikJari sj = new SidikJari(file, name);
            sjList.Add(sj);
            progress++;
            if(progress % 1000 == 0) Console.WriteLine(progress+" images seeded");
        }

        long count = (long) Math.Pow(10,16);
        foreach(SidikJari sj in sjList){
            MySqlDataReader reader = Database.Execute("INSERT INTO sidik_jari (berkas_citra, nama, ascii) VALUES (@berkas_citra, @nama, @ascii)", 
                ("@berkas_citra", sj.BerkasCitra),
                ("@nama", sj.Nama),
                ("@ascii", sj.Ascii)
            );

            reader.Close();

            reader = Database.Execute("INSERT INTO biodata (NIK, nama, tempat_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan) VALUES (@berkas_citra, @nama, @ascii)", 
            ("@NIK", count++),
            ("@nama", sj.Nama), 
            ("@tempat_lahir", "tempat_lahir"), 
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
    }
}
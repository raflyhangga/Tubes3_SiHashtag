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
        foreach(string file in filePathList){
            string name = Path.GetFileNameWithoutExtension(file);
            SidikJari sj = new SidikJari(file, name);
            MySqlDataReader reader = Database.Execute("INSERT INTO sidik_jari (berkas_citra, nama, ascii) VALUES (@berkas_citra, @nama, @ascii)", 
            
                new List<Tuple<string,string>>() {
                    new Tuple<string,string>("@berkas_citra", sj.BerkasCitra),
                    new Tuple<string,string>("@nama", sj.Nama),
                    new Tuple<string,string>("@ascii", sj.Ascii)
                }
            
            );
            reader.Close();
            progress++;
            if(progress % 1000 == 0) Console.WriteLine(progress+" images seeded");
        }
    }
}
using Avalonia.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;

public class SidikJari{
    string berkas_citra; // path to image file
    string nama;

    public static List<SidikJari> GetAll(){
        MySqlDataReader reader = Database.Execute("SELECT * FROM sidik_jari");
        List<SidikJari> list = new List<SidikJari>();
        while(reader.Read()){
            SidikJari sidikJari = new SidikJari();
            sidikJari.berkas_citra = reader.GetString("berkas_citra");
            sidikJari.nama = reader.GetString("nama");
            list.Add(sidikJari);
        }
        return list;
    }

    public string ReadImageASCII(){
        FileStream fs = new FileStream(berkas_citra, FileMode.Open, FileAccess.Read);
        List<byte> image = new List<byte>();
        while(fs.Position < fs.Length){
            image.Add((byte)fs.ReadByte());
        }

        // convert to ASCII
        string ascii = "";
        foreach(byte b in image){
            ascii += (char)b;
        }
        return ascii;
    }


    public static void SaveAllImagesInPathToDatabase(string path){
        string[] files = Directory.GetFiles(path);
        foreach(string file in files){
            SidikJari sj = new SidikJari();
            sj.berkas_citra = Path.Join(path, file);
            sj.nama = file;
            sj.Save();
        }
    }

    public void Save(){
        Database.Execute("INSERT INTO sidik_jari (berkas_citra, nama) VALUES ('"+berkas_citra+"', '"+nama+"')");
    }

}
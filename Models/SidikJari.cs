using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System;

public class SidikJari{
    string berkas_citra; // path to image file
    string nama;

    public SidikJari(string berkas_citra, string nama){
        this.berkas_citra = berkas_citra;
        this.nama = nama;
    }

    public static List<SidikJari> GetAll(){
        MySqlDataReader reader = Database.Execute("SELECT * FROM sidik_jari");
        List<SidikJari> list = new List<SidikJari>();
        while(reader.Read()){
            SidikJari sidikJari = new SidikJari(
                reader.GetString("berkas_citra"),
                reader.GetString("nama")
            );
            list.Add(sidikJari);
        }
        return list;
    }

    public string ReadImageASCII(){
        Bitmap image = new Bitmap(berkas_citra);

        string binaryStr = "";
        string ascii = "";
        for (int y = 0; y < image.Height; y++) {
            for (int x = 0; x < image.Width; x++) {
                Color pixelColor = image.GetPixel(x, y);
                double grayscale = (pixelColor.R * 0.299 + pixelColor.G * 0.587 + pixelColor.B * 0.114) / 255.0;
                int binaryValue = grayscale > 0.5 ? 1 : 0;
                binaryStr += binaryValue;
                
                if (binaryStr.Length == 8) {
                    byte binaryByte = Convert.ToByte(binaryStr, 2);
                    ascii += (char)binaryByte;
                    binaryStr = "";
                }
            }
        }


        // Handle if any remaining bits
        if (binaryStr.Length > 0) {
            // add 0 until 8 bits
            while (binaryStr.Length < 8) {
                binaryStr += "0";
            }
            byte binaryByte = Convert.ToByte(binaryStr, 2);
            ascii += (char)binaryByte;
        }

        // Dispose of the image
        image.Dispose();
        return ascii;
    }


    public static void SaveAllImagesInPathToDatabase(string path){
        string[] files = Directory.GetFiles(path);
        foreach(string file in files){
            SidikJari sj = new SidikJari(
                Path.Join(path, file),
                file
            );
            sj.Save();
        }
    }

    public void Save(){
        Database.Execute("INSERT INTO sidik_jari (berkas_citra, nama) VALUES ('"+berkas_citra+"', '"+nama+"')");
    }

}
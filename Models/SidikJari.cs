using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System;

public class SidikJari{
    public const double THRESHOLD = 0.2;
    public string BerkasCitra => _berkasCitra;
    public string Nama => _nama;
    public string Ascii => _ascii; // generated later

    string _berkasCitra; // path to image file
    string _nama;
    string _ascii;

    public SidikJari(string berkasCitra, string nama){
        _berkasCitra = berkasCitra;
        _nama = nama;
        _ascii = ReadImageASCII();
    }

    public SidikJari(string berkasCitra, string nama, string ascii){
        _berkasCitra = berkasCitra;
        _nama = nama;
        _ascii = ascii;
    }

    public static List<SidikJari> GetAll(){
        MySqlDataReader reader = Database.Execute("SELECT * FROM sidik_jari");
        List<SidikJari> list = new List<SidikJari>();
        while(reader.Read()){
            SidikJari sidikJari = new SidikJari(
                reader.GetString("berkas_citra"),
                reader.GetString("nama"),
                reader.GetString("ascii")
            );
            list.Add(sidikJari);
        }
        reader.Close();
        return list;
    }

    private string cleanPrefix(string path){
        return path.Replace("file:///", "");
    }

    // Return string representation of image by path
    public string ReadImageASCII(){
        _berkasCitra = cleanPrefix(_berkasCitra);
        Console.WriteLine(_berkasCitra);
        Bitmap image = new Bitmap(_berkasCitra);

        string binaryStr = "";
        string ascii = "";
        for (int y = 0; y < image.Height; y++) {
            for (int x = 0; x < image.Width; x++) {
                Color pixelColor = image.GetPixel(x, y);
                double grayscale = (pixelColor.R * 0.299 + pixelColor.G * 0.587 + pixelColor.B * 0.114) / 255.0;
                int binaryValue = grayscale > THRESHOLD ? 1 : 0;
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
            // TODO: generate ascii before saving
            sj.Save();
        }
    }

    public void Save(){
        // TODO: also save the ascii
        Database.Execute("INSERT INTO sidik_jari (berkas_citra, nama) VALUES ('"+_berkasCitra+"', '"+_nama+"')");
    }


    


}
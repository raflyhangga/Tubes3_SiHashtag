using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System;

public class SidikJari{
    const int CHOSEN_PIXEL_SIZE = 256;
    public const double THRESHOLD = 0.2;
    // Path to image file
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

    private SidikJari(string berkasCitra){
        _berkasCitra = berkasCitra;
        _nama = "";
    }
    public static SidikJari GetSidikJariInChosenPixelSize(string berkasCitra){
        SidikJari sj = new SidikJari(berkasCitra);
        sj._ascii = sj.ReadImageASCIIChosenPixelSize();
        return sj;
    }

    public static SidikJari GetSidikJariFull(string berkasCitra){
        SidikJari sj = new SidikJari(berkasCitra);
        sj._ascii = sj.ReadImageASCII();
        return sj;
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

    public void PopulateImageAscii(){
        _ascii = ReadImageASCII();
    }


    // Read only CHOSEN_PIXEL_SIZE middle binary
    public string ReadImageASCIIChosenPixelSize(){
        _berkasCitra = cleanPrefix(_berkasCitra);
        Bitmap image = new Bitmap(_berkasCitra);

        string binaryStr = "";
        string ascii = "";

        int binaryLength = image.Height * image.Width;


        int midPoint = image.Width*(image.Height/2) + image.Width/2;

        // Edge case if the image is too small
        if(binaryLength < CHOSEN_PIXEL_SIZE){
            // Add everything until binaryLength but ensure it can be divided by 8
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
            return ascii;
        }


        int middle = midPoint;
        int startingIndex = ((int)middle/8)*8 - CHOSEN_PIXEL_SIZE/2;
        int endingIndex = startingIndex + CHOSEN_PIXEL_SIZE;

        int firstY = startingIndex / image.Width;
        int firstX = startingIndex % image.Width;

        int lastY = endingIndex / image.Width;
        int lastX = endingIndex % image.Width;

        int currentX = firstX;
        int currentY = firstY;

        do {
            Color pixelColor = image.GetPixel(currentX, currentY);
            double grayscale = (pixelColor.R * 0.299 + pixelColor.G * 0.587 + pixelColor.B * 0.114) / 255.0;
            int binaryValue = grayscale > THRESHOLD ? 1 : 0;
            binaryStr += binaryValue;
            
            if (binaryStr.Length == 8) {
                byte binaryByte = Convert.ToByte(binaryStr, 2);
                ascii += (char)binaryByte;
                binaryStr = "";
            }

            currentX++;
            if(currentX == image.Width){
                currentX = 0;
                currentY++;
            }
        }while(currentX != lastX || currentY != lastY);


        // Dispose of the image
        image.Dispose();

        return ascii;
    }
}

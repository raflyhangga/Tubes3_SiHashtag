using System;
using System.IO;

public class TextReader: Reader{
    public override string[] getContentList(string file_path){
        string[] lines = File.ReadAllLines(file_path);
        return lines;
    }
}
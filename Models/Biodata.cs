using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class Biodata{
    string NIK;
    string nama;
    string tempat_lahir;
    DateTime tanggal_lahir;
    string jenis_kelamin;
    string golongan_darah;
    string alamat;
    string agama;
    string status_perkawinan;
    string pekerjaan;
    string kewarganegaraan;

    public static List<Biodata> GetAll(){
        MySqlDataReader reader = Database.Execute("SELECT * FROM biodata");
        List<Biodata> list = new List<Biodata>();
        while(reader.Read()){
            Biodata biodata = new Biodata();
            biodata.NIK = reader.GetString("NIK");
            biodata.nama = reader.GetString("nama");
            biodata.tempat_lahir = reader.GetString("tempat_lahir");
            biodata.tanggal_lahir = reader.GetDateTime("tanggal_lahir");
            biodata.jenis_kelamin = reader.GetString("jenis_kelamin");
            biodata.golongan_darah = reader.GetString("golongan_darah");
            biodata.alamat = reader.GetString("alamat");
            biodata.agama = reader.GetString("agama");
            biodata.status_perkawinan = reader.GetString("status_perkawinan");
            biodata.pekerjaan = reader.GetString("pekerjaan");
            biodata.kewarganegaraan = reader.GetString("kewarganegaraan");
            list.Add(biodata);
        }
        return list;
    }
}
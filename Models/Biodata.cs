using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class Biodata{
    // Getter
    public string NIK => nik;
    public string Nama => nama;
    public string TempatLahir => tempat_lahir;
    public DateTime TanggalLahir => tanggal_lahir;
    public string JenisKelamin => jenis_kelamin;
    public string GolonganDarah => golongan_darah;
    public string Alamat => alamat;
    public string Agama => agama;
    public string StatusPerkawinan => status_perkawinan;
    public string Pekerjaan => pekerjaan;
    public string Kewarganegaraan => kewarganegaraan;


    string nik;
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
            Biodata biodata = new Biodata
            {
                nik = reader.GetString("NIK"),
                nama = reader.GetString("nama"),
                tempat_lahir = reader.GetString("tempat_lahir"),
                tanggal_lahir = reader.GetDateTime("tanggal_lahir"),
                jenis_kelamin = reader.GetString("jenis_kelamin"),
                golongan_darah = reader.GetString("golongan_darah"),
                alamat = reader.GetString("alamat"),
                agama = reader.GetString("agama"),
                status_perkawinan = reader.GetString("status_perkawinan"),
                pekerjaan = reader.GetString("pekerjaan"),
                kewarganegaraan = reader.GetString("kewarganegaraan")
            };
            list.Add(biodata);
        }
        reader.Close();
        return list;
    }
}
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using System.IO;
using Tmds.DBus.Protocol;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Tubes3_SiHashtag.Views;

public partial class MainWindow : Window
{
    // SolveAlgorithm _solveAlgorithm = SolveAlgorithm.BM;
    FingerSolver _solver = new KMPSolver();

    IStorageFile _currentImageFile;
    public MainWindow()
    {
        Database.Initialize();
        FingerSolver.Initialize();
        InitializeComponent();
        AlgorithmSwitch.Checked += ToggleSwitch_Checked;
        AlgorithmSwitch.Unchecked += ToggleSwitch_Unchecked;
    }

    // string _imageSource = "/Assets/avalonia-logo.ico";

    public async void OnImagePickerClicked(object sender, RoutedEventArgs args)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Choose Image",
            AllowMultiple = false,
            FileTypeFilter = new[] { FilePickerFileTypes.ImageAll }
        });

        if (files.Count >= 1)
        {
            _currentImageFile = files[0];
            await using var stream = await files[0].OpenReadAsync();
            ImageDisplayerChoosen.Source = new Bitmap(stream);
        }
    }

    private void ToggleSwitch_Checked(object? sender, RoutedEventArgs e)
    {
        _solver = new BMSolver();
    }

    private void ToggleSwitch_Unchecked(object? sender, RoutedEventArgs e)
    {
        _solver = new KMPSolver();
    }

    public void OnSearch(object sender, RoutedEventArgs args)
    {
        // SidikJari sj = new SidikJari(_currentImageFile.Path.ToString(), "");
        SidikJari sj = SidikJari.GetSidikJariIn32Pixel(_currentImageFile.Path.ToString());
        FingerSolution solution = _solver.Solve(sj);
        Nama.Text = "Nama: " + solution.Biodata.Nama;
        NIK.Text = "NIK: " + solution.Biodata.NIK;
        TempatLahir.Text = "Tempat Lahir: " + solution.Biodata.TempatLahir;
        TanggalLahir.Text = "Tanggal Lahir: " + solution.Biodata.TanggalLahir.ToString();
        JenisKelamin.Text = "Jenis Kelamin: " + solution.Biodata.JenisKelamin;
        GolonganDarah.Text = "Golongan Darah: " + solution.Biodata.GolonganDarah;
        Alamat.Text = "Alamat: " + solution.Biodata.Alamat;
        Agama.Text = "Agama: " + solution.Biodata.Agama;
        StatusPerkawinan.Text = "Status Perkawinan: " + solution.Biodata.StatusPerkawinan;
        Pekerjaan.Text = "Pekerjaan: " + solution.Biodata.Pekerjaan;
        Kewarganegaraan.Text = "Kewarganegaraan: " + solution.Biodata.Kewarganegaraan;
        
        PersentaseKecocokan.Text = (solution.PersentaseKecocokan*100).ToString() + "%";
        ExecutionTime.Text = solution.ExecutionTime.ToString() + " ms";

        ImageDisplayerChoosen.Source = new Bitmap(solution.SidikJari.BerkasCitra);
    }

    public void OnSetKMP(object sender, RoutedEventArgs args)
        => _solver = new KMPSolver();

    public void OnSetBM(object sender, RoutedEventArgs args)
        => _solver = new BMSolver();
}
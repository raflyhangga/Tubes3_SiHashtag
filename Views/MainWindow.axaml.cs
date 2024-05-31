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
    FingerSolver _solver = new BMSolver();

    IStorageFile _currentImageFile;
    public MainWindow()
    {
        Database.Initialize();
        InitializeComponent();
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

    public void OnSearch(object sender, RoutedEventArgs args)
    {
        SidikJari sj = new SidikJari(_currentImageFile.Path.ToString(), "");
        FingerSolution solution = _solver.Solve(sj);
        Nama.Text = solution.Biodata.Nama;
        NIK.Text = solution.Biodata.NIK;
        TempatLahir.Text = solution.Biodata.TempatLahir;
        TanggalLahir.Text = solution.Biodata.TanggalLahir.ToString();
        JenisKelamin.Text = solution.Biodata.JenisKelamin;
        GolonganDarah.Text = solution.Biodata.GolonganDarah;
        Alamat.Text = solution.Biodata.Alamat;
        Agama.Text = solution.Biodata.Agama;
        StatusPerkawinan.Text = solution.Biodata.StatusPerkawinan;
        Pekerjaan.Text = solution.Biodata.Pekerjaan;
        Kewarganegaraan.Text = solution.Biodata.Kewarganegaraan;
        
        PersentaseKecocokan.Text = solution.PersentaseKecocokan.ToString() + "%";
        ExecutionTime.Text = solution.ExecutionTime.ToString() + " ms";
    }

    public void OnSetKMP(object sender, RoutedEventArgs args)
        => _solver = new KMPSolver();

    public void OnSetBM(object sender, RoutedEventArgs args)
        => _solver = new BMSolver();
}
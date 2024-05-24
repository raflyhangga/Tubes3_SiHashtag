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
    SolveAlgorithm _solveAlgorithm = SolveAlgorithm.BM;
    public MainWindow()
    {
        Database.Initialize();
        InitializeComponent();
    }

    // string _imageSource = "/Assets/avalonia-logo.ico";

    public async void OnImagePickerClicked(object sender, RoutedEventArgs args)
    {

        // img.Source = new Bitmap("/Assets/avalonia-logo.ico");

        var topLevel = TopLevel.GetTopLevel(this);

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Choose Image",
            AllowMultiple = false,
            FileTypeFilter = new[] { FilePickerFileTypes.ImageAll }
        });

        if (files.Count >= 1)
        {
            await using var stream = await files[0].OpenReadAsync();
            ImageDisplayerChoosen.Source = new Bitmap(stream);
        }
    }

    public async void OnSearch(object sender, RoutedEventArgs args)
    {
        Console.WriteLine("Helloooo");
    }

    public void OnSetKMP(object sender, RoutedEventArgs args)
        => _solveAlgorithm = SolveAlgorithm.KMP;

    public void OnSetBM(object sender, RoutedEventArgs args)
        => _solveAlgorithm = SolveAlgorithm.BM;
}
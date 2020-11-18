﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using RingingBloom;
using RingingBloom.WWiseTypes;
using RingingBloom.WWiseTypes.ViewModels;

namespace RingingBloom.Windows
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WWCTEditor : Window
    {
        public WWCTFile wwct = null;
        public WWCTViewModel viewModel = new WWCTViewModel();
        public WWCTEditor()
        {
            InitializeComponent();
            WWCTView.ItemsSource = viewModel.wwct;
        }

        public void MakeWWCT(object sender, RoutedEventArgs e)
        {
            wwct = new WWCTFile();
            viewModel.wwct.Clear();
            WWCTString strin = new WWCTString(Common.WWCTType.WWEV, "", 0);
            wwct.wwctStrings.Add(strin);
            viewModel.wwct.Add(strin);
        }
        public void ImportWWCT(object sender, RoutedEventArgs e)
        {
            OpenFileDialog importFile = new OpenFileDialog();
            importFile.Multiselect = false;
            importFile.Filter = "WWise Container files (*.wwct)|*.wwct";
            if(importFile.ShowDialog() == true)
            {
                BinaryReader readFile = new BinaryReader(new FileStream(importFile.FileName, FileMode.Open), Encoding.ASCII);
                wwct = new WWCTFile(readFile);
                for(int i = 0; i < wwct.wwctStrings.Count; i++)
                {
                    viewModel.wwct.Add(wwct.wwctStrings[i]);
                }
                readFile.Close();
            }
            

        }

        public void ImportNonDuplicate(object sender, RoutedEventArgs e)
        {
            if (wwct != null) {
                OpenFileDialog importFile = new OpenFileDialog();
                importFile.Multiselect = false;
                importFile.Filter = "WWise Container files (*.wwct)|*.wwct";
                WWCTFile import;
                List<WWCTString> newStrings = new List<WWCTString>();
                if (importFile.ShowDialog() == true)
                {
                    BinaryReader readFile = new BinaryReader(new FileStream(importFile.FileName, FileMode.Open), Encoding.ASCII);
                    import = new WWCTFile(readFile);
                    //look for non-duplicates
                    for (int i = 0; i < import.wwctStrings.Count; i++)
                    {
                        bool isDuplicate = false;
                        for(int j = 0; j < wwct.wwctStrings.Count; j++)
                        {
                            if (!isDuplicate)
                            {
                                isDuplicate = import.CompareWWCTString(import.wwctStrings[i], wwct.wwctStrings[j]);
                            }
                        }
                        if (!isDuplicate)
                        {
                            newStrings.Add(import.wwctStrings[i]);
                        }
                    }
                    //add strings to current file
                    for(int i = 0; i < newStrings.Count; i++)
                    {
                        wwct.wwctStrings.Add(newStrings[i]);
                        viewModel.wwct.Add(newStrings[i]);
                    }
                    readFile.Close();
                }
            }
            else
            {
                ImportWWCT(sender, e);
            }
        }

        public void ExportWWCT(object sender, RoutedEventArgs e)
        {
            WWCTView.Focus();
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "WWise Container files (*.wwct)|*.wwct";
            if (saveFile.ShowDialog() == true)
            {
                BinaryWriter exportFile = new BinaryWriter(new FileStream(saveFile.FileName, FileMode.CreateNew));
                wwct.ExportFile(exportFile);
                exportFile.Close();
            }
        }

        private void AddEntry(object sender, RoutedEventArgs e)
        {
            try
            {
                WWCTString strin = new WWCTString(Common.WWCTType.WWEV, "", 0);
                wwct.wwctStrings.Add(strin);
                viewModel.wwct.Add(strin);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("WWCT File not Loaded!");
            }
        }
        private void DeleteEntry(object sender, RoutedEventArgs e)
        {
            try
            {
                wwct.wwctStrings.RemoveAt(WWCTView.Items.IndexOf(WWCTView.SelectedItem));
                viewModel.wwct.RemoveAt(WWCTView.Items.IndexOf(WWCTView.SelectedItem));
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("WWCT File not Loaded!");
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("No entry selected!");
            }
        }
    }
}

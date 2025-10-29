// もう使わない

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;  // ファイル関連
using System;
using UnityEngine.UIElements;

public class csv_reader
{
    public static List<string[]> ReadCSV(string filename)
    {
        List<string[]> data = new List<string[]>();
        string[] lines = File.ReadAllLines(filename);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] row = lines[i].Split(',');
            data.Add(row);
        }

        return data;
    }
}

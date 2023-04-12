using System.IO;
using System.Text;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace DataProcessing
{
    public class LoadDataFileService
    {
        public List<int[]> LoadDataFile(string fileName, out Vector2 size)
        {
            size = Vector2.zero;
            List<int[]> rawdata = new List<int[]>();
            const Int32 BufferSize = 4096; //for NTFS
            bool firstLine = true;
            using (var fileStream = File.OpenRead(fileName))
            {
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                {
                    string line;
                    int lineIndex = 0;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (line.Equals(""))
                            continue; 
                        var parsedLine = ProcessLine(line).ToArray();
                        if (!firstLine)
                        {
                            if (parsedLine.Length < size.x)
                                throw new InvalidOperationException("Not enough data for level in line " + lineIndex);
                            rawdata.Add(parsedLine);
                            lineIndex++;
                        }
                        else
                        {
                            if (parsedLine.Length < 2)
                            {
                                throw new InvalidOperationException("Not enough size data for level size");
                            }
                            size = new Vector2(parsedLine[0], parsedLine[1]);
                            firstLine = false;
                        }
                    }
                    if (lineIndex < size.y)
                    {
                        throw new InvalidOperationException("Not enough data for level lines " + lineIndex + " size is " + size.y);
                    }
                }
            }
            return rawdata;
        }

        private List<int> ProcessLine(string line)
        {
            var values = line.Split(' ');
            var res = new List<int>();
            foreach (var v in values) 
            { 
                res.Add(int.Parse(v));
            }

            return res;
        }
    }
}
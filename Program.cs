using System;
using System.Collections.Generic;
using System.Xml;

class Program
{
    static double CalculateThroughput(double bandwidthMHz, int txAntennas)
    {
        const double spectralEfficiency = 6.0; // bps/Hz
        const double efficiencyFactor = 0.8;

        double mimoGain = Math.Log(txAntennas, 2);
        return bandwidthMHz * 1_000_000 * spectralEfficiency * mimoGain * efficiencyFactor / 1_000_000; // Mbps
    }

    static void Main(string[] args)
    {
        string xmlPath = "5GBTScomissioningFile-extended.xml";
        XmlDocument doc = new XmlDocument();

        try
        {
            doc.Load(xmlPath);
        }
        catch
        {
            Console.WriteLine("Failed to load XML file.");
            return;
        }

        XmlNodeList bands = doc.SelectNodes("//RadioSettings/Band");
        XmlNodeList mimoNodes = doc.SelectNodes("//CellConfiguration/Cell/MIMO");

        int count = Math.Min(bands.Count, mimoNodes.Count);
        double totalThroughput = 0.0;

        for (int i = 0; i < count; i++)
        {
            double bandwidthMHz = double.Parse(bands[i]["Bandwidth"].InnerText);
            int txAntennas = int.Parse(mimoNodes[i]["TxAntennas"].InnerText);

            double throughput = CalculateThroughput(bandwidthMHz, txAntennas);
            Console.WriteLine($"Cell {i} Throughput: {throughput:F2} Mbps");
            totalThroughput += throughput;
        }

        Console.WriteLine($"Total Base Station Throughput: {totalThroughput:F2} Mbps");
    }
}
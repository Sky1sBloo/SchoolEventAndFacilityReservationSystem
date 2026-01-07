namespace SFERS.Models.ViewModel
{
    public class AnalyticsViewModel
    {
        // Fix: Initialize arrays with empty values
        public string[] RoomLabels { get; set; } = Array.Empty<string>();
        public int[] RoomUsageData { get; set; } = Array.Empty<int>();

        public int ProjectorCount { get; set; }
        public int MicCount { get; set; }
        public int LaptopCount { get; set; }
    }
}
using Jint;
using System.Diagnostics;

namespace SharpBrowser.BrowserObjects
{
    public class Performance : ExpandableObject
    {
        private readonly Stopwatch stopwatch = Stopwatch.StartNew();
        private readonly Dictionary<string, double> marks = [];
        private readonly List<PerformanceMeasure> measures = [];

        public Performance(Engine engine) : base(engine, "Perfomance")
        {
        }

        public double Now()
        {
            return stopwatch.Elapsed.TotalMilliseconds;
        }

        public void Mark(string name)
        {
            marks[name] = Now();
        }

        public void Measure(string name, string startMark, string endMark = null)
        {
            if (!marks.TryGetValue(startMark, out var startTime))
            {
                throw new ArgumentException($"Start mark '{startMark}' not found.");
            }

            double endTime = endMark != null
                ? marks.GetValueOrDefault(endMark, Now())
                : Now();

            var duration = endTime - startTime;
            measures.Add(new PerformanceMeasure(name, startTime, duration));
        }

        public IReadOnlyList<PerformanceMeasure> GetMeasures()
        {
            return measures.AsReadOnly();
        }

        public void ClearMarks()
        {
            marks.Clear();
        }

        public void ClearMeasures()
        {
            measures.Clear();
        }
    }
    public class PerformanceMeasure
    {
        public string Name { get; }
        public double StartTime { get; }
        public double Duration { get; }

        public PerformanceMeasure(string name, double startTime, double duration)
        {
            Name = name;
            StartTime = startTime;
            Duration = duration;
        }
    }
}

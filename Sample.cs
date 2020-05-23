using System;
using System.Collections.Generic;
using System.Text;

namespace KeyStrokes
{
    public class Sample
    {
        public Sample()
        {
            Id = Guid.NewGuid();
            SampleTimes = new List<SampleTime>();
        }
        public Guid Id { get; set; }
        public String Name { get; set; }

        public List<SampleTime> SampleTimes{ get; }
    }
}

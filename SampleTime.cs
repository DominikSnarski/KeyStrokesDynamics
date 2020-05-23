using System;
using System.Collections.Generic;
using System.Text;

namespace KeyStrokes
{
    public class SampleTime
    {
        public SampleTime()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public int Code { get; set; }
        public long Time { get; set; }
    }
}

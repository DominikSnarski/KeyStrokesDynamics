using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyStrokes
{
    public class Repository
    {
        private ApplicationContext context;
        public Repository()
        {
            context = new ApplicationContext();
        }
        public void Add(Sample sample)
        {
                context.Add(sample);
                context.SaveChanges();
        }

        public IQueryable<Sample> GetQueryable()
        {
            return context.Samples.AsQueryable();
        }
    }
}

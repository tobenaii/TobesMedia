using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace TobesMediaServer.MediaPipeline
{
    public class ServiceLogger : IServiceLogger
    {
        public Dictionary<IMediaService, string> m_logs = new Dictionary<IMediaService, string>();
        private System.Timers.Timer m_timer = new System.Timers.Timer();

        public ServiceLogger()
        {
            m_timer.Elapsed += UpdateData;
            m_timer.Interval = 1000; // in miliseconds
            m_timer.AutoReset = true;
            m_timer.Enabled = true;
        }

        private void UpdateData(object sender, ElapsedEventArgs e)
        {
            if (m_logs.Count == 0)
                return;
            Console.Clear();
            Dictionary<IMediaService, string> logsCopy = new Dictionary<IMediaService, string>(m_logs);
            foreach (string message in logsCopy.Values)
                Console.WriteLine(message);
            m_logs.Clear();
        }

        public void Log(string message, IMediaService service)
        {
            if (!m_logs.ContainsKey(service))
                m_logs.Add(service, message);
            else
                m_logs[service] = message;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace org.theGecko.Utilities
{
    public class ColorConsoleTraceListener : ConsoleTraceListener
    {
        readonly Dictionary<TraceEventType, ConsoleColor> _eventColor = new Dictionary<TraceEventType, ConsoleColor>();

        public ColorConsoleTraceListener()
        {
            // Reserve white for "normal" console writing
            _eventColor.Add(TraceEventType.Verbose, ConsoleColor.DarkGray);
            _eventColor.Add(TraceEventType.Information, ConsoleColor.Gray);
            _eventColor.Add(TraceEventType.Warning, ConsoleColor.Yellow);
            _eventColor.Add(TraceEventType.Error, ConsoleColor.Red);
            _eventColor.Add(TraceEventType.Critical, ConsoleColor.DarkRed);
            _eventColor.Add(TraceEventType.Start, ConsoleColor.DarkYellow);
            _eventColor.Add(TraceEventType.Stop, ConsoleColor.DarkYellow);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            TraceEvent(eventCache, source, eventType, id, "{0}", message);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = GetEventColor(eventType, originalColor);
            base.TraceEvent(eventCache, source, eventType, id, format, args);
            Console.ForegroundColor = originalColor;
        }

        protected virtual ConsoleColor GetEventColor(TraceEventType eventType, ConsoleColor defaultColor)
        {
            if (_eventColor.ContainsKey(eventType))
            {
                return _eventColor[eventType];
            }

            return defaultColor;
        }
    }
}

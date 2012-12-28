﻿using System;
using System.Linq;

namespace Raven.Client.Bundles.ClockDocs
{
    public static class Extensions
    {
        /// <summary>
        /// Configures a clock document, using the specified name and interval.
        /// Uses a synchronization period identical to the interval.
        /// </summary>
        /// <param name="advancedSession">The advanced session, obtained from session.Advanced.</param>
        /// <param name="name">The name of the clock.</param>
        /// <param name="interval">The interval to update the clock.</param>
        public static void ConfigureClockDoc(this ISyncAdvancedSessionOperation advancedSession, string name, TimeSpan interval)
        {
            advancedSession.ConfigureClockDoc(name, interval, interval);
        }

        /// <summary>
        /// Configures a clock document, using the specified name, interval, and sync period.
        /// </summary>
        /// <param name="advancedSession">The advanced session, obtained from session.Advanced.</param>
        /// <param name="name">The name of the clock.</param>
        /// <param name="interval">The interval to update the clock.</param>
        /// <param name="sync">The synchronization period to align the clock to.</param>
        public static void ConfigureClockDoc(this ISyncAdvancedSessionOperation advancedSession, string name, TimeSpan interval, TimeSpan sync)
        {
            if (interval < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("interval", @"The interval cannot be negative.");

            if (sync < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("sync", @"The sync period cannot be negative.");

            if (sync > interval)
                throw new ArgumentOutOfRangeException("sync", @"The sync period cannot be greater than the interval.");

            var session = (IDocumentSession) advancedSession;

            var config = session.Load<ClocksConfig>(ClocksConfig.Id);
            if (config == null)
            {
                config = new ClocksConfig();
                config.AddClock(name, interval, sync);
                session.Store(config, ClocksConfig.Id);
            }

            var clock = config.Clocks.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (clock != null)
            {
                clock.Update(name, interval, sync);
                return;
            }
            
            config.AddClock(name, interval, sync);
        }
    }
}

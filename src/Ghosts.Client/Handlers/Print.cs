﻿// Copyright 2017 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using Ghosts.Domain;
using System;
using System.Diagnostics;
using System.Threading;
using Ghosts.Domain.Code;

namespace Ghosts.Client.Handlers
{
    public class Print : BaseHandler
    {
        public Print(TimelineHandler handler)
        {
            try
            {
                Log.Trace("Spawning printer job...");

                if (handler.Loop)
                {
                    while (true)
                    {
                        Ex(handler);
                    }
                }
                else
                {
                    Ex(handler);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void Ex(TimelineHandler handler)
        {
            foreach (var timelineEvent in handler.TimeLineEvents)
            {
                WorkingHours.Is(handler);

                if (timelineEvent.DelayBefore > 0)
                {
                    Thread.Sleep(timelineEvent.DelayBefore);
                }

                Log.Trace($"Print Job: {timelineEvent.Command} with delay after of {timelineEvent.DelayAfter}");

                Command(handler, timelineEvent, timelineEvent.Command);

                if (timelineEvent.DelayAfter > 0)
                {
                    Thread.Sleep(timelineEvent.DelayAfter);
                }
            }
        }

        public void Command(TimelineHandler handler, TimelineEvent timelineEvent, string command)
        {
            Thread.Sleep(1000);

            try
            {
                foreach (var fileToPrint in timelineEvent.CommandArgs)
                {
                    var info = new ProcessStartInfo();
                    info.Verb = "print";
                    info.FileName = fileToPrint.ToString();
                    info.CreateNoWindow = true;
                    info.WindowStyle = ProcessWindowStyle.Hidden;

                    var p = new Process();
                    p.StartInfo = info;
                    p.Start();

                    try
                    {
                        p.WaitForInputIdle();
                        Thread.Sleep(3000);
                        if (false == p.CloseMainWindow())
                            p.Kill();
                    }
                    catch
                    {
                        //
                    }

                    Report(handler.HandlerType.ToString(), command, "", timelineEvent.TrackableId);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }
    }
}
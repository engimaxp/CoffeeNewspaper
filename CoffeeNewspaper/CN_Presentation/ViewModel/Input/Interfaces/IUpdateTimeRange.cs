using System;

namespace CN_Presentation.Input
{
    public interface IUpdateTimeRange
    {
        void NotifyUpdateTimeRange(long timeRangeSeconds);
    }
}
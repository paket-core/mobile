using System;

namespace PaketGlobal.iOS
{
    public class EventSharedService : IEventSharedService
    {
        private bool IsRunning = false;

        public void StartUseEvent()
        {
            if (!IsRunning)
            {
                IsRunning = true;
            }
        }

        public void StopUseEvent()
        {
            if(IsRunning){
                IsRunning = false;
            }
        }
    }
}

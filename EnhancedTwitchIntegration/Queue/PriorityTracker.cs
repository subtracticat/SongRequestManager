using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SongRequestManager.Config;

namespace SongRequestManager.Queue
{
    public enum PriorityEventType
    {
        Subscription,
        GiftSubscription,
        Bits,
        Other
    }

    public class PriorityEvent
    {
        public PriorityEvent Type { get; set; }
        public float Value { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class PriorityItem
    {
        public List<PriorityEvent> Events { get; set; } = new List<PriorityEvent>();

        public float GetTotalValue() => this.Events
            .Where(ev => ev.Timestamp.AddDays(RequestBotSettings.Current.Data.PriorityExpirationDays) >= DateTime.Now)
            .Sum(ev => ev.Value);
    }

    public class PriorityTrackerData
    {
        public Dictionary<string, PriorityItem> PriorityItems = new Dictionary<string, PriorityItem>();
    }

    public class PriorityTracker : PersistedStateManager<PriorityTrackerData>
    {
        protected override string FileName { get; } = "SRMPrioCache.json";

        private static PriorityTracker instance;
        public static PriorityTracker Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new PriorityTracker();
                }

                return instance;
            }
        }

        public static bool TryRedeemPrio(string username, out PriorityItem priority)
        {
            var items = Current.Data.PriorityItems;

            if (items.TryGetValue(username, out PriorityItem currentPrio))
            {
                if (currentPrio.GetTotalValue() >= RequestBotSettings.Current.Data.MinimumPriorityRequestValue)
                {
                    Current.Update(data => data.PriorityItems.Remove(username));
                    priority = currentPrio;
                    return true;
                }
            }

            priority = null;
            return false;
        }
    }
}

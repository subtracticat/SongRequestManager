using System;
using System.Collections.Generic;
using System.Linq;
using SongRequestManager.Chat;
using SongRequestManager.Config;

namespace SongRequestManager.Queue
{
    public enum PriorityEventType
    {
        Subscription,
        GiftSubscription,
        Bits,
        Test,
        Unknown
    }

    public class PriorityEvent
    {
        public PriorityEventType Type { get; set; }
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

    public class PriorityTracker : StateManagerBase<PriorityTrackerData>
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
            var normalizedUsername = username.ToLower();
            var items = Current.Data.PriorityItems;

            if (items.TryGetValue(normalizedUsername, out PriorityItem currentPrio))
            {
                if (currentPrio.GetTotalValue() >= RequestBotSettings.Current.Data.MinimumPriorityRequestValue)
                {
                    Current.Update(data => data.PriorityItems.Remove(normalizedUsername));
                    priority = currentPrio;
                    return true;
                }
            }

            priority = null;
            return false;
        }

        public static void RegisterPriorityEvent(string username, PriorityEvent item)
        {
            if (RequestBotSettings.Current.Data.EnableAutoPrio)
            {
                var normalizedUsername = username.ToLower();
                var existingRequest = RequestQueue.Current.GetRequestByUsername(normalizedUsername);
                QueuePosition originalPosition = RequestQueue.Current.GetPositionOf(existingRequest);
                QueuePosition newPosition = originalPosition;

                Current.Update(priorityData =>
                {
                    PriorityItem availablePriorityItem;

                    // Use the existing entry if one exists, otherwise we'll make a new bucket
                    if (!priorityData.PriorityItems.TryGetValue(normalizedUsername, out availablePriorityItem))
                    {
                        availablePriorityItem = new PriorityItem();
                        priorityData.PriorityItems[normalizedUsername] = availablePriorityItem;
                    }

                    // Add the new event to the bucket
                    availablePriorityItem.Events.Add(item);

                    if (existingRequest != null)
                    {
                        var newPrioValue = availablePriorityItem.GetTotalValue();

                        // With the combined existing and new values, is this request a prio request?
                        if (existingRequest.PriorityValue + newPrioValue >= RequestBotSettings.Current.Data.MinimumPriorityRequestValue)
                        {
                            priorityData.PriorityItems.Remove(normalizedUsername);

                            RequestQueue.Current.Remove(existingRequest.Song.ID, RequestStatus.Queued);
                            existingRequest.PriorityValue += newPrioValue;
                            newPosition = RequestQueue.Current.AddPrio(existingRequest);
                        }
                    }
                });

                if (originalPosition != null && newPosition.Position != originalPosition.Position)
                {
                    ChatHandler.Send($"Request for {existingRequest.RequestedBy} ({existingRequest.Song.ID}) updated to from position #{originalPosition.Position} to #{newPosition.Position}.");
                }
            }
        }
    }
}

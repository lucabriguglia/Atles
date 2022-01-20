﻿using System;
using Atles.Infrastructure.Events;
using Newtonsoft.Json;

namespace Atles.Domain.Models
{
    /// <summary>
    /// Extensions for IEvent
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// Converts IEvent to HistoryItem
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public static HistoryItem ToHistoryItem(this IEvent @event)
        {
            if (@event.Id == Guid.Empty || 
                @event.TimeStamp == DateTime.MinValue || 
                @event.TargetId == Guid.Empty ||
                string.IsNullOrEmpty(@event.TargetType) || 
                @event.SiteId == Guid.Empty)
            {
                throw new ArgumentException("The event is not valid. Required values: " +
                                            $"{nameof(@event.Id)}, " +
                                            $"{nameof(@event.TimeStamp)}, " +
                                            $"{nameof(@event.TargetId)}, " +
                                            $"{nameof(@event.TargetType)}, " +
                                            $"{nameof(@event.SiteId)}.");
            }

            return new HistoryItem
            {
                Id = @event.Id,
                TimeStamp = @event.TimeStamp,

                Type = @event.GetType().Name,
                Data = JsonConvert.SerializeObject(@event),

                TargetId = @event.TargetId,
                TargetType = @event.TargetType,
                
                SiteId = @event.SiteId,
                UserId = @event.UserId
            };
        }
    }
}
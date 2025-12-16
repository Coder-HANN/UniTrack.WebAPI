namespace UniTrack.Application.Common.Constants
{
    public static class ValidationKeys
    {
        // Event Check-in 

        public const string EventNotFound = "EventNotFound";
        public const string AlreadyCheckedIn = "AlreadyCheckedIn";
        public const string CheckInSuccess = "CheckInSuccess";

        // Create Event 
        public const string NotAuthorized = "NotAuthorized";
        public const string EventCreatedSuccess = "EventCreatedSuccess";
        public const string EventCreatedNotification = "EventCreatedNotification";
        public const string EventTitleRequired = "EventTitleRequired";
        public const string EventDescriptionRequired = "EventDescriptionRequired";
        public const string EventStartDateInvalid = "EventStartDateInvalid";
        public const string EventEndDateInvalid = "EventEndDateInvalid";
        public const string EventClockRequired = "EventClockRequired";
        public const string EventTagInvalid = "EventTagInvalid";
        public const string EventQuotaInvalid = "EventQuotaInvalid";
        public const string EventLocationRequired = "EventLocationRequired";
        public const string EventStatusInvalid = "EventStatusInvalid";
        public const string EventClubRequired = "EventClubRequired";
        public const string EventCityRequired = "EventCityRequired";
        public const string EventUniversityRequired = "EventUniversityRequired";

        // Delete Event
        public const string EventIdRequired = "EventIdRequired";
        public const string EventDeletedSuccess = "EventDeletedSuccess";
        public const string EventDeletedNotification = "EventDeletedNotification";

        // Update Event
        public const string EventUpdatedSuccess = "EventUpdatedSuccess";
        public const string EventUpdatedNotification = "EventUpdatedNotification";
        public const string UpdateTitleInvalid = "UpdateTitleInvalid";
        public const string UpdateDescriptionInvalid = "UpdateDescriptionInvalid";
        public const string UpdateStartDateInvalid = "UpdateStartDateInvalid";
        public const string UpdateEndDateInvalid = "UpdateEndDateInvalid";
        public const string UpdateQuotaInvalid = "UpdateQuotaInvalid";
        public const string UpdateLocationInvalid = "UpdateLocationInvalid";
        public const string UpdateClockInvalid = "UpdateClockInvalid";
        public const string UpdateStatusInvalid = "UpdateStatusInvalid";
        public const string UpdateTagInvalid = "UpdateTagInvalid";
    }
}

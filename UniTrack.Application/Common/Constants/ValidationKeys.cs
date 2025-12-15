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

    }
}

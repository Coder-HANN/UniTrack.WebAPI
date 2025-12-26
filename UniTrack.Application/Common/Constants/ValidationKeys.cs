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
        public const string EventNotModified = "EventNotModified";

        // Joiner Event

        public const string AlreadyJoinedEvent = "AlreadyJoinedEvent";
        public const string EventExpired = "EventExpired";
        public const string EventQuotaFull = "EventQuotaFull";
        public const string EventUniversityOnly = "EventUniversityOnly";
        public const string EventJoinSuccess = "EventJoinSuccess";

        // Left Event
        public const string NotJoinedEvent = "NotJoinedEvent";
        public const string EventLeftSuccess = "EventLeftSuccess";

        // Get Event Details
        public const string GetEventSuccesses = "GetEventSuccesses";

        // Club Register 
        public const string PresidentEmailRequired = "PresidentEmailRequired";
        public const string ClubEmailAlreadyExists = "ClubEmailAlreadyExists";
        public const string VerificationCodeSent = "VerificationCodeSent";
        public const string ClubRegisterFailed = "ClubRegisterFailed";
        public const string PresidentEmailMustBeEdu = "PresidentEmailMustBeEdu";

        public const string FieldRequired = "FieldRequired";
        public const string PresidentEmailInvalid = "PresidentEmailInvalid";
      
        public const string PasswordTooShort = "PasswordTooShort";
        public const string ClubTagInvalid = "ClubTagInvalid";

        // User Login
        public const string LoginSuccess = "LoginSuccess";
        public const string InvalidEmailOrPassword = "InvalidEmailOrPassword";

        // User Register
        public const string UserEmailAlreadyExists = "UserEmailAlreadyExists";
        public const string UserRegisterSuccess = "UserRegisterSuccess";

        // Club Created 
            public const string InvalidEmail = "InvalidEmail";
            public const string InvalidDate = "InvalidDate";
            public const string InvalidTag = "InvalidTag";

        public const string NotFollowingClub = "NotFollowingClub";
        public const string UnfollowClubSuccess = "UnfollowClubSuccess";

        public const string AlreadyFollowingClub = "AlreadyFollowingClub";
        public const string FollowClubSuccess = "FollowClubSuccess";
      
        public const string UserMustFollowClub = "UserMustFollowClub";
        public const string ClubTeamCreatedSuccess = "ClubTeamCreatedSuccess";

        public const string ClubTeamNotFound = "ClubTeamNotFound";
        public const string ClubTeamDeletedSuccess = "ClubTeamDeletedSuccess";
        // Create Command 

        public const string EventNotFinished = "EventNotFinished";
        public const string AlreadyCommented = "AlreadyCommented";
        public const string UserNotJoinedEvent = "UserNotJoinedEvent";
        public const string CommentCreatedSuccess = "CommentCreatedSuccess";
        public const string PointInvalid = "PointInvalid";
        public const string CommentDeleted = "CommentDeleted";
        public const string CommentNotFound = "CommentNotFound";

        // Bize ulaşın
        public const string MaxLengthExceeded = "MaxLengthExceeded";

        public const string ContactMessageSent = "ContactMessageSent";
        public const string SupportEmailNotConfigured = "SupportEmailNotConfigured";

        // Departman
        public const string DepartmentAlreadyExists = "DepartmentAlreadyExists";
        public const string DepartmentCreatedSuccessfully = "DepartmentCreatedSuccessfully";
        public const string MinLength2 = "MinLength2";
        public const string MaxLength100 = "MaxLength100";

        public const string DepartmentNotFound = "DepartmentNotFound";
        public const string DepartmentDeletedSuccessfully = "DepartmentDeletedSuccessfully";


        public const string UserNotFollowingClub = "UserNotFollowingClub";
        public const string NotificationAlreadyClosed = "NotificationAlreadyClosed";
        public const string NotificationClosedSuccess = "NotificationClosedSuccess";

        public const string NotificationAlreadyOpened = "NotificationAlreadyOpened";
        public const string NotificationOpenedSuccess = "NotificationOpenedSuccess";

        public const string ClubNotFound = "ClubNotFound";
        public const string EmailAlreadyUsed = "EmailAlreadyUsed";
        public const string ProfileUpdatedSuccessfully = "ProfileUpdatedSuccessfully";

        public const string UniversityCreatedSuccesses = "UniversityCreatedSuccesses";
        public const string UniversityNotFound = "UniversityNotFound";
        public const string UniversityDeletedSuccessfully = "UniversityDeletedSuccessfully";
        public const string InvalidOrExpiredCode = "InvalidOrExpiredCode";
        public const string ClubNotFoundByEmail = "ClubNotFoundByEmail";
        public const string ClubPasswordResetSuccess = "ClubPasswordResetSuccess";

        public const string PasswordChangedSuccess = "PasswordChangedSuccess";
        public const string UserNotFound = "UserNotFound";

        public const string ClubVerifiedSuccess = "ClubVerifiedSuccess";
        public const string VerificationCodeInvalidOrExpired = "VerificationCodeInvalidOrExpired";
        public const string ClubCreatedSuccess = "ClubCreatedSuccess";

        public const string ClubFansNotFound = "ClubFansNotFound";

        public const string GoogleSheetsTableNotFound = "GoogleSheetsTableNotFound";
        public const string NotificationNotFound = "NotificationNotFound";

        public const string AlreadyLiked = "AlreadyLiked";

        public const string CreatedNewEvent = "CreatedNewEvent";
        public const string EventIsUpdated = "EventIsUpdated";
        public const string EventIsDeleted = "EventIsDeleted";

        public static string NotificationSendSuccess = "NotificationSendSuccess";
    }
}

namespace SachkovTech.Issues.Infrastructure;

public static class Constants
{
    public const string DATABASE = "Database";

    public const string GRPC_NOTIFICATIONSERVICE_ConnectionStringKey = "NotificationServiceChannel";

    public const int DELETE_EXPIRED_ISSUES_SERVICE_REDUCTION_HOURS = 24;
    
    public static class Issues
    {
        public const int LIFETIME_AFTER_DELETION = 30;
    }
}
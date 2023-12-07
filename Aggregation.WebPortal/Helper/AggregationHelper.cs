namespace Website.Helper
{
    public static class AggregationHelper
    {
        public static string Get_BGColor_FromIndex(int level)
        {
            return level % 2 == 0 ? "bg-dark" : "bg-secondary";
        }

        public static string Get_Button_FromIndex(int level)
        {
            return level % 2 == 0 ? "btn-primary" : "btn-dark";
        }
    }
}

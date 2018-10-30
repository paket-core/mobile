using System;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Java.Lang;

namespace PaketGlobal.Droid
{
    public static class JobSchedulerHelpers
    {
        public static readonly int EventJobId = 110;
   
        public static JobInfo.Builder CreateJobInfoBuilder(this Context context)
        {
            var component = context.GetComponentNameForJob<EventJob>();
            JobInfo.Builder builder = new JobInfo.Builder(EventJobId, component);
            return builder;
        }

        public static ComponentName GetComponentNameForJob<T>(this Context context) where T : JobService
        {
            Type t = typeof(T);
            Class javaClass = Class.FromType(t);
            return new ComponentName(context, javaClass);
        }

    }
}
using System;
using System.Diagnostics;
using System.Linq;

namespace Caliburn.Micro
{
//MPSA: Tracer
    public static class Tracer
    {
        public static bool Active { get; set; } = false;

        [Conditional("TracePropertyChangeOn")]
        internal static void PropertyChange(string propertyName, Delegate[] list = null)
        {
            if (Active == false)
                return;

#if TraceListenersOn       
            PropertyChangedWithTrace(propertyName, list);
#else
            PropertyChangedWithTrace(propertyName);
#endif
        }

        internal static void PropertyChangedWithTrace(string propertyName, Delegate[] list = null)
        {
            if (Active == false)
                return;
            
            StackFrame[] stackFrames = (new StackTrace()).GetFrames();

            int  cnt      = 0;
            bool setFound = false;

            foreach (StackFrame stackFrame in stackFrames)
            {
                var met      = stackFrame.GetMethod();
                var fullName = met.DeclaringType?.FullName ?? met.Name;

                if (fullName.StartsWith("System")
                //||  fullName.StartsWith("Caliburn.Micro") && !fullName.StartsWith("Caliburn.Micro.BindableCollection")
                ||  fullName.StartsWith("Microsoft.EntityFrameworkCore")
                ||  fullName.StartsWith("MS.Win32")
                ||  fullName.StartsWith("MS.Internal")
                ||  fullName.StartsWith("Castle")
                //||  met.Module.Name.StartsWith("Caliburn")
                ||  fullName.StartsWith("lambda_method"))
                    continue;

                if (++cnt == 1)
                {
                    Debug.WriteLine("");

                    if (propertyName == string.Empty)
                        Debug.WriteLine($"Refresh performed:                                   {fullName}.{met.Name}  ");
                    else
                        Debug.WriteLine($"Property Changed: {fullName}.{propertyName}");

                    Debug.WriteLine($"              in:     {fullName}.{met.Name}");
                }
                else
                {
                    Debug.WriteLine($"                      {fullName}.{met.Name}");

                    if (cnt >  10)
                        break;

                    if (met.Name.StartsWith("set_"))
                    {
                        if (!setFound)
                            setFound = true;
                    }
                    else
                        if (setFound)
                            break;
                }
            }

            cnt = 0;

            if (list is null || list.Count() == 0)
                Debug.Write($"         No listeners!");
            else
            {
                var first = true;

                foreach (var x in list)
                {
                    var method = x.Method;
                    var parent = method.ReflectedType.Name;

                    if (parent.StartsWith("ObservableCollectionExt")
                    ||  parent.StartsWith("PropertyChangedEventManager"))
                        continue;

                    if (first)
                    {
                        first = false;
                        Debug.Write($"       Listeners: ");
                    }
                    else
                        Debug.Write($"                : ");

                    Debug.WriteLine($"    {parent}.{method.Name}()");
                    cnt++;
                }
            }

            if (cnt == 0)
                Debug.Write($"         No listeners!");
        }
    }
}

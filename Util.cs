using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace Ranger
{
    internal static class Util
    {
        //public static readonly List<string> DaysOfWeek = new List<string> { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

        //public static readonly List<string> HoursOfDay = new List<string> {"00:00", "00:30", "01:00", "01:30", "02:00", "02:30", "03:00", "03:30", "04:00", "04:30", 
        //                                                            "05:00", "05:30", "06:00", "06:30", "07:00", "07:30", "08:00", "08:30", "09:00", "09:30",
        //                                                            "10:00", "10:30", "11:00", "11:30", "12:00", "12:30", "13:00", "13:30", "14:00", "14:30",
        //                                                            "15:00", "15:30", "16:00", "16:30", "17:00", "17:30", "18:00", "18:30", "19:00", "19:30",
        //                                                            "20:00", "20:30", "21:00", "21:30", "22:00", "22:30", "23:00", "00:30"};


        

        private static Random random = new Random();

        public static string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static void SerializeObject(Object Obj, string path)
        {
            string jsonString = JsonSerializer.Serialize(Obj);
            File.WriteAllText(path, jsonString);
        }

        public static object? DeSerializeObject(Object Obj, string path)
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize(json, Obj.GetType());
        }

        public static Resource GetResourceCopy(Resource resource)
        {
            Resource newResource = new Resource(resource.Name + "_New");

            foreach(Skill skl in resource.Skills)
            {
                Skill newSkill = new Skill(skl.Name);
                newResource.Skills.Add(newSkill);
            }

            foreach(AvailabilityWindow avw in resource.AvailabilityWindows)
            {
                AvailabilityWindow newAvw = new AvailabilityWindow(avw.DayOfWeek, avw.StartTime, avw.EndTime);
                newResource.AvailabilityWindows.Add(newAvw);
            }

            return newResource;
        }
    }
}

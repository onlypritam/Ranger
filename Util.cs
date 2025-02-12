using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Ranger
{
    internal static class Util
    {
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

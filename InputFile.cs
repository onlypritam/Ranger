using System.Collections.Generic;

namespace Ranger
{
    public class InputFile
    {
        public InputFile()
        {
            Skills = new List<Skill>();
            Resources = new List<Resource>();
        }

        public List<Skill> Skills { get; set; }

        public List<Resource> Resources { get; set; }

    }
}

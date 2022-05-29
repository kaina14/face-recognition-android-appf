using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Demo1.Model
{
    [XmlRoot(ElementName = "Attendances")]
    public class Attendance
    {
        [XmlElement(nameof(SName))]
        public String SName { get; set; }

        [XmlElement(nameof(Date))]
        public String Date { get; set; }

        [XmlElement(nameof(InTime))]
        public string InTime { get; set; }

        [XmlElement(nameof(Status))]
        public bool Status { get; set; }
    }
}

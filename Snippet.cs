using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SnippetManager
{
    /// <author email="joke@java-bg.org">Naiden Gochev a.k.a. JOKe</author>
    /// <license> GPL v2.0</license>
    [XmlRoot("Root"), Serializable]
    public class Snippet
    {
        [XmlIgnore]
        public static int counter = 0;

        [XmlIgnore]
        public static readonly string TheSeparatorSnippet = " ---------------------------- ";


        public Snippet()
        {
            counter++;
            Id = counter;
        }

        [XmlElement("Id")]
        public int Id { get; set; }

        private String label;
        [XmlElement("Label")]
        public String Label
        {
            get
            {
                return label;
            }
            set
            {
                if (value != null && value.Length > 20)
                {
                    value = value.Substring(0, 20);
                    value.Replace("\r\n", "");
                }
                this.label = value;
            }
        }
        [XmlElement("Data")]
        public String Data { get; set; }
        public Snippet(String label, String data)
        {
            this.Label = label;
            this.Data = data;
            if (this.Data != TheSeparatorSnippet)
            {
                counter++;
                Id = counter;
            }
        }

        public override string ToString()
        {
            if (this.Data != TheSeparatorSnippet)
            {
                return Id + ") " + label;
            }
            else
            {
                return label;
            }
            
        }

    }
}

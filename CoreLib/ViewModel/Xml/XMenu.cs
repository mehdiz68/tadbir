using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace CoreLib.ViewModel.Xml
{
    [Serializable]
    [XmlRoot("XMenus"), XmlType("XMenus")]
    public class XMenu
    {

        public XMenu()
        {

        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Data { get; set; }
        public string Cover { get; set; }
        public int PlaceShow { get; set; }
        public int DisplayOrder { get; set; }
        public int? MenuId { get; set; }
        public int? LinkId { get; set; }
        public int? TypeId { get; set; }


    }
}

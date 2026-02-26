using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryQuest
{
    internal class Settings
    {
        public static string Amount { get; set; } = "10";
        public static string Difficulty { get; set; } = "";
        public static string CategoryId { get; set; } = "";
        
        public static string AmountDisplay
        {
            get
            {
                return Amount switch
                {
                    "10" => "Kort",
                    "25" => "Medel",
                    "50" => "Långt",
                    _    => "Fel! Välj längd igen"
                };
            }
        }
        public static string DifficultyDisplay
        {
            get
            {
                return Difficulty switch
                {
                    ""          => "Blandat",
                    "easy"      => "Lätt",
                    "medium"    => "Medel",
                    "hard"     => "Svårt",
                    _           => "Fel! Välj svårighetsgrad igen"
                };
            }
        }
        public static string CategoryIdDisplay => CategoryId switch
        {
            "" => "Blandade ämnen",
            "9" => "Allmänbildning",
            "11" => "Film & Bio",
            "12" => "Musik",
            "14" => "TV-serier",
            "15" => "Dataspel",
            "16" => "Brädspel",         
            "17" => "Natur & Vetenskap",
            "18" => "Teknik & IT",
            "20" => "Mytologi",         
            "21" => "Sport",
            "22" => "Geografi",
            "23" => "Historia",
            "26" => "Kändisar",
            "27" => "Djur",
            _ => "Okänd kategori"
        };
        public static Color AmountColor => AmountDisplay switch
        {
            "Fel! Välj längd igen" => Colors.Red,
            _ => Colors.Gold
        };
        public static Color DifficultyColor => DifficultyDisplay switch
        {
            "Fel! Välj svårighetsgrad igen" => Colors.Red,
            _ => Colors.Gold
        };
        public static Color CategoryColor => CategoryIdDisplay switch
        {
            "Fel! Välj kategori igen" => Colors.Red,
            _ => Colors.Gold
        };
    }

}

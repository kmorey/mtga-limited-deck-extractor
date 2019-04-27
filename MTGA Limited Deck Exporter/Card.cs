using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGA_Limited_Deck_Exporter
{
    public class CardImages
    {
        [JsonProperty("small")]
        public string Small { get; set; }
    }

    public class Card : IComparable
    {
        public class RarityComparer : IComparer<Card>
        {
            public int Compare(Card x, Card y)
            {
                return x.Rarity.CompareTo(y.Rarity);
            }
        }

        public Card() { }
        public Card(Card copy)
        {
            Name = copy.Name;
            ArenaId = copy.ArenaId;
            Rarity = copy.Rarity;
            CardImages = copy.CardImages;
        }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("arena_id")]
        public int ArenaId { get; set; }
        [JsonProperty("rarity")]
        public string Rarity { get; set; }
        [JsonProperty("image_uris")]
        public CardImages CardImages { get; set; }

        public int Count { get; set; } = 1;

        public int CompareTo(object obj)
        {
            return ArenaId.CompareTo(((Card)obj).ArenaId);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
